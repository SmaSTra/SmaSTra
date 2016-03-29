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