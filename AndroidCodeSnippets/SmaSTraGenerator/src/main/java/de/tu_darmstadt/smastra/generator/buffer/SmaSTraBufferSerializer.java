package de.tu_darmstadt.smastra.generator.buffer;

import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;

import java.lang.reflect.Type;

import de.tu_darmstadt.smastra.generator.AbstractSmaSTraSerializer;

/**
 * Serializes a SmaSTra Collection.
 * @author Tobias Welther
 */
public class SmaSTraBufferSerializer extends AbstractSmaSTraSerializer<SmaSTraBuffer> {


    private static final String BUFFER_GET_METHOD_PATH = "bufferGet";
    private static final String BUFFER_ADD_METHOD_PATH = "bufferAdd";

    /**
     * Creates a new Serializer for the SmaSTra Collection.
     */
    public SmaSTraBufferSerializer() {
        super("buffer");
    }


    @Override
    public JsonObject serialize(SmaSTraBuffer src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = super.serialize(src, typeOfSrc, context);

        obj.addProperty(BUFFER_ADD_METHOD_PATH, src.getBufferAddMethodName());
        obj.addProperty(BUFFER_GET_METHOD_PATH, src.getBufferGetMethodName());

        return obj;
    }
}
