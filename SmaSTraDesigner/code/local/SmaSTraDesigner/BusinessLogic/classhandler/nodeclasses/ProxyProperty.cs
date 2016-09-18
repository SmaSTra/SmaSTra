using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public class ProxyProperty
    {

        /// <summary>
        /// The Type of the property.
        /// </summary>
        public DataType PropertyType { get; }

        /// <summary>
        /// The Method name to use.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// The Name of the Property
        /// </summary>
        public string Name { get; }


        public ProxyProperty(DataType type, string name, string methodName)
        {
            this.PropertyType = type;
            this.Name = name;
            this.MethodName = methodName;
        }
    }
}
