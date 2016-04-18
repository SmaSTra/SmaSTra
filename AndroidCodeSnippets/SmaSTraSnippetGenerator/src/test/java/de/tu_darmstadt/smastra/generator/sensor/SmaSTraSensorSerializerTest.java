package de.tu_darmstadt.smastra.generator.sensor;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import org.junit.Test;

import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.sensors.Vector3d;

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
                .setOutput(new Output(Vector3d.class))
                .build();

        JsonElement element = sut.serialize(sensor, SmaSTraSensor.class, null);

        //Now validate serialization:
        assertTrue(element.isJsonObject());
        JsonObject obj = element.getAsJsonObject();

        assertEquals("sensor", obj.get("type").getAsString());
        assertEquals(this.getClass().getCanonicalName(), obj.get("class").getAsString());
        assertEquals("TEST", obj.get("description").getAsString());
        assertEquals("TEST", obj.get("dataMethod").getAsString());
        assertEquals("TEST", obj.get("display").getAsString());
        assertEquals(Vector3d.class.getCanonicalName(), obj.get("output").getAsString());

        assertTrue(obj.get("needs").isJsonArray());
        assertEquals(0, obj.get("needs").getAsJsonArray().size());
    }

}
