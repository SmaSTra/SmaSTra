package de.tu_darmstadt.smastra.sensors;

import junit.framework.Assert;

import org.junit.Test;

import de.tu_darmstadt.smastra.sensors.hardware.AndroidTimeSensor;

/**
 * A test for the Time stuff.
 * @author  Tobias Welther
 */
public class AndroidTimeSensorTest {

    @Test
    public void testIfValueIsOkay(){
        AndroidTimeSensor sut = new AndroidTimeSensor(null);

        long now = System.currentTimeMillis();
        long data = sut.getLastData();

        //About 100ms should be enough.
        Assert.assertEquals(now,data, 100);
    }

}
