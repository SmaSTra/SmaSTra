using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
