package de.tu_darmstadt.smastra.sensors;

import org.junit.Test;

import static org.junit.Assert.assertEquals;

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
}
