package de.tu_darmstadt.smastra.time;

import java.util.Calendar;

import de.tu_darmstadt.smastra.markers.elements.transformation.Transformation;

/**
 * This handles transformations from the
 * @author Tobias Welther
 */
public class TimeTransformations implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    @Transformation(displayName = "TimeBetweenHour", description = "Returns true if the Time is between these two hours.")
    public static boolean isBetween(int startHour, int endHour){
        int hour = Calendar.getInstance().get(Calendar.HOUR_OF_DAY);
        if(endHour < startHour) endHour += 24;

        return hour >= startHour && hour < endHour;
    }

    @Transformation(displayName = "TimeBetween", description = "Returns true if the Time is between these two times.")
    public static boolean isBetween(int startHour, int startMinute, int endHour, int endMinute){
        int hour = Calendar.getInstance().get(Calendar.HOUR_OF_DAY);
        int minute = Calendar.getInstance().get(Calendar.MINUTE);
        if(endHour < startHour) endHour += 24;

        return hour >= startHour && minute >= startMinute && hour <= endHour && minute <= endMinute;
    }
}
