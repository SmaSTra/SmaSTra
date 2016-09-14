using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.IO;
using static SmaSTraDesigner.BusinessLogic.ClassManager;

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

                default:
                    throw new Exception(String.Format("Unrecognized node type \"{0}\".", type));
            }
        }

        #endregion static methods


        #region Vars

        private readonly Dictionary<NodeType, AbstractNodeLoader> loaders = new Dictionary<ClassManager.NodeType, AbstractNodeLoader>();

        #endregion Vars

        #region Constructor

        public NodeLoader(ClassManager cManager)
        {
            addLoader(new TransformationLoader(cManager));
            addLoader(new SensorLoader(cManager));
            addLoader(new CombinedLoader(cManager));
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
        public NodeClass loadFromFolder(string path)
        {
            if(path == null)
            {
                throw new ArgumentException("Path may not be null!");
            }


            //Read the Name:
            string name = Path.GetFileName(path);

            //Read the Metadata:
            path = Path.Combine(path, METADATA_FILENAME);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find folder: " + path);
            }

            JObject root = JObject.Parse(File.ReadAllText(path));

            //No root => Nothing to do!
            if(root == null)
            {
                throw new FileNotParseableException("Could not parse file: " + path);
            }

            string typeName = root.GetValueAsString(JSON_PROP_TYPE);
            if (String.IsNullOrEmpty(typeName))
            {
                throw new MissingTypeException("Could not find the type element in: " + path);
            }

            NodeType type = GetNodeType(typeName);
            AbstractNodeLoader loader = loaders[type];
            if(loader == null)
            {
                throw new MissingTypeException("Did not recognize Type: " + type);
            }

            return loader.loadFromJson(name, root);
        }
    }


    class FileNotParseableException : Exception
    {

        public FileNotParseableException(string reason)
            :base(reason)
        {}

    }


    class MissingTypeException : Exception
    {

        public MissingTypeException(string reason)
            : base(reason)
        { }

    }
}
