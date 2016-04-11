package de.tu_darmstadt.smastra.generator.sensor;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

/**
 * Serializes a SmaSTra Sensor.
 * @author Tobias Welther
 */
public class SmaSTraSensorSerializer implements JsonSerializer<SmaSTraSensor> {


    @Override
    public JsonElement serialize(SmaSTraSensor src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = new JsonObject();
        obj.addProperty("type", "sensor");
        obj.addProperty("class", src.getElementClass().getCanonicalName());
        obj.addProperty("display", src.getDisplayName());
        obj.addProperty("dataMethod", src.getDataMethodName());
        obj.addProperty("description", src.getDescription());
        obj.addProperty("output", src.getOutput().getOutputParam().getCanonicalName());

        //Needs others:
        JsonArray others = new JsonArray();
        for(Class<?> clazz : src.getNeededClasses()) others.add(clazz.getCanonicalName());
        obj.add("needs", others);

        return obj;
    }
}
