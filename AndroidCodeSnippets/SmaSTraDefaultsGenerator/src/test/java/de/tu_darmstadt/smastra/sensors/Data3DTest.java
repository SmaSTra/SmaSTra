package de.tu_darmstadt.smastra.sensors;

import org.junit.Test;

import static org.junit.Assert.assertEquals;

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

}