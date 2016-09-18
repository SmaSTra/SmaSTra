using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator
{


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
        /// The Permissions that need to be granted.
        /// </summary>
        private readonly List<string> permissionsNeeded = new List<string>();

        /// <summary>
        /// The internal dictionary to use.
        /// </summary>
        private readonly Dictionary<int, DataSource> sensors = new Dictionary<int, DataSource>();

        /// <summary>
        /// The internal dictionary to use.
        /// </summary>
        private readonly Dictionary<int, Transformation> transformationOuts = new Dictionary<int, Transformation>();

        /// <summary>
        /// The internal dictionary of Buffers.
        /// </summary>
        private readonly Dictionary<int, BufferNode> buffers = new Dictionary<int, BufferNode>();

        /// <summary>
        /// A counter for the next Sensor to set.
        /// </summary>
        private int nextSensor = 0;

        /// <summary>
        /// A counter for the next Transformation to set.
        /// </summary>
        private int nextTrans = 0;

        /// <summary>
        /// A counter for the next Buffer to set.
        /// </summary>
        private int nextBuffer = 0;

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
            if (!imports.Contains(import) && !importBlacklist.Contains(import))
            {
                this.imports.Add(import);
            }
        }

        /// <summary>
        /// Adds a new Buffer to the system.
        /// </summary>
        /// <param name="node"></param>
        public void AddBuffer(BufferNode node)
        {
            this.buffers.Add(nextBuffer, node);
            nextBuffer++;
        }

        /// <summary>
        /// Adds a needed Permission.
        /// </summary>
        public void AddNeededPermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission)) return;

            if (!permissionsNeeded.Contains(permission))
            {
                this.permissionsNeeded.Add(permission);
            }
        }

        /// <summary>
        /// Adds the Permissions passed to the System.
        /// </summary>
        /// <param name="permissions">To add</param>
        public void AddNeededPermissions(string[] permissions)
        {
            if (permissions == null || permissions.Count() == 0) return;

            permissions.ForEach(AddNeededPermission);
        }

        /// <summary>
        /// Gets all Needed Permissions.
        /// </summary>
        public string[] GetNeededPermissions()
        {
            return permissionsNeeded.Distinct().ToArray();
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
            if (node is DataSource) return GetSensorId(node as DataSource);
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
        /// Gets all used Nodes.
        /// </summary>
        public Node[] GetAllUsedNodes()
        {
            return sensors
                .Select(e => (Node) e.Value)
                .Concat(transformationOuts.Select(e => (Node) e.Value))
                .ToArray();
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
            for (int i = 0; i < nextSensor; i++)
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
            for (int i = 0; i < nextTrans - 1; i++)
            {
                Transformation transform = transformationOuts[i];
                TransformationNodeClass transClass = transform.Class as TransformationNodeClass;
                if (transClass.IsStatic) continue;

                transOut += string.Format("   private {0} transformation{1};\n", transform.Class.OutputType.MinimizedName, i);
            }

            for (int i = 0; i < nextTrans - 1; i++)
            {
                Transformation transform = transformationOuts[i];
                transOut += string.Format("   private {0} transform{1};\n", transform.Class.OutputType.MinimizedName, i);
            }

            return transOut;
        }


        /// <summary>
        /// Builds the Data for the Sensor output.
        /// </summary>
        public string BuildInitCode()
        {
            string initCode = "";
            for (int i = 0; i < nextSensor; i++)
            {
                DataSource sensor = sensors[i];
                DataSourceNodeClass clazz = sensor.Class as DataSourceNodeClass;

                initCode += string.Format("       sensor{0} = new {1}(context);\n", i, DataType.minimizeToClass(sensor.Class.MainClass));
            }

            for (int i = 0; i < nextTrans; i++)
            {
                Transformation transformation = transformationOuts[i];
                TransformationNodeClass clazz = transformation.Class as TransformationNodeClass;
                if (clazz.IsStatic) continue;

                initCode += string.Format("       trans{0} = new {1}();\n", i, DataType.minimizeToClass(transformation.Class.MainClass));
            }

            return initCode;
        }


        /// <summary>
        /// Builds the Data for the Start method.
        /// </summary>
        public string BuildStartCode()
        {
            string startCode = "";
            for (int i = 0; i < nextSensor; i++)
            {
                DataSource sensor = sensors[i];
                DataSourceNodeClass clazz = sensor.Class as DataSourceNodeClass;

                string startMethod = clazz.StartMethod;
                if (!string.IsNullOrWhiteSpace(startMethod)) startCode += string.Format("       sensor{0}.{1}();\n", i, startMethod);
            }

            return startCode;
        }


        /// <summary>
        /// Builds the Data for the Stop method.
        /// </summary>
        public string BuildStopCode()
        {
            string stopCode = "";
            for (int i = 0; i < nextSensor; i++)
            {
                DataSource sensor = sensors[i];
                DataSourceNodeClass clazz = sensor.Class as DataSourceNodeClass;

                string stopMethod = clazz.StopMethod;
                if (!string.IsNullOrWhiteSpace(stopMethod)) stopCode += string.Format("       sensor{0}.{1}();\n", i, stopMethod);
            }

            return stopCode;
        }

        /// <summary>
        /// Builds the Proxy Property Code.
        /// </summary>
        /// <returns>The </returns>
        public string BuildProxyPropertyCode()
        {
            Node[] allNodes = GetAllUsedNodes();
            List<Node> added = new List<Node>();
            string proxyPropertyCode = "";

            allNodes.ForEach(
                n =>
                {
                    foreach (ProxyProperty prop in n.Class.ProxyProperties)
                    {
                        string methodName = prop.MethodName;
                        string className = n.Class.Name.RemoveAll(" ","_");
                        int varID = added.Count(a => a.Class == n.Class);
                        string inputTypeName = prop.PropertyType.MinimizedName;
                        string varName = n.Class.NodeType.ToString().ToLower();
                        int classID = GetOutputIndex(n);

                        proxyPropertyCode += string.Format(ClassTemplates.GENERATION_TEMPLATE_PROXY_PROP,
                            methodName,
                            className,
                            varID.ToString(),
                            inputTypeName,
                            varName,
                            classID.ToString()
                        );
                    }

                    added.Add(n);
                }
            );

            return proxyPropertyCode;
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


}
