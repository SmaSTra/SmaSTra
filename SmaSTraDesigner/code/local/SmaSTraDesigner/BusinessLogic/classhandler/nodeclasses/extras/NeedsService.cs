using System;
using System.Linq;
using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras
{
    public class NeedsService : INeedsExtra
    {
        

        public NeedsService(string serviceClassName, bool exportable)
        {
            this.ServiceClassName = serviceClassName;
            this.Exportable = exportable;
        }

        

        /// <summary>
        /// This is the main-Class name of the Service.
        /// </summary>
        public string ServiceClassName { get; }


        /// <summary>
        /// This indicates if the Service may be called from outside.
        /// </summary>
        public bool Exportable { get; }


        public void ApplyToManifest(XElement root)
        {
            //Get the Application context:
            var application = root.Element("application");
            if (application == null)
            {
                //Nothing to do here! :(
                return;
            }

            XElement[] nodes = application.Elements("service").ToArray();
            if (nodes.Any(e => e.FirstAttribute.Value == ServiceClassName)) return;

            XName name = "{http://schemas.android.com/apk/res/android}" + "name";
            XName exported = "{http://schemas.android.com/apk/res/android}" + "exported";
            application.Add( new XElement("service", new XAttribute(name, ServiceClassName), new XAttribute(exported, Exportable) ));
            application.Add(Environment.NewLine);
        }
    }
}
