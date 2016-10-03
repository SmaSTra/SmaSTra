package de.tu_darmstadt.smastra.math;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.transformation.Transformation;
import de.tu_darmstadt.smastra.sensors.Vector3d;

/**
 * A static Math class for 3d values.
 *
 * @author Tobias Welther
 */
@NeedsOtherClass(Vector3d.class)
public class Math3d implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    /**
     * Adds 2 Vectors.
     * @param vector1 to add
     * @param vector2 to add
     * @return the result.
     */
    @Transformation(displayName = "Add Vectors")
    public static Vector3d add(Vector3d vector1, Vector3d vector2){
        double x = vector1.getX() + vector2.getX();
        double y = vector1.getY() + vector2.getY();
        double z = vector1.getZ() + vector2.getZ();

        return new Vector3d(x,y,z);
    }

    /**
     * Subtracts 2 Vectors.
     * @param vector1 to subtract
     * @param vector2 to subtract
     * @return the result.
     */
    @Transformation(displayName = "Subtract Vectors")
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
    @Transformation(displayName = "Multiply Vectors")
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
    @Transformation(displayName = "Devide Vectors")
    public static Vector3d divide(Vector3d vector1, double value){
        double x = vector1.getX() / value;
        double y = vector1.getY() / value;
        double z = vector1.getZ() / value;

        return new Vector3d(x,y,z);
    }


    /**
     * Squares a Vector and returns it.
     * @param toSquare the vector to square.
     * @return the squared Vector.
     */
    @Transformation(displayName = "Square Vector")
    public static Vector3d square(Vector3d toSquare) {
        double x = toSquare.getX()*toSquare.getX();
        double y = toSquare.getY()*toSquare.getY();
        double z = toSquare.getZ()*toSquare.getZ();

        return new Vector3d(x,y,z);
    }


    /**
     * Gives the Length of the Vector from the origin.
     * @param vector to use.
     * @return the length from origin.
     */
    @Transformation(displayName = "Vector Length")
    public static double length(Vector3d vector){
        return Math.sqrt(lengthSquare(vector));
    }


    /**
     * Gives the Squared Length of the Vector from the origin.
     * @param vector to use.
     * @return the squared length from origin.
     */
    @Transformation(displayName = "Vector squared length")
    public static double lengthSquare(Vector3d vector){
        vector = square(vector);
        return vector.getX() + vector.getY() + vector.getZ();
    }

    /**
     * Gives the distance between 2 vectors.
     * @param vector1 to use.
     * @param vector2 to use.
     * @return the distance.
     */
    @Transformation(displayName = "Vectors distance")
    public static double distance(Vector3d vector1, Vector3d vector2){
        return Math.sqrt(distanceSquare(vector1,vector2));
    }


    /**
     * Gives the squared distance between 2 vectors.
     * @param vector1 to use.
     * @param vector2 to use.
     * @return the distance.
     */
    @Transformation(displayName = "Vectors squared distance")
    public static double distanceSquare(Vector3d vector1, Vector3d vector2){
        return Math3d.lengthSquare(Math3d.subtract(vector1,vector2));
    }
}