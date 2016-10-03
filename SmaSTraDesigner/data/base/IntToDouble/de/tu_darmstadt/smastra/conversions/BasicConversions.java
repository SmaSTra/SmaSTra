package de.tu_darmstadt.smastra.conversions;

import de.tu_darmstadt.smastra.markers.elements.Transformation;

/**
 * A simple conversion class for SmaSTra.
 * Should be moved to the tool internally later on.
 * @author Tobias Welther.
 */

public class BasicConversions implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {

    /**
     * Converts an double to an int.
     * @param val to convert.
     * @return the converted value.
     */
    @Transformation(displayName = "DoubleToInt", description = "Converts a double to an Int value.")
    public static int DoubleToInt(double val){
        return (int) val;
    }


    /**
     * Converts an double to an int.
     * @param val to convert.
     * @return the converted value.
     */
    @Transformation(displayName = "IntToDouble", description = "Converts an Integer to a double value.")
    public static double DoubleToInt(int val){
        return val;
    }


}
