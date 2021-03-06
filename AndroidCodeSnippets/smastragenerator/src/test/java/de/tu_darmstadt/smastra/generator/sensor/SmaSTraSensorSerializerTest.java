package de.tu_darmstadt.smastra.generator.sensor;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonPrimitive;

import org.junit.Test;

import de.tu_darmstadt.smastra.generator.elements.Output;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;

/**
 * Test for the SmaSTraSensor Serializer
 * @author Tobias Welther
 */
public class SmaSTraSensorSerializerTest {


    @Test
    public void serializingSensorWorks() throws Throwable {
        SmaSTraSensorSerializer sut = new SmaSTraSensorSerializer();
        SmaSTraSensor sensor = new SmaSTraSensorBuilder()
                .setDescription("TEST")
                .setDisplayName("TEST")
                .setDataMethodName("TEST")
                .setClass(this.getClass())
                .setOutput(new Output(String.class))
                .build();

        JsonObject obj = sut.serialize(sensor, SmaSTraSensor.class, null);

        //Now validate serialization:
        assertEquals("sensor", obj.get("type").getAsString());
        assertEquals(this.getClass().getCanonicalName(), obj.get("mainClass").getAsString());
        assertEquals("TEST", obj.get("description").getAsString());
        assertEquals("TEST", obj.get("dataMethod").getAsString());
        assertEquals("TEST", obj.get("display").getAsString());
        assertEquals(String.class.getCanonicalName(), obj.get("output").getAsString());

        assertTrue(obj.get("needs").isJsonArray());
        assertEquals(0, obj.get("needs").getAsJsonArray().size());
    }

}
