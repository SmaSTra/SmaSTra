package de.tu_darmstadt.smastra.generator;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonElement;

import org.reflections.Reflections;

import java.util.Collection;
import java.util.HashSet;

import de.tu_darmstadt.smastra.generator.transaction.SmaSTraClassTransactionParser;
import de.tu_darmstadt.smastra.generator.transaction.SmaSTraTransformation;
import de.tu_darmstadt.smastra.generator.transaction.SmaSTraTransformationSerializer;
import de.tu_darmstadt.smastra.markers.SkipParsing;
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
            .registerTypeAdapter(SmaSTraTransformation.class, new SmaSTraTransformationSerializer())
            .create();
    }

    /**
     * Reads the transformations present in the Classloaded.
     * @return the transformatiaons passed.
     */
    public Collection<SmaSTraTransformation> readTransformationsFromClasslaoded(){
        Collection<SmaSTraTransformation> transformations = new HashSet<>();
        Collection<Class<? extends Transformation>> classes = getAllClassesOf(Transformation.class);
        for(Class<?> clazz : classes) {
            if (hasSkipAnnotation(clazz)) continue;

            Collection<SmaSTraTransformation> classTransformations = SmaSTraClassTransactionParser.readFromClass(clazz);
            if (classTransformations != null && !classTransformations.isEmpty()) {
                transformations.addAll(classTransformations);
            }
        }

        return transformations;
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
