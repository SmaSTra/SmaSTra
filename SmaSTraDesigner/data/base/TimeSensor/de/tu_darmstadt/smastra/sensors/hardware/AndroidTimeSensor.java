package de.tu_darmstadt.smastra.sensors.hardware;

import android.content.Context;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStop;
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

    @Override
    public void configure(String key, Object value) {}

    @SensorOutput
    public long getLastData() {
        return System.currentTimeMillis();
    }
}
