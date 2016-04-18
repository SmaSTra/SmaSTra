package de.tu_darmstadt.smastra.generator.sensor;

import java.lang.reflect.Method;
import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.Set;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * Parses a class to a bunch of SmaSTra Sensors.
 *
 * @author Tobias Welther
 */
public class SmaSTraClassSensorParser {


    /**
     * Reads the Sensor from the Class.
     * @param clazz to read from.
     * @return the Transactions
     */
    public static SmaSTraSensor readFromClass(Class<?> clazz){
        if(clazz == null) return null;

        //Read the class:
        if(!isSensor(clazz)) return null;

        try{
            SmaSTraSensorBuilder builder = new SmaSTraSensorBuilder();
            builder.setClass(clazz);

            builder.setDisplayName(readDisplayName(clazz));
            builder.setDataMethodName(readDataMethodName(clazz));
            builder.setDescription(readDescription(clazz));
            builder.setOutput(readOutput(clazz));
            builder.addNeededClass(readNeededClasses(clazz));

           return builder.build();
        }catch(ElementGenerationFailedException exp){
            exp.printStackTrace();
        }

        return null;
    }

    /**
     * Reads the Data Method from the Class.
     * <br>If not found, null is returned.
     * @param clazz to parse
     * @return the name of the Method.
     */
    private static String readDataMethodName(Class<?> clazz) {
        for(Method method : clazz.getMethods()){
            if(method.getAnnotation(SensorOutput.class) != null) return method.getName();
        }

        return null;
    }


    /**
     * If the Class is a sensor.
     * @param clazz to check.
     * @return true if is sensor.
     */
    private static boolean isSensor(Class<?> clazz) {
        return clazz != null && Sensor.class.isAssignableFrom(clazz) && clazz.getAnnotation(SensorConfig.class) != null;
    }

    /**
     * Reads the DisplayName from the Class.
     * @param clazz th read from.
     * @return the displayName.
     */
    private static String readDisplayName(Class<?> clazz) {
        SensorConfig sensorConfig = clazz.getAnnotation(SensorConfig.class);
        return sensorConfig == null ? null : sensorConfig.displayName();
    }

    /**
     * Reads an Output from the Sensor-Class passed.
     * @param clazz to read from.
     * @return the read Output.
     */
    private static Output readOutput(Class<?> clazz) {
        for(Method method : clazz.getMethods()){
            if(method.getAnnotation(SensorOutput.class) != null) {
                return new Output(method.getReturnType());
            }
        }

        return null;
    }


    /**
     * Reads the Description from the Sensor.
     * @return the read Description or 'None' if none given.
     */
    private static String readDescription(Class<?> clazz) {
        SensorConfig sensorConfig = clazz.getAnnotation(SensorConfig.class);
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


}
