using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public class DataSourceNodeClass : AbstractNodeClass
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

        public DataSourceNodeClass(string name, string displayName, string description, string creator, DataType outputType,
            string mainClass, string[] needsOtherClasses, INeedsExtra[] needsExtra, ConfigElement[] config, ProxyProperty[] proxyProperties,
            bool userCreated, string nodePath,
            string dataMethod, string startMethod, string stopMethod)
            : base(ClassManager.NodeType.Sensor, name, displayName, description, creator, outputType, 
                  mainClass, needsOtherClasses, needsExtra,
                  config, proxyProperties, new DataType[0], userCreated, nodePath)
        {
            this.DataMethod = dataMethod;
            this.StartMethod = startMethod;
            this.StopMethod = stopMethod;
        }


        public override Node GenerateNode()
        {
            return new DataSource(this);
        }

    }
}
