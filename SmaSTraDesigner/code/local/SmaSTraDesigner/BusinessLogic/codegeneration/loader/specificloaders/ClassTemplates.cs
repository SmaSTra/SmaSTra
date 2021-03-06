﻿namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders
{
    static class ClassTemplates
    {
        private const string NL = "\r\n";


        /// <summary>
        /// The template for the Transformation.
        /// <br>{0} : Package name
        /// <br>{1} : import section
        /// <br>{2} : Name of the Class / Method
        /// <br>{3} : Return Type
        /// <br>{4} : Code-Content
        /// </summary>
        public const string SENSOR_TEMPLATE = 
                "package {0};" + NL +
                "" + NL +
                "{1}" + NL +
                "" + NL +
                "import android.content.Context;" + NL +
                "import de.tu_darmstadt.smastra.markers.interfaces.Sensor;" + NL +
                "" + NL +
                "public class {2} implements Sensor {{" + NL +
                "" + NL +
                "   private final Context context;" + NL +
                "" + NL +
                "   public {2}(Context context) {{" + NL +
                "       this.context = context;" + NL +
                "   }}" + NL +
                "" + NL +
                "   @Override public void start() {{}}" + NL +
                "   @Override public void stop(){{}}" + NL +
                "" + NL +
                "   @Override public void configure(String key, Object value) {{}}" + NL + 
                "" + NL +
                "   @Override public void configure(Map<String, Object> configuration) {{}}" + NL +
                "" + NL +
                "   public {3} getData(){{" + NL +
                "   {4}" + NL +
                "   }}" + NL +
                "}}"
        ;


        /// <summary>
        /// The template for the Transformation.
        /// <br>{0} : pacakge
        /// <br>{1} : Imports
        /// <br>{2} : Name of the Class / Method
        /// <br>{3} : Return Type
        /// <br>{4} : Input-Types as Method-Args
        /// <br>{5} : Code-Content
        /// </summary>
        public const string TRANSFORMATION_TEMPLATE =
               "package {0};" + NL +
               "" + NL +
               "{1}" + NL +
               "" + NL +
               "public class {2} implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {{" + NL +
               "" + NL +
               "   public static {3} {2}({4}) {{" + NL +
               "   {5}" + NL +
               "   }}" + NL +
               "}}"
       ;


        /// <summary>
        /// The Template for generation of final class.
        /// <br> {0} : package name
        /// <br> {1} : imports
        /// <br> {2} : ClassName
        /// <br> {3} : Class return value
        /// <br> {4} : Total-Init
        /// <br> {5} : Sensor Vars
        /// <br> {6} : Transform Vars
        /// <br> {7} : Start-Code
        /// <br> {8} : Stop-Code
        /// <br> {9} : ProxyProperty-Code
        /// <br> {10} : transformations-Code
        /// </summary>
        private const string GENERATION_TEMPLATE_TOTAL =
            "package {0};" + NL +
            "" + NL +
            "{1}" + NL + 
            "" + NL +
            "public class {2} extends SmaSTraTreeExecutor<{3}> {{" + NL + 
            "" + NL +
            "   public {2}(Context context){{" + NL +
            "       super(context);" + NL +
            "" + NL +
            "{4}" + NL +
            "   }}" + NL +
            "" + NL +
            "{5}" + NL +
            "" + NL +
            "{6}" + NL +
            "" + NL +
            "   @Override" + NL +
            "   protected void startIntern(){{" + NL +
            "{7}" + NL +
            "   }}" + NL +
            "" + NL +
            "   @Override" + NL +
            "   protected void stopIntern(){{" + NL +
            "{8}" + NL +
            "   }}" + NL +
            "" + NL +
            "{9}" + NL +
            "" + NL +
            "{10}" + NL +
            "" + NL +
            "}}";


        /// <summary>
        /// Generates the Total Text with the vars passed.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="imports"></param>
        /// <param name="className"></param>
        /// <param name="classReturnValue"></param>
        /// <param name="sensorVars"></param>
        /// <param name="transformVals"></param>
        /// <param name="initCode"></param>
        /// <param name="startCode"></param>
        /// <param name="stopCode"></param>
        /// <param name="transformationCode"></param>
        /// <returns></returns>
        public static string GenerateTotal(string packageName, string imports, string className, string classReturnValue, string initCode,
            string sensorVars, string transformVals, string startCode, string stopCode, string proxyPropertyCode,
            string transformationCode)
        {
            return string.Format(GENERATION_TEMPLATE_TOTAL,
                packageName, imports, className, classReturnValue, initCode, 
                sensorVars, transformVals, startCode, stopCode, proxyPropertyCode, 
                transformationCode);
        }


        /// <summary>
        /// The Constant for a transformation:
        /// <br>{0} : the index.
        /// <br>{1} : the content
        /// <br>{2} : Method call
        /// </summary>
        public const string GENERATION_TEMPLATE_TRANSFORM =
            "   public void transform{0}(){{" + NL +
            "{1}" +
            "       transform{0} = {2};" + NL +
            "   }}"

            ;

        /// <summary>
        /// The Constant for a transformation:
        /// <br>{0} : the index.
        /// <br>{1} : the content.
        /// <br>{2} : method call
        /// </summary>
        public const string GENERATION_TEMPLATE_TRANSFORM_LAST =
            "   public void transform{0}(){{" + NL +
            "{1}" +
            "       data = {2};" + NL +
            "   }}"

            ;

        /// <summary>
        /// The Constant for a transformation:
        /// <br>{0} : the type.
        /// <br>{1} : data index (0-x).
        /// <br>{2} : typeName (sensor,transform,...)
        /// <br>{3} : typeId
        /// <br>{4} : optional Sensor call
        /// </summary>
        public const string GENERATION_TEMPLATE_TRANSFORM_VAR_LINE =
            "       {0} data{1} = {2}{3}{4};" + NL;


        /// <summary>
        /// The Constant for a transformation:
        /// <br>{0} : the type.
        /// <br>{1} : data index (0-x).
        /// <br>{2} : the static data to put there.
        /// </summary>
        public const string GENERATION_TEMPLATE_TRANSFORM_VAR_LINE_STATIC =
            "       {0} data{1} = {2};" + NL;


        /// <summary>
        /// A simple template for android permissions
        /// <br>{0} : permission node.
        /// </summary>
        public const string GENERATION_TEMPLATE_PERMISSION =
            "  <uses-permission android:name=\"{0}\" />" + NL;

        /// <summary>
        /// The Proxy template method.
        /// <br> {0} : methodName
        /// <br> {1} : ClassName
        /// <br> {2} : ClassID
        /// <br> {3} : inputTypeName
        /// <br> {4} : varName
        /// <br> {5} : varID
        /// </summary>
        public const string GENERATION_TEMPLATE_PROXY_PROP =
            "   public void {0}For{1}{2}({3} arg) {{" + NL +
            "       {4}{5}.{0}(arg);" + NL +
            "   }}" + NL +
            "" + NL;

    }
}
