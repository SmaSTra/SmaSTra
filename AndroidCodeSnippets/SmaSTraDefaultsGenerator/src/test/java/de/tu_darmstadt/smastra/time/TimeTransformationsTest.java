package de.tu_darmstadt.smastra.time;

import org.junit.Test;

import java.util.Calendar;

import static org.junit.Assert.*;

/**
 * This are simple tests to confirm that the Time-Comparison Lib is working as intended.
 */
public class TimeTransformationsTest {


    @Test
    public void testTimeBetweenHoursWorksIfBetween(){
        int start = Calendar.getInstance().get(Calendar.HOUR_OF_DAY) - 1;
        int end = Calendar.getInstance().get(Calendar.HOUR_OF_DAY) + 1;

        boolean sut = TimeTransformations.isBetween(start, end);
        assertTrue(sut);
    }

    @Test
    public void testTimeBetweenHoursWorksOutside(){
        int start = Calendar.getInstance().get(Calendar.HOUR_OF_DAY) - 2;
        int end = Calendar.getInstance().get(Calendar.HOUR_OF_DAY) - 1;

        boolean sut = TimeTransformations.isBetween(start, end);
        assertFalse(sut);
    }


}