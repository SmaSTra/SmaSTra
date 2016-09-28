package de.tu_darmstadt.smastra.generator.datatype;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

/**
 * This is the Serializer for Data-Types.
 *
 * @author Tobias Welther
 */

public class DataTypeSerializer implements JsonSerializer<DataType> {


    private static final String NAME_PATH = "name";
    private static final String TEMPLATE_PATH = "template";
    private static final String TYPES_PATH = "types";
    private static final String CREATABLE_PATH = "creatable";



    @Override
    public JsonElement serialize(DataType src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject root = new JsonObject();
        root.addProperty(NAME_PATH, src.getClazz().getCanonicalName());
        root.addProperty(TEMPLATE_PATH, src.getTemplate());
        root.addProperty(CREATABLE_PATH, src.isCreatable());

        JsonArray typesArray = new JsonArray();
        root.add(TYPES_PATH, typesArray);
        for(Class<?> type : src.getTypeParams()){
            typesArray.add(type.getSimpleName());
        }

        return root;
    }
}
