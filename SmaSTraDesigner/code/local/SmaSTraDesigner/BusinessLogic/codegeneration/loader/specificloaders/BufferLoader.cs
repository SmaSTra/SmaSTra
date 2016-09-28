using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using static SmaSTraDesigner.BusinessLogic.ClassManager;
using SmaSTraDesigner.BusinessLogic.utils;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders
{
    class BufferLoader : AbstractNodeLoader
    {
        
        private const string JSON_PROP_BUFFER_ADD_METHOD = "bufferAdd";
        private const string JSON_PROP_BUFFER_GET_METHOD = "bufferGet";

        public BufferLoader(ClassManager cManager) 
            : base(NodeType.Buffer, cManager)
        {}


        public override JObject classToJson(AbstractNodeClass nodeClass)
        {
            BufferNodeClass bufferClass = nodeClass as BufferNodeClass;

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

            AddBufferAdd(root, bufferClass.BufferAddMethod);
            AddBufferGet(root, bufferClass.BufferGetMethod);

            return root;
        }


        private void AddBufferAdd(JObject toAddTo, string methodName)
        {
            toAddTo.Add(JSON_PROP_BUFFER_ADD_METHOD, methodName);
        }

        private void AddBufferGet(JObject toAddTo, string methodName)
        {
            toAddTo.Add(JSON_PROP_BUFFER_ADD_METHOD, methodName);
        }


        public override void CreateCode(Node node, CodeExtension codeExtension)
        {
            BufferNodeClass nodeClass = node.Class as BufferNodeClass;
            codeExtension.AddNeededPermissions(nodeClass.NeedsPermissions);
            codeExtension.AddImport(nodeClass.MainClass);

            codeExtension.AddBuffer(node as BufferNode);

            //Add the proxy methods to the Imports:
            nodeClass.ProxyProperties
                .Select(p => p.PropertyType.Name)
                .ForEach(codeExtension.AddImport);
        }


        public override string GenerateClassFromSnippet(AbstractNodeClass nodeClass, string methodCode)
        {
            //TODO IMPLEMENT ME!
            return "";
        }


        public override AbstractNodeClass loadFromJson(string path, JObject root)
        {
            DataType genericData = new DataType("Generic");

            string name = ReadName(root);
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            string creator = ReadCreator(root).EmptyDefault("Unknown");
            DataType output = genericData;
            string mainClass = ReadMainClass(root);
            string[] needsOtherClasses = ReadNeededClasses(root);
            string[] neededPermissions = ReadNeededPermissions(root);
            ConfigElement[] config = ReadConfig(root);
            ProxyProperty[] proxyProperties = ReadProxyProperties(root);
            DataType[] inputTypes = ReadInputs(root).AddBefore(genericData);
            bool userCreated = ReadUserCreated(root);

            string bufferAddMethod = ReadBufferAdd(root);
            string bufferGetMethod = ReadBufferGet(root);

            return new BufferNodeClass(name, displayName, description, creator, output, mainClass,
                needsOtherClasses, neededPermissions, config, proxyProperties, inputTypes, userCreated, path,
                bufferAddMethod, bufferGetMethod);
        }


        /// <summary>
        /// Gets the Buffer add read.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>the value read</returns>
        private string ReadBufferAdd(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_BUFFER_ADD_METHOD, "");
        }

        /// <summary>
        /// Gets the Buffer get read.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>the value read</returns>
        private string ReadBufferGet(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_BUFFER_GET_METHOD, "");
        }

    }
}
