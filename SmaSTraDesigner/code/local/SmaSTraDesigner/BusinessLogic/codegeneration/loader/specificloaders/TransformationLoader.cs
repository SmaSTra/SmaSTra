using Common.ExtensionMethods;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class TransformationLoader : AbstractNodeLoader
    {

        /// <summary>
        /// This is the Path for the Method Name to call.
        /// </summary>
        private const string JSON_PROP_METHOD_NAME = "method";

        /// <summary>
        /// This is the path to identify if the method is static.
        /// </summary>
        private const string JSON_PROP_STATIC = "static";



        public TransformationLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Transformation, cManager)
        {}

        public override AbstractNodeClass loadFromJson(string name, JObject root)
        {
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            DataType output = ReadOutput(root);
            string mainClass = ReadMainClass(root);
            string[] needsOtherClasses = ReadNeededClasses(root);
            string[] neededPermissions = ReadNeededPermissions(root);
            ConfigElement[] config = ReadConfig(root);
            string methodName = ReadMethodName(root);
            bool isStatic = ReadIsStatic(root);
            DataType[] inputs = ReadInputs(root);

            return new TransformationNodeClass(name, displayName, description, output, inputs, mainClass, needsOtherClasses, neededPermissions, config, methodName, isStatic);
        }

        /// <summary>
        /// Reads the name of the Method to call from the Json.
        /// </summary>
        /// <param name="toReadFrom">JObject to read from</param>
        /// <returns>The method to call</returns>
        protected string ReadMethodName(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsString(JSON_PROP_METHOD_NAME, "");
        }

        /// <summary>
        /// Reads if the Method to call is static or not.
        /// </summary>
        /// <param name="toReadFrom">The JObject to look for</param>
        /// <returns>if the method is static</returns>
        protected bool ReadIsStatic(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsBool(JSON_PROP_STATIC, true);
        }



        public override JObject classToJson(AbstractNodeClass nodeClass)
        {
            TransformationNodeClass transClass = nodeClass as TransformationNodeClass;

            JObject root = new JObject();
            AddOwnType(root);
            AddDescription(root, nodeClass.Description);
            AddDisplayName(root, nodeClass.DisplayName);
            AddOutput(root, nodeClass.OutputType);
            AddMainClass(root, nodeClass.MainClass);
            AddNeededClasses(root, nodeClass.NeedsOtherClasses);
            AddPermissions(root, nodeClass.NeedsPermissions);
            AddConfig(root, nodeClass.Configuration);

            AddMethodName(root, transClass.Method);
            AddStatic(root, transClass.IsStatic);

            return root;
        }


        /// <summary>
        /// Adds the method to call to the Json.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="methodName">The Methodname to add</param>
        protected void AddMethodName(JObject toAddTo, string methodName)
        {
            toAddTo.Add(JSON_PROP_METHOD_NAME, methodName);
        }

        /// <summary>
        /// Adds if the method is static to the Json
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="isStatic">If the method is static</param>
        protected void AddStatic(JObject toAddTo, bool isStatic)
        {
            toAddTo.Add(JSON_PROP_STATIC, isStatic);
        }


        public override string GenerateClassFromSnippet(AbstractNodeClass nodeClass, string methodCode)
        {
            string methodArgs = "";
            for (int i = 0 ; i < nodeClass.InputTypes.Count(); i++) 
            {
                if (i > 0) methodArgs += ", ";
                methodArgs += nodeClass.InputTypes[i] + " arg"+i;
            }

            return string.Format(ClassTemplates.TRANSFORMATION_TEMPLATE,
                nodeClass.Name, nodeClass.OutputType.Name.RemoveAll(" ", "_"), methodArgs, methodCode);
        }

    }
}
