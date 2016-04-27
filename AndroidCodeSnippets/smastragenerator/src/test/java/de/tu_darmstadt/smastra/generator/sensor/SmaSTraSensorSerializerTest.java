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
                .setAndroidPermissions(new String[]{"Test1", "Test2"})
                .setOutput(new Output(String.class))
                .build();

        JsonElement element = sut.serialize(sensor, SmaSTraSensor.class, null);

        //Now validate serialization:
        assertTrue(element.isJsonObject());
        JsonObject obj = element.getAsJsonObject();

        assertEquals("sensor", obj.get("type").getAsString());
        assertEquals(this.getClass().getCanonicalName(), obj.get("mainClass").getAsString());
        assertEquals("TEST", obj.get("description").getAsString());
        assertEquals("TEST", obj.get("dataMethod").getAsString());
        assertEquals("TEST", obj.get("display").getAsString());
        assertEquals(String.class.getCanonicalName(), obj.get("output").getAsString());

        assertEquals(true, obj.get("neededPermissions").isJsonArray());
        assertTrue(obj.get("neededPermissions").getAsJsonArray().contains(new JsonPrimitive("Test1")));
        assertTrue(obj.get("neededPermissions").getAsJsonArray().contains(new JsonPrimitive("Test2")));


        assertTrue(obj.get("needs").isJsonArray());
        assertEquals(0, obj.get("needs").getAsJsonArray().size());
    }

}
