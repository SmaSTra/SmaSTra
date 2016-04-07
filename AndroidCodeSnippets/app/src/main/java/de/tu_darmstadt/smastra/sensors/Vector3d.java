package de.tu_darmstadt.smastra.sensors;

/**
 * This is a sub-part of the Data-3d.
 * It can handle Mathematical things.
 *
 * @author Tobias Welther
 */
public class Vector3d {

    /**
     * The x-Value of the vector.
     */
    protected double x;

    /**
     * The y-Value of the Vector.
     */
    protected double y;

    /**
     * The z-Value of the Vector.
     */
    protected double z;


    /**
     * Construct the vector with all components as 0.
     */
    public Vector3d() {
        this.x = 0;
        this.y = 0;
        this.z = 0;
    }

    /**
     * Construct the vector with provided integer components.
     *
     * @param x X component
     * @param y Y component
     * @param z Z component
     */
    public Vector3d(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /**
     * Construct the vector with provided double components.
     *
     * @param x X component
     * @param y Y component
     * @param z Z component
     */
    public Vector3d(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /**
     * Construct the vector with provided float components.
     *
     * @param x X component
     * @param y Y component
     * @param z Z component
     */
    public Vector3d(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /**
     * Generates a Vector out of a float[].
     * This needs EXACT 3 elements or it will throw an IllegalArgumentException.
     * @param array to use.
     */
    public Vector3d(float[] array) {
        if(array == null || array.length != 3) {
            throw new IllegalArgumentException("3D data need to be created by an Array with 3 elements! " +
                    "Found " + (array == null ? "0" : array.length) + " elements.");
        }

        this.x = array[0];
        this.y = array[1];
        this.z = array[2];
    }


    /**
     * Generates a Vector out of a double[].
     * This needs EXACT 3 elements or it will throw an IllegalArgumentException.
     * @param array to use.
     */
    public Vector3d(double[] array) {
        if(array == null || array.length != 3) {
            throw new IllegalArgumentException("3D data need to be created by an Array with 3 elements! " +
                    "Found " + (array == null ? "0" : array.length) + " elements.");
        }

        this.x = array[0];
        this.y = array[1];
        this.z = array[2];
    }


    /**
     * Creates a vector.
     * @param original to use.
     */
    public Vector3d(Vector3d original) {
        this.x = original.x;
        this.y = original.y;
        this.z = original.z;
    }


    /**
     * Returns the X-Component of the
     */
    public double getX(){
        return x;
    }


    /**
     * Returns the Y-Component of the
     */
    public double getY() {
        return y;
    }


    /**
     * Returns the Z-Component of the
     */
    public double getZ() {
        return z;
    }


    /**
     * Creates a copy of the vector.
     * @return a copy.
     */
    public Vector3d copy(){
        return new Vector3d(this);
    }

}
