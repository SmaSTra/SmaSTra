package de.tu_darmstadt.smastra.generator.datatype;

import org.reflections.Reflections;
import org.reflections.scanners.SubTypesScanner;
import org.reflections.scanners.TypeAnnotationsScanner;

import java.util.ArrayList;
import java.util.Collection;

import de.tu_darmstadt.smastra.markers.SkipParsing;

/**
 * @author Tobias Welther
 */

public class EnumDataTypeParser {



    /**
     * Reads the Data from the class passed.
     * @param clazz to read from.
     * @return the generated DataType.
     */
    public static EnumDataType readFromClass(Class<?> clazz){
        if(hasSkipAnnotation(clazz)) return null;


        de.tu_darmstadt.smastra.markers.elements.datatype.EnumDataType annotation =
                clazz.getAnnotation(de.tu_darmstadt.smastra.markers.elements.datatype.EnumDataType.class);

        //We have an Annotation -> Create stuff:
        if(annotation != null) return new EnumDataType((Class<? extends Enum>) clazz);
        return null;
    }

    /**
     * Reads all the Types present in the System.
     * @return a collection of all present element.
     */
    public static Collection<EnumDataType> getAllFromClassLoader(){
        Collection<EnumDataType> dataTypes = new ArrayList<>();

        Reflections reflections = new Reflections(new TypeAnnotationsScanner(), new SubTypesScanner());
        for(Class<?> type : reflections.getTypesAnnotatedWith(de.tu_darmstadt.smastra.markers.elements.datatype.EnumDataType.class)){
           if(type.isEnum()) dataTypes.add(new EnumDataType((Class<? extends Enum>) type));
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
