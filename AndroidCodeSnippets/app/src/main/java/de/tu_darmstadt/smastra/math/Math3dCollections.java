package de.tu_darmstadt.smastra.math;

import java.util.Collection;

import de.tu_darmstadt.smastra.collectors.WindowCollection;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.Transformation;
import de.tu_darmstadt.smastra.sensors.Vector3d;

/**
 * This class does math functions with collections of 3d-Data.
 * @author Tobias Welther
 */
@NeedsOtherClass( {Math3d.class, Vector3d.class, WindowCollection.class} )
public class Math3dCollections implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    /**
     * The mean of the data passed.
     * @param toMean to mean.
     * @return the mean of the data.
     */
    @Transformation(displayName = "Vectors mean Collection")
    public static Vector3d mean(Collection<? extends  Vector3d> toMean){
        Vector3d result = new Vector3d();

        //add the data to the result.
        for(Vector3d data : toMean) result = Math3d.add(result,data);

        return Math3d.divide(result, toMean.size());
    }


    /**
     * The variance of the data passed.
     * @param toVariance to mean.
     * @return the mean of the data.
     */
    @Transformation(displayName = "Vectors variance Collection")
    public static Vector3d variance(Collection<? extends  Vector3d> toVariance){
        Vector3d result = new Vector3d();
        Vector3d mean = mean(toVariance);

        //add the data to the result.
        for(Vector3d data : toVariance) {
            result = Math3d.add(result, Math3d.square(Math3d.subtract(data,mean)));
        }

        return Math3d.divide(result, Math.max(1,toVariance.size()-1));
    }

    /**
     * The mean of the data passed.
     * @param toMean to mean.
     * @return the mean of the data.
     */
    @Transformation(displayName = "Vectors mean Window")
    public static Vector3d mean(WindowCollection<? extends  Vector3d> toMean){
        return mean(toMean.getData());
    }


    /**
     * The variance of the data passed.
     * @param toVariance to mean.
     * @return the mean of the data.
     */
    @Transformation(displayName = "Vectors variance Collection")
    public static Vector3d variance(WindowCollection<? extends  Vector3d> toVariance){
        return variance(toVariance.getData());
    }

}
