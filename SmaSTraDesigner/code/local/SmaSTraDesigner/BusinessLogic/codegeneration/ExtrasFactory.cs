using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.codegeneration
{
    class ExtrasFactory
    {


        private const string TYPE_PATH = "type";


        /// <summary>
        /// Reads an Extra from the Root of the Json.
        /// </summary>
        /// <param name="root">To Read from.</param>
        /// <returns></returns>
        public static INeedsExtra read(JObject root)
        {
            string type = root.GetValueAsString(TYPE_PATH, "");
            if (type.Empty()) return null;

            switch (type)
            {
                case "broadcast": return readBroadcast(root);
                case "permission": return readPermission(root);
                case "service": return readService(root);
                case "lib": return readLibrary(root);

                default: return null;
            }
        }


        private const string PERMISSION_PATH = "permission";

        private static NeedsPermission readPermission(JObject root)
        {
            return root.GetValue(PERMISSION_PATH) == null 
                ? null 
                : new NeedsPermission(root.GetValueAsString(PERMISSION_PATH));
        }


        private const string LIB_PATH = "lib";

        private static NeedsLibrary readLibrary(JObject root)
        {
            return root.GetValue(LIB_PATH) == null
                ? null
                : new NeedsLibrary(root.GetValueAsString(LIB_PATH));
        }


        private const string BROADCAST_PATH = "broadcast";
        private const string EXPORTABLE_PATH = "exportable";

        private static NeedsBroadcast readBroadcast(JObject root)
        {
            return root.GetValue(BROADCAST_PATH) == null
                ? null
                : new NeedsBroadcast(
                    root.GetValueAsString(BROADCAST_PATH), 
                    root.GetValueAsBool(EXPORTABLE_PATH, false)
                );
        }

        private const string SERVICE_PATH = "service";
        
        private static NeedsService readService(JObject root)
        {
            return root.GetValue(SERVICE_PATH) == null
                ? null
                : new NeedsService(
                    root.GetValueAsString(SERVICE_PATH),
                    root.GetValueAsBool(EXPORTABLE_PATH, false)
                );
        }



        public static JObject serialize(INeedsExtra extra)
        {
            if (extra is NeedsBroadcast) return serializeBroadcast(extra as NeedsBroadcast);
            if (extra is NeedsService) return serializeService(extra as NeedsService);
            if (extra is NeedsLibrary) return serializeLibrary(extra as NeedsLibrary);
            if (extra is NeedsPermission) return serializePermission(extra as NeedsPermission);

            return null;
        }


        private static JObject serializeBroadcast(NeedsBroadcast broadcast)
        {
            JObject obj = new JObject();
            obj.Add(TYPE_PATH, "broadcast");
            obj.Add(BROADCAST_PATH, broadcast.BroadcastClassName);
            obj.Add(EXPORTABLE_PATH, broadcast.Exportable);

            return obj;
        }

        private static JObject serializeService(NeedsService service)
        {
            JObject obj = new JObject();
            obj.Add(TYPE_PATH, "service");
            obj.Add(SERVICE_PATH, service.ServiceClassName);
            obj.Add(EXPORTABLE_PATH, service.Exportable);

            return obj;
        }

        private static JObject serializeLibrary(NeedsLibrary lib)
        {
            JObject obj = new JObject();
            obj.Add(TYPE_PATH, "lib");
            obj.Add(LIB_PATH, lib.LibName);

            return obj;
        }


        private static JObject serializePermission(NeedsPermission permission)
        {
            JObject obj = new JObject();
            obj.Add(TYPE_PATH, "permission");
            obj.Add(PERMISSION_PATH, permission.Permission);

            return obj;
        }

    }
}
