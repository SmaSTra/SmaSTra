package de.tu_darmstadt.smastra.sensors;

import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v4.content.LocalBroadcastManager;

import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;
import com.google.android.gms.location.ActivityRecognition;
import com.google.android.gms.location.ActivityRecognitionResult;
import com.google.android.gms.location.DetectedActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.SensorOutput;
import de.tu_darmstadt.smastra.markers.elements.SensorStart;
import de.tu_darmstadt.smastra.markers.elements.SensorStop;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * This represents the Android Activity Recognotion Sensor.
 *
 * @author Tobias Welther
 */
@NeedsAndroidPermissions("com.google.android.gms.permission.ACTIVITY_RECOGNITION")
@SensorConfig(displayName = "Activity Recognition Sensor", description = "This sensor represents the Android Activity Recognition")
public class AndroidActivityReognitionSensor extends BroadcastReceiver implements Sensor, GoogleApiClient.ConnectionCallbacks, GoogleApiClient.OnConnectionFailedListener, ResultCallback<Status> {

    private static final String BROADCAST_NAME = "ACTIVITY_RECOGNITION";

    /**
     * The Context to use.
     */
    private final Context context;

    /**
     * The API CLient to use for receiving ActivityRecognition Data.
     */
    private final GoogleApiClient apiClient;

    /**
     * The map activity -> Probability.
     */
    private final Map<String,Integer> activitiesToProbabilitiesMap = new HashMap<>();


    public AndroidActivityReognitionSensor(Context context){
        this.context = context;

        this.apiClient  = new GoogleApiClient.Builder(context)
                .addApi(ActivityRecognition.API)
                .addConnectionCallbacks(this)
                .addOnConnectionFailedListener(this)
                .build();
    }


    /**
     * Starts the Sensor.
     */
    @SensorStart
    @Override
    public void start(){
        LocalBroadcastManager.getInstance(context).registerReceiver(this, new IntentFilter(BROADCAST_NAME));
        apiClient.connect();
    }

    /**
     * Stops the Sensor.
     */
    @SensorStop
    @Override
    public void stop(){
        LocalBroadcastManager.getInstance(context).unregisterReceiver(this);
        apiClient.disconnect();
    }


    @Override
    public void configure(Map<String, Object> configuration) {}

    @Override
    public void configure(String key, Object value) {}

    @Override
    public void onConnected(@Nullable Bundle bundle) {
        ActivityRecognition.ActivityRecognitionApi.requestActivityUpdates(
                apiClient,
                1000,
                getActivityDetectionPendingIntent()
        ).setResultCallback(this);
    }

    /**
     * Gets a PendingIntent to be sent for each activity detection.
     */
    private PendingIntent getActivityDetectionPendingIntent() {
        Intent intent = new Intent(context, AndroidActivityReognitionSensor.class);

        // We use FLAG_UPDATE_CURRENT so that we get the same pending intent back when calling
        // requestActivityUpdates() and removeActivityUpdates().
        return PendingIntent.getBroadcast(context, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
    }


    @Override
    public void onConnectionSuspended(int i) {}


    @Override
    public void onConnectionFailed(@NonNull ConnectionResult connectionResult) {}


    @Override
    public void onReceive(Context context, Intent intent) {
        ActivityRecognitionResult result = ActivityRecognitionResult.extractResult(intent);

        // Get the list of the probable activities associated with the current state of the
        // device. Each activity is associated with a confidence level, which is an int between
        // 0 and 100.
        ArrayList<DetectedActivity> detectedActivities = (ArrayList<DetectedActivity>) result.getProbableActivities();
        for(DetectedActivity activity : detectedActivities){
            String readable = getFriendlyName(activity.getType());
            this.activitiesToProbabilitiesMap.put(readable, activity.getConfidence());
        }
    }


    /**
     * Gets the Current Activitys.
     * @return the current Activities.
     */
    @SensorOutput
    public String getMostLikelyActivity() {
        return findBestResult(activitiesToProbabilitiesMap);
    }

    /**
     * This finds the best result of the Map passed.
     * @param map to check.
     * @return the best result.
     */
    private String findBestResult(Map<String,Integer> map){
        if(map == null || map.isEmpty()) return "";

        String result = "";
        int prop = -1;

        for(Map.Entry<String,Integer> entry : activitiesToProbabilitiesMap.entrySet()){
            if(entry.getValue() > prop){
                prop = entry.getValue();
                result = entry.getKey();
            }
        }

        return result;
    }


    @Override
    public void onResult(@NonNull Status status) {}


    /**
     * Gets a friends name for the current activity.
     * @param detected_activity_type to resolve
     * @return the friendly readable name.
     */
    private static String getFriendlyName(int detected_activity_type){
        switch (detected_activity_type ) {
            case DetectedActivity.IN_VEHICLE:
                return "in vehicle";
            case DetectedActivity.ON_BICYCLE:
                return "on bike";
            case DetectedActivity.ON_FOOT:
                return "on foot";
            case DetectedActivity.TILTING:
                return "tilting";
            case DetectedActivity.STILL:
                return "still";
            default:
                return "unknown";
        }
    }

}
