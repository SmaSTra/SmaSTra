using Common;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration
{
    class NewJavaGenerator
    {

        /// <summary>
        /// The tree to use for generation.
        /// </summary>
        private readonly TransformationTree tree;


        /// <summary>
        /// All the code extension stuff needed.
        /// </summary>
        private readonly CodeExtension codeExtension = new CodeExtension();

        /// <summary>
        /// Creates a new Generator.
        /// </summary>
        /// <param name="tree">To generate with.</param>
        public NewJavaGenerator(TransformationTree tree)
        {
            this.tree = tree;
        }

        /// <summary>
        /// Returns true if the Tree is valid for serialization.
        /// </summary>
        /// <returns>true if valid for building.</returns>
        public bool isValidTree()
        {
            return isValidTree(tree.OutputNode, new List<Node>());
        }


        private bool isValidTree(Node current, List<Node> visited)
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
                for (int i = 0; i < current.InputNodes.Count(); i++)
                {
                    Node inputNode = current.InputNodes[i];
                    IOData data = current.InputIOData[i];
                    if (inputNode == null && !data.IsSet())
                    {
                        return false;
                    }
                }
            }
            

            visited.Add(current);
            return !current.InputNodes.NonNull().Any(n => !isValidTree(current, visited));
        }


        /// <summary>
        /// Creates the Java Class and all it's dependencies at the destination
        /// </summary>
        /// <param name="destination">To create at.</param>
        public void CreateJavaSource(string destinationFolder, string name)
        {
            if (!name.EndsWith(".java")) name += ".java";

            //Little hack, since the visited list is populated!
            OutputNode rootNode = tree.OutputNode;
            List<Node> nodes = new List<Node>();
            if (!isValidTree(rootNode, nodes))
            {
                throw new InvalidTreeExection();
            }


            //Set the root node:
            Node root = tree.OutputNode.InputNodes[0];
            if (root is CombinedNode) root = (root as CombinedNode).outputNode;

            codeExtension.RootNode = root;
            codeExtension.Package = "Test";

            //Get our Output type.
            codeExtension.OutputType = rootNode.InputNodes[0].Class.OutputType;
            codeExtension.ClassName = name.RemoveAll(".java");

            //Generate the Traverse Data:
            Stack<QueueStatus> stack = new Stack<QueueStatus>();
            Stack<Node> executeStack = new Stack<Node>();

            stack.Push(new QueueStatus(tree.OutputNode, 0));
            executeStack.Push(root);

            //Get the loader for generating.
            NodeLoader loader = Singleton<NodeLoader>.Instance;

            //Pre-Process the Stack:
            while (!stack.Empty())
            {
                QueueStatus current = stack.Pop();
                if(current == null) { break;  } //Should not happen! Safetynet!

                int index = current.index;
                Node node = current.node.InputNodes[index];

                //Check for combined nodes:
                if (node is CombinedNode)
                {
                    node = (node as CombinedNode).outputNode;
                }

                //Add next all to Stack:
                if (current != null)
                {
                    node.InputNodes
                        .ForEachNonNull((n, i) => {
                            //Prevent doubles:
                            QueueStatus status = new QueueStatus(n, i);
                            if (!stack.Contains(status))
                            {
                                stack.Push(new QueueStatus(node, i));
                                executeStack.Push(node.InputNodes[i]);
                            }
                        });
                }
            }

            //Now lets execute!
            while (!executeStack.Empty())
            {
                Node next = executeStack.Pop();
                if (next == null) continue;

                //Now really process:
                loader.CreateCode(next, codeExtension);
            }


            //Build the rest:
            string sensorVars = codeExtension.BuildSensorDataVars();
            string transVars = codeExtension.BuildTransformDataVars();
            string initCode = codeExtension.BuildInitCode();
            string startCode = codeExtension.BuildStartCode();
            string stopCode = codeExtension.BuildStopCode();
            string proxyPropertyCode = codeExtension.BuildProxyPropertyCode();
            string transformations = codeExtension.BuildTransformations();

            string TheCode = ClassTemplates.GenerateTotal(
                codeExtension.BuildPackage(),
                codeExtension.BuildImports(),
                codeExtension.BuildClassName(),
                codeExtension.BuildOutputType(),
                initCode,
                sensorVars,
                transVars,
                startCode,
                stopCode,
                proxyPropertyCode,
                transformations
                );

            CodeExporter exporter = Singleton<CodeExporter>.Instance;
            exporter.save(destinationFolder, TheCode, codeExtension);
        }



    }


    public class InvalidTreeExection : Exception
    {
        public InvalidTreeExection() : base("Tree is not valid") { }
    }


    class QueueStatus
    {
        public Node node { get; set; }
        public int index { get; set; }

        public QueueStatus(Node node, int index)
        {
            this.node = node;
            this.index = index;
        }

        public override bool Equals(object obj)
        {
            if (obj is QueueStatus) return (obj as QueueStatus).node == node;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return node.GetHashCode();
        }
    }

}
