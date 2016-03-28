package de.tu_darmstadt.smastra.math;

import java.util.Collection;

import de.tu_darmstadt.smastra.sensors.Data3d;
import de.tu_darmstadt.smastra.sensors.Vector3d;

/**
 * A static Math class for 3d values.
 *
 * @author Tobias Welther
 */
public class Math3d {


    /**
     * The mean of the data passed.
     * @param toMean to mean.
     * @return the mean of the data.
     */
    public static Vector3d mean(Collection<Data3d> toMean){
        Vector3d result = new Vector3d();

        //add the data to the result.
        for(Data3d data : toMean) result.add(data);

        return result.multiply(1d/toMean.size());
    }

}
