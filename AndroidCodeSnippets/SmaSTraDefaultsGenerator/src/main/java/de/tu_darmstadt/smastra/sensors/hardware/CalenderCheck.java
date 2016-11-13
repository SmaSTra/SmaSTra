package de.tu_darmstadt.smastra.sensors.hardware;

import android.Manifest;
import android.content.Context;
import android.database.Cursor;
import android.provider.CalendarContract;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.extras.ExtraPermission;
import de.tu_darmstadt.smastra.markers.elements.extras.Extras;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * Check Calender
 */
@SuppressWarnings("MissingPermission")
@Extras(permissions = @ExtraPermission(permission = Manifest.permission.READ_CALENDAR))
@SensorConfig(displayName = "CalenderCheck", description = "check how much event in calender now")
public class CalenderCheck implements Sensor {

    /**
     * The Context to use.
     */
    private final Context context;
    protected double lastData = 0;
    Cursor cursor = null;

    public CalenderCheck(Context context){this.context = context;}

    @SensorStart
    @Override
    public void start() {
        check();
   }

    @SensorStop
    @Override
    public void stop() {
        cursor.close();
    }

    @Override
    public void configure(Map<String, Object> configuration) {

    }

    public void check(){
        long time = System.currentTimeMillis();
        String selection = "((" + CalendarContract.Events.DTSTART + "<=" + time + " ) AND ( " + CalendarContract.Events.DTEND + ">=" + time + "))";
        cursor = context.getContentResolver().query(CalendarContract.Events.CONTENT_URI, null, selection, null, null);
        lastData = cursor.getCount();
        System.out.println(lastData);
    }
    @Override
    public void configure(String key, Object value) {

    }

    @SensorOutput
    public double getLastData(){
        check();
        return lastData;
    }

}
