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

public class EnumDataTypeSerializer implements JsonSerializer<EnumDataType> {


    private static final String TYPE_PATH = "type";
    private static final String NAME_PATH = "name";
    private static final String VALUES_PATH = "values";



    @Override
    public JsonElement serialize(EnumDataType src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject root = new JsonObject();
        root.addProperty(TYPE_PATH, "enum");
        root.addProperty(NAME_PATH, src.getEnumClass().getCanonicalName());

        JsonArray array = new JsonArray();
        root.add(VALUES_PATH, array);
        for(Enum con : src.getEnumClass().getEnumConstants()){
            array.add(con.name());
        }

        return root;
    }
}
