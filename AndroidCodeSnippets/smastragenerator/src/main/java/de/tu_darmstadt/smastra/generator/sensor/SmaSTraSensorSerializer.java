package de.tu_darmstadt.smastra.generator.sensor;

import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;

import java.lang.reflect.Type;

import de.tu_darmstadt.smastra.generator.AbstractSmaSTraSerializer;

/**
 * Serializes a SmaSTra Sensor.
 * @author Tobias Welther
 */
public class SmaSTraSensorSerializer extends AbstractSmaSTraSerializer<SmaSTraSensor> {


    private static final String DATA_METHOD_PATH = "dataMethod";
    private static final String OUTPUT_PATH = "output";

    private static final String START_METHOD_PATH = "start";
    private static final String STOP_METHOD_PATH = "stop";

    /**
     * Creates a new Serializer for the SmaSTra Sensor.
     */
    public SmaSTraSensorSerializer() {
        super("sensor");
    }


    @Override
    public JsonObject serialize(SmaSTraSensor src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject obj = super.serialize(src, typeOfSrc, context);

        obj.addProperty(DATA_METHOD_PATH, src.getDataMethodName());
        obj.addProperty(OUTPUT_PATH, src.getOutput().getOutputParam().getCanonicalName());

        //Add start - stop method:
        if(!src.getStartMethod().isEmpty()) obj.addProperty(START_METHOD_PATH, src.getStartMethod());
        if(!src.getStopMethod().isEmpty()) obj.addProperty(STOP_METHOD_PATH, src.getStopMethod());

        return obj;
    }
}
