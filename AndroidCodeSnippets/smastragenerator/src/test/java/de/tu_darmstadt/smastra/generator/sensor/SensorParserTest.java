package de.tu_darmstadt.smastra.generator.sensor;

import org.junit.Test;

import java.util.Collection;
import java.util.Map;

import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.SkipParsing;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.ProxyProperty;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertFalse;
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
        assertEquals(String.class, sut.getOutput().getOutputParam());
        assertEquals("Does Stuff", sut.getDescription());

        assertTrue(sut.getNeededClasses().contains(SensorParserTest.class));
    }


    /* For testMinimal1ParseWorks  */
    @SkipParsing
    @SensorConfig(displayName = "method1", description = "Does Stuff")
    @NeedsOtherClass(SensorParserTest.class)
    private static class TestClass1 implements Sensor {

        @SensorOutput
        public String method1(){ return null; }

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}

    }



    @Test
    public void testMinimal2ParseWorks(){
        SmaSTraSensor sut = SmaSTraClassSensorParser.readFromClass(TestClass2.class);

        assertNotNull(sut);
        assertEquals(TestClass2.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("method1", sut.getDataMethodName());
        assertEquals(String.class, sut.getOutput().getOutputParam());
        assertEquals("None", sut.getDescription());
        assertTrue(sut.getNeededClasses().isEmpty());
    }


    /* For testMinimal2ParseWorks */
    @SkipParsing
    @SensorConfig(displayName = "method1")
    private static class TestClass2 implements Sensor {

        @SensorOutput
        public String method1(){ return null; }

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testSkipMethodWithoutBaseAnnotationWorks() {
        assertNull(SmaSTraClassSensorParser.readFromClass(TestClass3.class));
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    private static class TestClass3 implements Sensor {

        public String method1(String vec1){ return null; }

        public void method2(){}

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testSkipMethodWithoutMethodAnnotationWorks() throws Throwable {
        assertNull(SmaSTraClassSensorParser.readFromClass(TestClass4.class));
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @SensorConfig(displayName = "fsaf")
    private static class TestClass4 implements Sensor {

        public String method1(String vec1){ return null; }

        public void method2(){}

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testNeedsPermissionsReadingWorks() throws Throwable {
        SmaSTraSensor list = SmaSTraClassSensorParser.readFromClass(TestClass5.class);
        assertEquals("TEST", list.getAndroidPermissions()[0]);
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @NeedsAndroidPermissions("TEST")
    @SensorConfig(displayName = "fsaf")
    private static class TestClass5 implements Sensor {

        public String method1(String vec1){ return null; }

        @SensorOutput
        public int method2(){ return 1; }

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testStartStopIsReadCorrectlyIfPresent() throws Throwable {
        SmaSTraSensor list = SmaSTraClassSensorParser.readFromClass(TestClass6.class);
        assertEquals("testStart", list.getStartMethod());
        assertEquals("testStop", list.getStopMethod());
    }



    /* For testStartStopIsReadCorrectlyIfPresent */
    @SkipParsing
    @NeedsAndroidPermissions("TEST")
    @SensorConfig(displayName = "fsaf")
    private static class TestClass6 implements Sensor {

        public String method1(String vec1){ return null; }

        @SensorOutput
        public int method2(){ return 1; }

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}

        @SensorStart public void testStart() {}
        @SensorStop public void testStop() {}
    }

    @Test
    public void testStartStopIsReadCorrectlyIfNotPresent() throws Throwable {
        SmaSTraSensor sensor = SmaSTraClassSensorParser.readFromClass(TestClass7.class);
        assertTrue(sensor.getStartMethod().isEmpty());
        assertTrue(sensor.getStopMethod().isEmpty());
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @NeedsAndroidPermissions("TEST")
    @SensorConfig(displayName = "fsaf")
    private static class TestClass7 implements Sensor {

        public String method1(String vec1){ return null; }

        @SensorOutput
        public int method2(){ return 1; }

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testReadProxyPropertiesWorks() throws Throwable {
        SmaSTraSensor sensor = SmaSTraClassSensorParser.readFromClass(TestClass8.class);
        Collection<ProxyPropertyObj> proxies = sensor.getProxyProperties();
        assertFalse(proxies.isEmpty());

        ProxyPropertyObj first = proxies.iterator().next();
        assertEquals(Object.class, first.getProxyClass());
        assertEquals("Test", first.getProperty().name());
        assertEquals("setBanane", first.getMethod().getName());
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @NeedsAndroidPermissions("TEST")
    @SensorConfig(displayName = "fsaf")
    private static class TestClass8 implements Sensor {

        public String method1(String vec1){ return null; }

        @SensorOutput
        public int method2(){ return 1; }

        @ProxyProperty(name = "Test")
        public void setBanane(Object obj){}

        @Override public void start(){}
        @Override public void stop(){}
        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}
    }

}
