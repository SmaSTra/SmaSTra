package de.tu_darmstadt.smastra.generator.buffer;

import com.google.gson.JsonObject;
import com.google.gson.JsonPrimitive;

import org.junit.Test;

import de.tu_darmstadt.smastra.generator.sensor.SmaSTraSensor;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;

/**
 * Test for the SmaSTraSensor Serializer
 * @author Tobias Welther
 */
public class SmaSTraBufferSerializerTest {


    @Test
    public void serializingSensorWorks() throws Throwable {
        SmaSTraBufferSerializer sut = new SmaSTraBufferSerializer();
        SmaSTraBuffer buffer = new SmaSTraBufferBuilder()
                .setDescription("TEST")
                .setDisplayName("TEST")
                .setBufferAddMethodName("TEST")
                .setBufferGetMethodName("TEST")
                .setClass(this.getClass())
                .build();

        JsonObject obj = sut.serialize(buffer, SmaSTraSensor.class, null);

        //Now validate serialization:
        assertEquals("buffer", obj.get("type").getAsString());
        assertEquals(this.getClass().getCanonicalName(), obj.get("mainClass").getAsString());
        assertEquals("TEST", obj.get("description").getAsString());
        assertEquals("TEST", obj.get("display").getAsString());
        assertEquals("TEST", obj.get("bufferGet").getAsString());
        assertEquals("TEST", obj.get("bufferAdd").getAsString());

        assertTrue(obj.get("needs").isJsonArray());
        assertEquals(0, obj.get("needs").getAsJsonArray().size());
    }

}
