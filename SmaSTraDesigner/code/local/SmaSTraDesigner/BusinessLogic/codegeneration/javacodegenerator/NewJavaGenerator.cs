using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator
{
    class NewJavaGenerator
    {

        /// <summary>
        /// The tree to use for generation.
        /// </summary>
        private readonly TransformationTree _tree;


        /// <summary>
        /// All the code extension stuff needed.
        /// </summary>
        private readonly CodeExtension _codeExtension = new CodeExtension();

        /// <summary>
        /// Creates a new Generator.
        /// </summary>
        /// <param name="tree">To generate with.</param>
        public NewJavaGenerator(TransformationTree tree)
        {
            this._tree = tree;
        }

        /// <summary>
        /// Returns true if the Tree is valid for serialization.
        /// </summary>
        /// <returns>true if valid for building.</returns>
        public bool IsValidTree()
        {
            return IsValidTree(_tree.OutputNode, new List<Node>());
        }


        private bool IsValidTree(Node current, ICollection<Node> visited)
        {
            //Found a already visited node:
            if (visited.Contains(current))
            {
                return true;
            }
            
            //Find an empty input!
            if(current is OutputNode)
            {
                //Output node has to be handled seperately!
                if (current.InputNodes[0] == null) return false;
            }
            else
            {
                //No-Output node!
                for (var i = 0; i < current.InputNodes.Count(); i++)
                {
                    var inputNode = current.InputNodes[i];
                    var data = current.InputIOData[i];
                    if (inputNode == null && !data.IsSet())
                    {
                        return false;
                    }
                }
            }
            

            visited.Add(current);
            return current.InputNodes.NonNull().All(n => IsValidTree(current, visited));
        }


        /// <summary>
        /// Creates the Java Class and all it's dependencies at the destination
        /// </summary>
        /// <param name="destinationFolder">To create at.</param>
        /// <param name="name">To use for creation</param>
        public void CreateJavaSource(string destinationFolder, string name)
        {
            if (!name.EndsWith(".java")) name += ".java";

            //Little hack, since the visited list is populated!
            var rootNode = _tree.OutputNode;
            var nodes = new List<Node>();
            if (!IsValidTree(rootNode, nodes))
            {
                throw new InvalidTreeExection();
            }


            //Set the root node:
            var root = _tree.OutputNode.InputNodes[0];
            if (root is CombinedNode) root = (root as CombinedNode).outputNode;

            _codeExtension.RootNode = root;
            _codeExtension.Package = "Test";

            //Get our Output type.
            _codeExtension.OutputType = rootNode.InputNodes[0].Class.OutputType;
            _codeExtension.ClassName = name.RemoveAll(".java");

            //Generate the Traverse Data:
            var stack = new Stack<QueueStatus>();
            var executeStack = new Stack<Node>();

            stack.Push(new QueueStatus(_tree.OutputNode, 0));
            executeStack.Push(root);

            //Get the loader for generating.
            var loader = Singleton<NodeLoader>.Instance;

            //Pre-Process the Stack:
            while (!stack.Empty())
            {
                var current = stack.Pop();
                if(current == null) { break;  } //Should not happen! Safetynet!

                var index = current.Index;
                var node = current.Node.InputNodes[index];

                //Check for combined nodes:
                if (node is CombinedNode)
                {
                    node = (node as CombinedNode).outputNode;
                    executeStack.Push(node);
                }

                //Add next all to Stack:
                node.InputNodes
                    .ForEachNonNull((n, i) => {
                        //Prevent doubles:
                        var status = new QueueStatus(n, i);
                        if (stack.Contains(status)) return;

                        stack.Push(new QueueStatus(node, i));
                        executeStack.Push(node.InputNodes[i]);
                    });
            }

            //Now lets execute!
            while (!executeStack.Empty())
            {
                var next = executeStack.Pop();
                if (next == null) continue;

                //Now really process:
                loader.CreateCode(next, _codeExtension);
            }


            //Build the rest:
            var sensorVars = _codeExtension.BuildSensorDataVars();
            var transVars = _codeExtension.BuildTransformDataVars();
            var initCode = _codeExtension.BuildInitCode();
            var startCode = _codeExtension.BuildStartCode();
            var stopCode = _codeExtension.BuildStopCode();
            var proxyPropertyCode = _codeExtension.BuildProxyPropertyCode();
            var transformations = _codeExtension.BuildTransformations();

            var theCode = ClassTemplates.GenerateTotal(
                _codeExtension.BuildPackage(),
                _codeExtension.BuildImports(),
                _codeExtension.BuildClassName(),
                _codeExtension.BuildOutputType(),
                initCode,
                sensorVars,
                transVars,
                startCode,
                stopCode,
                proxyPropertyCode,
                transformations
                );

            var exporter = Singleton<CodeExporter>.Instance;
            exporter.save(destinationFolder, theCode, _codeExtension);
        }



    }


    public class InvalidTreeExection : Exception
    {
        public InvalidTreeExection() : base("Tree is not valid") { }
    }


    class QueueStatus
    {
        public Node Node { get; set; }
        public int Index { get; set; }

        public QueueStatus(Node node, int index)
        {
            this.Node = node;
            this.Index = index;
        }

        public override bool Equals(object obj)
        {
            if (obj is QueueStatus) return (obj as QueueStatus).Node == Node;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Node.GetHashCode();
        }
    }

}
