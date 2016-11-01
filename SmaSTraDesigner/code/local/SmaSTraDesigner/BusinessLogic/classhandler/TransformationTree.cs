using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.savingloading.serializers;

namespace SmaSTraDesigner.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Microsoft.Win32;
    using System.Windows;
    using Controls;
    using codegeneration;

    /// <summary>
    /// Represents a tree graph of data transformations.
    /// </summary>
    public class TransformationTree
	{

        #region constructors

        public TransformationTree(UcTreeDesigner ucTreeDesigner)
        {
            this.DesignTree = ucTreeDesigner;
            this.Nodes = new ObservableCollection<Node>();
            this.Connections = new ObservableCollection<Connection>();
            this.Nodes.CollectionChanged += this.Nodes_CollectionChanged;
        }

        #endregion constructors

        #region properties


        /// <summary>
        /// This is a reference to the DesignTree used for back-Handling.
        /// </summary>
        public UcTreeDesigner DesignTree
        {
            private set;
            get;
        }


        /// <summary>
        /// Gets the list of connections between nodes in this tree.
        /// </summary>
        public ObservableCollection<Connection> Connections
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the list of nodes this tree contains.
		/// </summary>
		public ObservableCollection<Node> Nodes
		{
			get;
			private set;
		}

		/// <summary>
		/// OutputNode for this tree.
		/// </summary>
		public OutputNode OutputNode
		{
			get;
			set;
		}

        #endregion properties

        #region methods

        /// <summary>
        /// Opens a Save File Dialog where the file to save can be specified. Then generates the java-Code and copies all needed subdirectories.
        /// Careful: current assumption is, that all our files are in Debug\\generated, which is hardcoded in the beginning of processSensor and processTransform. Changes have to be made there.
        /// </summary>
        public void CreateJava()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Java Source File Java Code (*.java)|*.java";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;

            Dictionary<Node, List<string>[]> visited = new Dictionary<Node, List<string>[]>();
            int[] numbers = new int[2];
            numbers[1] = 0;
            numbers[0] = 0;
            List<string>[] code = new List<string>[5];

            //show the dialog and save the file
            if (saveFileDialog.ShowDialog() == true)
            {
                string className = saveFileDialog.SafeFileName.Remove(saveFileDialog.SafeFileName.Length - 5);
                string directory = saveFileDialog.FileName.Remove(saveFileDialog.FileName.Length - (className.Length + 5));
                Console.WriteLine(directory);
                try
                {
                    new NewJavaGenerator(this).CreateJavaSource(directory, className);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


        /// <summary>
        /// Saves the current state to a file.
        /// </summary>
        public void SaveToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All files|*";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;

            //show the dialog and save the file
            if (saveFileDialog.ShowDialog() == true)
            {
                string name = saveFileDialog.SafeFileName;
                if (name.EndsWith(".SmaSTra")) name = name.Remove(".SmaSTra".Count());

                string directory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                TreeSerilizer.Serialize(OutputNode.Tree, directory + "\\" + name + ".SmaSTra");
            }
        }

        /// <summary>
        /// Loads a saved state and restores it.
        /// </summary>
        public void LoadFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SmaSTra Save file|*.SmaSTra";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;

            //show the dialog and save the file
            if (openFileDialog.ShowDialog() == true)
            {
                TreeSerilizer.Deserialize(OutputNode.Tree, openFileDialog.FileName);
            }
        }


        #endregion methods

        #region event handlers

        /// <summary>
        /// Handles the CollectionChanged event of the Nodes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e != null && e.NewItems != null)
            {
                foreach (Node node in e.NewItems.OfType<Node>())
                {
                    node.Tree = this;
                }
            }
		}

		#endregion event handlers
	}
}