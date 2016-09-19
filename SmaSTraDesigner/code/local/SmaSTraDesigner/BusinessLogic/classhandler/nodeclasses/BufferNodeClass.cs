﻿using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{


    class BufferNodeClass : AbstractNodeClass
    {


        /// <summary>
        /// The Method to add stuff to the Buffer.
        /// </summary>
        public string BufferAddMethod { get; }


        /// <summary>
        /// The Method to Get stuff from the Buffer.
        /// </summary>
        public string BufferGetMethod { get; }


        public BufferNodeClass(string name, string displayName, string description, 
            DataType outputType, string mainClass, string[] needsOtherClasses, string[] needsPermissions, 
            ConfigElement[] configuration, ProxyProperty[] proxyProperties, DataType[] inputTypes,
            bool userCreated,
            string bufferAdd, string bufferGet) 
            : base(ClassManager.NodeType.Buffer, name, displayName, description, outputType, mainClass, needsOtherClasses, needsPermissions, 
                  configuration, proxyProperties, inputTypes, userCreated)
        {
            this.BufferAddMethod = bufferAdd;
            this.BufferGetMethod = bufferGet;
        }


        protected override Node generateBaseNode()
        {
            return new BufferNode()
            {
                Name = this.DisplayName
            };
        }

    }
}
