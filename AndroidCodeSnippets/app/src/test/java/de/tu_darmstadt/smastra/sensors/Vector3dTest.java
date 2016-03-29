package de.tu_darmstadt.smastra.sensors;

import org.junit.Test;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

/**
 * Test for the Vector class.
 */
public class Vector3dTest {



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

        assertEquals(3, sut.distanceSquared(data2), 0.01);
    }


    @Test(expected = IllegalArgumentException.class)
    public void DistanceThrowsErrorIfOtherIsNull() throws Exception{
        float array1[] = new float[]{1,1,1};
        new Data3d(0,0, array1).distance(null);
    }

    @Test
    public void SquareWorks(){
        double x = 10;
        double y = 11;
        double z = 12;
        Vector3d sut = new Vector3d(x,y,z).square();

        assertEquals(x*x, sut.getX(), 0.01);
        assertEquals(y*y, sut.getY(), 0.01);
        assertEquals(z*z, sut.getZ(), 0.01);
    }

    @Test
    public void SquareNegativeValuesWorks(){
        double x = -10;
        double y = -11;
        double z = -12;
        Vector3d sut = new Vector3d(x,y,z).square();

        assertEquals(x*x, sut.getX(), 0.01);
        assertEquals(y*y, sut.getY(), 0.01);
        assertEquals(z*z, sut.getZ(), 0.01);
    }

    @Test
    public void SubWorks(){
        double x = 10;
        double y = 20;
        double z = 30;
        Vector3d sut = new Vector3d(x,y,z).subtract(new Vector3d(10,10,10));

        assertEquals(x-10, sut.getX(), 0.01);
        assertEquals(y-10, sut.getY(), 0.01);
        assertEquals(z-10, sut.getZ(), 0.01);
    }

    @Test
    public void AddWorks(){
        double x = 10;
        double y = 20;
        double z = 30;
        Vector3d sut = new Vector3d(x,y,z).add(new Vector3d(10,10,10));

        assertEquals(x+10, sut.getX(), 0.01);
        assertEquals(y+10, sut.getY(), 0.01);
        assertEquals(z+10, sut.getZ(), 0.01);
    }

    @Test
    public void MultiWorks(){
        double x = 10;
        double y = 20;
        double z = 30;
        Vector3d sut = new Vector3d(x,y,z).multiply(2);

        assertEquals(x*2, sut.getX(), 0.01);
        assertEquals(y*2, sut.getY(), 0.01);
        assertEquals(z*2, sut.getZ(), 0.01);
    }

    @Test
    public void DiffWorks(){
        double x = 10;
        double y = 20;
        double z = 30;
        Vector3d sut = new Vector3d(x,y,z).divide(2);

        assertEquals(x/2d, sut.getX(), 0.01);
        assertEquals(y/2d, sut.getY(), 0.01);
        assertEquals(z/2d, sut.getZ(), 0.01);
    }

    @Test
    public void CopyWorksAndCreatesNewObject(){
        double x = 10;
        double y = 20;
        double z = 30;

        Vector3d original = new Vector3d(x,y,z);
        Vector3d sut = original.copy();

        assertEquals(x, sut.getX(), 0.01);
        assertEquals(y, sut.getY(), 0.01);
        assertEquals(z, sut.getZ(), 0.01);

        assertNotEquals(original, sut);
    }

    @Test
    public void CopyWithNewValues(){
        double x = 10;
        double y = 20;
        double z = 30;

        Vector3d sut = new Vector3d().copy(new Vector3d(x,y,z));

        assertEquals(x, sut.getX(), 0.01);
        assertEquals(y, sut.getY(), 0.01);
        assertEquals(z, sut.getZ(), 0.01);
    }
}
