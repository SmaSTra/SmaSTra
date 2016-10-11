using System;
using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras
{
    public class NeedsLibrary : INeedsExtra
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
