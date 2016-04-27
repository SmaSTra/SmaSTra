package de.tu_darmstadt.smastra.generator.transformation;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

import de.tu_darmstadt.smastra.generator.elements.Input;

/**
 * Serializes a Transformation.
 * @author Tobias Welther
 */
public class SmaSTraTransformationSerializer implements JsonSerializer<SmaSTraTransformation> {


    @Override
    public JsonElement serialize(SmaSTraTransformation src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = new JsonObject();
        obj.addProperty("type", "transformation");
        obj.addProperty("mainClass", src.getElementClass().getCanonicalName());
        obj.addProperty("display", src.getDisplayName());
        obj.addProperty("method", src.getMethodName());
        obj.addProperty("description", src.getDescription());
        obj.addProperty("static", src.isStatic());
        obj.addProperty("output", src.getOutput().getOutputParam().getCanonicalName());

        //Needed Permissions:
        JsonArray permsArray = new JsonArray();
        for(String perm : src.getAndroidPermissions()) permsArray.add(perm);
        obj.add("neededPermissions", permsArray);

        //Needs others:
        JsonArray others = new JsonArray();
        for(Class<?> clazz : src.getNeededClasses()) others.add(clazz.getCanonicalName());
        obj.add("needs", others);


        //Input:
        JsonObject input = new JsonObject();
        for(Input in : src.getInputs()){ input.addProperty(in.getName(), in.getParameter().getCanonicalName()); }
        obj.add("input", input);

        return obj;
    }
}
