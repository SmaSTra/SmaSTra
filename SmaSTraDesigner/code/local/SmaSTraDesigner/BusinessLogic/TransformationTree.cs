﻿using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
    [Serializable]
	public class TransformationTree
	{
		// TODO: (PS) Comment this.
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

        public void test()
        {
            HardcodingForTesting();

            createJava();
        }

        /// <summary>
        /// Hardcoding some fun for testing purposes. Random demo tree.
        /// </summary>
        public void HardcodingForTesting()
        {
            OutputNode = new OutputNode();
                Transformation first = new Transformation();
                first.Name = "Add_Vectors";
                first.InputNodes = new ObservableCollection<Node>();
                    Transformation second = new Transformation();
                    second.Name = "Add_Vectors";
                    second.InputNodes = new ObservableCollection<Node>();
                    DataSource data1 = new DataSource();
                        data1.Name = "Accelerometer_Sensor";
                    DataSource data2 = new DataSource();
                        data2.Name = "Gyroscope_Sensor";
                    second.InputNodes.Add(data1);
                    second.InputNodes.Add(data2);
                first.InputNodes.Add(second);
                    DataSource data3 = new DataSource();
                    data3.Name = "Gps_Sensor";
                first.InputNodes.Add(data3);
            OutputNode.InputNode = first;
            OutputNode.Name = "Output Node";
            Console.WriteLine("initialized all the things!");

    }

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
                string directory = saveFileDialog.FileName.Remove(saveFileDialog.FileName.Length - (className.Length+5));
                Console.WriteLine(directory);
                code = javaGenerator.traverse(OutputNode.InputNode, visited, numbers, true, directory);
                string completeJavaText = javaGenerator.assembleText(className, code);
                File.WriteAllText(saveFileDialog.FileName, completeJavaText);
            }


        }

    }
}