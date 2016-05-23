package de.tu_darmstadt.smastra.sensors;

import android.Manifest;
import android.content.Context;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * A android sensor for GPS data.
 * @author Tobias Welther
 */
@SuppressWarnings("MissingPermission")
@NeedsOtherClass(Data3d.class)
@NeedsAndroidPermissions(Manifest.permission.ACCESS_COARSE_LOCATION)
@SensorConfig(displayName = "Gps Sensor", description = "The GPS sensor of the Device.")
public class AndroidGPSSensor implements Sensor, LocationListener {

    /**
     * The Location manager to use.
     */
    private final LocationManager locationManager;

    /**
     * The last known data.
     */
    private Data3d lastData = new Data3d(0,100000,0,0,0);


    public AndroidGPSSensor(Context context) {
        locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
    }


    /**
     * Starts the Listener.
     */
    @SensorStart
    @Override
    public void start(){
        locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 0, this);
    }


    /**
     * stops the Listener.
     */
    @SensorStop
    @Override
    public void stop(){
        locationManager.removeUpdates(this);
    }


    /**
     * Gets the last GPS data.
     * @return the last GPS data.
     */
    @SensorOutput
    public Vector3d getLastData() {
        return lastData;
    }

    @Override
    public void onLocationChanged(Location location) {
        long time = location.getTime();
        float accuracy = location.getAccuracy();
        float latitude = (float) location.getLatitude();
        float longitude = (float) location.getLongitude();
        float altitude = (float) location.getAltitude();

        this.lastData = new Data3d(time, accuracy, latitude, longitude, altitude);
    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {}

    @Override
    public void onProviderEnabled(String provider) {}

    @Override
    public void onProviderDisabled(String provider) {}
}
