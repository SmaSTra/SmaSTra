namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    class DataSourceNodeClass : AbstractNodeClass
    {

        /// <summary>
        /// The method to get the data.
        /// </summary>
        public string DataMethod { get; }

        /// <summary>
        /// The Method to start the Sensor / Datasource.
        /// </summary>
        public string StartMethod { get; }

        /// <summary>
        /// The Method to stop the Sensor / Datasource.
        /// </summary>
        public string StopMethod { get; }

        public DataSourceNodeClass(string name, string displayName, string description, DataType outputType,
            string mainClass, string[] needsOtherClasses, string[] needsPermissions, ConfigElement[] config, ProxyProperty[] proxyProperties,
            string dataMethod, string startMethod, string stopMethod)
            : base(ClassManager.NodeType.Sensor, name, displayName, description, outputType, 
                  mainClass, needsOtherClasses, needsPermissions,
                  config, proxyProperties, new DataType[0])
        {
            this.DataMethod = dataMethod;
            this.StartMethod = startMethod;
            this.StopMethod = stopMethod;
        }


        protected override Node generateBaseNode()
        {
            return new DataSource()
            {
                Name = this.DisplayName
            };
        }

    }
}
