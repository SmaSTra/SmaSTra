using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras
{
    public class NeedsLibrary : NeedsExtra
    {


        
        public NeedsLibrary(string libName)
        {
            this.LibName = libName;
        }


        /// <summary>
        /// The Library name needed.
        /// </summary>
        public String LibName { get; }


        public void ApplyToManifest(XElement root)
        {
            //Nothing to do here!
        }
    }
}
