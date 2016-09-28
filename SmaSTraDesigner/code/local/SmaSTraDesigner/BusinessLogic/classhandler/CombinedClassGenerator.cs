using Common;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    class CombinedClassGenerator
    {


        #region StaticMethods


        private static bool SubTreeContains(Node root, List<Node> nodes)
        {
            //Remove the first element:
            if (root != null) nodes.Remove(root);

            //If empty -> Everything is okay!
            if (!nodes.Any()) return true;

            //Check recursivcely:
            foreach (Node input in root.InputNodes.NonNull())
            {
                if (SubTreeContains(input, nodes)) return true;
            }

            return false;
        }

        private static bool IsCyclic(Node root, List<Node> nodes, List<Node> visited)
        {
            //If empty -> Everything is okay!
            if (!nodes.Any()) return false;

            //Remove the first element:
            if (root != null) nodes.Remove(root);

            visited.Add(root);

            //Check recursivcely:
            foreach (Node input in root.InputNodes.NonNull())
            {
                if (visited.Contains(input)) return true;
                if (IsCyclic(input, nodes, visited)) return true;
            }

            return false;
        }

        #endregion StaticMethods

        #region fields

        /// <summary>
        /// The Nodes to use for generation.
        /// </summary>
        private List<Node> nodes = new List<Node>();

        /// <summary>
        /// The Cached node class to not having to rebuild it all the time when calling the getter.
        /// </summary>
        private CombinedNodeClass cachedNodeClass = null;

        #endregion fields

        #region properties

        /// <summary>
        /// The Name to set.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The Description to set.
        /// </summary>
        public String Description { get; set; }


        #endregion properties

        #region constructors

        public CombinedClassGenerator()
        {
            this.Name = "TMP" + new Random().Next();
            this.Description = "No description provided";
        }

        public CombinedClassGenerator(String name) : this()
        {
            this.Name = name;
        }

        public CombinedClassGenerator(String name, IEnumerable<Node> nodes) : this(name)
        {
            this.AddNodes(nodes);
        }

        public CombinedClassGenerator(IEnumerable<Node> nodes) : this()
        {
            this.AddNodes(nodes);
        }

        #endregion constructors

        #region Methods

        /// <summary>
        /// This adds a Node to the node generation list.
        /// </summary>
        /// <param name="node">to add</param>
        public void AddNode(Node node)
        {
            if (node != null)
            {
                this.nodes.Add(node);
                this.cachedNodeClass = null;
            }
        }

        /// <summary>
        /// This adds all Nodes to the node generation list.
        /// </summary>
        /// <param name="nodes">to add</param>
        public void AddNodes(IEnumerable<Node> nodes)
        {
            if (nodes != null && nodes.Any())
            {
                this.nodes.AddRange(nodes);
                this.cachedNodeClass = null;
            }
        }

        /// <summary>
        /// Checks if the Nodes are connected.
        /// </summary>
        /// <returns>true if connected</returns>
        public bool IsConnected()
        {
            return GetRootNode() != null;
        }

        /// <summary>
        /// Checks if the Nodes are connected.
        /// </summary>
        /// <returns>true if connected</returns>
        public bool IsCyclic()
        {
            return IsCyclic(GetRootNode(), nodes, new List<Node>());
        }


        /// <summary>
        /// Generates the NodeClass.
        /// </summary>
        /// <returns>The node class or null if not possible.</returns>
        public CombinedNodeClass GenerateClass()
        {
            if (cachedNodeClass != null) return cachedNodeClass;

            Node root = GetRootNode();
            if (root == null) return null;

            //Generate Inputs + Sub-Hirachy.
            List<DataType> inputs = new List<DataType>();
            List<SimpleConnection> connections = new List<SimpleConnection>();
            List<SimpleSubNode> subNodes = new List<SimpleSubNode>();

            double centerX = nodes.Average(n => n.PosX);
            double centerY = nodes.Average(n => n.PosY);

            int input = 0;

            foreach ( Node node in nodes)
            {
                AbstractNodeClass nodeClass = node.Class;
                Node[] nodeInputs = node.InputNodes;

                subNodes.Add(new SimpleSubNode(node, centerX, centerY));
                for(int i = 0; i < nodeInputs.Count(); i++)
                {
                    Node subNode = nodeInputs[i];
                    if (subNode == null || !nodes.Contains(subNode))
                    {
                        inputs.Add(nodeClass.InputTypes[i]);
                        connections.Add(new SimpleConnection(node.NodeUUID, "input"+input, i));
                        input++;
                    }else{
                        connections.Add(new SimpleConnection(node.NodeUUID, subNode.NodeUUID, i));
                    }
                }
            }

            //Generate the BaseNode:
            DataType output = root.Class.OutputType;
            string creator = Environment.UserName;

            //Finally generate the NodeClass
            CombinedNodeClass finalNodeClass = new CombinedNodeClass(Name, Name, Description, creator, 
                subNodes, connections, output, root.NodeUUID, true, Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR, Name.RemoveAll(" ","_")),
                inputs.ToArray());

            this.cachedNodeClass = finalNodeClass;
            return finalNodeClass;
        }


        /// <summary>
        /// Saves the Current state to the disc.
        /// </summary>
        public bool SaveToDisc()
        {
            CombinedNodeClass toSave = GenerateClass();
            if (toSave == null) return false;

            string savePath = Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR);
            savePath = Path.Combine(savePath, toSave.DisplayName);

            if (Directory.Exists(savePath)) return false;
            Singleton<NodeLoader>.Instance.saveToFolder(toSave, savePath);

            //Now save the depencies to the depencies.zip.
            string tmpSaveFolder = Path.Combine(savePath, "dependencies");
            Directory.CreateDirectory(tmpSaveFolder);
            this.nodes
                .Select(n => n.Class)
                .Where(n => n.UserCreated && n != toSave)
                .Distinct()
                .ForEach(c =>
                {
                    string nodeDir = Path.Combine(tmpSaveFolder, c.Name);
                    Directory.CreateDirectory(nodeDir);
                    DirCopy.PlainCopy(c.NodePath, nodeDir);
                });

            //Generate the Zip file from that directory:
            if (!Directory.EnumerateFileSystemEntries(tmpSaveFolder).Empty())
            {
                string zipPath = Path.Combine(savePath, "dependencies.zip");
                ZipFile.CreateFromDirectory(tmpSaveFolder, zipPath, CompressionLevel.NoCompression, false);
            }

            Directory.Delete(tmpSaveFolder, true);
            return true;
        }


        /// <summary>
        /// Gets the Root node of the Generated class.
        /// </summary>
        /// <returns>the Root node or null</returns>
        public Node GetRootNode()
        {
            foreach (Node node in nodes) if (SubTreeContains(node, nodes.ToList())) return node;
            return null;
        }

        /// <summary>
        /// Returns true if the Name already exists.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        internal bool ExistsName(string newName)
        {
            return Singleton<ClassManager>.Instance.GetNodeClassForType(newName) != null;
        }

        #endregion Methods

    }
}
