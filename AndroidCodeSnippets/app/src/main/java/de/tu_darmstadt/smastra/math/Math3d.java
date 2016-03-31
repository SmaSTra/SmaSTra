package de.tu_darmstadt.smastra.math;

import java.util.Collection;

import de.tu_darmstadt.smastra.markers.Exportable;
import de.tu_darmstadt.smastra.markers.SmaStraMethod;
import de.tu_darmstadt.smastra.sensors.Vector3d;

/**
 * A static Math class for 3d values.
 *
 * @author Tobias Welther
 */
@Exportable
public class Math3d {


    /**
     * Adds 2 Vectors.
     * @param vector1 to add
     * @param vector2 to add
     * @return the result.
     */
    @SmaStraMethod
    public static Vector3d add(Vector3d vector1, Vector3d vector2){
        double x = vector1.getX() + vector2.getX();
        double y = vector1.getY() + vector2.getY();
        double z = vector1.getZ() + vector2.getZ();

        return new Vector3d(x,y,z);
    }

    /**
     * Subtracts 2 Vectors.
     * @param vector1 to add
     * @param vector2 to add
     * @return the result.
     */
    @SmaStraMethod
    public static Vector3d subtract(Vector3d vector1, Vector3d vector2){
        double x = vector1.getX() - vector2.getX();
        double y = vector1.getY() - vector2.getY();
        double z = vector1.getZ() - vector2.getZ();

        return new Vector3d(x,y,z);
    }


    /**
     * Multiplies a value to the vector.
     * @param vector1 to multiply.
     * @param value to multiply
     * @return the result.
     */
    @SmaStraMethod
    public static Vector3d multiply(Vector3d vector1, double value){
        double x = vector1.getX() * value;
        double y = vector1.getY() * value;
        double z = vector1.getZ() * value;

        return new Vector3d(x,y,z);
    }

    /**
     * Devides a the vector by a value.
     * @param vector1 to device.
     * @param value to device by.
     * @return the result.
     */
    @SmaStraMethod
    public static Vector3d devide(Vector3d vector1, double value){
        double x = vector1.getX() / value;
        double y = vector1.getY() / value;
        double z = vector1.getZ() / value;

        return new Vector3d(x,y,z);
    }


    /**
     * The mean of the data passed.
     * @param toMean to mean.
     * @return the mean of the data.
     */
    @SmaStraMethod
    public static Vector3d mean(Collection<? extends  Vector3d> toMean){
        Vector3d result = new Vector3d();

        //add the data to the result.
        for(Vector3d data : toMean) result.add(data);

        return result.divide(toMean.size());
    }


    /**
     * The variance of the data passed.
     * @param toVariance to mean.
     * @return the mean of the data.
     */
    @SmaStraMethod
    public static Vector3d variance(Collection<? extends  Vector3d> toVariance){
        Vector3d result = new Vector3d();
        Vector3d mean = mean(toVariance);

        //add the data to the result.
        for(Vector3d data : toVariance) {
            result.add(data.copy().subtract(mean).square());
        }

        return result.divide(Math.max(1,toVariance.size()-1));
    }

}