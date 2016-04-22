package de.tu_darmstadt.smastra.generator.sensor;

import org.junit.Test;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.SkipParsing;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;
import de.tu_darmstadt.smastra.sensors.Vector3d;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertNotNull;
import static junit.framework.Assert.assertNull;
import static junit.framework.Assert.assertTrue;

/**
 * This is a Test for the Sensor Parser class.
 * @author Tobias Welther
 */
@SkipParsing
public class SensorParserTest {



    @Test
    public void testMinimal1ParseWorks(){
        SmaSTraSensor sut = SmaSTraClassSensorParser.readFromClass(TestClass1.class);

        assertNotNull(sut);

        assertEquals(TestClass1.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("method1", sut.getDataMethodName());
        assertEquals(Vector3d.class, sut.getOutput().getOutputParam());
        assertEquals("Does Stuff", sut.getDescription());

        assertTrue(sut.getNeededClasses().contains(SensorParserTest.class));
    }


    /* For testMinimal1ParseWorks  */
    @SkipParsing
    @SensorConfig(displayName = "method1", description = "Does Stuff")
    @NeedsOtherClass(SensorParserTest.class)
    private static class TestClass1 implements Sensor {

        @SensorOutput
        public Vector3d method1(){ return null; }

    }



    @Test
    public void testMinimal2ParseWorks(){
        SmaSTraSensor sut = SmaSTraClassSensorParser.readFromClass(TestClass2.class);

        assertNotNull(sut);
        assertEquals(TestClass2.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("method1", sut.getDataMethodName());
        assertEquals(Vector3d.class, sut.getOutput().getOutputParam());
        assertEquals("None", sut.getDescription());
        assertTrue(sut.getNeededClasses().isEmpty());
    }


    /* For testMinimal2ParseWorks */
    @SkipParsing
    @SensorConfig(displayName = "method1")
    private static class TestClass2 implements Sensor {

        @SensorOutput
        public Vector3d method1(){ return null; }
    }


    @Test
    public void testSkipMethodWithoutBaseAnnotationWorks() {
        assertNull(SmaSTraClassSensorParser.readFromClass(TestClass3.class));
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    private static class TestClass3 implements Sensor {

        public Vector3d method1(Vector3d vec1){ return null; }

        public void method2(){}
    }


    @Test
    public void testSkipMethodWithoutMethodAnnotationWorks() throws Throwable {
        assertNull(SmaSTraClassSensorParser.readFromClass(TestClass4.class));
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @SensorConfig(displayName = "fsaf")
    private static class TestClass4 implements Sensor {

        public Vector3d method1(Vector3d vec1){ return null; }

        public void method2(){}
    }

}