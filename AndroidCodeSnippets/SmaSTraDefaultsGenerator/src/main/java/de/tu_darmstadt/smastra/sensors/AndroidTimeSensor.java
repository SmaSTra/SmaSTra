package de.tu_darmstadt.smastra.sensors;

import android.content.Context;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * @author Tobias Welther
 */
@SensorConfig(displayName = "Time Sensor", description = "Gets the current Time in MS")
public class AndroidTimeSensor implements Sensor {


    /**
     * This is just for comparability.
     * @param context not used.
     */
    public AndroidTimeSensor(Context context){}


    @SensorStart
    @Override
    public void start(){}

    @SensorStop
    @Override
    public void stop(){}


    @Override
    public void configure(Map<String, Object> configuration) {}

    @SensorOutput
    public long getLastData() {
        return System.currentTimeMillis();
    }
}
