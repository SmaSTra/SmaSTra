package de.tu_darmstadt.smastra.utils;

import org.junit.Test;

import static junit.framework.Assert.assertEquals;

/**
 * Tests for Config parser.
 * @author Tobias Welther
 */
public class ConfigParserUtilsTest {

    /**
     * Tests for Int parsing:
     */
    public static class TestIntParsing {

        @Test
        public void testConvertIntFromInt() {
            int val = 1;
            int defaultVal = 2;

            int result = ConfigParserUtils.parseInt(val, defaultVal);
            assertEquals(val, result);
        }

        @Test
        public void testConvertIntWithNullGivesDefaultValue() {
            int defaultVal = 2;

            int result = ConfigParserUtils.parseInt(null, defaultVal);
            assertEquals(defaultVal, result);
        }

        @Test
        public void testConvertIntWithDouble() {
            double val = 1d;
            int defaultVal = 2;

            int result = ConfigParserUtils.parseInt(val, defaultVal);
            assertEquals((int) val, result);
        }

        @Test
        public void testConvertIntWithFloat() {
            float val = 1f;
            int defaultVal = 2;

            int result = ConfigParserUtils.parseInt(val, defaultVal);
            assertEquals((int) val, result);
        }

        @Test
        public void testConvertIntWithString() {
            String val = "1";
            int defaultVal = 2;

            int result = ConfigParserUtils.parseInt(val, defaultVal);
            assertEquals(1, result);
        }

        @Test
        public void testConvertIntWithIncorrectString() {
            String val = "1abc";
            int defaultVal = 2;

            int result = ConfigParserUtils.parseInt(val, defaultVal);
            assertEquals(defaultVal, result);
        }

    }


    /**
     * Tests for Double parsing:
     */
    public static class TestDoubleParsing {

        @Test
        public void testConvertDoubleFromInt() {
            int val = 1;
            double defaultVal = 2;

            double result = ConfigParserUtils.parseDouble(val, defaultVal);
            assertEquals((double)val, result, 0.001);
        }

        @Test
        public void testConvertDoubleWithNullGivesDefaultValue() {
            double defaultVal = 2;

            double result = ConfigParserUtils.parseDouble(null, defaultVal);
            assertEquals(defaultVal, result, 0.0001);
        }

        @Test
        public void testConvertDoubleWithDouble() {
            double val = 1d;
            double defaultVal = 2;

            double result = ConfigParserUtils.parseDouble(val, defaultVal);
            assertEquals((int) val, result, 0.001);
        }

        @Test
        public void testConvertDoubleWithFloat() {
            float val = 1f;
            double defaultVal = 2;

            double result = ConfigParserUtils.parseDouble(val, defaultVal);
            assertEquals((int) val, result, 0.001);
        }

        @Test
        public void testConvertDoubleWithString() {
            String val = "1";
            double defaultVal = 2;

            double result = ConfigParserUtils.parseDouble(val, defaultVal);
            assertEquals(1, result, 0.0001);
        }

        @Test
        public void testConvertDoubleWithIncorrectString() {
            String val = "1abc";
            double defaultVal = 2;

            double result = ConfigParserUtils.parseDouble(val, defaultVal);
            assertEquals(defaultVal, result, 0.0001);
        }

    }


    /**
     * Tests for Double parsing:
     */
    public static class TestStringParsing {

        @Test
        public void testConvertStringFromString() {
            String val = "Test";
            String defaultVal = "123";

            String result = ConfigParserUtils.parseString(val, defaultVal);
            assertEquals(val, result);
        }


        @Test
        public void testConvertStringFromOther() {
            double val = 0.00113d;
            String defaultVal = "123";

            String result = ConfigParserUtils.parseString(val, defaultVal);
            assertEquals(String.valueOf(val), result);
        }

    }

}
