namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders
{
    static class ClassTemplates
    {
        private const string NL = "\r\n";


        /// <summary>
        /// The template for the Transformation.
        /// <br>{0} : Name of the Class / Method
        /// <br>{1} : Return Type
        /// <br>{2} : Code-Content
        /// </summary>
        public const string SENSOR_TEMPLATE = 
                "package created;" + NL +
                "" + NL +
                "import android.content.Context;" + NL +
                "import de.tu_darmstadt.smastra.markers.interfaces.Sensor;" + NL +
                "" + NL +
                "public class {0} implements Sensor {{" + NL +
                "" + NL +
                "   private final Context context;" + NL +
                "" + NL +
                "   public {0}(Context context) {{" + NL +
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
                "   public {1} getData(){{" + NL +
                "   {2}" + NL +
                "   }}" + NL +
                "}}"
        ;


        /// <summary>
        /// The template for the Transformation.
        /// <br>{0} : Name of the Class / Method
        /// <br>{1} : Return Type
        /// <br>{2} : Input-Types as Method-Args
        /// <br>{3} : Code-Content
        /// </summary>
        public const string TRANSFORMATION_TEMPLATE =
               "package created;" + NL +
               "" + NL +
               "public class {0} implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {{" + NL +
               "" + NL +
               "   public static {1} {0}({2}) {{" + NL +
               "   {3}" + NL +
               "   }}" + NL +
               "}}"
       ;


        /// <summary>
        /// The Template for generation of final class.
        /// <br> {0} : package name
        /// <br> {1} : imports
        /// <br> {2} : ClassName
        /// <br> {3} : Class return value
        /// <br> {4} : Amount of steps
        /// <br> {5} : Sensor-Inits
        /// <br> {6} : switchTransfor
        /// <br> {7} : transformations
        /// </summary>
        public const string GENERATION_TEMPLATE =
            "package {0}" + NL +
            "{1}" + NL + 
            "" + NL +
            "public class {2} extends SmaSTraTreeExecutor<{3}> {{" + NL + 
            "" + NL +
            "   public {2}(Context context){{" + NL +
            "       super({4}, context);" + NL +
            "   }}" + NL +
            "" + NL +
            "{5}" + NL +
            "" + NL +
            "   @Override" + NL +
            "   protected void transform(int level) {{" + NL +
            "       switch(level) {{" + NL +
            "           {6}" + NL +
            "       }}" + NL +
            "   }}" + NL +
            "" + NL +
            "{7}" + NL +
            "" + NL +
            "}}";
            

    }
}
