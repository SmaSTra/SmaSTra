package de.tu_darmstadt.smastra.generator.buffer;

import org.junit.Test;

import java.util.Collection;
import java.util.Map;

import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.SkipParsing;
import de.tu_darmstadt.smastra.markers.elements.buffer.BufferAdd;
import de.tu_darmstadt.smastra.markers.elements.buffer.BufferGet;
import de.tu_darmstadt.smastra.markers.elements.buffer.BufferInfo;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.proxyproperties.ProxyProperty;
import de.tu_darmstadt.smastra.markers.interfaces.Buffer;

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
public class BufferParserTest {



    @Test
    public void testMinimal1ParseWorks(){
        SmaSTraBuffer sut = SmaSTraClassBufferParser.readFromClass(TestClass1.class);

        assertNotNull(sut);

        assertEquals(TestClass1.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("addData", sut.getBufferAddMethodName());
        assertEquals("getData", sut.getBufferGetMethodName());
        assertEquals("Does Stuff", sut.getDescription());

        assertTrue(sut.getNeededClasses().contains(BufferParserTest.class));
    }


    /* For testMinimal1ParseWorks  */
    @SkipParsing
    @BufferInfo(displayName = "method1", description = "Does Stuff")
    @NeedsOtherClass(BufferParserTest.class)
    private static class TestClass1 implements Buffer<String> {

        @BufferAdd @Override public void addData(String element) {}
        @BufferGet @Override public Collection<String> getData() { return null; }
        @Override public void configure(Map<String, Object> configuration) {}
        @Override public void configure(String key, Object value) {}

    }



    @Test
    public void testMinimal2ParseWorks(){
        SmaSTraBuffer sut = SmaSTraClassBufferParser.readFromClass(TestClass2.class);

        assertNotNull(sut);
        assertEquals(TestClass2.class, sut.getElementClass());
        assertEquals("method1", sut.getDisplayName());
        assertEquals("addData", sut.getBufferAddMethodName());
        assertEquals("getData", sut.getBufferGetMethodName());
        assertEquals("None", sut.getDescription());
        assertTrue(sut.getNeededClasses().isEmpty());
    }


    /* For testMinimal2ParseWorks */
    @SkipParsing
    @BufferInfo(displayName = "method1")
    private static class TestClass2 implements Buffer<String> {

        @BufferAdd @Override public void addData(String element) {}
        @BufferGet @Override public Collection<String> getData() { return null; }
        @Override public void configure(Map<String, Object> configuration) {}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testSkipMethodWithoutBaseAnnotationWorks() {
        assertNull(SmaSTraClassBufferParser.readFromClass(TestClass3.class));
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    private static class TestClass3 implements Buffer<String> {

        @BufferAdd @Override public void addData(String element) {}
        @BufferGet @Override public Collection<String> getData() { return null; }
        @Override public void configure(Map<String, Object> configuration) {}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testSkipMethodWithoutMethodAnnotationWorks() throws Throwable {
        assertNull(SmaSTraClassBufferParser.readFromClass(TestClass4.class));
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @BufferInfo(displayName = "fsaf")
    private static class TestClass4 implements Buffer<String> {

        @Override public void addData(String element) {}
        @Override public Collection<String> getData() { return null; }
        @Override public void configure(Map<String, Object> configuration) {}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testNeedsPermissionsReadingWorks() throws Throwable {
        SmaSTraBuffer list = SmaSTraClassBufferParser.readFromClass(TestClass5.class);
        assertEquals("TEST", list.getAndroidPermissions()[0]);
    }



    /* For testSkipMethodWithoutAnnotationWorks */
    @SkipParsing
    @NeedsAndroidPermissions("TEST")
    @BufferInfo(displayName = "fsaf")
    private static class TestClass5 implements Buffer<String> {

        @BufferAdd @Override public void addData(String element) {}
        @BufferGet @Override public Collection<String> getData() { return null; }
        @Override public void configure(Map<String, Object> configuration) {}
        @Override public void configure(String key, Object value) {}
    }


    @Test
    public void testReadProxyPropertiesWorks() throws Throwable {
        SmaSTraBuffer buffer = SmaSTraClassBufferParser.readFromClass(BufferParserTest.TestClass6.class);
        assertNotNull(buffer);

        Collection<ProxyPropertyObj> proxies = buffer.getProxyProperties();
        assertFalse(proxies.isEmpty());

        ProxyPropertyObj first = proxies.iterator().next();
        assertEquals(Object.class, first.getProxyClass());
        assertEquals("Test", first.getProperty().name());
        assertEquals("setBanane", first.getMethod().getName());
    }



    /* For testReadProxyPropertiesWorks */
    @SkipParsing
    @NeedsAndroidPermissions("TEST")
    @BufferInfo(displayName = "fsaf")
    private static class TestClass6 implements Buffer<Double> {

        @ProxyProperty(name = "Test")
        public void setBanane(Object obj){}

        @Override public void configure(Map<String,Object> config){}
        @Override public void configure(String key, Object value) {}

        @BufferAdd @Override public void addData(Double element) {}
        @BufferGet @Override public Collection<Double> getData() { return null; }

    }

}
