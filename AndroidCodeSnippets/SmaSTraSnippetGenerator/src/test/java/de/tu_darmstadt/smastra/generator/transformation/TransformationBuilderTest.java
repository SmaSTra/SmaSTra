package de.tu_darmstadt.smastra.generator.transformation;

import org.junit.Test;

import java.util.Arrays;
import java.util.Collection;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;

import static junit.framework.Assert.*;

/**
 * Test for the TransformationBuilder.
 * @author Tobias Welther
 */
public class TransformationBuilderTest {


    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfEmpty() throws Throwable{
        new SmaSTraTransformationBuilder().build();
    }


    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfNameMissing() throws Throwable{
        new SmaSTraTransformationBuilder().setClass(this.getClass()).setDisplayName("Test").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfClassMissing() throws Throwable{
        new SmaSTraTransformationBuilder().setMethodName("TEST").setDisplayName("Test").build();
    }

    @Test(expected = ElementGenerationFailedException.class)
    public void testBuilderThrowsErrorIfDisplayNameMissing() throws Throwable{
        new SmaSTraTransformationBuilder().setMethodName("TEST").setClass(this.getClass()).build();
    }


    @Test
    public void testBuilderSettersAndGettersWorkCorrect() throws ElementGenerationFailedException {
        String name = "Test";
        String description = "Something";
        String displayName = "Test";
        Class<?> clazz = name.getClass();
        Input input = new Input("TEST", this.getClass());
        Output output = new Output(this.getClass());
        String[] androidPermissions = new String[]{"TEST", "TEST2"};
        Collection<Class<?>> needsOtherClasses = Arrays.asList(this.getClass(), SmaSTraTransformation.class);

        SmaSTraTransformationBuilder sut = new SmaSTraTransformationBuilder()
            .setMethodName(name)
            .setClass(clazz)
            .setDescription(description)
            .setDisplayName(displayName)
            .addInput(input)
            .setOutput(output)
            .setAndroidPermissions(androidPermissions)
            .addNeededClass(needsOtherClasses);

        assertEquals(name, sut.getMethodName());
        assertEquals(description, sut.getDescription());
        assertEquals(clazz, sut.getClazz());
        assertEquals(input, sut.getInputs().get(0));
        assertEquals(output, sut.getOutput());
        assertEquals(androidPermissions, sut.getAndroidPermissions());

        //At least check for Needed Classes:
        for(Class<?> cl : needsOtherClasses) assertTrue(sut.getNeedsOtherClasses().contains(cl));
    }

    @Test
    public void testBuilderBuildsCorrectTransaction() throws ElementGenerationFailedException {
        String name = "Test";
        String description = "Something";
        String displayName = "Test";
        Class<?> clazz = name.getClass();
        Input input = new Input("TEST", this.getClass());
        Output output = new Output(this.getClass());
        Collection<Class<?>> needsOtherClasses = Arrays.asList(this.getClass(), SmaSTraTransformation.class);

        SmaSTraTransformationBuilder sut = new SmaSTraTransformationBuilder()
            .setMethodName(name)
            .setClass(clazz)
            .setDescription(description)
            .setDisplayName(displayName)
            .addInput(input)
            .setOutput(output)
            .addNeededClass(needsOtherClasses);

        SmaSTraTransformation transaction = sut.build();
        assertEquals(name, transaction.getMethodName());
        assertEquals(description, transaction.getDescription());
        assertEquals(displayName, transaction.getDisplayName());
        assertEquals(clazz, transaction.getElementClass());
        assertEquals(input, transaction.getInputs().get(0));
        assertEquals(output, transaction.getOutput());

        //At least check for Needed Classes:
        for(Class<?> cl : needsOtherClasses) assertTrue(transaction.getNeededClasses().contains(cl));
    }

}
