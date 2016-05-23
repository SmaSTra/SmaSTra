package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;

import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * This sensor sais if and with what the device is charged.
 * @author Tobias Welther
 */
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
