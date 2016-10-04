using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    abstract class AbstractNodeLoader
    {

        #region constants

        /// <summary>
        /// Name of the Name property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_NAME = "name";

        /// <summary>
        /// Name of the description property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_DESCRIPTION = "description";

        /// <summary>
        /// Name of the display name property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_DISPLAY = "display";

        /// <summary>
        /// Name of the person created this element.
        /// </summary>
        private const string JSON_PROP_CREATOR = "creator";

        /// <summary>
        /// Name of the input type(s) property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_INPUT = "input";

        /// <summary>
        /// Name of the output type property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_OUTPUT = "output";

        /// <summary>
        /// Name of the needed classes property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_NEEDED_CLASSES = "needs";

        /// <summary>
        /// Name of the Config property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_CONFIG = "config";

        /// <summary>
        /// Name of the Config Key property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_CONFIG_KEY = "key";

        /// <summary>
        /// Name of the Config Description property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_CONFIG_DESCRIPTION = "description";

        /// <summary>
        /// Name of the Config Type property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_CONFIG_CLASS_TYPE = "classType";


        /// <summary>
        /// Name of the mainClass property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_MAIN_CLASS = "mainClass";

        /// <summary>
        /// Name of the ProxyProperty root property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_PROXY_PROPERTIES = "proxyProperties";

        /// <summary>
        /// Name of the ProxyProperties type property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_PROXY_PROPERTIES_TYPE = "type";

        /// <summary>
        /// Name of the ProxyProperties name property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_PROXY_PROPERTIES_NAME = "name";

        /// <summary>
        /// Name of the ProxyProperties methodName property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_PROXY_PROPERTIES_METHOD = "method";

        /// <summary>
        /// The Name of the Path if the user created a node.
        /// </summary>
        private const string JSON_PROP_USER_CREATED = "userCreated";

        /// <summary>
        /// The Name of the Path for the Extras of a Node
        /// </summary>
        private const string JSON_PROP_EXTRA_PATH = "extras";

        #endregion constants

        #region Vars

        /// <summary>
        /// The Classmanager to cast Types.
        /// </summary>
        protected readonly ClassManager cManager;

        /// <summary>
        /// The Type this can load.
        /// </summary>
        protected readonly ClassManager.NodeType nodeType;

        #endregion Vars



        #region Constructor

        public AbstractNodeLoader(ClassManager.NodeType nodeType, ClassManager cManager)
        {
            this.cManager = cManager;
            this.nodeType = nodeType;
        }

        #endregion Constructor


        #region AbstractMethods

        /// <summary>
        /// The Implementation to load a Class from the JObject passed.
        /// </summary>
        /// <param name="path">The path this comes from</param>
        /// <param name="root">To read from.</param>
        /// <returns></returns>
        public abstract AbstractNodeClass loadFromJson(string path, JObject root);


        /// <summary>
        /// Serializes the Nodeclass to a Json respective for the Meta-Data.
        /// </summary>
        /// <param name="nodeClass">to generate for.</param>
        /// <returns>The generated root object</returns>
        public abstract JObject classToJson(AbstractNodeClass nodeClass);

        #endregion AbsstractMethods


        #region Methods


        /// <summary>
        /// Makes the pacakge name from the MainClass.
        /// </summary>
        /// <param name="mainclass">To get from.</param>
        /// <returns></returns>
        protected string GetPackageFromMainclass(string mainclass)
        {
            string[] split = mainclass.Split('.');
            if (split.Length == 1) return split[0];
            else return split
                    .Take(split.Count() - 1)
                    .ToArray()
                    .StringJoin(".");
        }


        /// <summary>
        /// This gets the Node type this loader can load.
        /// </summary>
        /// <returns>The type this loader can load.</returns>
        public ClassManager.NodeType getNodeType()
        {
            return nodeType;
        }

        /// <summary>
        /// Reads the Name of the Element from the root.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The name or an empty string if not found!</returns>
        protected string ReadName(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_NAME, "");
        }

        /// <summary>
        /// Reads the Inputs from the Json.
        /// </summary>
        /// <param name="root">to read from.</param>
        /// <returns>the inputs.</returns>
        protected DataType[] ReadInputs(JObject root)
        {
            return root
                .GetValueAsJObject(JSON_PROP_INPUT, new JObject())
                .ToStringString()
                .Values
                .Select(DataType.GetCachedType)
                .ToArray();
        }

        /// <summary>
        /// Reads the Display name from the Json.
        /// </summary>
        /// <param name="root">to read from.</param>
        /// <returns>the Display name.</returns>
        protected string ReadDisplayName(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_DISPLAY, "");
        }

        /// <summary>
        /// Reads the Description from the Json.
        /// </summary>
        /// <param name="root">to read from.</param>
        /// <returns>the Description.</returns>
        protected string ReadDescription(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_DESCRIPTION, "No description");
        }

        /// <summary>
        /// Reads the Creator from the Json.
        /// </summary>
        /// <param name="root">to read from.</param>
        /// <returns>the Creator.</returns>
        protected string ReadCreator(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_CREATOR, "Unknown");
        }

        /// <summary>
        /// Reads if this node is Created by a user.
        /// </summary>
        /// <param name="root">to read from</param>
        /// <returns>true if created by a user.</returns>
        protected bool ReadUserCreated(JObject root)
        {
            return root.GetValueAsBool(JSON_PROP_USER_CREATED, false);
        }

        /// <summary>
        /// Reads the Output type.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The output type.</returns>
        protected DataType ReadOutput(JObject root)
        {
            return DataType.GetCachedType(root.GetValueAsString(JSON_PROP_OUTPUT, ""));
        }

        /// <summary>
        /// Reads the needed other classes.
        /// </summary>
        /// <param name="root">to read from</param>
        /// <returns>The needed other classes.</returns>
        protected string[] ReadNeededClasses(JObject root)
        {
            return root.GetValueAsStringArray(JSON_PROP_NEEDED_CLASSES, new string[0]);
        }

        /// <summary>
        /// Reads the needed permissions.
        /// </summary>
        /// <param name="root">to read from</param>
        /// <returns>The needed Extras.</returns>
        protected NeedsExtra[] ReadExtras(JObject root)
        {
            return root
                .GetValueAsJArray(JSON_PROP_EXTRA_PATH, new JArray())
                .ToJObj()
                .Select(ExtrasFactory.read)
                .NonNull()
                .ToArray();
        }

        /// <summary>
        /// Reads the Java MainClass for this element.
        /// </summary>
        /// <param name="root">to read from</param>
        /// <returns>The Java MainClass</returns>
        protected string ReadMainClass(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_MAIN_CLASS, "");
        }

        /// <summary>
        /// Reads the configuration of this element from the Root.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The loaded Config</returns>
        protected ConfigElement[] ReadConfig(JObject root)
        {
            return root
                .GetValueAsJArray(JSON_PROP_CONFIG, new JArray())
                .ToJObj()
                .Select(o =>
                    {
                        string key = o.GetValueAsString(JSON_PROP_CONFIG_KEY);
                        string description = o.GetValueAsString(JSON_PROP_DESCRIPTION);
                        DataType type = DataType.GetCachedType(o.GetValueAsString(JSON_PROP_CONFIG_CLASS_TYPE));
                        return new ConfigElement(key, description, type);
                    }
                ).NonNull().ToArray();
        }

        /// <summary>
        /// Reads the Proxy-Properties from the root.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The ProxyProperties.</returns>
        protected ProxyProperty[] ReadProxyProperties(JObject root)
        {
            return root
                .GetValueAsJArray(JSON_PROP_PROXY_PROPERTIES, new JArray())
                .ToJObj()
                .Select(o =>
                    {
                        DataType type = DataType.GetCachedType(o.GetValueAsString(JSON_PROP_PROXY_PROPERTIES_TYPE));
                        string name = o.GetValueAsString(JSON_PROP_PROXY_PROPERTIES_NAME);
                        string methodName = o.GetValueAsString(JSON_PROP_PROXY_PROPERTIES_METHOD);
                        return new ProxyProperty(type, name, methodName);
                    }
                ).NonNull().ToArray();
        }




        /// <summary>
        /// Adds the own type to the Obj.
        /// </summary>
        /// <param name="toAddTo">ToAdd to.</param>
        protected void AddOwnType(JObject toAddTo)
        {
            toAddTo.Add("type", this.nodeType.ToString().ToLower());
        }

        /// <summary>
        /// Adds the Output to the Node passed.
        /// </summary>
        /// <param name="toAddTo">the Json Object to add to</param>
        /// <param name="type">To add</param>
        protected void AddOutput(JObject toAddTo, DataType type)
        {
            toAddTo.Add(JSON_PROP_OUTPUT, type.Name);
        }

        /// <summary>
        /// Adds the description passed to the Object.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="description">to add.</param>
        protected void AddDescription(JObject toAddTo, string description)
        {
            toAddTo.Add(JSON_PROP_DESCRIPTION, description);
        }


        /// <summary>
        /// Adds the Name passed to the Object.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="name">to add.</param>
        protected void AddName(JObject toAddTo, string name)
        {
            toAddTo.Add(JSON_PROP_NAME, name);
        }


        /// <summary>
        /// Adds the Display name passed to the Object.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="display">to add.</param>
        protected void AddDisplayName(JObject toAddTo, string display)
        {
            toAddTo.Add(JSON_PROP_DISPLAY, display);
        }

        /// <summary>
        /// Adds the Creator name passed to the Object.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="creator">to add.</param>
        protected void AddCreator(JObject toAddTo, string creator)
        {
            toAddTo.Add(JSON_PROP_CREATOR, creator);
        }

        /// <summary>
        /// Adds the inputs to the JObject.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="inputs">to add</param>
        protected void AddInputs(JObject toAddTo, DataType[] inputs)
        {
            if (inputs == null) inputs = new DataType[0];

            JObject inputObj = new JObject();
            for(int i = 0; i < inputs.Count(); i++)
            {
                inputObj.Add("arg" + i, inputs[i].Name);
            }

            toAddTo.Add(JSON_PROP_INPUT, inputObj);
        }

        /// <summary>
        /// Adds the Main class to the JObject passed.
        /// </summary>
        /// <param name="toAddTo">JObject to add to</param>
        /// <param name="mainClass">The Main class to add</param>
        protected void AddMainClass(JObject toAddTo, string mainClass)
        {
            toAddTo.Add(JSON_PROP_MAIN_CLASS, mainClass);
        }

        /// <summary>
        /// Adds if the node is user created.
        /// </summary>
        /// <param name="toAddTo">JObject to add to</param>
        /// <param name="userCreated">if the node is user created</param>
        protected void AddUserCreated(JObject toAddTo, bool userCreated)
        {
            toAddTo.Add(JSON_PROP_USER_CREATED, userCreated);
        }

        /// <summary>
        /// Adds the Extras to the JObject passed.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="permissions">The permissions to add</param>
        protected void AddExtras(JObject toAddTo, NeedsExtra[] extras)
        {
            toAddTo.Add(JSON_PROP_EXTRA_PATH, extras
                .Select(ExtrasFactory.serialize)
                .ToJArray()
            );
        }

        /// <summary>
        /// Adds the needed Classes to the JObject passed.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="permissions">The needed classes to add</param>
        protected void AddNeededClasses(JObject toAddTo, string[] neededClasses)
        {
            toAddTo.Add(JSON_PROP_NEEDED_CLASSES, neededClasses.ToJArray());
        }

        /// <summary>
        /// Adds the config to the JObject passed.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="config">to add</param>
        protected void AddConfig(JObject toAddTo, ConfigElement[] config)
        {
            toAddTo.Add(JSON_PROP_CONFIG,
                config.Select(c =>
                {
                    JObject obj = new JObject();
                    obj.Add(JSON_PROP_CONFIG_KEY, c.Key);
                    obj.Add(JSON_PROP_CONFIG_DESCRIPTION, c.Description);
                    obj.Add(JSON_PROP_CONFIG_CLASS_TYPE, c.DataType.Name);

                    return obj;
                }).NonNull().ToJArray()
            );
        }


        /// <summary>
        /// Adds the ProxyProperties to the JObject passed.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="proxyProperties">to add</param>
        protected void AddProxyProperties(JObject toAddTo, ProxyProperty[] proxyProperties)
        {
            toAddTo.Add(JSON_PROP_PROXY_PROPERTIES,
                proxyProperties.Select(c =>
                {
                    JObject obj = new JObject();
                    obj.Add(JSON_PROP_PROXY_PROPERTIES_METHOD, c.MethodName);
                    obj.Add(JSON_PROP_PROXY_PROPERTIES_NAME, c.Name);
                    obj.Add(JSON_PROP_PROXY_PROPERTIES_TYPE, c.PropertyType.Name);

                    return obj;
                }).NonNull().ToJArray()
            );
        }



        /// <summary>
        /// Generates a new Class from a snippet.
        /// </summary>
        /// <param name="methodCode">to use.</param>
        /// <param name="nodeClass">to use.</param>
        /// <returns>the generated Java class</returns>
        public abstract string GenerateClassFromSnippet(AbstractNodeClass nodeClass, string methodCode);


        /// <summary>
        /// Creates the Code for the Node and stores it in the code-Extension.
        /// </summary>
        /// <param name="node">To use</param>
        /// <param name="codeExtension">To use.</param>
        public abstract void CreateCode(Node node, CodeExtension codeExtension);


        #endregion Methods
    }
}