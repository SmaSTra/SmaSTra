package de.tu_darmstadt.smastra.generator.transformation;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import org.junit.Test;

import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;

/**
 * Test for the SmaSTraTransformation Serializer
 * @author Tobias Welther
 */
public class SmaSTraTransformationSerializerTest {


    @Test
    public void serializingTransformationWorks() throws Throwable {
        SmaSTraTransformationSerializer sut = new SmaSTraTransformationSerializer();
        SmaSTraTransformation transformation = new SmaSTraTransformationBuilder()
                .addInput(new Input("arg0",this.getClass()))
                .setDescription("TEST")
                .setDisplayName("TEST")
                .setMethodName("TEST")
                .setClass(this.getClass())
                .setOutput(Output.VOID_OUTPUT)
                .setStatic(false)
                .build();

        JsonElement element = sut.serialize(transformation, SmaSTraTransformation.class, null);

        //Now validate serialization:
        assertTrue(element.isJsonObject());
        JsonObject obj = element.getAsJsonObject();

        assertEquals("transformation", obj.get("type").getAsString());
        assertEquals(this.getClass().getCanonicalName(), obj.get("class").getAsString());
        assertEquals("TEST", obj.get("description").getAsString());
        assertEquals("TEST", obj.get("method").getAsString());
        assertEquals("TEST", obj.get("display").getAsString());
        assertEquals(false, obj.get("static").getAsBoolean());
        assertEquals(Output.VOID_OUTPUT.getOutputParam().getCanonicalName(), obj.get("output").getAsString());

        assertTrue(obj.get("needs").isJsonArray());
        assertEquals(0, obj.get("needs").getAsJsonArray().size());

        assertTrue(obj.get("input").isJsonObject());
        assertEquals(1, obj.get("input").getAsJsonObject().entrySet().size());
        assertEquals(this.getClass().getCanonicalName(), obj.get("input").getAsJsonObject().get("arg0").getAsString());
    }

}
