namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Linq;

	using Microsoft.Win32;

	/// <summary>
	/// Represents a tree graph of data dransformations.
	/// </summary>
	[Serializable]
	public class TransformationTree
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public TransformationTree()
		{
			this.Nodes = new ObservableCollection<Node>();
			this.Connections = new ObservableCollection<Connection>();
			this.Nodes.CollectionChanged += this.Nodes_CollectionChanged;
		}

		#endregion constructors

		#region properties

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

		//stores the path to the generated folder
		//currently not in use. But should be used in the final form.
		//TODO!
		public string pathToGenerated
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
		public void createJava()
		{
			JavaGenerator javaGenerator = new JavaGenerator();

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
				code = javaGenerator.traverse(OutputNode.InputNode, visited, numbers, true, directory);
				string completeJavaText = javaGenerator.assembleText(className, code);
				File.WriteAllText(saveFileDialog.FileName, completeJavaText);
			}
		}

		/// <summary>
		/// Hardcoding some fun for testing purposes. Random demo tree.
		/// </summary>
		public void HardcodingForTesting()
		{
			//OutputNode = new OutputNode();
			//Transformation first = new Transformation();
			//first.Name = "Add_Vectors";
			//Transformation second = new Transformation();
			//second.Name = "Add_Vectors";
			//DataSource data1 = new DataSource();
			//data1.Name = "Accelerometer_Sensor";
			//DataSource data2 = new DataSource();
			//data2.Name = "Gyroscope_Sensor";
			//second.InputNodes[0] = data1;
			//second.InputNodes[1] = data2;
			//first.InputNodes[0] = second;
			//DataSource data3 = new DataSource();
			//data3.Name = "Gps_Sensor";
			//first.InputNodes[1] = data3;
			//OutputNode.InputNode = first;
			//OutputNode.Name = "Output Node";
			//Console.WriteLine("initialized all the things!");
		}

		public void HardcodingForTestingII()
		{
			///*                                      Length
			//										Subtract
			//							  Multiply         Sensor:Gyro
			//						 Add            GPS
			//				Sensor:Acc Senso:Gyro
			//				Both Gyros should be from the same source, so this one is interconnected!
			//*/
			//OutputNode = new OutputNode();
			//Transformation Multiply = new Transformation();
			//Transformation first = new Transformation();
			//first.Name = "Vector_Length";
			//first.InputNodes = new ObservableCollection<Node>();
			//Transformation Subtract = new Transformation();
			//Subtract.Name = "Subtract_Vectors";
			//Subtract.InputNodes = new ObservableCollection<Node>();
			//Multiply.Name = "Multiply_Vectors";
			//Multiply.InputNodes = new ObservableCollection<Node>();
			//Transformation Add = new Transformation();
			//Add.Name = "Add_Vectors";
			//Add.InputNodes = new ObservableCollection<Node>();
			//DataSource data1 = new DataSource();
			//data1.Name = "Accelerometer_Sensor";
			//DataSource data2 = new DataSource();
			//data2.Name = "Gyroscope_Sensor";
			//Add.InputNodes.Add(data1);
			//Add.InputNodes.Add(data2);
			//Multiply.InputNodes.Add(Add);
			//DataSource data3 = new DataSource();
			//data3.Name = "Gps_Sensor";
			//Multiply.InputNodes.Add(data3);
			//Subtract.InputNodes.Add(Multiply);
			//Subtract.InputNodes.Add(data2);
			//first.InputNodes.Add(Subtract);
			//OutputNode.InputNode = first;
			//OutputNode.Name = "Output Node";
			//Console.WriteLine("initialized all the things!");
		}

		public void secondTest()
		{
			HardcodingForTestingII();

			createJava();
		}

		public void test()
		{
			HardcodingForTesting();

			createJava();
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
			foreach (Node node in e.NewItems.OfType<Node>())
			{
				node.Tree = this;
			}
		}

		#endregion event handlers
	}
}