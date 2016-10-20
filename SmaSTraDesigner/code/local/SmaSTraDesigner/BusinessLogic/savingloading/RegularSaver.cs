using System.IO;
using Common;
using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.savingloading.serializers;
using SmaSTraDesigner.Controls;

namespace SmaSTraDesigner.BusinessLogic.savingloading
{

    public class RegularSaver
    {

        /// <summary>
        /// The current work file.
        /// </summary>
        private const string FileName = "current.work";


        /// <summary>
        /// Saves the current State.
        /// <param name="designer">The designer to save from.</param>
        /// </summary>
        public static void Save(UcTreeDesigner designer)
        {
            //No designer, nothing to do.
            if (designer == null) return;

            var path = Path.Combine(WorkSpace.DIR, FileName);
            TreeSerilizer.Serialize(designer.Tree, path);
        }


        /// <summary>
        /// Loads the last known state and restores it.
        /// <param name="designer">The designer to load into</param>
        /// </summary>
        public static void TryLoad(UcTreeDesigner designer)
        {
            //No designer, nothing to do.
            if (designer == null) return;

            //If loading is disabled by config -> do not load.
            var load = true;
            bool.TryParse(Singleton<SmaSTraConfiguration>.Instance.GetConfigOption(SmaSTraConfiguration.LoadLastStatePath, "true"), out load);
            if (!load) return;

            //Finally load if we have a file!
            var path = Path.Combine(WorkSpace.DIR, FileName);
            if (File.Exists(path))
            {
                TreeSerilizer.Deserialize(designer.Tree, path);
            }
        }

    }
}
