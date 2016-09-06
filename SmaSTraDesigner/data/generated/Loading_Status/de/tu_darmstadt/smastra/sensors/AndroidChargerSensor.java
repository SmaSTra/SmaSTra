package de.tu_darmstadt.smastra.sensors;

import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;

import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * This sensor sais if and with what the device is charged.
 * @author Tobias Welther
 */
@SensorConfig(displayName = "Loading Status", description = "Gives the Loading status (1 = AC, 2 = USB, 4 = Wireless)")
public class AndroidChargerSensor implements Sensor {

    /**
     * The Context to use.
     */
    private final Context context;


    public AndroidChargerSensor(Context context) {
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

        return batteryStatus.getIntExtra(BatteryManager.EXTRA_PLUGGED, -1);
    }




}
