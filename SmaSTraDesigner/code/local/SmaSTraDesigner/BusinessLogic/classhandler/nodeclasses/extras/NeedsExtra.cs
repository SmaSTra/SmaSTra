using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras
{
    public interface INeedsExtra
    {

        /// <summary>
        /// Applies the Extra to the Manifest passed.
        /// <param name="root">The root of the document to edit.</param>
        /// </summary>
        void ApplyToManifest(XElement root);
    }
}
