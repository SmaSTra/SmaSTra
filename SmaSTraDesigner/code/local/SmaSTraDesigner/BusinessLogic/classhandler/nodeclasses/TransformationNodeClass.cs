namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    class TransformationNodeClass : AbstractNodeClass
    {

        public string Method { get;  }

        public bool IsStatic { get; }

        public TransformationNodeClass(string name, string displayName, string description, DataType outputType, DataType[] inputTypes,
            string mainClass, string[] needsOtherClasses, string[] needsPermissions, ConfigElement[] config,
            string methodName, bool isStatic)
            : base(ClassManager.NodeType.Transformation, name, displayName, description, outputType, 
                  mainClass, needsOtherClasses, needsPermissions, 
                  config, inputTypes)
        {
            this.Method = methodName;
            this.IsStatic = isStatic;
        }


        protected override Node generateBaseNode()
        {
            return new Transformation()
            {
                Name = this.DisplayName
            };
        }
    }
}
