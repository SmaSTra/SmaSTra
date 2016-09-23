using Common;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.serializers;
using SmaSTraDesigner.BusinessLogic.utils;
using SmaSTraDesigner.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace SmaSTraDesigner.BusinessLogic
{
    class Library : INotifyPropertyChanged
    {

        private const string LIB_FILE_NAME = "SmaStraLib";

        private ObservableCollection<UcNodeViewer> libraryNodeViewerList = new ObservableCollection<UcNodeViewer>();

        private string nodeName = "default name";

        public Library()
        {
            
        }

        public string NodeName
        {
            get
            {
                    return nodeName;
            }
            set
            {
                nodeName = value;
                this.NotifyPropertyChanged("NodeName");
            }
        }

        public ObservableCollection<UcNodeViewer> LibraryNodeViewerList
        {
            get
            {
                return libraryNodeViewerList;
            }
            set
            {
                libraryNodeViewerList = value;
                this.NotifyPropertyChanged("LibraryNodeViewerList");
            }
        }

        public void Library_DragEnter(object sender, DragEventArgs e)
        {

        }

        public void addLibraryNode(Node node)
        {
            addLibraryNodeWithoutSaving(node);
            saveLibrary();
        }
        


        private void addLibraryNodeWithoutSaving(Node node)
        {
            if (node == null)
            {
                return;
            }

            foreach (UcNodeViewer libraryNode in libraryNodeViewerList)
            {
                if (node.Name.Equals(libraryNode.Node.Name))
                {
                    return;
                }
            }
            Transformation nodeAsTransformation;
            DataSource nodeAsDataSource;
            OutputNode nodeAsOutputNode;
            CombinedNode nodeAsCombinedNode;

            UcNodeViewer nodeViewer = null;
            if ((nodeAsTransformation = node as Transformation) != null)
            {
                nodeViewer = new UcTransformationViewer();
            }
            else if ((nodeAsDataSource = node as DataSource) != null)
            {
                nodeViewer = new UcDataSourceViewer();
            }
            else if ((nodeAsOutputNode = node as OutputNode) != null)
            {
                nodeViewer = new UcOutputViewer();
            }
            else if ((nodeAsCombinedNode = node as CombinedNode) != null)
            {
                if (node.Class.InputTypes.Count() > 0) nodeViewer = new UcTransformationViewer();
                else nodeViewer = new UcDataSourceViewer();
            }

            LibraryNodeViewerList.Add(nodeViewer);
            nodeViewer.DataContext = node;
            nodeViewer.IsLibrary = true;
        }


        /// <summary>
        /// Saves the library in a File.
        /// </summary>
        private void saveLibrary()
        {
            NodeSerializer serializer = new NodeSerializer();

            JObject lib = new JObject();
            JArray array = new JArray();
            lib.Add("lib", array);

            this.libraryNodeViewerList
                .Select(n => (Node)n.DataContext)
                .Select(serializer.serializeNode)
                .ForEach(array.Add);

            File.WriteAllText(LIB_FILE_NAME, lib.ToString());
        }


        /// <summary>
        /// Loads the Library from a file.
        /// </summary>
        public void loadLibrary()
        {
            this.libraryNodeViewerList.Clear();

            if (!File.Exists(LIB_FILE_NAME))
            {
                return;
            }

            NodeSerializer serializer = new NodeSerializer();
            ClassManager cManager = Singleton<ClassManager>.Instance;

            JObject json = JObject.Parse(File.ReadAllText(LIB_FILE_NAME));
            JToken arrayToken = new JObject();
            json.TryGetValue("lib", out arrayToken);

            JArray array = arrayToken as JArray;
            if (array != null)
            {
                array
                    .Select(n => serializer.deserializeNode(n as JObject, cManager))
                    .ForEach(addLibraryNode);
            }
        }


        public void removeLibraryNode(UcNodeViewer nodeViewer)
        {
            LibraryNodeViewerList.Remove(nodeViewer);
            saveLibrary();
        }

        public void Library_Drop(object sender, DragEventArgs e)
        {
            Node node = (Node)((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1.Class.generateNode();
            
            addLibraryNode(node);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
