package de.tu_darmstadt.smastra.generator.sensor;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.generator.extras.AbstractSmaSTraExtra;
import de.tu_darmstadt.smastra.generator.extras.ExtraFactory;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.config.Configuration;
import de.tu_darmstadt.smastra.markers.elements.config.ConfigurationElement;
import de.tu_darmstadt.smastra.markers.elements.extras.Extras;
import de.tu_darmstadt.smastra.markers.elements.proxyproperties.ProxyProperty;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStop;
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
            builder.addProxyProperties(readProxyProperties(clazz));
            builder.addExtras(readExtras(clazz));

            builder.addConfigurationElements(readConfigElements(clazz));

            builder.setStartMethod(readStartMethod(clazz));
            builder.setStopMethod(readStopMethod(clazz));

           return builder.build();
        }catch(ElementGenerationFailedException exp){
            exp.printStackTrace();
        }

        return null;
    }


    /**
     * Reads the extras from a class.
     * @param clazz to read from.
     * @return the extras.
     */
    private static List<AbstractSmaSTraExtra> readExtras(Class<?> clazz){
        List<AbstractSmaSTraExtra> result = new ArrayList<>();

        //Check for Super-Definitions:
        while(clazz != null && clazz != Object.class) {
            Extras extras = clazz.getAnnotation(Extras.class);
            if (extras != null) {
                for (Object obj : extras.broadcasts()) result.add(ExtraFactory.buildFromExtra(obj));
                for (Object obj : extras.services()) result.add(ExtraFactory.buildFromExtra(obj));
                for (Object obj : extras.libraries()) result.add(ExtraFactory.buildFromExtra(obj));
                for (Object obj : extras.permissions()) result.add(ExtraFactory.buildFromExtra(obj));
            }

            clazz = clazz.getSuperclass();
        }

        return result;
    }

    /**
     * Reads the start method from the Class passed.
     * @return the name of the start Method.
     */
    private static String readStartMethod(Class<?> clazz) {
        //Check for Super-Definitions:
        while(clazz != null && clazz != Object.class){
            for(Method method : clazz.getMethods()){
                if(method.isAnnotationPresent(SensorStart.class)) return method.getName();
            }

            clazz = clazz.getSuperclass();
        }

        return null;
    }

    /**
     * Reads the stop method from the Class passed.
     * @return the name of the Stop Method.
     */
    private static String readStopMethod(Class<?> clazz) {
        //Check for Super-Definitions:
        while(clazz != null && clazz != Object.class){
            for(Method method : clazz.getMethods()){
                if(method.isAnnotationPresent(SensorStop.class)) return method.getName();
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
    private static String readDataMethodName(Class<?> clazz) {
        while(clazz != null && clazz != Object.class){
            for(Method method : clazz.getMethods()){
                if(method.isAnnotationPresent(SensorOutput.class)) return method.getName();
            }

            clazz = clazz.getSuperclass();
        }

        return null;
    }


    /**
     * Reads the proxy properties from the Class
     * @return the ProxyProperties read.
     */
    private static List<ProxyPropertyObj> readProxyProperties(Class<?> clazz){
        List<ProxyPropertyObj> properties = new ArrayList<>();
        while(clazz != null && clazz != Object.class){
            for(Method method : clazz.getMethods()){
                ProxyProperty annotation = method.getAnnotation(ProxyProperty.class);
                if(annotation == null) continue;

                try{
                    properties.add(new ProxyPropertyObj(method, annotation));
                }catch(IllegalArgumentException exp){
                    System.err.println(exp.getMessage());
                }
            }

            clazz = clazz.getSuperclass();
        }

        return properties;
    }


    /**
     * If the Class is a sensor.
     * @param clazz to check.
     * @return true if is sensor.
     */
    private static boolean isSensor(Class<?> clazz) {
        return clazz != null && Sensor.class.isAssignableFrom(clazz) && clazz.isAnnotationPresent(SensorConfig.class);
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
        while(clazz != null && clazz != Object.class) {
            for (Method method : clazz.getMethods()) {
                if (method.getAnnotation(SensorOutput.class) != null) {
                    return new Output(method.getReturnType());
                }
            }

            clazz = clazz.getSuperclass();
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

    /**
     * Gets all Configuration Elements from the class passed.
     * @param classToInspect to read from.
     * @return a list of all Elements present.
     */
    private static List<ConfigurationElement> readConfigElements(Class<? extends Object> classToInspect){
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
