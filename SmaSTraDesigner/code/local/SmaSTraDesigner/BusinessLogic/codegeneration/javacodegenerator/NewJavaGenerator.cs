using Common;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                DirCopy.DirectoryCopy(DirCopy.GetPathForNode(next.Class), destinationFolder, true);
            }


            //Build the rest:
            string codeSteps = codeExtension.BuildCodeSteps();
            string sensorVars = codeExtension.BuildSensorDataVars();
            string transVars = codeExtension.BuildTransformDataVars();
            string sensorInits = codeExtension.BuildSensorInit();
            string switchTransform = codeExtension.BuildSwitchTransform();
            string transformations = codeExtension.BuildTransformations();

            string TheCode = string.Format(ClassTemplates.GENERATION_TEMPLATE_TOTAL,
                codeExtension.BuildPackage(),
                codeExtension.BuildImports(),
                codeExtension.BuildClassName(),
                codeExtension.BuildOutputType(),
                codeSteps,
                sensorVars,
                transVars,
                sensorInits,
                switchTransform,
                transformations
                );

            //Write the code:
            File.WriteAllText(Path.Combine(destinationFolder, name), TheCode);
            DirCopy.FinalizeCopy("generated\\", destinationFolder);
        }



    }



    public class CodeExtension
    {

        #region consts

        private readonly string[] StaticImports = { "android.content.Context", "de.tu_darmstadt.smastra.base.SmaSTraTreeExecutor" };
        private readonly string[] importBlacklist = { "double", "byte", "long", "float", "short", "char", "int", "integer", "boolean" };

        #endregion consts

        #region variables

        /// <summary>
        /// The Imports to use.
        /// </summary>
        private readonly List<string> imports = new List<string>();

        /// <summary>
        /// The internal dictionary to use.
        /// </summary>
        private readonly Dictionary<int, DataSource> sensors = new Dictionary<int, DataSource>();

        /// <summary>
        /// The internal dictionary to use.
        /// </summary>
        private readonly Dictionary<int, Transformation> transformationOuts = new Dictionary<int, Transformation>();

        /// <summary>
        /// A counter for the next Sensor to set.
        /// </summary>
        private int nextSensor = 0;

        /// <summary>
        /// A counter for the next Transformation to set.
        /// </summary>
        private int nextTrans = 0;

        /// <summary>
        /// The code steps totally present.
        /// </summary>
        private List<string> codeSteps = new List<string>();



        /// <summary>
        /// The Output type.
        /// </summary>
        public DataType OutputType { get; set; }

        /// <summary>
        /// The name of the class to generate
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The Package name to use.
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// This is the node 1 after the Output node.
        /// </summary>
        public Node RootNode { get; set; }


        #endregion varibles


        #region methods

        /// <summary>
        /// Returns the current Step.
        /// </summary>
        public int GetCurrentStep()
        {
            return codeSteps.Count();
        }

        /// <summary>
        /// Adds the Import if not present.
        /// </summary>
        /// <param name="import"></param>
        public void AddImport(string import)
        {
            if(!imports.Contains(import) && !importBlacklist.Contains(import))
            {
                this.imports.Add(import);
            }
        }

        /// <summary>
        /// Adds a new Source to the Data.
        /// </summary>
        public void AddSensor(DataSource source)
        {
            this.sensors.Add(this.nextSensor, source);
            this.nextSensor++;
        }

        /// <summary>
        /// Adds the Transformation passed
        /// </summary>
        public void AddTransformation(Transformation trans)
        {
            this.transformationOuts.Add(this.nextTrans, trans);
            this.nextTrans++;
        }

        /// <summary>
        /// Gets the output index of the node passed.
        /// </summary>
        public int GetOutputIndex(Node node)
        {
            if(node is DataSource) return GetSensorId(node as DataSource);
            if (node is Transformation) return GetTransformId(node as Transformation);

            return -1;
        }

        /// <summary>
        /// Gets the ID for the Sensor.
        /// </summary>
        public int GetSensorId(DataSource source)
        {
            return sensors.GetKeyForValue(source, -1);
        }

        /// <summary>
        /// Gets the ID for the Transformation
        /// </summary>
        public int GetTransformId(Transformation trans)
        {
            return transformationOuts.GetKeyForValue(trans, -1);
        }



        /// <summary>
        /// Adds the Import if not present.
        /// </summary>
        /// <param name="imports">to add</param>
        public void AddImports(string[] imports)
        {
            imports.ForEach(AddImport);
        }

        /// <summary>
        /// Adds a code-step.
        /// </summary>
        public void AddCodeStep(string code)
        {
            this.codeSteps.Add(code);
        }


        /// <summary>
        /// Gets all current Imports joined.
        /// </summary>
        public string BuildImports()
        {
            return string.Join("\n", this.imports
                .Concat(StaticImports)
                .Distinct()
                .Select(i => "import " + i + ";"));
        }


        /// <summary>
        /// Builds Package
        /// </summary>
        public string BuildPackage()
        {
            return Package;
        }

        /// <summary>
        /// Builds the Output type.
        /// </summary>
        public string BuildOutputType()
        {
            string OutName = OutputType.MinimizedName;
            if (importBlacklist.Contains(OutName))
            {
                OutName = OutName.First().ToString().ToUpper() + OutName.Substring(1);
            }

            return OutName;
        }

        /// <summary>
        /// Builds the Classname for the Class.
        /// </summary>
        public string BuildClassName()
        {
            return ClassName;
        }

        /// <summary>
        /// Builds the Data for the Sensor output.
        /// </summary>
        public string BuildSensorDataVars()
        {
            string sensorOutput = "";
            for(int i = 0; i < nextSensor; i++)
            {
                DataSource sensor = sensors[i];
                sensorOutput += string.Format("   private {0} sensor{1};\n", DataType.minimizeToClass(sensor.Class.MainClass), i);
            }

            return sensorOutput;
        }

        /// <summary>
        /// Builds the Data for the Sensor output.
        /// </summary>
        public string BuildTransformDataVars()
        {
            string transOut = "";
            for (int i = 0; i < nextTrans; i++)
            {
                Transformation transform = transformationOuts[i];
                transOut += string.Format("   private {0} transformation{1};\n", transform.Class.OutputType.MinimizedName, i);
            }

            return transOut;
        }


        /// <summary>
        /// Builds the Data for the Sensor output.
        /// </summary>
        public string BuildSensorInit()
        {
            string sensorOutput = "";
            for (int i = 0; i < nextSensor; i++)
            {
                DataSource sensor = sensors[i];
                DataSourceNodeClass clazz = sensor.Class as DataSourceNodeClass;

                sensorOutput += string.Format("       sensor{0} = new {1}(context);\n", i, DataType.minimizeToClass(sensor.Class.MainClass));
                if(!string.IsNullOrWhiteSpace(clazz.StartMethod)) sensorOutput += string.Format("       sensor{0}.{1}();\n", i, clazz.StartMethod);
            }

            return sensorOutput;
        }

        /// <summary>
        /// Builds the codesteps.
        /// </summary>
        public string BuildCodeSteps()
        {
            return (codeSteps.Count() - 1).ToString();
        }

        /// <summary>
        /// Builds the Switch statement needed for execution.
        /// </summary>
        public string BuildSwitchTransform()
        {
            string switchStatement = "";
            for(int i = 0; i < codeSteps.Count(); i++)
            {
                switchStatement += string.Format("			case {0} : transform{0}(); break;\n", i);
            }

            return switchStatement;
        }

        /// <summary>
        /// Builds the Transformation code.
        /// </summary>
        public string BuildTransformations()
        {
            return string.Join("\n\n", this.codeSteps);
        }

        #endregion methods
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
