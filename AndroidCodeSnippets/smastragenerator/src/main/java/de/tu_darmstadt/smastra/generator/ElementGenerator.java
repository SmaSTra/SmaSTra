package de.tu_darmstadt.smastra.generator;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import org.reflections.Reflections;

import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;

import de.tu_darmstadt.smastra.generator.buffer.SmaSTraBuffer;
import de.tu_darmstadt.smastra.generator.buffer.SmaSTraBufferSerializer;
import de.tu_darmstadt.smastra.generator.buffer.SmaSTraClassBufferParser;
import de.tu_darmstadt.smastra.generator.sensor.SmaSTraClassSensorParser;
import de.tu_darmstadt.smastra.generator.sensor.SmaSTraSensor;
import de.tu_darmstadt.smastra.generator.sensor.SmaSTraSensorSerializer;
import de.tu_darmstadt.smastra.generator.transformation.SmaSTraClassTransformationParser;
import de.tu_darmstadt.smastra.generator.transformation.SmaSTraTransformation;
import de.tu_darmstadt.smastra.generator.transformation.SmaSTraTransformationSerializer;
import de.tu_darmstadt.smastra.markers.SkipParsing;
import de.tu_darmstadt.smastra.markers.interfaces.Buffer;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;
import de.tu_darmstadt.smastra.markers.interfaces.Transformation;

/**
 * This class reads the Classes present in the classloader and Generates Meta infos from them.
 * <br>This only works from source!
 *
 * @author Tobias Welther
 */
public class ElementGenerator {

    /**
     * The Gson Serializer to use.
     */
    private final Gson gson;


    public ElementGenerator() {
        this.gson = new GsonBuilder()
            //Register Serializers:
            .registerTypeAdapter(SmaSTraTransformation.class, new SmaSTraTransformationSerializer())
            .registerTypeAdapter(SmaSTraSensor.class, new SmaSTraSensorSerializer())
            .registerTypeAdapter(SmaSTraBuffer.class, new SmaSTraBufferSerializer())

            //Set the Rest of the config:
            .setPrettyPrinting()

            //Finally build:
            .create();
    }

    /**
     * Reads the transformations present in the Class loaded.
     * @return the transformations passed.
     */
    public Collection<SmaSTraTransformation> readTransformationsFromClassLoaded(){
        Collection<Class<?>> var = new HashSet<>();
        for(Class<? extends Transformation> cl : getAllClassesOf(Transformation.class)) var.add(cl);
        return readTransformationsFromClasses(var);
    }

    /**
     * Reads the transformations present in the Class loaded.
     * @return the transformations passed.
     */
    public Collection<SmaSTraTransformation> readTransformationsFromClasses(Collection<Class<?>> classes){
        Collection<SmaSTraTransformation> transformations = new HashSet<>();
        for(Class<?> clazz : classes) {
            if (hasSkipAnnotation(clazz)) continue;

            Collection<SmaSTraTransformation> classTransformations = SmaSTraClassTransformationParser.readFromClass(clazz);
            if (classTransformations != null && !classTransformations.isEmpty()) {
                transformations.addAll(classTransformations);
            }
        }

        return transformations;
    }

    /**
     * Reads the transformations present in the Class loaded.
     * @return the transformations passed.
     */
    public Collection<SmaSTraTransformation> readTransformationsFromClasses(Class<?>... classes){
        return readTransformationsFromClasses(Arrays.asList(classes));
    }


    /**
     * Reads the Sensors present in the Class loaded.
     * @return the Sensors passed.
     */
    public Collection<SmaSTraSensor> readSensorsFromClassLoaded(){
        Collection<Class<?>> var = new HashSet<>();
        for(Class<? extends Sensor> cl : getAllClassesOf(Sensor.class)) var.add(cl);
        return readSensorsFromClasses(var);
    }


    /**
     * Reads the Sensors present From the classes passed.
     * @param classes to load from
     * @return the Sensors passed.
     */
    public Collection<SmaSTraSensor> readSensorsFromClasses(Class<?>... classes){
        return readSensorsFromClasses(Arrays.asList(classes));
    }


    /**
     * Reads the Sensors present From the classes passed.
     * @param classes to load from
     * @return the Sensors passed.
     */
    public Collection<SmaSTraSensor> readSensorsFromClasses(Collection<Class<?>> classes){
        Collection<SmaSTraSensor> transformations = new HashSet<>();
        for(Class<?> clazz : classes) {
            if (hasSkipAnnotation(clazz)) continue;

            SmaSTraSensor sensor = SmaSTraClassSensorParser.readFromClass(clazz);
            if (sensor != null) transformations.add(sensor);
        }

        return transformations;
    }



    /**
     * Reads the Buffers present in the Class loaded.
     * @return the Buffers passed.
     */
    public Collection<SmaSTraBuffer> readBuffersFromClassLoaded(){
        Collection<Class<?>> var = new HashSet<>();
        for(Class<? extends Buffer> cl : getAllClassesOf(Buffer.class)) var.add(cl);
        return readBuffersFromClasses(var);
    }


    /**
     * Reads the Buffers present From the classes passed.
     * @param classes to load from
     * @return the buffers passed.
     */
    public Collection<SmaSTraBuffer> readBuffersFromClasses(Class<?>... classes){
        return readBuffersFromClasses(Arrays.asList(classes));
    }


    /**
     * Reads the Buffer present From the classes passed.
     * @param classes to load from
     * @return the Buffer passed.
     */
    public Collection<SmaSTraBuffer> readBuffersFromClasses(Collection<Class<?>> classes){
        Collection<SmaSTraBuffer> buffers = new HashSet<>();
        for(Class<?> clazz : classes) {
            if (hasSkipAnnotation(clazz)) continue;

            SmaSTraBuffer sensor = SmaSTraClassBufferParser.readFromClass(clazz);
            if (sensor != null) buffers.add(sensor);
        }

        return buffers;
    }

    /**
     * Reads all Present elements from the Classloaded.
     * @return all present Elements from the Class loader.
     */
    public Collection<SmaSTraElement> getAllElementsFromClassloader(){
        Collection<SmaSTraElement> elements = new HashSet<>();
        elements.addAll(readSensorsFromClassLoaded());
        elements.addAll(readTransformationsFromClassLoaded());
        return elements;
    }


    /**
     * Reads all Present elements from the Classloaded.
     * @return all present Elements from the Class loader.
     */
    public Collection<SmaSTraElement> getAllElementsFromClasses(Class<?>... classes){
        return getAllElementsFromClasses(Arrays.asList(classes));
    }

    /**
     * Reads all Present elements from the Classloaded.
     * @return all present Elements from the Class loader.
     */
    public Collection<SmaSTraElement> getAllElementsFromClasses(Collection<Class<?>> classes){
        Collection<SmaSTraElement> elements = new HashSet<>();
        elements.addAll(readSensorsFromClasses(classes));
        elements.addAll(readTransformationsFromClasses(classes));
        elements.addAll(readBuffersFromClasses(classes));
        return elements;
    }


    /**
     * Returns all Classes present.
     * <br>WARNING: This is pretty slow!
     *
     * @return all classes of that type.
     */
    private <T> Collection<Class<? extends T>> getAllClassesOf(Class<T> clazz){
        Collection<Class<? extends T>> classes = new HashSet<>();
        try{
            Reflections reflections = new Reflections();
            return reflections.getSubTypesOf(clazz);
        }catch(Throwable exp){
            exp.printStackTrace();
        }

        return classes;
    }



    /**
     * Returns true if the Annotation for skipping is present.
     * @param clazz to check
     * @return true if is present.
     */
    private boolean hasSkipAnnotation(Class<?> clazz){
        return clazz != null && clazz.getAnnotation(SkipParsing.class) != null;
    }


    /**
     * Gets the Gson to use for serialization.
     * <br>This one has all the serializers for the SmaSTra Classes.
     *
     * @return the Gson serializer to use for Serialization.
     */
    public Gson getGson() {
        return gson;
    }
}
