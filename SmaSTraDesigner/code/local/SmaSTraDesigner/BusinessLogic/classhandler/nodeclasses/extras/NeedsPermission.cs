using System;
using System.Linq;
using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras
{
    public class NeedsPermission : NeedsExtra
    {
        

        public NeedsPermission(string permission)
        {
            this.Permission = permission;
        }

        

        /// <summary>
        /// This is the main-Class name of the Service.
        /// </summary>
        public string Permission { get; }


        public void ApplyToManifest(XElement root)
        {
            XElement[] nodes = root.Elements("uses-permission").ToArray();
            if (nodes.Any(e => e.FirstAttribute.Value == Permission)) return;

            XName name = "{http://schemas.android.com/apk/res/android}" + "name";
            root.Add( new XElement("uses-permission", new XAttribute(name, Permission)));
            root.Add(Environment.NewLine);
        }
    }
}
