using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class SensorLoader : AbstractNodeLoader
    {

        private const string JSON_PROP_DATA_METHOD = "dataMethod";
        private const string JSON_PROP_START_METHOD = "start";
        private const string JSON_PROP_STOP_METHOD = "stop";



        public SensorLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Sensor, cManager)
        {}

        public override AbstractNodeClass loadFromJson(string path, JObject root)
        {
            string name = ReadName(root);
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            string creator = ReadCreator(root).EmptyDefault("Unknown");
            DataType output = ReadOutput(root);
            string mainClass = ReadMainClass(root);
            string[] needsOtherClasses = ReadNeededClasses(root);
            string[] neededPermissions = ReadNeededPermissions(root);
            ConfigElement[] config = ReadConfig(root);
            ProxyProperty[] proxyProperties = ReadProxyProperties(root);
            string dataMethod = ReadDataMethod(root);
            string startMethod = ReadStartMethod(root);
            string stopMethod = ReadStopMethod(root);
            bool userCreated = ReadUserCreated(root);

            return new DataSourceNodeClass(name, displayName, description, creator, output, mainClass, 
                needsOtherClasses, neededPermissions, config, proxyProperties, userCreated, path,
                dataMethod, startMethod, stopMethod);
        }


        protected string ReadDataMethod(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsString(JSON_PROP_DATA_METHOD, "");
        }

        protected string ReadStartMethod(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsString(JSON_PROP_START_METHOD, "");
        }

        protected string ReadStopMethod(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsString(JSON_PROP_STOP_METHOD, "");
        }



        public override JObject classToJson(AbstractNodeClass nodeClass)
        {
            DataSourceNodeClass sourceClass = nodeClass as DataSourceNodeClass;

            JObject root = new JObject();
            AddOwnType(root);
            AddName(root, nodeClass.Name);
            AddDescription(root, nodeClass.Description);
            AddDisplayName(root, nodeClass.DisplayName);
            AddOutput(root, nodeClass.OutputType);
            AddMainClass(root, nodeClass.MainClass);
            AddNeededClasses(root, nodeClass.NeedsOtherClasses);
            AddPermissions(root, nodeClass.NeedsPermissions);
            AddConfig(root, nodeClass.Configuration);
            AddProxyProperties(root, nodeClass.ProxyProperties);
            AddUserCreated(root, nodeClass.UserCreated);
            AddCreator(root, nodeClass.Creator);

            AddDataMethod(root, sourceClass.DataMethod);
            AddStartMethod(root, sourceClass.StartMethod);
            AddStopMethod(root, sourceClass.StopMethod);

            return root;
        }

        protected void AddDataMethod(JObject toAddTo, string dataMethod)
        {
            toAddTo.Add(JSON_PROP_DATA_METHOD, dataMethod);
        }
        protected void AddStartMethod(JObject toAddTo, string startMethod)
        {
            toAddTo.Add(JSON_PROP_START_METHOD, startMethod);
        }
        protected void AddStopMethod(JObject toAddTo, string stopMethod)
        {
            toAddTo.Add(JSON_PROP_STOP_METHOD, stopMethod);
        }



        public override string GenerateClassFromSnippet(AbstractNodeClass nodeClass, string methodCode)
        {
            string package = GetPackageFromMainclass(nodeClass.MainClass);
            string imports = nodeClass.InputTypes
                .ToArray()
                .Concat(new[] { nodeClass.OutputType })
                .Distinct()
                .Select(i => "import " + i.MinimizedName + ";")
                .StringJoin("\n");

            return string.Format(
                ClassTemplates.SENSOR_TEMPLATE,

                package,
                imports,
                nodeClass.Name.RemoveAll(" ", "_"), 
                nodeClass.OutputType.Name, 
                methodCode);
        }


        public override void CreateCode(Node node, CodeExtension codeExtension)
        {
            DataSourceNodeClass nodeClass = node.Class as DataSourceNodeClass;
            codeExtension.AddSensor(node as DataSource);
            codeExtension.AddNeededPermissions(nodeClass.NeedsPermissions);

            codeExtension.AddImport(nodeClass.OutputType.Name);
            codeExtension.AddImport(nodeClass.MainClass);


            //Add the proxy methods to the Imports:
            nodeClass.ProxyProperties
                .Select(p => p.PropertyType.Name)
                .ForEach(codeExtension.AddImport);


            //We have a special case: Only a DataSource:
            if (codeExtension.RootNode == node)
            {
                string code = " private void transform0(){\n" 
                            + "     data = sensor0." + nodeClass.DataMethod + "();\n"
                            + " }\n";

                codeExtension.AddCodeStep(code);
            }
        }

    }
}
