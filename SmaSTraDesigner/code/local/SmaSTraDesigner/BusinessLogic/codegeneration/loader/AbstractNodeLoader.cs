using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    abstract class AbstractNodeLoader
    {

        #region constants

        /// <summary>
        /// Name of the description property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_DESCRIPTION = "description";

        /// <summary>
        /// Name of the display name property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_DISPLAY = "display";

        /// <summary>
        /// Name of the input type(s) property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_INPUT = "input";

        /// <summary>
        /// Name of the output type property field in JSON metadata.
        /// </summary>
        private const string JSON_PROP_OUTPUT = "output";

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
        /// <param name="root">To read from.</param>
        /// <returns></returns>
        public abstract NodeClass loadFromJson(string name, JObject root);


        /// <summary>
        /// Serializes the Nodeclass to a Json respective for the Meta-Data.
        /// </summary>
        /// <param name="nodeClass">to generate for.</param>
        /// <returns>The generated root object</returns>
        public abstract JObject classToJson(NodeClass nodeClass);

        #endregion AbsstractMethods


        #region Methods

        /// <summary>
        /// This gets the Node type this loader can load.
        /// </summary>
        /// <returns>The type this loader can load.</returns>
        public ClassManager.NodeType getNodeType()
        {
            return nodeType;
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
                .Select(cManager.AddDataType)
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
        /// Reads the Output type.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The output type.</returns>
        protected DataType ReadOutput(JObject root)
        {
            return cManager.AddDataType(root.GetValueAsString(JSON_PROP_OUTPUT, ""));
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
        /// Adds the Display name passed to the Object.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="display">to add.</param>
        protected void AddDisplayName(JObject toAddTo, string display)
        {
            toAddTo.Add(JSON_PROP_DISPLAY, display);
        }

        /// <summary>
        /// Adds the inputs to the JObject.
        /// </summary>
        /// <param name="toAddTo">The JObject to add to</param>
        /// <param name="inputs">to add</param>
        public void AddInputs(JObject toAddTo, DataType[] inputs)
        {
            if (inputs == null) inputs = new DataType[0];

            JObject inputObj = new JObject();
            for(int i = 0; i < inputs.Count(); i++)
            {
                inputObj.Add("arg" + i, inputs[i].Name);
            }

            toAddTo.Add(JSON_PROP_INPUT, inputObj);
        }


        #endregion Methods
    }
}