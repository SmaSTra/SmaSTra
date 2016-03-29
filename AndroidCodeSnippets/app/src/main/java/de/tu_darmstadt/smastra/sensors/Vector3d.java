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
     * Adds a vector to this one
     *
     * @param vec The other vector
     * @return the same vector
     */
    public Vector3d add(Vector3d vec) {
        x += vec.x;
        y += vec.y;
        z += vec.z;
        return this;
    }

    /**
     * Subtracts a vector from this one.
     *
     * @param vec The other vector
     * @return the same vector
     */
    public Vector3d subtract(Vector3d vec) {
        x -= vec.x;
        y -= vec.y;
        z -= vec.z;
        return this;
    }

    /**
     * Multiplies the vector by another.
     *
     * @param vec The other vector
     * @return the same vector
     */
    public Vector3d multiply(Vector3d vec) {
        x *= vec.x;
        y *= vec.y;
        z *= vec.z;
        return this;
    }

    /**
     * Divides the vector by another.
     *
     * @param vec The other vector
     * @return the same vector
     */
    public Vector3d divide(Vector3d vec) {
        x /= vec.x;
        y /= vec.y;
        z /= vec.z;
        return this;
    }

    /**
     * Copies another vector
     *
     * @param vec The other vector
     * @return the same vector
     */
    public Vector3d copy(Vector3d vec) {
        x = vec.x;
        y = vec.y;
        z = vec.z;
        return this;
    }

    /**
     * Gets the magnitude of the vector, defined as sqrt(x^2+y^2+z^2). The
     * value of this method is not cached and uses a costly square-root
     * function, so do not repeatedly call this method to get the vector's
     * magnitude. NaN will be returned if the inner result of the sqrt()
     * function overflows, which will be caused if the length is too long.
     *
     * @return the magnitude
     */
    public double length() {
        return Math.sqrt(x*x + y*y + z*z);
    }

    /**
     * Gets the magnitude of the vector squared.
     *
     * @return the magnitude
     */
    public double lengthSquared() {
        return x*x + y*y + z*z;
    }

    /**
     * Get the distance between this vector and another. The value of this
     * method is not cached and uses a costly square-root function, so do not
     * repeatedly call this method to get the vector's magnitude. NaN will be
     * returned if the inner result of the sqrt() function overflows, which
     * will be caused if the distance is too long.
     *
     * @param other The other vector
     * @return the distance
     */
    public double distance(Vector3d other) {
        if(other == null) throw new IllegalArgumentException("Distance check in Vector: other may not be null!");
        return Math.sqrt((x-other.x)*(x-other.x) + (y-other.y)*(y-other.y) + (z-other.z)*(z-other.z));
    }

    /**
     * Get the squared distance between this vector and another.
     *
     * @param other The other vector
     * @return the distance
     */
    public double distanceSquared(Vector3d other) {
        if(other == null) throw new IllegalArgumentException("DistanceSquare check in Vector: other may not be null!");
        return (x-other.x)*(x-other.x) + (y-other.y)*(y-other.y) + (z-other.z)*(z-other.z);
    }

    /**
     * Gets the angle between this vector and another in radians.
     *
     * @param other The other vector
     * @return angle in radians
     */
    public float angle(Vector3d other) {
        double dot = dot(other) / (length() * other.length());
        return (float) Math.acos(dot);
    }

    /**
     * Sets this vector to the midpoint between this vector and another.
     *
     * @param other The other vector
     * @return this same vector (now a midpoint)
     */
    public Vector3d midpoint(Vector3d other) {
        x = (x + other.x) / 2;
        y = (y + other.y) / 2;
        z = (z + other.z) / 2;
        return this;
    }

    /**
     * Gets a new midpoint vector between this vector and another.
     *
     * @param other The other vector
     * @return a new midpoint vector
     */
    public Vector3d getMidpoint(Vector3d other) {
        double x = (this.x + other.x) / 2;
        double y = (this.y + other.y) / 2;
        double z = (this.z + other.z) / 2;
        return new Vector3d(x, y, z);
    }

    /**
     * Performs scalar multiplication, multiplying all components with a
     * scalar.
     *
     * @param m The factor
     * @return the same vector
     */
    public Vector3d multiply(int m) {
        x *= m;
        y *= m;
        z *= m;
        return this;
    }

    /**
     * Performs scalar multiplication, multiplying all components with a
     * scalar.
     *
     * @param m The factor
     * @return the same vector
     */
    public Vector3d multiply(double m) {
        x *= m;
        y *= m;
        z *= m;
        return this;
    }
    /**
     * Performs division,  all components with a
     * scalar.
     *
     * @param m The factor
     * @return the same vector
     */
    public Vector3d divide(double m) {
        x /= m;
        y /= m;
        z /= m;
        return this;
    }

    /**
     * Performs scalar multiplication, multiplying all components with a
     * scalar.
     *
     * @param m The factor
     * @return the same vector
     */
    public Vector3d multiply(float m) {
        x *= m;
        y *= m;
        z *= m;
        return this;
    }

    /**
     * Calculates the dot product of this vector with another. The dot product
     * is defined as x1*x2+y1*y2+z1*z2. The returned value is a scalar.
     *
     * @param other The other vector
     * @return dot product
     */
    public double dot(Vector3d other) {
        return x * other.x + y * other.y + z * other.z;
    }

    /**
     * Calculates the cross product of this vector with another. The cross
     * product is defined as:
     * <ul>
     * <li>x = y1 * z2 - y2 * z1
     * <li>y = z1 * x2 - z2 * x1
     * <li>z = x1 * y2 - x2 * y1
     * </ul>
     *
     * @param o The other vector
     * @return the same vector
     */
    public Vector3d crossProduct(Vector3d o) {
        double newX = y * o.z - o.y * z;
        double newY = z * o.x - o.z * x;
        double newZ = x * o.y - o.x * y;

        x = newX;
        y = newY;
        z = newZ;
        return this;
    }

    /**
     * Calculates the cross product of this vector with another without mutating
     * the original. The cross product is defined as:
     * <ul>
     * <li>x = y1 * z2 - y2 * z1
     * <li>y = z1 * x2 - z2 * x1
     * <li>z = x1 * y2 - x2 * y1
     * </ul>
     *
     * @param o The other vector
     * @return a new vector
     */
    public Vector3d getCrossProduct(Vector3d o) {
        double x = this.y * o.z - o.y * this.z;
        double y = this.z * o.x - o.z * this.x;
        double z = this.x * o.y - o.x * this.y;
        return new Vector3d(x, y, z);
    }

    /**
     * Converts this vector to a unit vector (a vector with length of 1).
     *
     * @return the same vector
     */
    public Vector3d normalize() {
        double length = length();

        x /= length;
        y /= length;
        z /= length;

        return this;
    }

    /**
     * Zero this vector's components.
     *
     * @return the same vector
     */
    public Vector3d zero() {
        x = 0;
        y = 0;
        z = 0;
        return this;
    }


    /**
     * Creates a copy of the vector.
     * @return a copy.
     */
    public Vector3d copy(){
        return new Vector3d(this);
    }


    /**
     * Squares the Vector.
     */
    public Vector3d square() {
        this.x*=this.x;
        this.y*=this.y;
        this.z*=this.z;

        return this;
    }
}
