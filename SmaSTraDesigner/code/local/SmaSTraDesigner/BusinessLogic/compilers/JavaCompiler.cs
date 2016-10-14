using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace SmaSTraDesigner.BusinessLogic.compilers
{


    public class JavaCompiler
    {

        /// <summary>
        /// The Const for the Environment var.
        /// </summary>
        private const string JavaHome = "JAVA_HOME";


        /// <summary>
        /// The path for the JavaC compiler.
        /// </summary>
        private readonly string _javacPath;
 

        public JavaCompiler()
        {
            try
            {
                _javacPath = Environment.GetEnvironmentVariable(JavaHome);
            }
            catch (SecurityException e)
            {
                //He may not read this. :(
                Debug.Print(e.ToString());
                this._javacPath = null;
            }
        }

        /// <summary>
        /// If the Java-compiler is set in the System.
        /// </summary>
        /// <returns>True if set in the Java path.</returns>
        public bool IsInstalled()
        {
            return _javacPath != null;
        }


        /// <summary>
        /// Returns true if the Java-Home variable is set.
        /// </summary>
        public bool IsJavHomeSet()
        {
            return Environment.GetEnvironmentVariable(JavaHome) != null;
        }


        /// <summary>
        /// Returns if the Class is compilable by the JavaC compiler.
        /// </summary>
        /// <param name="path">The file is saved at.</param>
        /// <returns>true if compileable, false if not compileable or JavaC is not inited.</returns>
        public bool IsCompileable(string path)
        {
            //Could not find JavaC
            if (!IsJavHomeSet() || !IsInstalled())
            {
                return false;
            }

            //Check for existence of Java-Compiler.
            var javaCPath = Path.Combine(_javacPath, "bin", "javac.exe");
            if (!File.Exists(javaCPath))
            {
                return false;
            }

            //Now execute the Java-Compiler and check the result:
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = javaCPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                //TODO we need to extract the base-jar to pass it to the javac compiler!
                Arguments = " -cp " + "libs" + ";." + " " + path
            };

            try
            {
                using (var javacProcess = Process.Start(startInfo))
                {
                    if (javacProcess == null) return false;
                    javacProcess.WaitForExit();
                }

                var classFilePath = path.Replace(".java", ".class");
                var exists = File.Exists(classFilePath);
                File.Delete(classFilePath);

                return exists;
            }
            catch (Exception)
            {
                // do something e.g. throw a more appropriate exception
                return false;
            }
        }



    }
}
