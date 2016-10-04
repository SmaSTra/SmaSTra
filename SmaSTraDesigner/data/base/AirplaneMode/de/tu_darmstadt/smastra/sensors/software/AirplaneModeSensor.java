package de.tu_darmstadt.smastra.sensors.software;

import android.content.Context;
import android.provider.Settings;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * This indecates if the Airplane mode is on.
 * @author Tobias Welther
 */
@SensorConfig(displayName = "Airplane Mode", description = "Detects if the Airplane mode is toggled.")
public class AirplaneModeSensor implements Sensor {

    /**
     * The context to call on.
     */
    private final Context context;


    public AirplaneModeSensor(Context context){
        this.context = context;
    }


    @Override public void start() {}
    @Override public void stop() {}


    @Override public void configure(Map<String, Object> configuration) {}
    @Override public void configure(String key, Object value) {}


    @SensorOutput
    public boolean isOn(){
        return Settings.Global.getInt(context.getContentResolver(),
                Settings.Global.AIRPLANE_MODE_ON, 0) != 0;
    }

}
