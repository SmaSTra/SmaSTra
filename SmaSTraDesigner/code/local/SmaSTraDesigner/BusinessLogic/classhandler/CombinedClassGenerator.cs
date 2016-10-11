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
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    public class CombinedClassGenerator
    {


        #region StaticMethods


        private static bool SubTreeContains(Node root, ICollection<Node> nodes)
        {
            //Remove the first element:
            if (root != null) nodes.Remove(root);

            //If empty -> Everything is okay!
            if (!nodes.Any()) return true;

            //Check recursivcely:
            return root != null && root.InputNodes.NonNull().Any(input => SubTreeContains(input, nodes));
        }

        private static bool IsCyclic(Node root, ICollection<Node> nodes, ICollection<Node> visited)
        {
            //If empty -> Everything is okay!
            if (!nodes.Any()) return false;

            //Remove the first element:
            if (root != null) nodes.Remove(root);

            visited.Add(root);

            //Check recursivcely:
            if (root == null) return false;

            foreach (var input in root.InputNodes.NonNull())
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
        private readonly List<Node> _nodes = new List<Node>();

        /// <summary>
        /// The Cached node class to not having to rebuild it all the time when calling the getter.
        /// </summary>
        private CombinedNodeClass _cachedNodeClass = null;

        #endregion fields

        #region properties

        /// <summary>
        /// The Name to set.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Description to set.
        /// </summary>
        public string Description { get; set; }


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
            if (node == null) return;

            this._nodes.Add(node);
            this._cachedNodeClass = null;
        }

        /// <summary>
        /// This adds all Nodes to the node generation list.
        /// </summary>
        /// <param name="nodes">to add</param>
        public void AddNodes(IEnumerable<Node> nodes)
        {
            if (nodes == null || !nodes.Any()) return;

            this._nodes.AddRange(nodes);
            this._cachedNodeClass = null;
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
            return IsCyclic(GetRootNode(), _nodes, new List<Node>());
        }


        /// <summary>
        /// Generates the NodeClass.
        /// </summary>
        /// <returns>The node class or null if not possible.</returns>
        public CombinedNodeClass GenerateClass()
        {
            if (_cachedNodeClass != null) return _cachedNodeClass;

            var root = GetRootNode();
            if (root == null) return null;

            //Generate Inputs + Sub-Hirachy.
            var inputs = new List<DataType>();
            var connections = new List<SimpleConnection>();
            var subNodes = new List<SimpleSubNode>();

            var centerX = _nodes.Average(n => n.PosX);
            var centerY = _nodes.Average(n => n.PosY);

            var input = 0;

            foreach ( var node in _nodes)
            {
                var nodeClass = node.Class;
                var nodeInputs = node.InputNodes;

                subNodes.Add(new SimpleSubNode(node, centerX, centerY));
                for(var i = 0; i < nodeInputs.Count(); i++)
                {
                    var subNode = nodeInputs[i];
                    if (subNode == null || !_nodes.Contains(subNode))
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
            var output = root.Class.OutputType;
            var creator = Environment.UserName;

            //Finally generate the NodeClass
            var finalNodeClass = new CombinedNodeClass(Name, Name, Description, creator, 
                subNodes, connections, output, root.NodeUUID, true, Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR, Name.RemoveAll(" ","_")),
                inputs.ToArray());

            this._cachedNodeClass = finalNodeClass;
            return finalNodeClass;
        }


        /// <summary>
        /// Saves the Current state to the disc.
        /// </summary>
        public bool SaveToDisc()
        {
            var toSave = GenerateClass();
            if (toSave == null) return false;

            var savePath = Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR);
            savePath = Path.Combine(savePath, toSave.DisplayName);

            if (Directory.Exists(savePath)) return false;
            Singleton<NodeLoader>.Instance.saveToFolder(toSave, savePath);

            //Now save the depencies to the depencies.zip.
            var tmpSaveFolder = Path.Combine(savePath, "dependencies");
            Directory.CreateDirectory(tmpSaveFolder);
            this._nodes
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
                var zipPath = Path.Combine(savePath, "dependencies.zip");
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
            return _nodes.FirstOrDefault(node => SubTreeContains(node, _nodes.ToList()));
        }

        /// <summary>
        /// Returns true if the Name already exists.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        internal static bool ExistsName(string newName)
        {
            return Singleton<ClassManager>.Instance.GetNodeClassForType(newName) != null;
        }

        #endregion Methods

    }
}
