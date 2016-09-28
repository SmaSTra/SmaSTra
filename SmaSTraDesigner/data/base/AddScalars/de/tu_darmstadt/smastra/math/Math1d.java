package de.tu_darmstadt.smastra.math;

import de.tu_darmstadt.smastra.markers.elements.Transformation;

/**
 * A static Math class.
 * @author Tobias Welther
 */
public class Math1d implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    /**
     * Adds 2 Vectors.
     * @param value1 to add
     * @param value2 to add
     * @return the result.
     */
    @Transformation(displayName = "Add Scalars")
    public static double add(double value1, double value2){
        return value1 + value2;
    }

    /**
     * Subtracts 2 scalars
     * @param value1 to use as base
     * @param value2 to subtract
     * @return the result.
     */
    @Transformation(displayName = "Subtract Scalars")
    public static double subtract(double value1, double value2){
        return value1 - value2;
    }


    /**
     * Multiplies a value to another one.
     * @param value1 to multiply.
     * @param value2 to multiply
     * @return the result.
     */
    @Transformation(displayName = "Multiply scalars")
    public static double multiply(double value1, double value2){
        return value1 * value2;
    }

    /**
     * Devides a value by another.
     * @param value1 to device.
     * @param value2 to device by.
     * @return the result.
     */
    @Transformation(displayName = "Device scalars")
    public static double divide(double value1, double value2){
        return value1 / value2;
    }


    /**
     * Squares a Scalar and returns it.
     * @param toSquare the Sclalar to square.
     * @return the squared scalar.
     */
    @Transformation(displayName = "Square scalar")
    public static double square(double toSquare) {
        return toSquare * toSquare;
    }

}