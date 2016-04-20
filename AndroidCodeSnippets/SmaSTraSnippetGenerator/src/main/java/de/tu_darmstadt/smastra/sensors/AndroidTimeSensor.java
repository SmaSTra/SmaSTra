package de.tu_darmstadt.smastra.sensors;

import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * @author Tobias Welther
 */
@SensorConfig(displayName = "Time Sensor", description = "Gets the current Time in MS")
public class AndroidTimeSensor implements Sensor {

    @SensorOutput
    public long getLastData() {
        return System.currentTimeMillis();
    }
}
