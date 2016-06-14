package de.tu_darmstadt.smastra.utils;

/**
 * This is a helper Class to read configuration stuff.
 * @author Tobias Welther
 */
public class ConfigParserUtils {

    /**
     * Parses an int from an object.
     * @param obj to parse.
     * @param defaultValue to use.
     * @return the parsed int.
     */
    public static int parseInt(Object obj, int defaultValue){
        if(obj == null) return defaultValue;
        if(obj instanceof Integer) return (Integer) obj;
        if(obj instanceof Double) return (int) Math.round((double)obj);
        if(obj instanceof Float) return Math.round((float)obj);
        if(obj instanceof String) try{ return Integer.parseInt(obj.toString()); }catch (Throwable exp){ return defaultValue; }

        return defaultValue;
    }


    /**
     * Parses an String from an object.
     * @param obj to parse.
     * @param defaultValue to use.
     * @return the parsed String.
     */
    public static String parseString(Object obj, String defaultValue){
        return obj == null ? defaultValue : obj.toString();
    }


    /**
     * Parses an double from an object.
     * @param obj to parse.
     * @param defaultValue to use.
     * @return the parsed double.
     */
    public static double parseDouble(Object obj, double defaultValue){
        if(obj == null) return defaultValue;
        if(obj instanceof Integer) return (Integer) obj;
        if(obj instanceof Double) return (double)obj;
        if(obj instanceof Float) return (float)obj;
        if(obj instanceof String) try{ return Double.parseDouble(obj.toString()); }catch (Throwable exp){ return defaultValue; }

        return defaultValue;
    }
}
