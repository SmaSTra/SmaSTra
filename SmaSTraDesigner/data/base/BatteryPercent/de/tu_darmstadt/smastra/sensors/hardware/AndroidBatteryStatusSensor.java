package de.tu_darmstadt.smastra.sensors.hardware;

import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;
import de.tu_darmstadt.smastra.sensors.Abstract1dAndroidSensor;

/**
 * This sensor sais if and with what the device is charged.
 * @author Tobias Welther
 */
@NeedsOtherClass({ Abstract1dAndroidSensor.class })
@SensorConfig(displayName = "Battery Percent", description = "Gives the percent of the battery (0-1, -1 if not readable))")
public class AndroidBatteryStatusSensor implements Sensor {

    /**
     * The Context to use.
     */
    private final Context context;


    public AndroidBatteryStatusSensor(Context context) {
        this.context = context;
    }

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


    /**
     * Gets the Status of the Charger.
     * @return the charger status.
     */
    @SensorOutput
    public double getStatus(){
        IntentFilter ifilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
        Intent batteryStatus = context.registerReceiver(null, ifilter);
        if(batteryStatus == null) return -1;

        int level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
        int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);

        return level / (float)scale;
    }




}
