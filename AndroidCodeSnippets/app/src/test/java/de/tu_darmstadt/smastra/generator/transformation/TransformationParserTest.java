package de.tu_darmstadt.smastra.generator.transformation;

import org.junit.Test;

import java.util.Collection;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.SkipParsing;
import de.tu_darmstadt.smastra.markers.elements.Transformation;
import de.tu_darmstadt.smastra.sensors.Vector3d;

import static de.tu_darmstadt.smastra.generator.elements.Output.VOID_OUTPUT;
import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertNotNull;
import static junit.framework.Assert.assertTrue;

/**
 * This is a Test for the Transformation Parser class.
 * @author Tobias Welther
 */
@SkipParsing
public class TransformationParserTest {



    @Test
    public void testMinimal1ParseWorks(){
        Collection<SmaSTraTransformation> transactions = SmaSTraClassTransformationParser.readFromClass(TestClass1.class);

        assertEquals(1, transactions.size());

        SmaSTraTransformation sut = transactions.iterator().next();
        assertEquals(TestClass1.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("method1", sut.getMethodName());
        assertEquals(VOID_OUTPUT, sut.getOutput());
        assertEquals("Does Stuff", sut.getDescription());
        assertEquals(1, sut.getInputs().size());
        assertEquals(Vector3d.class, sut.getInputs().get(0).getParameter());
        assertEquals(false, sut.isStatic());

        assertTrue(sut.getNeededClasses().contains(TransformationParserTest.class));
    }


    /* For testMinimal1ParseWorks  */
    @SkipParsing
    @NeedsOtherClass(TransformationParserTest.class)
    private static class TestClass1 implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {

        @Transformation(displayName = "method1", desctiption = "Does Stuff")
        public void method1(Vector3d vec1){}

    }



    @Test
    public void testMinimal2ParseWorks(){
        Collection<SmaSTraTransformation> transactions = SmaSTraClassTransformationParser.readFromClass(TestClass2.class);

        assertEquals(1, transactions.size());

        SmaSTraTransformation sut = transactions.iterator().next();
        assertEquals(TestClass2.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("method1", sut.getMethodName());
        assertEquals(Vector3d.class, sut.getOutput().getOutputParam());
        assertEquals("None", sut.getDescription());
        assertEquals(1, sut.getInputs().size());
        assertEquals(Vector3d.class, sut.getInputs().get(0).getParameter());
        assertEquals(true, sut.isStatic());
        assertTrue(sut.getNeededClasses().isEmpty());
    }


    /* For testMinimal2ParseWorks */
    @SkipParsing
    private static class TestClass2 implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {

        @Transformation(displayName = "method1")
        public static Vector3d method1(Vector3d vec1){ return null; }
    }

    @Test
    public void testParseMultipleMethodsWorks(){
        Collection<SmaSTraTransformation> transactions = SmaSTraClassTransformationParser.readFromClass(TestClass3.class);

        assertEquals(2, transactions.size());

        SmaSTraTransformation sut1 = getWithName("method1", transactions);
        assertNotNull(sut1);

        assertEquals(TestClass3.class, sut1.getElementClass());
        assertEquals("method1", sut1.getDisplayName());
        assertEquals("method1", sut1.getMethodName());
        assertEquals(Vector3d.class, sut1.getOutput().getOutputParam());
        assertEquals("None", sut1.getDescription());
        assertEquals(1, sut1.getInputs().size());
        assertEquals(Vector3d.class, sut1.getInputs().get(0).getParameter());
        assertEquals(true, sut1.isStatic());
        assertTrue(sut1.getNeededClasses().isEmpty());


        SmaSTraTransformation sut2 = getWithName("method2", transactions);
        assertNotNull(sut2);

        assertEquals(TestClass3.class, sut2.getElementClass());
        assertEquals("method2", sut2.getDisplayName());
        assertEquals("method2", sut2.getMethodName());
        assertEquals(VOID_OUTPUT, sut2.getOutput());
        assertEquals("Does Stuff", sut2.getDescription());
        assertEquals(1, sut2.getInputs().size());
        assertEquals(Vector3d.class, sut2.getInputs().get(0).getParameter());
        assertEquals(false, sut2.isStatic());
    }


    /* For testParseMultipleMethodsWorks */
    @SkipParsing
    private static class TestClass3 implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {

        @Transformation(displayName = "method1")
        public static Vector3d method1(Vector3d vec1){ return null; }

        @Transformation(displayName = "method2", desctiption = "Does Stuff")
        public void method2(Vector3d vec1){}

    }


    @Test
    public void testSkipMethodWithoutAnnotationWorks() {
        Collection<SmaSTraTransformation> transactions = SmaSTraClassTransformationParser.readFromClass(TestClass4.class);

        assertEquals(1, transactions.size());
        assertEquals("method1", transactions.iterator().next().getMethodName());
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    private static class TestClass4 implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {

        @Transformation(displayName = "method1")
        public Vector3d method1(Vector3d vec1){ return null; }

        public void method2(){}
    }

    @Test
    public void testClassNotInstanceOfTransformationDoesNotReadIt() {
        Collection<SmaSTraTransformation> transactions = SmaSTraClassTransformationParser.readFromClass(TestClass5.class);
        assertEquals(0, transactions.size());
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    private static class TestClass5 {

        @Transformation(displayName = "method1")
        public Vector3d method1(Vector3d vec1){ return null; }

        public void method2(){}
    }


    /**
     * Helper method to read the correct method from methods.
     * @param name to search
     * @param transformations to search in.
     * @return the method if found, null otherwise.
     */
    private SmaSTraTransformation getWithName(String name, Collection<SmaSTraTransformation> transformations){
        for(SmaSTraTransformation transformation : transformations){
            if(transformation.getMethodName().equals(name)) return transformation;
        }

        return null;
    }

}
