package de.tu_darmstadt.smastra.generator.datatype;

import org.reflections.Reflections;
import org.reflections.scanners.MethodAnnotationsScanner;

import java.lang.reflect.Constructor;
import java.util.ArrayList;
import java.util.Collection;

import de.tu_darmstadt.smastra.markers.SkipParsing;

/**
 * Created by Toby on 28.09.2016.
 */

public class DataTypeParser {


    /**
     * Reads the Data from the class passed.
     * @param clazz to read from.
     * @return the generated DataType.
     */
    public static DataType readFromClass(Class<?> clazz){
        if(hasSkipAnnotation(clazz)) return null;

        for(Constructor<?> constructor : clazz.getConstructors()){
            de.tu_darmstadt.smastra.markers.elements.DataType annotation =
                    constructor.getAnnotation(de.tu_darmstadt.smastra.markers.elements.DataType.class);

            //We have an Annotation -> Create stuff:
            if(annotation != null){
                if(!annotation.creatable()) return DataType.Unconstructable(clazz);

                String template = annotation.template();
                Class<?>[] params = constructor.getParameterTypes();
                if(template.isEmpty()){
                   template = "new " + clazz.getSimpleName() + "(";
                    for(int i = 0; i < params.length; i++){
                        if(i != 0) template += ",";
                        template += "{" + i + "}";
                    }

                    template += ");";
                }

                return new DataType(clazz, template, params, true);
            }
        }

        return null;
    }

    /**
     * Reads all the Types present in the System.
     * @return a collection of all present element.
     */
    public static Collection<DataType> getAllFromClassLoader(){
        Collection<DataType> dataTypes = new ArrayList<>();

        Reflections reflections = new Reflections(new MethodAnnotationsScanner());
        for(Constructor constructor : reflections.getConstructorsAnnotatedWith(de.tu_darmstadt.smastra.markers.elements.DataType.class)){
            Class<?> clazz = constructor.getDeclaringClass();
            DataType type = readFromClass(clazz);
            if(type != null) dataTypes.add(type);
        }

        return dataTypes;
    }


    /**
     * Returns true if the Annotation for skipping is present.
     * @param clazz to check
     * @return true if is present.
     */
    private static boolean hasSkipAnnotation(Class<?> clazz){
        return clazz != null && clazz.getAnnotation(SkipParsing.class) != null;
    }

}
