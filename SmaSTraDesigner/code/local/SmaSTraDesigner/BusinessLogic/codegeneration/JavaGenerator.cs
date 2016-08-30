namespace SmaSTraDesigner.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json.Linq;
    using nodes;

    /// <summary>
    /// Class handling all necessary functionality for the generation of java-code
    /// </summary>
    public class JavaGenerator
	{
		#region static methods

		/// <summary>
		/// Copying all files from a given source directory to a destination directory, is capable of copying subdirs too. Deletes File "metadata.json" from dest in the end.
		/// Copied from MSDN
		/// </summary>
		/// <param name="sourceDirName">The source directory</param>
		/// <param name="destDirName">The target directory</param>
		/// <param name="copySubDirs">whether substrings should be copied</param>
		private static void DirectoryCopy(
			string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			// If the source directory does not exist, throw an exception.
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory does not exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the file contents of the directory to copy.
			FileInfo[] files = dir.GetFiles();

			foreach (FileInfo file in files)
			{
				// Create the path to the new copy of the file.
				string temppath = Path.Combine(destDirName, file.Name);

				// Copy the file.
				file.CopyTo(temppath, true);
			}

			// If copySubDirs is true, copy the subdirectories.
			if (copySubDirs)
			{

				foreach (DirectoryInfo subdir in dirs)
				{
					// Create the subdirectory.
					string temppath = Path.Combine(destDirName, subdir.Name);

					// Copy the subdirectories.
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}

        /// <summary>
        /// Removing the metadata.json and adding the SmaSTraBase.aar 
        /// </summary>
        /// <param name="sourceDirName">location of the generated-folder</param>
        /// <param name="destDirName">destination directory, where the java-generation has taken place</param>
        private static void FinalizeCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            string libsDirName = destDirName + "\\libs\\";

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // If the libs directory does not yet exist, create it.
            if (!Directory.Exists(libsDirName))
            {
                Directory.CreateDirectory(libsDirName);
            }


            FileInfo[] aar = dir.GetFiles("SmaSTraBase.aar"); //aar-file is the first element

            // Create the path to the new copy of the file.
            string libspath = Path.Combine(libsDirName, aar[0].Name);

            // Copy the file.
            aar[0].CopyTo(libspath, true);

            string temppath = Path.Combine(destDirName, "metadata.json");

            File.Delete(temppath);
        }

        #endregion static methods

        #region fields

        string breaks = "\n \n";

        string[] importBlacklist = { "double", "byte", "long", "float", "short", "char", "int", "integer", "boolean" };

		#endregion fields

		#region methods

		/// <summary>
		/// Combining all those string-Lists so that in the end there will be a nice string containing wonderful java code!
		/// </summary>
		/// <param name="className">The name of the class. Should be equal to the fileName, but do whatever you want.</param>
		/// <param name="code">An array of five string-Lists. First element are the imports, second are the sensor-inits (of which every first element is the init as a global variable and every second
        /// element is the acutal init needed for the respective sensor), third are the transformation-methods, fourth is a list containing a single element:
		/// the type of the last variable, that is going to be the output of this calls, the last element is another list with one element, containing a parsed integer, the number of transformations.</param>
		/// <returns>A string that is just one big java class.</returns>
		public string assembleText(string className, List<string>[] code)
		{
			string levelString = code[4][0];
            int level;
            if(!int.TryParse(levelString, out level))
            {
                throw new InvalidArgumentException("Tried parsing " + levelString + ", but failed.");
            }

			string extends = minimizeToClass( code[3][0] );
            //Check if we have a primitive as Extend. If yes, we need to make it to an Object:
            if (importBlacklist.Contains(extends))
            {
                extends = extends.First().ToString().ToUpper() + extends.Substring(1);
            }


			/////////import section///////
			List<string> imports = code[0];
			imports = imports.Distinct().ToList();
			string import = "";

            foreach (string s in imports)
            {
                ///Avoid imports of Primitves!
                if (importBlacklist.Contains(s.ToLower())) continue;
                import += "import " + s + ";\n";
            }

			import += breaks;

			/////////intro section///////
			string intro = "public class " + className + " extends SmaSTraTreeExecutor<" + extends + "> {" + breaks +
				"\tpublic " + className + "(Context context) { \n\t\tsuper(" + level + ", context); \n\t}" + breaks;

            /////////sensor init section///////
            string prep = "";
			string init = "\tprotected void init(){\n";
            for(int i = 0; i<code[1].Count; i++)
            {
                if (i % 2 == 0)
                {
                    prep = prep + code[1][i]; // add the variable init
                }
                else
                {
                    init = init + code[1][i]; // init the sensor, so we can listen
                }
            }

			init = prep + breaks + init + "\t}" + breaks;

            //////////transformation call///////
            string transformMethod = "\t@Override\n\tprotected void transform(int level) {\n\t\tswitch (level){\n";
            for (int i = 0; i < level; i++)
            {
                transformMethod = transformMethod + "\t\t\tcase " + i + " : transform" + i + "(); break;\n";
            }
            transformMethod = transformMethod + "\t\t}\n\t}" + breaks;

            /////////transformation section///////
            string transforms = "";
            foreach (string s in code[2])
            {
                transforms = transforms + s + breaks;
            }

            /////////combining///////
            string text = import + intro + init + transformMethod + transforms + "}";
			text.Replace("\n", Environment.NewLine);
			return text;
		}

		/// <summary>
		/// Processing a sensor-node for code production. Will take care of storing all imports, every sensor initialization, and is returning viable recursive data back upwards.
		/// Also takes care of copying needed java code around. To change the destination check the second line of code.
		/// </summary>
		/// <param name="currentNode">The current node that will be processed.</param>
		/// <param name="number">The sensor-number, so the sensor can be identified.</param>
		/// <param name="targetDirectory">The directory all the files are going to be saved into.</param>
		/// <returns>An array of five string-Lists. First element is imports, second is inits (of which every first element is the initialization of the sensor as a variable and the second element
        /// are the calls needed to actually initalize it), third is transforms (empty in this case, but added for consistency), fourth are the 
		/// functions' returnValues, which in this case contains an int, the number of the current sensor, last are the methodcalls needed to fetch data from these sensors</returns>
		public List<string>[] processSensor(Node currentNode, int number, bool first, string targetDirectory)
		{
			string fileName = currentNode.Class.Name;
			string sourceDirectory = "generated\\" + fileName;
            var json = JObject.Parse(File.ReadAllText(sourceDirectory + "\\metadata.json"));

			List<string>[] code = new List<string>[5];
			List<string> imports = new List<string>();
			List<string> inits = new List<string>();
			List<string> transforms = new List<string>();
			List<string> newReturnValues = new List<string>();
			List<string> newFunctionCalls = new List<string>();

			Console.WriteLine("sensor processing called!");

            //imports
            imports.Add(json["mainClass"].ToString());
			foreach (string need in json["needs"])
			{
				imports.Add(need);
				DirectoryCopy(sourceDirectory, targetDirectory, true);
			}

            string sensorMainClass = minimizeToClass(json["mainClass"].ToString());
            string prep = "\tprivate " + sensorMainClass + " sensor" + number + ";\n";

            //initiating sensor
            string init = "\t\t" + "sensor" + number + " = new " + sensorMainClass + "(context);\n";
            
            //Start the sensor if startable. This means that .start is present in the dynamic json:
            if(json["start"] != null) init += "\t\tsensor" + number + "." + json["start"] + "();\n";

            inits.Add(prep);
			inits.Add(init);

            if (first)
            {
                string transform = "\tprivate void transform0(){\n";
                string methodCall = "\t\tdata = sensor0." + json["dataMethod"] + "();\n";

                transform = transform + methodCall + "\t}\n";

                transforms.Add(transform);

                newReturnValues.Add(minimizeToClass( json["output"].ToString() )); //Needed so the text-assembly will know what to derive from
                newFunctionCalls.Add("1");
            }
            else
            {
                newReturnValues.Add(number.ToString()); //in every other case this will be needed for the following transform
                newFunctionCalls.Add(json["dataMethod"].ToString());
            } 
			
			

			code[0] = imports;
			code[1] = inits;
			code[2] = transforms;
			code[3] = newReturnValues;
			code[4] = newFunctionCalls;

			return code;
        }

        /// <summary>
        /// This gets the last part of a class from an complete Class name.
        /// Eg.: java.util.List -> List.
        /// </summary>
        /// <param name="completeClassName"> This is the complete class name. For example 'java.util.List'</param>
        /// <returns>The last part of the Class name.</returns>
        private string minimizeToClass(string completeClassName)
        {
            return completeClassName.Split('.').Last();
        }


		/// <summary>
		/// Processing a transform-node for code production. Will take care of storing all imports, the transformation itself including its variable init, and is returning viable recursive data back upwards.
		/// Also takes care of copying needed java code around. To change the destination check the second line of code.
		/// </summary>
		/// <param name="currentNode">The current node that will be processed.</param>
		/// <param name="number">The transform-number, so the transform can be identified.</param>
		/// <param name="returnValues">The return values of other transforms that provide data for this one -> the output-variables of other transforms. Entries are either a string containing just an int, if there is
		/// no other transformation but a sensor, or will be a string containing their outputVariable</param>
		/// <param name="functionCalls">The names of functions that are needed to access data from a sensor providing data for this transform. Will either be "", if there is no sensor but
		/// another transform, or a string containing the methods name, eg "getData"</param>
		/// <param name="first">Whether this is the first transform that's visited by the recursion. Will happen to the last transform before the output.
		/// Needed for some changes to code generation, so the final return can be accessed.</param>
		/// <param name="targetDirectory">The directory all the files are going to be saved into.</param>
		/// <returns>An array of five string-Lists. First element is imports, second is inits (empty but added for consistency), third is transforms, fourth are the 
		/// transforms returnValues (containing one element, the name of the outputVariable), last are methodcalls (empty in this case). Important note: If the transform is the last one before the 
		/// output-node is reached, "first" will be true (this is a bad name choice, but whatever), so the fourth List will contain no variable name but the type of the last return. It is needed
		/// for initializing the class just right.</returns>
		public List<string>[] processTransform(Node currentNode, int number, List<string> returnValues, List<string> functionCalls, bool first, string targetDirectory)
		{
            string fileName = currentNode.Class.Name;
			string sourceDirectory = "generated\\" + fileName;
            var json = JObject.Parse(File.ReadAllText(sourceDirectory + "\\metadata.json"));

            Console.WriteLine("transform processing called!");
			List<string>[] code = new List<string>[5];
			HashSet<string> imports = new HashSet<string>();
			List<string> inits = new List<string>();
			List<string> transforms = new List<string>();
			List<string> newReturnValues = new List<string>();
			List<string> newFunctionCalls = new List<string>();

            string mainClass = json["mainClass"].ToString();
            string method = json["method"].ToString();
            string output = json["output"].ToString();


            //imports
            imports.Add(mainClass);
            imports.Add(output);
			foreach (string need in json["needs"])
			{
				imports.Add(need);
				DirectoryCopy(sourceDirectory, targetDirectory, true);
			}

            //First shorten classes:
            mainClass = minimizeToClass(mainClass);
            output = minimizeToClass(output);

			//transforms
			string transform = "";
			string methodCall = "";
			if (!first)
			{
				methodCall = "\t\t" + "resultTransform" + number + " = " + mainClass + "." + method + "(";
			}
			else
			{
				methodCall = "\t\tdata = " + mainClass + "." + method + "(";
			}

			string prep = "";
			if (!first) { prep = "\tprivate " + output + " resultTransform" + number + ";\n"; }
			transform = "\tprivate void transform" + number + "(){\n";

			dynamic temp = json["input"];
            int currentSensorNumber;
			int counter = 0;
			foreach (JProperty property in temp.Properties())
			{
				if (Int32.TryParse(returnValues[counter], out currentSensorNumber)) //-> this one is a sensor
				{
                    string dataType = minimizeToClass(property.Value.ToString());
                    methodCall = methodCall + "data" + counter + ", ";
					transform += "\t\t" + dataType + " data" + counter + " = sensor" + currentSensorNumber + "." + functionCalls[counter] + "();\n";
				}
				else //-> this one is another transform
				{
					methodCall += returnValues[counter] + ", ";
				}
				counter++;
			}

			methodCall = methodCall.Remove(methodCall.Length - 2);
			methodCall = methodCall + ");\n";
			transform = prep + "\n" + transform + methodCall + "\t}\n";

			transforms.Add(transform);
			if (first)
			{
				newReturnValues.Add(json["output"].ToString()); //Needed so the text-assembly will know what to derive from
			}
			else
			{
				newReturnValues.Add("resultTransform" + number);
			}
			newFunctionCalls.Add("");

			code[0] = imports.ToList();
			code[1] = inits;
			code[2] = transforms;
			code[3] = newReturnValues;
			code[4] = newFunctionCalls;

			return code;
		}

		/// <summary>
		/// Recursive method traversing the whole TransformationTree. Recursion Anchors are either sensors, who have no Input-Nodes, or nodes that have already been visited (in less linear, nested trees). In the latter case the information
		/// will be fetched from "visited" a dicitonary containing the stuff needed in that case.
		/// Should be started at the last transformation before Output, so the first currentNode should be set to Output.InputNode.
		/// </summary>
		/// <param name="currentNode">The current node.</param>
		/// <param name="visited">A Dictionary containing Nodes as keys and two lists of strings as values. The first is the returnValues list, needed by nested transformations to access the output of other transformations.
		/// It will have one element for transformations and none for sensors. The second is the functionCalls list, which contains the getData-method-name for sensors. It will have one entry for
		/// a sensor and none for a transform node. The dictionary is needed so the algorithm is not entering the same node twice in nested structures.</param>
		/// <param name="numbers">Int-Array containing two values. numbers[0] is the current number of transformations. numbers[1] is the current number of sensors. Important for exclusive identifiers for all
		/// code snippets and for correct processing of the class as a whole.</param>
		/// <param name="first">Whether this is the first recursion step.</param>
		/// <param name="targetDirectory">The directory the final .java-file will be saved to. Needed here so the imports can be moved there.</param>
		/// <returns>An array of five string-Lists. First element is imports, second is inits, third is transforms, fourth are the 
		/// transforms returnValues, last are methodcalls.  Important note: If the transform is the last one before the 
		/// output-node is reached, "first" will be true (this is a bad name choice, but whatever), so the functionCalls will receive an int parsed as a string. This is the final number of transformations, which
		/// will be needed for generating the complete file.</returns>
		public List<string>[] traverse(Node currentNode, Dictionary<Node, List<string>[]> visited, int[] numbers, bool first, string targetDirectory)
		{
            //checking for valid input
            if(currentNode == null)
            {
                throw new NullNodeException("Current Node is Null after " + numbers[0] + " visited transforms and " + numbers[1] + " visited sensors. Is your tree complete?");
            }


            //initializing stuff
            List<string>[] code = new List<string>[5];

			List<string> imports = new List<string>();
			List<string> inits = new List<string>();
			List<string> transforms = new List<string>();
			List<string> returnValues = new List<string>();
			List<string> functionCalls = new List<string>();

			//anchor 1: has already been visited -> just return the stored info
			List<string>[] myElements = new List<string>[2];
			if (visited.TryGetValue(currentNode, out myElements))
			{
				//return
				code[0] = imports; //empty
				code[1] = inits; //empty
				code[2] = transforms; //empty
				code[3] = myElements[0];
				code[4] = myElements[1];

				return code;
			}

			//anchor 2: is end node -> sensor
			if (currentNode is DataSource)
			{
				List<string>[] sensorData = processSensor(currentNode, numbers[1], first, targetDirectory);

				numbers[1]++;

				//return
				code[0] = sensorData[0];
				code[1] = sensorData[1];
				code[2] = sensorData[2];
				code[3] = sensorData[3];
				code[4] = sensorData[4];

                List<string>[] dictionaryValues = new List<string>[2];
                dictionaryValues[0] = sensorData[3];
                dictionaryValues[1] = sensorData[4];

                visited.Add(currentNode, dictionaryValues);

                return code;
			}

            //If node is transformation:
            if (currentNode is Transformation)
            {
                Transformation transformNode = (Transformation)currentNode;
                List<string>[] temp;
                foreach (Node child in transformNode.InputNodes)
                {
                    temp = traverse(child, visited, numbers, false, targetDirectory);
                    imports.AddRange(temp[0]);
                    inits.AddRange(temp[1]);
                    transforms.AddRange(temp[2]);
                    returnValues.AddRange(temp[3]);
                    functionCalls.AddRange(temp[4]);
                }
                List<string>[] transformData = processTransform(currentNode, numbers[0], returnValues, functionCalls, first, targetDirectory);
                imports.AddRange(transformData[0]);
                inits.AddRange(transformData[1]);
                transforms.AddRange(transformData[2]);
                returnValues = transformData[3];
                functionCalls = transformData[4];

                List<string>[] dictionaryValue = new List<string>[2];
                dictionaryValue[0] = returnValues;
                dictionaryValue[1] = functionCalls;

                visited.Add(currentNode, dictionaryValue);

                numbers[0]++;

                if (first)
                {
                    functionCalls.Clear();
                    functionCalls.Add(numbers[0].ToString()); //at this point we are at the last node before returning all code snippets. numbers[0] will contain the number of transformations.
                    string sourceDirectory = "generated\\";
                    FinalizeCopy(sourceDirectory, targetDirectory);
                }

                code[0] = imports;
                code[1] = inits;
                code[2] = transforms;
                code[3] = returnValues;
                code[4] = functionCalls;

                return code;
            }


            //If node is Combined:
            if (currentNode is CombinedNode)
            {
                CombinedNode combinedNode = (CombinedNode)currentNode;
                //TODO add this somehow.
                return code;
            }


            return code;
		}

		#endregion methods
	}


    [Serializable]
    public class NullNodeException : Exception
    {
        public NullNodeException() { }
        public NullNodeException(string message) : base(message) { }
    }

    public class InvalidArgumentException : Exception
    {
        public InvalidArgumentException() { }
        public InvalidArgumentException(string message) : base(message) { }
    }
}