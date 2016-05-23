package de.tu_darmstadt.smastra.generator;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

/**
 * This is the abstract base for a SmaSTra serialization to a metadata object.
 * @author Tobias Welther
 */
public abstract class AbstractSmaSTraSerializer <T extends SmaSTraElement> implements JsonSerializer<T> {

    protected static final String TYPE_PATH = "type";
    protected static final String MAIN_CLASS_PATH = "mainClass";
    protected static final String DISPLAY_PATH = "display";
    protected static final String DESCRIPTION_PATH = "description";

    protected static final String NEEDED_PERMISSIONS_PATH = "neededPermissions";
    protected static final String NEEDED_CLASSES_PATH = "needs";


    /**
     * The Type of the Serializer.
     */
    protected final String type;


    protected AbstractSmaSTraSerializer(String type) {
        this.type = type;
    }


    @Override
    public JsonObject serialize(T src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = new JsonObject();
        obj.addProperty(TYPE_PATH, type);
        obj.addProperty(MAIN_CLASS_PATH, src.getElementClass().getCanonicalName());
        obj.addProperty(DISPLAY_PATH, src.getDisplayName());
        obj.addProperty(DESCRIPTION_PATH, src.getDescription());

        //Needed Permissions:
        JsonArray permsArray = new JsonArray();
        for(String perm : src.getAndroidPermissions()) permsArray.add(perm);
        obj.add(NEEDED_PERMISSIONS_PATH, permsArray);

        //Needs others:
        JsonArray others = new JsonArray();
        for(Class<?> clazz : src.getNeededClasses()) others.add(clazz.getCanonicalName());
        obj.add(NEEDED_CLASSES_PATH, others);

        return obj;
    }
}
