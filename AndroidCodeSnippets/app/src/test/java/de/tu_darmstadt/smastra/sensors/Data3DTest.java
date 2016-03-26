package de.tu_darmstadt.smastra.sensors;

import org.junit.Test;

import de.tu_darmstadt.smastra.sensors.Data3d;

import static org.junit.Assert.*;

/**
 * A test to work with Data3d
 */
public class Data3DTest {

    @Test
    public void ConstructorSinglesAndGetterWork() {
        long time = System.currentTimeMillis();
        float accuracy = 42.42f;

        float x = 0.1f;
        float y = 0.2f;
        float z = 0.3f;

        Data3d sut = new Data3d(time, accuracy, x,y,z);

        assertEquals(time, sut.getTime(), 0.01);
        assertEquals(accuracy, sut.getAccuracy(), 0.01);

        assertEquals(x, sut.getX(), 0.01);
        assertEquals(y, sut.getY(), 0.01);
        assertEquals(z, sut.getZ(), 0.01);
    }


    @Test
    public void ConstructorArrayAndGetterWork() {
        long time = System.currentTimeMillis();
        float accuracy = 42.42f;

        float[] array = new float[]{0.1f, 0.2f, 0.3f};
        Data3d sut = new Data3d(time, accuracy, array);

        assertEquals(time, sut.getTime(), 0.01);
        assertEquals(accuracy, sut.getAccuracy(), 0.01);

        assertEquals(array[0], sut.getX(), 0.01);
        assertEquals(array[1], sut.getY(), 0.01);
        assertEquals(array[2], sut.getZ(), 0.01);
    }

    @Test(expected = IllegalArgumentException.class)
    public void ConstructorArrayThrowsErrorOnNull() throws Exception {
        new Data3d(0,0,null);
    }

    @Test(expected = IllegalArgumentException.class)
    public void ConstructorArrayThrowsErrorOnWrongDataSize() throws Exception {
        float[] array = new float[]{0.1f, 0.2f};
        new Data3d(0,0,array);
    }


    @Test
    public void LengthCalculatesCorrectLengths(){
        float[] array = new float[]{1f, 2f, 2f};
        Data3d sut = new Data3d(0,0,array);

        //= 1² + 2² + 2² = 9
        assertEquals(3, sut.length(), 0.01f);
    }

    @Test
    public void DistanceWorks(){
        float array1[] = new float[]{1,1,1};
        float array2[] = new float[]{0,0,0};

        Data3d sut = new Data3d(0,0, array1);
        Data3d data2 = new Data3d(0,0, array2);

        assertEquals(Math.sqrt(3), sut.distance(data2), 0.01);
    }

    @Test
    public void SquareDistanceWorks(){
        float array1[] = new float[]{1,1,1};
        float array2[] = new float[]{0,0,0};

        Data3d sut = new Data3d(0,0, array1);
        Data3d data2 = new Data3d(0,0, array2);

        assertEquals(3, sut.distanceSquare(data2), 0.01);
    }

    @Test(expected = IllegalArgumentException.class)
    public void DistanceThrowsErrorIfOtherIsNull() throws Exception{
        float array1[] = new float[]{1,1,1};
        new Data3d(0,0, array1).distance(null);
    }

}