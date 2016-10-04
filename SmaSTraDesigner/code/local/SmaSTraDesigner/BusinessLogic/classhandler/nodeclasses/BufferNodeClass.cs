using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{


    public class BufferNodeClass : AbstractNodeClass
    {


        /// <summary>
        /// The Method to add stuff to the Buffer.
        /// </summary>
        public string BufferAddMethod { get; }


        /// <summary>
        /// The Method to Get stuff from the Buffer.
        /// </summary>
        public string BufferGetMethod { get; }


        public BufferNodeClass(string name, string displayName, string description, string creator,
            DataType outputType, string mainClass, string[] needsOtherClasses, NeedsExtra[] needsExtra, 
            ConfigElement[] configuration, ProxyProperty[] proxyProperties, DataType[] inputTypes,
            bool userCreated, string nodePath,
            string bufferAdd, string bufferGet) 
            : base(ClassManager.NodeType.Buffer, name, displayName, description, creator, outputType, mainClass, needsOtherClasses, needsExtra, 
                  configuration, proxyProperties, inputTypes, userCreated, nodePath)
        {
            this.BufferAddMethod = bufferAdd;
            this.BufferGetMethod = bufferGet;
        }


        public override Node generateNode()
        {
            return new BufferNode(this);
        }

    }
}
