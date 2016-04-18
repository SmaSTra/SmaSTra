package de.tu_darmstadt.smastra.generator.sensor;

import org.junit.Test;

import java.util.Arrays;
import java.util.Collection;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Output;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;

/**
 * Test for the SensorBuilder.
 * @author Tobias Welther
 */
public class SensorBuilderTest {


    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfEmpty() throws Throwable{
        new SmaSTraSensorBuilder().build();
    }


    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfNameMissing() throws Throwable{
        new SmaSTraSensorBuilder().setClass(this.getClass()).setDisplayName("Test").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfClassMissing() throws Throwable{
        new SmaSTraSensorBuilder().setDataMethodName("TEST").setDisplayName("Test").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfDisplayNameMissing() throws Throwable{
        new SmaSTraSensorBuilder().setDataMethodName("TEST").setClass(this.getClass()).build();
    }


    @Test
    public void testBuilderSettersAndGettersWorkCorrect() throws ElementGenerationFailedException {
        String name = "Test";
        String description = "Something";
        String displayName = "Test";
        Class<?> clazz = name.getClass();
        Output output = new Output(this.getClass());
        Collection<Class<?>> needsOtherClasses = Arrays.asList(this.getClass(), SmaSTraSensor.class);

        SmaSTraSensorBuilder sut = new SmaSTraSensorBuilder()
            .setDataMethodName(name)
            .setClass(clazz)
            .setDescription(description)
            .setDisplayName(displayName)
            .setOutput(output)
            .addNeededClass(needsOtherClasses);

        assertEquals(name, sut.getDataMethodName());
        assertEquals(description, sut.getDescription());
        assertEquals(clazz, sut.getClazz());
        assertEquals(output, sut.getOutput());

        //At least check for Needed Classes:
        for(Class<?> cl : needsOtherClasses) assertTrue(sut.getNeedsOtherClasses().contains(cl));
    }

    @Test
    public void testBuilderBuildsCorrectSensor() throws ElementGenerationFailedException {
        String name = "Test";
        String description = "Something";
        String displayName = "Test";
        Class<?> clazz = name.getClass();
        Output output = new Output(this.getClass());
        Collection<Class<?>> needsOtherClasses = Arrays.asList(this.getClass(), SmaSTraSensor.class);

        SmaSTraSensorBuilder sut = new SmaSTraSensorBuilder()
            .setDataMethodName(name)
            .setClass(clazz)
            .setDescription(description)
            .setDisplayName(displayName)
            .setOutput(output)
            .addNeededClass(needsOtherClasses);

        SmaSTraSensor sensor = sut.build();
        assertEquals(name, sensor.getDataMethodName());
        assertEquals(description, sensor.getDescription());
        assertEquals(displayName, sensor.getDisplayName());
        assertEquals(clazz, sensor.getElementClass());
        assertEquals(output, sensor.getOutput());

        //At least check for Needed Classes:
        for(Class<?> cl : needsOtherClasses) assertTrue(sensor.getNeededClasses().contains(cl));
    }

}
