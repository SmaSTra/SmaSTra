using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders
{
    class TransformationLoader : AbstractNodeLoader
    {

        /// <summary>
        /// This is the Path for the Method Name to call.
        /// </summary>
        private const string JsonPropMethodName = "method";

        /// <summary>
        /// This is the path to identify if the method is static.
        /// </summary>
        private const string JsonPropStatic = "static";



        public TransformationLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Transformation, cManager)
        {}

        public override AbstractNodeClass loadFromJson(string path, JObject root)
        {
            var name = ReadName(root);
            var displayName = ReadDisplayName(root).EmptyDefault(name);
            var description = ReadDescription(root).EmptyDefault("No Description");
            var creator = ReadCreator(root).EmptyDefault("Unknown");
            var output = ReadOutput(root);
            var mainClass = ReadMainClass(root);
            var needsOtherClasses = ReadNeededClasses(root);
            var needsExtras = ReadExtras(root);
            var config = ReadConfig(root);
            var proxyProperties = ReadProxyProperties(root);
            var methodName = ReadMethodName(root);
            var isStatic = ReadIsStatic(root);
            var inputs = ReadInputs(root);
            var userCreated = ReadUserCreated(root);

            return new TransformationNodeClass(name, displayName, description, creator, output, inputs, mainClass, 
                needsOtherClasses, needsExtras, config, proxyProperties, userCreated, path,
                methodName, isStatic);
        }

        /// <summary>
        /// Reads the name of the Method to call from the Json.
        /// </summary>
        /// <param name="toReadFrom">JObject to read from</param>
        /// <returns>The method to call</returns>
        protected string ReadMethodName(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsString(JsonPropMethodName, "");
        }

        /// <summary>
        /// Reads if the Method to call is static or not.
        /// </summary>
        /// <param name="toReadFrom">The JObject to look for</param>
        /// <returns>if the method is static</returns>
        protected bool ReadIsStatic(JObject toReadFrom)
        {
            return toReadFrom.GetValueAsBool(JsonPropStatic, true);
        }



        public override JObject classToJson(AbstractNodeClass nodeClass)
        {
            var transClass = nodeClass as TransformationNodeClass;

            var root = new JObject();
            AddOwnType(root);
            AddName(root, nodeClass.Name);
            AddDescription(root, nodeClass.Description);
            AddDisplayName(root, nodeClass.DisplayName);
            AddCreator(root, nodeClass.Creator);
            AddOutput(root, nodeClass.OutputType);
            AddMainClass(root, nodeClass.MainClass);
            AddNeededClasses(root, nodeClass.NeedsOtherClasses);
            AddExtras(root, nodeClass.NeededExtras);
            AddConfig(root, nodeClass.Configuration);
            AddInputs(root, nodeClass.InputTypes);
            AddProxyProperties(root, nodeClass.ProxyProperties);
            AddUserCreated(root, nodeClass.UserCreated);

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
            toAddTo.Add(JsonPropMethodName, methodName);
        }

        /// <summary>
        /// Adds if the method is static to the Json
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="isStatic">If the method is static</param>
        protected void AddStatic(JObject toAddTo, bool isStatic)
        {
            toAddTo.Add(JsonPropStatic, isStatic);
        }


        public override string GenerateClassFromSnippet(AbstractNodeClass nodeClass, string methodCode)
        {
            var package = GetPackageFromMainclass(nodeClass.MainClass);
            var imports = nodeClass.InputTypes
                .ToArray()
                .Concat(new[] { nodeClass.OutputType })
                .Distinct()
                .Select(i => i.MinimizedName)
                .Where(i => !CodeExtension.importBlacklist.Contains(i))
                .Select(i => "import " + i + ";")
                .StringJoin("\n");
                

            var methodArgs = "";
            for (var i = 0 ; i < nodeClass.InputTypes.Count(); i++) 
            {
                if (i > 0) methodArgs += ", ";
                methodArgs += nodeClass.InputTypes[i].MinimizedName + " arg"+i;
            }

            return string.Format(ClassTemplates.TRANSFORMATION_TEMPLATE,
                package,
                imports,
                nodeClass.Name, 
                nodeClass.OutputType.MinimizedName, 
                methodArgs, 
                methodCode);
        }



        public override void CreateCode(Node node, CodeExtension codeExtension)
        {
            var nodeClass = node.Class as TransformationNodeClass;
            codeExtension.AddTransformation(node as Transformation);
            codeExtension.AddExtras(nodeClass.NeededExtras);

            var content = "";
            var args = "(";
            var currentTransform = codeExtension.GetCurrentStep();

            for(var i = 0; i < nodeClass.InputTypes.Length; i++)
            {
                var inputType = nodeClass.InputTypes[i];
                var inputNode = node.InputNodes[i];
                var ioData = node.InputIOData[i];

                //If we have a combined node, we have to proxy it's output node:
                if (inputNode is CombinedNode) inputNode = ((CombinedNode) inputNode).outputNode;

                //Add the args:
                if (i != 0) args += ", ";
                args += "data" + i;

                if(inputNode == null)
                {
                    //We have a default value in the GUI:
                    content += string.Format(ClassTemplates.GENERATION_TEMPLATE_TRANSFORM_VAR_LINE_STATIC,
                        inputType.MinimizedName,
                        i,
                        ioData.Value);
                }
                else
                {
                    var inAsSource = inputNode as DataSource;
                    var inAsTransform = inputNode as Transformation;
                    var inAsBuffer = inputNode as BufferNode;

                    var inAsSourceClass = inAsSource?.Class as DataSourceNodeClass;
                    var inAsTransClass = inAsTransform?.Class as TransformationNodeClass;
                    var inAsBufferClass = inAsBuffer?.Class as BufferNodeClass;


                    var typeName = "";
                    var optionalMethodCall = "";

                    switch (inputNode.Class.NodeType)
                    {
                        case ClassManager.NodeType.Transformation:
                            typeName = "transform";
                            break;

                        case ClassManager.NodeType.Sensor:
                            typeName = "sensor";
                            optionalMethodCall = "." + inAsSourceClass.DataMethod + "()";
                            break;

                        case ClassManager.NodeType.Buffer:
                            typeName = "buffer";
                            optionalMethodCall = "." + inAsBufferClass.BufferGetMethod + "()";
                            break;

                        case ClassManager.NodeType.Combined:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }



                    var outputIndex = codeExtension.GetOutputIndex(inputNode);

                    content += string.Format(ClassTemplates.GENERATION_TEMPLATE_TRANSFORM_VAR_LINE,
                        inputType.MinimizedName,
                        i,
                        typeName,
                        outputIndex,
                        optionalMethodCall);
                }

            }

            args += ")";

            //Extract the method call:
            var methodCall = (nodeClass.IsStatic
                ? (DataType.MinimizeToClass(nodeClass.MainClass))
                : (nodeClass.NodeType.ToString() + codeExtension.GetTransformId(node as Transformation)))
                + "." + nodeClass.Method + args;

            var template = codeExtension.RootNode == node 
                ? ClassTemplates.GENERATION_TEMPLATE_TRANSFORM_LAST 
                : ClassTemplates.GENERATION_TEMPLATE_TRANSFORM;

            var code = string.Format( 
                template,
                currentTransform,
                content,
                methodCall
                );

            //At last add the important stuff:
            codeExtension.AddCodeStep(code);
            codeExtension.AddImport(nodeClass.MainClass);
            codeExtension.AddImport(nodeClass.OutputType.Name);

            //Add the proxy methods to the Imports:
            nodeClass.ProxyProperties
                .Select(p => p.PropertyType.Name)
                .ForEach(codeExtension.AddImport);
        }

    }
}
