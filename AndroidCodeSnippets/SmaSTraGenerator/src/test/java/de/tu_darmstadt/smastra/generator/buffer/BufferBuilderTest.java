package de.tu_darmstadt.smastra.generator.buffer;

import org.junit.Test;

import java.util.Arrays;
import java.util.Collection;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.generator.sensor.SmaSTraSensor;
import de.tu_darmstadt.smastra.generator.sensor.SmaSTraSensorBuilder;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;

/**
 * Test for the BufferBuilder.
 * @author Tobias Welther
 */
public class BufferBuilderTest {


    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfEmpty() throws Throwable{
        new SmaSTraBufferBuilder().build();
    }


    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfAddMissing() throws Throwable{
        new SmaSTraBufferBuilder().setClass(this.getClass()).setDisplayName("Test").setBufferGetMethodName("TEST").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfGetMissing() throws Throwable {
        new SmaSTraBufferBuilder().setClass(this.getClass()).setDisplayName("Test").setBufferAddMethodName("TEST").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfClassMissing() throws Throwable{
        new SmaSTraBufferBuilder().setBufferAddMethodName("TEST").setBufferGetMethodName("TEST").setDisplayName("Test").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfDisplayNameMissing() throws Throwable{
        new SmaSTraBufferBuilder().setBufferAddMethodName("TEST").setBufferGetMethodName("TEST").setClass(this.getClass()).build();
    }


    @Test
    public void testBuilderSettersAndGettersWorkCorrect() throws ElementGenerationFailedException {
        String name = "Test";
        String name2 = "Test2";
        String description = "Something";
        String displayName = "Test";
        Class<?> clazz = name.getClass();
        String[] permissions = new String[]{"Perm1", "Perm2"};
        Collection<Class<?>> needsOtherClasses = Arrays.asList(this.getClass(), SmaSTraSensor.class);

        SmaSTraBufferBuilder sut = new SmaSTraBufferBuilder()
            .setBufferAddMethodName(name)
            .setBufferGetMethodName(name2)
            .setClass(clazz)
            .setDescription(description)
            .setDisplayName(displayName)
            .addNeededClass(needsOtherClasses)
            .setAndroidPermissions(permissions);

        assertEquals(description, sut.getDescription());
        assertEquals(clazz, sut.getClazz());
        assertEquals(permissions, sut.getAndroidPermissions());
        assertEquals(name, sut.getBufferAddMethodName());
        assertEquals(name2, sut.getBufferGetMethodName());

        //At least check for Needed Classes:
        for(Class<?> cl : needsOtherClasses) assertTrue(sut.getNeedsOtherClasses().contains(cl));
    }

    @Test
    public void testBuilderBuildsCorrectSensor() throws ElementGenerationFailedException {
        String name = "Test";
        String name2 = "Test2";
        String description = "Something";
        String displayName = "Test";
        Class<?> clazz = name.getClass();
        Collection<Class<?>> needsOtherClasses = Arrays.asList(this.getClass(), SmaSTraSensor.class);

        SmaSTraBufferBuilder sut = new SmaSTraBufferBuilder()
            .setBufferAddMethodName(name)
            .setBufferGetMethodName(name2)
            .setClass(clazz)
            .setDescription(description)
            .setDisplayName(displayName)
            .addNeededClass(needsOtherClasses);

        SmaSTraBuffer buffer = sut.build();
        assertEquals(description, buffer.getDescription());
        assertEquals(displayName, buffer.getDisplayName());
        assertEquals(clazz, buffer.getElementClass());
        assertEquals(name, buffer.getBufferAddMethodName());
        assertEquals(name2, buffer.getBufferGetMethodName());

        //At least check for Needed Classes:
        for(Class<?> cl : needsOtherClasses) assertTrue(buffer.getNeededClasses().contains(cl));
    }

}
