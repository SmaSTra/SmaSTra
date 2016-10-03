package de.tu_darmstadt.smastra.stringcomp;

import de.tu_darmstadt.smastra.markers.elements.Transformation;

/**
 * This is a lib for String comparisons.
 * @author Tobias Welther
 */

public class StringComparison implements de.tu_darmstadt.smastra.markers.interfaces.Transformation{


    /**
     * Returns true if the string are exactly the same.
     * @param input to compare
     * @param toCompareTo to compare to.
     */
    @Transformation(displayName = "StringEquals", description = "Checks if 2 strings are equals.")
    public static boolean StringEquals(String input, String toCompareTo){
        return !(input == null || toCompareTo == null) && input.equals(toCompareTo);
    }

    /**
     * Returns true if the strings are the same ignoring the case-sensibility.
     * @param input to compare
     * @param toCompareTo to compare to.
     */
    @Transformation(displayName = "StringEqualsIgnoreCase", description = "Checks if 2 strings are equals, ignores case-sensibility")
    public static boolean StringEqualsIgnoreCase(String input, String toCompareTo){
        return !(input == null || toCompareTo == null) && input.equalsIgnoreCase(toCompareTo);
    }

    /**
     * Returns true if the strings are the same ignoring the case-sensibility.
     * @param input to measure
     */
    @Transformation(displayName = "StringLength", description = "Gets the length of a String")
    public static int StringLength(String input){
        return input == null ? 0 : input.length();
    }


    /**
     * Returns true if the strings are the same ignoring the case-sensibility.
     * @param input to measure
     */
    @Transformation(displayName = "SubString", description = "Gets a String from-to the wanted length")
    public static String SubString(String input, int from, int to){
        int length = StringLength(input);
        if(length <= 0) return "";

        from = Math.max(0, Math.min(length, from));
        to = Math.min(length, Math.max(to, 0));

        //sanity check form wrong direction.
        if(from > to){
            int tmp = from;
            from = to;
            to = tmp;
        }

        return input.substring(from, to);
    }

}
