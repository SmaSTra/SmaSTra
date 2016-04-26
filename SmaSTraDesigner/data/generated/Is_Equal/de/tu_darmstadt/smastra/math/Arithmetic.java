package de.tu_darmstadt.smastra.math;

import de.tu_darmstadt.smastra.markers.elements.Transformation;

/**
 * This class does some basic Arithmetics.
 *
 * @author Tobias Welther
 */
public class Arithmetic implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    /**
     * Return if var1 is greater than var2
     * @param var1 to use.
     * @param var2 to use.
     * @return true if is greater.
     */
    @Transformation(displayName = "Is Greater")
    public static boolean isGreater(double var1, double var2){
        return var1 > var2;
    }



    /**
     * Return if var1 is smaller than var2
     * @param var1 to use.
     * @param var2 to use.
     * @return true if is smaller.
     */
    @Transformation(displayName = "Is Smaller")
    public static boolean isSmaller(double var1, double var2){
        return var1 < var2;
    }

    /**
     * Return if var1 and var2 are equal.
     * @param var1 to use.
     * @param var2 to use.
     * @return true if are equal.
     */
    @Transformation(displayName = "Is Equal")
    public static boolean isEqual(double var1, double var2){
        return Math.abs(Math.abs(var1) - Math.abs(var2)) < 0.001;
    }


    /**
     * Return if both booleans are true.
     * @param var1 to use.
     * @param var2 to use.
     * @return true if are equal.
     */
    @Transformation(displayName = "Bool 2 And")
    public static boolean and2(boolean var1, boolean var2){
        return var1 && var2;
    }

    /**
     * Return if any boolean is true.
     * @param var1 to use.
     * @param var2 to use.
     * @return true if are equal.
     */
    @Transformation(displayName = "Bool 2 Or")
    public static boolean or2(boolean var1, boolean var2){
        return var1 || var2;
    }
    /**
     * Return if all 3 booleans are true.
     * @param var1 to use.
     * @param var2 to use.
     * @param var3 to use.
     * @return true if are equal.
     */
    @Transformation(displayName = "Bool 3 And")
    public static boolean and(boolean var1, boolean var2, boolean var3){
        return var1 && var2 && var3;
    }

    /**
     * Return if any boolean is true.
     * @param var1 to use.
     * @param var2 to use.
     * @param var3 to use.
     * @return true if are equal.
     */
    @Transformation(displayName = "Bool 3 Or")
    public static boolean or(boolean var1, boolean var2, boolean var3){
        return var1 || var2 || var3;
    }

}
