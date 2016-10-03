package de.tu_darmstadt.smastra.math;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.transformation.Transformation;
import de.tu_darmstadt.smastra.sensors.Vector3d;


/**
 * A vector utils class for simple Transformations with Vectors.
 *
 * @author Tobias Welther
 */
@NeedsOtherClass(Vector3d.class)
public class VectorUtils implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    /**
     * Creates a vector with the values passed.
     * @param value1 to use.
     * @param value2 to use.
     * @param value3 to use.
     * @return a generated Value.
     */
    @Transformation(displayName = "Create Vector3d")
    public static Vector3d createVector(double value1, double value2, double value3){
        return new Vector3d(value1, value2, value3);
    }


    /**
     * Returns the X-Component of the Vector.
     * @param vector the component to use.
     * @return the x-Component.
     */
    @Transformation(displayName = "Get X")
    public static double getX(Vector3d vector){
        return vector.getX();
    }

    /**
     * Returns the Y-Component of the Vector.
     * @param vector the component to use.
     * @return the Y-Component.
     */
    @Transformation(displayName = "Get Y")
    public static double getY(Vector3d vector){
        return vector.getY();
    }

    /**
     * Returns the Z-Component of the Vector.
     * @param vector the component to use.
     * @return the Z-Component.
     */
    @Transformation(displayName = "Get Z")
    public static double getZ(Vector3d vector){
        return vector.getZ();
    }

}
