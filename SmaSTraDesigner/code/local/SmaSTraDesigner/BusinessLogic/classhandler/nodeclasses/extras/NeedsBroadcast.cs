using System.Linq;
using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras
{
    public class NeedsBroadcast : NeedsExtra
    {
        

        public NeedsBroadcast(string broadcastClassName, bool exportable)
        {
            this.BroadcastClassName = broadcastClassName;
            this.Exportable = exportable;
        }

        

        /// <summary>
        /// This is the main-Class name of the Broadcast.
        /// </summary>
        public string BroadcastClassName { get; }


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

            XElement[] nodes = application.Elements("receiver").ToArray();
            if (nodes.Any(e => e.FirstAttribute.Value == BroadcastClassName)) return;

            XName name = "{http://schemas.android.com/apk/res/android}" + "name";
            XName exported = "{http://schemas.android.com/apk/res/android}" + "exported";
            application.Add( new XElement("receiver", new XAttribute(name, BroadcastClassName), new XAttribute(exported, Exportable) ));
        }
    }
}
