package de.tu_darmstadt.smastra.sensors;

import org.junit.Test;

import de.tu_darmstadt.smastra.math.Math3d;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

/**
 * Test for the Vector class.
 */
public class Vector3dTest {



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
}
