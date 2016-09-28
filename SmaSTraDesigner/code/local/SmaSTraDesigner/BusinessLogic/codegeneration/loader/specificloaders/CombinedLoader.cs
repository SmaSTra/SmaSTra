using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.Linq;
using Common;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class CombinedLoader : AbstractNodeLoader
    {

        #region consts

        /// <summary>
        /// Name of the connections if this is a Combined node.
        /// </summary>
        private const string JSON_PROP_CONNECTIONS = "connections";

        /// <summary>
        /// Name of the path to the sub-Elements if this is a CombinedNode.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS = "subElements";

        /// <summary>
        /// Name of the path for the Output Node in a Combined Node.
        /// </summary>
        private const string JSON_PROP_OUTPUT_NODE_NAME = "outputNodeName";

        /// <summary>
        /// The uuid of the first node of the connection.
        /// </summary>
        private const string JSON_PROP_CONNECTION_FIRST_NODE = "firstNode";

        /// <summary>
        /// The uuid of the second node of the connection.
        /// </summary>
        private const string JSON_PROP_CONNECTION_SECOND_NODE = "secondNode";

        /// <summary>
        /// The index of the connection on the input.
        /// </summary>
        private const string JSON_PROP_CONNECTION_INDEX_NODE = "position";


        /// <summary>
        /// The path to the name Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_NAME = "name";

        /// <summary>
        /// The path to the uuid Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_UUID = "uuid";

        /// <summary>
        /// The path to the type Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_TYPE = "type";

        /// <summary>
        /// The path to the posx Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_POSX = "posx";

        /// <summary>
        /// The path to the posy Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_POSY = "posy";

        /// <summary>
        /// The path to the Input-Data Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_INPUT_DATA = "inputData";


        /// <summary>
        /// The path to the Input-Data Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_INPUT_DATA_TYPE = "type";

        /// <summary>
        /// The path to the Input-Data Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_INPUT_DATA_VALUE = "value";

        /// <summary>
        /// The path to the Config-Key Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_CONFIG = "Configuration";

        /// <summary>
        /// The path to the Config-Key Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_CONFIG_KEY = "key";

        /// <summary>
        /// The path to the Config-Data Property of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_CONFIG_VALUE = "value";


        #endregion consts


        public CombinedLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Combined, cManager)
        { }


        public override AbstractNodeClass loadFromJson(string path, JObject root)
        {
            string name = ReadName(root);
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            string creator = ReadCreator(root).EmptyDefault("Unknown");
            DataType output = ReadOutput(root);
            DataType[] inputs = ReadInputs(root);
            List<SimpleSubNode> subNodes = ReadSubNodes(root);
            List<SimpleConnection> connection = ReadConnections(root);
            string outputNodeID = ReadOutputNodeID(root);
            bool userCreated = ReadUserCreated(root);

            return new CombinedNodeClass(name, name, description, creator, subNodes, connection, output, outputNodeID, userCreated, path, inputs);
        }

        public override JObject classToJson(AbstractNodeClass nodeClass)
        {
            CombinedNodeClass combinedClass = nodeClass as CombinedNodeClass;
            if(combinedClass == null)
            {
                return null;
            }

            JObject root = new JObject();
            AddOwnType(root);
            AddName(root, nodeClass.Name);
            AddDescription(root, nodeClass.Description);
            AddDisplayName(root, nodeClass.DisplayName);
            AddOutput(root, nodeClass.OutputType);
            AddInputs(root, nodeClass.InputTypes);
            AddProxyProperties(root, combinedClass.ProxyProperties);
            AddUserCreated(root, nodeClass.UserCreated);
            AddCreator(root, nodeClass.Creator);

            AddConnections(root, combinedClass.Connections);
            AddSubNodes(root, combinedClass.SubElements);
            AddOutputNodeID(root, combinedClass.OutputNodeUUID);

            return root;
        }

        private List<SimpleSubNode> ReadSubNodes(JObject root)
        {
            ClassManager cManager = Singleton<ClassManager>.Instance;
            return root
                .GetValueAsJArray(JSON_PROP_SUB_ELEMENTS, new JArray())
                .ToJObj().NonNull()
                .Select(o =>
                {
                    string name = o.GetValueAsString(JSON_PROP_SUB_ELEMENTS_NAME, "");
                    string uuid = o.GetValueAsString(JSON_PROP_SUB_ELEMENTS_UUID, "");
                    string type = o.GetValueAsString(JSON_PROP_SUB_ELEMENTS_TYPE, "");
                    double posx = o.GetValueAsDouble(JSON_PROP_SUB_ELEMENTS_POSX, 0);
                    double posy = o.GetValueAsDouble(JSON_PROP_SUB_ELEMENTS_POSY, 0);
                    IOData[] data = o.GetValueAsJArray(JSON_PROP_SUB_ELEMENTS_INPUT_DATA, new JArray())
                        .ToJObj()
                        .Select(o2 =>
                        {
                            string key = o2.GetValueAsString(JSON_PROP_SUB_ELEMENTS_INPUT_DATA_TYPE, "");
                            string value = o2.GetValueAsString(JSON_PROP_SUB_ELEMENTS_INPUT_DATA_VALUE, "");
                            return new IOData(DataType.GetCachedType(key), value);
                        }).ToArray();

                    DataConfigElement[] config = o.GetValueAsJArray(JSON_PROP_SUB_ELEMENTS_CONFIG, new JArray())
                        .ToJObj()
                        .Select(o2 =>
                        {
                            string key = o2.GetValueAsString(JSON_PROP_SUB_ELEMENTS_CONFIG_KEY, "");
                            string value = o2.GetValueAsString(JSON_PROP_SUB_ELEMENTS_CONFIG_VALUE, "");
                            return new DataConfigElement(key, "", DataType.GetCachedOrNonRegisteredType("EMPTY"), value);
                        }).ToArray();


                    return new SimpleSubNode(name, uuid, type, posx, posy, data, config);
                })
                .ToList();
        }

        private List<SimpleConnection> ReadConnections(JObject root)
        {
            return root
                .GetValueAsJArray(JSON_PROP_CONNECTIONS)
                .Collect(ReadConnection)
                .NonNull()
                .ToList();
        }

        private SimpleConnection ReadConnection(JToken tObj)
        {
            JObject obj = tObj as JObject;
            if (obj == null) return null;

            string firstNode = obj.GetValueAsString(JSON_PROP_CONNECTION_FIRST_NODE);
            string secondNode = obj.GetValueAsString(JSON_PROP_CONNECTION_SECOND_NODE);
            int position = obj.GetValueAsInt(JSON_PROP_CONNECTION_INDEX_NODE);
            return new SimpleConnection(firstNode, secondNode, position);
        }

        /// <summary>
        /// Reads the Output node ID.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The output node ID</returns>
        private string ReadOutputNodeID(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_OUTPUT_NODE_NAME, "");
        }


        private void AddSubNodes(JObject root, List<SimpleSubNode> subElements)
        {
            root.Add(JSON_PROP_SUB_ELEMENTS,
                subElements.Select(s =>
                {
                    JObject obj = new JObject();
                    obj.Add(JSON_PROP_SUB_ELEMENTS_NAME, s.Name);
                    obj.Add(JSON_PROP_SUB_ELEMENTS_UUID, s.Uuid);
                    obj.Add(JSON_PROP_SUB_ELEMENTS_TYPE, s.Type);
                    obj.Add(JSON_PROP_SUB_ELEMENTS_POSX, s.PosX);
                    obj.Add(JSON_PROP_SUB_ELEMENTS_POSY, s.PosY);
                    obj.Add(JSON_PROP_SUB_ELEMENTS_INPUT_DATA,
                            s.Data.Select(d =>
                            {
                                JObject obj2 = new JObject();
                                obj2.Add(JSON_PROP_SUB_ELEMENTS_INPUT_DATA_TYPE, d.Type.Name);
                                obj2.Add(JSON_PROP_SUB_ELEMENTS_INPUT_DATA_VALUE, d.Value);

                                return obj2;
                            }).ToJArray());
                    obj.Add(JSON_PROP_SUB_ELEMENTS_CONFIG,
                            s.Configuration.Select(d =>
                            {
                                JObject obj2 = new JObject();
                                obj2.Add(JSON_PROP_SUB_ELEMENTS_CONFIG_KEY, d.Key);
                                obj2.Add(JSON_PROP_SUB_ELEMENTS_CONFIG_VALUE, d.Value);

                                return obj2;
                            }).ToJArray());
                    return obj;
                }
            ).ToJArray());
        }

        private void AddConnections(JObject root, List<SimpleConnection> connections)
        {
            root.Add(JSON_PROP_CONNECTIONS,
                connections.Select(c =>
                {
                    JObject obj = new JObject();
                    obj.Add(JSON_PROP_CONNECTION_FIRST_NODE, c.firstNode);
                    obj.Add(JSON_PROP_CONNECTION_SECOND_NODE, c.secondNode);
                    obj.Add(JSON_PROP_CONNECTION_INDEX_NODE, c.position);
                    return obj;
                }).ToJArray()
            );
        }

        private void AddOutputNodeID(JObject root, string outputNodeUUID)
        {
            root.Add(JSON_PROP_OUTPUT_NODE_NAME, outputNodeUUID);
        }

        
        public override string GenerateClassFromSnippet(AbstractNodeClass nodeClass, string methodCode)
        {
            //Combined node have nothing to generate on classes.
            return null;
        }


        public override void CreateCode(Node node, CodeExtension codeExtension)
        {
            //Nothing to generate!
        }

    }
}
