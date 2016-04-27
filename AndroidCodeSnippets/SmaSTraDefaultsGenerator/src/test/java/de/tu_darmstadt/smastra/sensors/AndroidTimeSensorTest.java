package de.tu_darmstadt.smastra.sensors;

import junit.framework.Assert;

import org.junit.Test;

/**
 * A test for the Time stuff.
 * @author  Tobias Welther
 */
public class AndroidTimeSensorTest {

    @Test
    public void testIfValueIsOkay(){
        AndroidTimeSensor sut = new AndroidTimeSensor();

        long now = System.currentTimeMillis();
        long data = sut.getLastData();

        //About 100ms should be enough.
        Assert.assertEquals(now,data, 100);
    }

}
