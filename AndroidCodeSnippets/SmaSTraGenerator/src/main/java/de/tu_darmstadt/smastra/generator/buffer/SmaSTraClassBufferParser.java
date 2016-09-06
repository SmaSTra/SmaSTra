package de.tu_darmstadt.smastra.generator.buffer;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.BufferAdd;
import de.tu_darmstadt.smastra.markers.elements.BufferGet;
import de.tu_darmstadt.smastra.markers.elements.BufferInfo;
import de.tu_darmstadt.smastra.markers.elements.Configuration;
import de.tu_darmstadt.smastra.markers.elements.ConfigurationElement;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.interfaces.Buffer;

/**
 * Parses a class to a bunch of SmaSTra Sensors.
 *
 * @author Tobias Welther
 */
public class SmaSTraClassBufferParser {


    /**
     * Reads the Sensor from the Class.
     * @param clazz to read from.
     * @return the Transactions
     */
    public static SmaSTraBuffer readFromClass(Class<?> clazz){
        if(clazz == null) return null;

        //Read the class:
        if(!isBuffer(clazz)) return null;

        try{
            SmaSTraBufferBuilder builder = new SmaSTraBufferBuilder();
            builder.setClass(clazz);

            builder.setDisplayName(readDisplayName(clazz));
            builder.setBufferGetMethodName(readBufferGetMethodName(clazz));
            builder.setBufferAddMethodName(readBufferAddMethodName(clazz));
            builder.setDescription(readDescription(clazz));
            builder.setAndroidPermissions(readNeededPermissions(clazz));
            builder.addNeededClass(readNeededClasses(clazz));

            builder.addConfigurationElements(readConfigElements(clazz));

           return builder.build();
        }catch(ElementGenerationFailedException exp){
            exp.printStackTrace();
        }

        return null;
    }


    /**
     * Reads the needed Permissions from the Class.
     * @param clazz to use.
     * @return the needed Permissions. Empty Array if none present.
     */
    private static String[] readNeededPermissions(Class<?> clazz) {
        List<String> needed = new ArrayList<>();

        //Iterate through the Super-Classes.
        while(clazz != null && clazz != Object.class){
            NeedsAndroidPermissions permsAnnotation = clazz.getAnnotation(NeedsAndroidPermissions.class);
            if(permsAnnotation != null) needed.addAll(Arrays.asList(permsAnnotation.value()));

            clazz = clazz.getSuperclass();
        }

        return needed.toArray(new String[needed.size()]);
    }

    /**
     * Reads the Data Method from the Class.
     * <br>If not found, null is returned.
     * @param clazz to parse
     * @return the name of the Method.
     */
    private static String readBufferGetMethodName(Class<?> clazz) {
        while(clazz != null && clazz != Object.class){
            for(Method method : clazz.getMethods()){
                if(method.isAnnotationPresent(BufferGet.class)) return method.getName();
            }

            clazz = clazz.getSuperclass();
        }

        return null;
    }

    /**
     * Reads the Data Method from the Class.
     * <br>If not found, null is returned.
     * @param clazz to parse
     * @return the name of the Method.
     */
    private static String readBufferAddMethodName(Class<?> clazz) {
        while(clazz != null && clazz != Object.class){
            for(Method method : clazz.getMethods()){
                if(method.isAnnotationPresent(BufferAdd.class)) return method.getName();
            }

            clazz = clazz.getSuperclass();
        }

        return null;
    }


    /**
     * If the Class is a sensor.
     * @param clazz to check.
     * @return true if is sensor.
     */
    private static boolean isBuffer(Class<?> clazz) {
        return clazz != null && Buffer.class.isAssignableFrom(clazz) && clazz.isAnnotationPresent(BufferInfo.class);
    }

    /**
     * Reads the DisplayName from the Class.
     * @param clazz th read from.
     * @return the displayName.
     */
    private static String readDisplayName(Class<?> clazz) {
        BufferInfo sensorConfig = clazz.getAnnotation(BufferInfo.class);
        return sensorConfig == null ? null : sensorConfig.displayName();
    }


    /**
     * Reads the Description from the Sensor.
     * @return the read Description or 'None' if none given.
     */
    private static String readDescription(Class<?> clazz) {
        BufferInfo sensorConfig = clazz.getAnnotation(BufferInfo.class);
        return sensorConfig == null ? "None" : sensorConfig.description();
    }



    /**
     * Reads the Needed Classes from the Class passed.
     * <br>This does a iterative Lookup!
     * @param toReadFrom to use.
     * @return the needed Classes.
     */
    private static Collection<Class<?>> readNeededClasses(Class<?> toReadFrom){
       Set<Class<?>> neededClasses = new HashSet<>();

        NeedsOtherClass annotation = toReadFrom.getAnnotation(NeedsOtherClass.class);
        if(annotation == null) return neededClasses;

        //Do a iterative lookup!
        neededClasses.addAll(Arrays.asList(annotation.value()));
        int size = 0;
        while(size != neededClasses.size()){
            size = neededClasses.size();
            for(Class<?> clazz : new HashSet<>(neededClasses)){
                annotation = clazz.getAnnotation(NeedsOtherClass.class);
                if(annotation == null) continue;

                neededClasses.addAll(Arrays.asList(annotation.value()));
            }
        }

        return neededClasses;
    }

    /**
     * Gets all Configuration Elements from the class passed.
     * @param classToInspect to read from.
     * @return a list of all Elements present.
     */
    private static List<ConfigurationElement> readConfigElements(Class<?> classToInspect){
        if(classToInspect == null) return new ArrayList<>();

        List<ConfigurationElement> elements = new ArrayList<>();
        while(classToInspect != null && classToInspect != Object.class){
            Configuration annotation = classToInspect.getAnnotation(Configuration.class);
            if(annotation != null) elements.addAll(Arrays.asList(annotation.elements()));
            classToInspect = classToInspect.getSuperclass();
        }

        return elements;
    }


}
