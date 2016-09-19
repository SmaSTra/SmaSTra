namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public class ConfigElement
    {

        /// <summary>
        /// The Key of the Config element.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The Description of the Config Element.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The DataType wanted in this Element.
        /// </summary>
        public DataType DataType { get; }

        public ConfigElement(string key, string description, DataType type)
        {
            this.Key = key;
            this.Description = description;
            this.DataType = type;
        }

        /// <summary>
        /// Generates a Data Element from the own Config.
        /// </summary>
        /// <returns>The generated element</returns>
        public DataConfigElement GenerateDataElement()
        {
            return new DataConfigElement(Key, Description, DataType);
        }

    }

    public class DataConfigElement : ConfigElement
    {

        /// <summary>
        /// The Data for the configuration to set.
        /// </summary>
        public string Value { get; set; }

        public DataConfigElement(string key, string description, DataType type) 
            : base(key, description, type)
        {
            this.Value = "";
        }
    }
}
