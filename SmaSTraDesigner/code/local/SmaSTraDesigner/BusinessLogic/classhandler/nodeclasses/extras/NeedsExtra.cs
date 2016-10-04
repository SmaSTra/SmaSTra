using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public interface NeedsExtra
    {

        /// <summary>
        /// Applies the Extra to the Manifest passed.
        /// <param name="root">The root of the document to edit.</param>
        /// </summary>
        void ApplyToManifest(XElement root);
    }
}
