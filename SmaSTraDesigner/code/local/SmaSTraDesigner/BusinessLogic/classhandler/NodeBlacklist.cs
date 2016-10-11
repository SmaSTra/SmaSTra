using SmaSTraDesigner.BusinessLogic.config;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    public class NodeBlacklist
    {

        private const string NodeBlacklistFileName = "node.blacklist";


        /// <summary>
        /// The Blacklist to use.
        /// </summary>
        private List<string> NodesOnBlacklist { get; }



        public NodeBlacklist()
        {
            NodesOnBlacklist = new List<string>();
            Reload();
        }



        /// <summary>
        /// Reloads from the File-System.
        /// </summary>
        public void Reload()
        {
            NodesOnBlacklist.Clear();

            var path = Path.Combine(WorkSpace.DIR, NodeBlacklistFileName);
            if (!File.Exists(path)) return;

            var names = File.ReadAllLines(path);
            NodesOnBlacklist.AddRange(names);
        }

        /// <summary>
        /// Adds a new Name to the Blacklist.
        /// </summary>
        /// <param name="name"></param>
        public void Add(string name)
        {
            NodesOnBlacklist.Add(name);
            Save();
        }

        /// <summary>
        /// Removes the Node from the Blacklist.
        /// </summary>
        /// <param name="clazz"></param>
        public void Remove(AbstractNodeClass clazz)
        {
            Remove(clazz.Name);
        }

        /// <summary>
        /// Removes the Node from the Blacklist.
        /// </summary>
        /// <param name="name">to remove</param>
        public void Remove(string name)
        {
            NodesOnBlacklist.Remove(name);
            Save();
        }

        /// <summary>
        /// Returns true if is on Blacklist.
        /// </summary>
        /// <param name="name">To to search for.</param>
        /// <returns>true if is on Blacklist</returns>
        public bool IsOnBlackList(string name)
        {
            return NodesOnBlacklist.Contains(name);
        }


        /// <summary>
        /// Returns true if is on Blacklist.
        /// </summary>
        /// <param name="clazz">To to search for.</param>
        /// <returns>true if is on Blacklist</returns>
        public bool IsOnBlackList(AbstractNodeClass clazz)
        {
            return IsOnBlackList(clazz.Name);
        }


        /// <summary>
        /// Saves the File.
        /// </summary>
        private void Save()
        {
            var path = Path.Combine(WorkSpace.DIR, NodeBlacklistFileName);
            if (File.Exists(path)) File.Delete(path);

            File.WriteAllLines(path, NodesOnBlacklist.Distinct().ToArray());
        }
    
    }

}
