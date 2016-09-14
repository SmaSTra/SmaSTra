using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion AbstractMethods


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


        #endregion Methods
    }
}