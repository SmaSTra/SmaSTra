namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public class TransformationNodeClass : AbstractNodeClass
    {

        public string Method { get;  }

        public bool IsStatic { get; }

        public TransformationNodeClass(string name, string displayName, string description, string creator, DataType outputType, DataType[] inputTypes,
            string mainClass, string[] needsOtherClasses, string[] needsPermissions, ConfigElement[] config, ProxyProperty[] proxyProperties,
            bool userCreated,
            string methodName, bool isStatic)
            : base(ClassManager.NodeType.Transformation, name, displayName, description, creator, outputType, 
                  mainClass, needsOtherClasses, needsPermissions, 
                  config, proxyProperties, inputTypes, userCreated)
        {
            this.Method = methodName;
            this.IsStatic = isStatic;
        }


        public override Node generateNode()
        {
            return new Transformation(this);
        }
    }
}
