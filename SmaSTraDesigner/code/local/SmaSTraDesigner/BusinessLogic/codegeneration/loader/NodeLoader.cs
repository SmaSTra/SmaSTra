using Common;
using Common.ExtensionMethods;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.nodes;
using static SmaSTraDesigner.BusinessLogic.classhandler.ClassManager;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class NodeLoader
    {

        #region Const

        /// <summary>
        /// Name of the node type (sensor/transformation) property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_TYPE = "type";

        /// <summary>
        /// Filename for metadata.
        /// </summary>
        private const string METADATA_FILENAME = "metadata.json";

        /// <summary>
        /// The Folder for created stuff
        /// </summary>
        private const string CREATED_PATH = "created";


        /// <summary>
        /// Possible value for node type.
        /// </summary>
        private const string NODE_TYPE_SENSOR = "sensor";

        /// <summary>
        /// Possible value for node type.
        /// </summary>
        private const string NODE_TYPE_TRANSFORMATION = "transformation";

        /// <summary>
        /// Possible value for node type.
        /// </summary>
        private const string NODE_TYPE_COMBINED = "combined";

        /// <summary>
        /// Possible value for node type.
        /// </summary>
        private const string NODE_TYPE_BUFFER = "buffer";

        #endregion Const

        #region static methods

        /// <summary>
        /// Determins and verifies node type read from metadata.
        /// </summary>
        /// <param name="type">Node type as string.</param>
        /// <returns>Verified node type</returns>
        private static NodeType GetNodeType(string type)
        {
            switch (type)
            {
                case NODE_TYPE_TRANSFORMATION:
                    return NodeType.Transformation;

                case NODE_TYPE_SENSOR:
                    return NodeType.Sensor;

                case NODE_TYPE_COMBINED:
                    return NodeType.Combined;

                case NODE_TYPE_BUFFER:
                    return NodeType.Buffer;

                default:
                    throw new Exception(String.Format("Unrecognized node type \"{0}\".", type));
            }
        }

        #endregion static methods


        #region Vars

        private readonly Dictionary<NodeType, AbstractNodeLoader> loaders = new Dictionary<ClassManager.NodeType, AbstractNodeLoader>();

        #endregion Vars

        #region Constructor

        public NodeLoader()
        {
            ClassManager cManager = Singleton<ClassManager>.Instance;
            addLoader(new TransformationLoader(cManager));
            addLoader(new SensorLoader(cManager));
            addLoader(new CombinedLoader(cManager));
            addLoader(new BufferLoader(cManager));
        }

        private void addLoader(AbstractNodeLoader loader)
        {
            this.loaders.Add(loader.getNodeType(), loader);
        }


        #endregion Constructor

        /// <summary>
        /// Loads the Class from a Path.
        /// <br>This path is ment to be the parent folder of the metadata file.
        /// <br>Many different exceptions may be thrown here:
        /// <br>- FileNotFoundException: No metadata file could be found.
        /// <br>- FileNotParseableException: The metadata file is not parseable by the Json parser.
        /// <br>- MissingTypeException: The 'type' tag is not found in the metadata file.
        /// <br>- MissingTypeException: The 'type' is not recognized.
        /// </summary>
        /// <param name="path">To load</param>
        /// <returns>The loaded Class or an exception</returns>
        public AbstractNodeClass loadFromFolder(string path)
        {
            if(path == null)
            {
                throw new ArgumentException("Path may not be null!");
            }

            //Read the Metadata:
            string metaDataPath = Path.Combine(path, METADATA_FILENAME);
            if (!File.Exists(metaDataPath))
            {
                throw new FileNotFoundException("Could not find Metadata in folder: " + path);
            }

            JObject root = JObject.Parse(File.ReadAllText(metaDataPath));

            //No root => Nothing to do!
            if(root == null)
            {
                throw new FileNotParseableException("Could not parse file: " + metaDataPath);
            }

            string typeName = root.GetValueAsString(JSON_PROP_TYPE);
            if (String.IsNullOrEmpty(typeName))
            {
                throw new MissingTypeException("Could not find the type element in: " + metaDataPath);
            }

            NodeType type = GetNodeType(typeName);
            AbstractNodeLoader loader = loaders[type];
            if(loader == null)
            {
                throw new MissingTypeException("Did not recognize Type: " + type);
            }

            return loader.loadFromJson(path, root);
        }


        /// <summary>
        /// Creates the Java source-Code for this element and stores it in the CodeExtension.
        /// </summary>
        /// <param name="node">To Generate for</param>
        /// <param name="codeExtension">To store in</param>
        public void CreateCode(Node node, CodeExtension codeExtension)
        {
            loaders
                .ExecuteOnFirst(
                e => e.Key == node.Class.NodeType, 
                l => l.Value.CreateCode(node, codeExtension)
            );
        }


        /// <summary>
        /// Saves the Node passed to the path passed.
        /// The passed path is taken AS IS. No fiddeling with names afterwards.
        /// </summary>
        /// <param name="nodeClass">To Save</param>
        /// <param name="path">To save to</param>
        /// <param name="methodCode">To save, can be null.</param>
        public void saveToFolder(AbstractNodeClass nodeClass, string path, string methodCode = null)
        {
            if(path == null)
            {
                throw new ArgumentException("Path may not be null!");
            }

            if(nodeClass == null)
            {
                throw new ArgumentException("NodeClass may not be null!");
            }

            string metadataPath = Path.Combine(path, METADATA_FILENAME);
            string newClassPath = Path.Combine(path, Path.Combine(nodeClass.MainClass.Split('.').ToArray()).RemoveAll(" ", "_") + ".java");
            if (Directory.Exists(path))
            {
                throw new FileAlreadyPresentException("Folder: " + path + " already exists.");
            }

            AbstractNodeLoader loader = loaders[nodeClass.NodeType];
            if (loader == null)
            {
                throw new MissingTypeException("Did not recognize Type for serialization: " + nodeClass.NodeType);
            }

            JObject metadata = loader.classToJson(nodeClass);
            string generatedClass = methodCode == null ? null : loader.GenerateClassFromSnippet(nodeClass, methodCode);
            if(metadata == null)
            {
                throw new Exception("Ehhh, Something gone wrong, doc!");
            }

            //Write the Metadata + New class if exists:
            Directory.CreateDirectory(path);
            File.WriteAllText(metadataPath, metadata.ToBeautifulJson());
            if (generatedClass != null)
            {
                Directory.CreateDirectory(Path.Combine(path, newClassPath));
                Directory.Delete(newClassPath);
                File.WriteAllText(newClassPath, generatedClass);
            }

            //TODO add other stuff to add, like dependencies, etc.
        }

    }


    class FileNotParseableException : Exception
    {

        public FileNotParseableException(string reason)
            :base(reason)
        {}

    }

    class FileAlreadyPresentException : Exception
    {

        public FileAlreadyPresentException(string reason)
            : base(reason)
        { }

    }


    class MissingTypeException : Exception
    {

        public MissingTypeException(string reason)
            : base(reason)
        { }

    }
}
