package de.tu_darmstadt.smastra.generator.transformation;

import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;

import java.lang.reflect.Type;

import de.tu_darmstadt.smastra.generator.AbstractSmaSTraSerializer;
import de.tu_darmstadt.smastra.generator.elements.Input;

/**
 * Serializes a Transformation.
 * @author Tobias Welther
 */
public class SmaSTraTransformationSerializer extends AbstractSmaSTraSerializer<SmaSTraTransformation> {

    private static final String OUTPUT_PATH = "output";

    private static final String METHOD_PATH = "method";
    private static final String STATIC_PATH = "static";
    private static final String INPUT_PATH = "input";


    /**
     * Creates a new Serializer for Transformations.
     */
    public SmaSTraTransformationSerializer() {
        super("transformation");
    }


    @Override
    public JsonObject serialize(SmaSTraTransformation src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = super.serialize(src, typeOfSrc, context);

        obj.addProperty(METHOD_PATH, src.getMethodName());
        obj.addProperty(STATIC_PATH, src.isStatic());
        obj.addProperty(OUTPUT_PATH, src.getOutput().getOutputParam().getCanonicalName());

        //Input:
        JsonObject input = new JsonObject();
        for(Input in : src.getInputs()){ input.addProperty(in.getName(), in.getParameter().getCanonicalName()); }
        obj.add(INPUT_PATH, input);

        return obj;
    }
}
