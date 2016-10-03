package de.tu_darmstadt.smastra.sensors.online;

import android.Manifest;
import android.content.Context;
import android.location.Location;
import android.location.LocationManager;
import android.os.AsyncTask;
import android.util.Log;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Map;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.proxyproperties.ProxyProperty;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorConfig;
import de.tu_darmstadt.smastra.markers.elements.sensors.SensorOutput;
import de.tu_darmstadt.smastra.markers.interfaces.Sensor;

/**
 * This is a simple Weather API call.
 * @author Tobias Welther
 */
@NeedsAndroidPermissions({Manifest.permission.INTERNET, Manifest.permission.ACCESS_COARSE_LOCATION})
@NeedsOtherClass(Weather.class)
@SensorConfig(displayName = "Current Weather", description = "Displays the current weather of the user.")
public class CurrentWeatherSensor implements Sensor {

    /**
     * The Template for the Address
     */
    private static final String WEATHER_ADDRESS_TEMPLATE = "http://api.openweathermap.org/data/2.5/weather?lat={LAT}&lon={LON}&APPID={API_KEY}";


    /**
     * The last cached weather.
     */
    private Weather lastWeather = Weather.UNKNOWN;

    /**
     * The API key to use.
     */
    private String API_KEY = "";

    /**
     * When we tried last.
     */
    private long lastTried = 0;

    /**
     * The last time the weather got successfully polled.
     */
    private long lastSuccess = 0;


    /**
     * The context to use.
     */
    private final Context context;


    public CurrentWeatherSensor(Context context){
        this.context = context;
    }


    /**
     * Sets the API key to use.
     * @param apiKey to use.
     */
    @ProxyProperty(name = "SetAPIKey")
    public void setAPIKey(String apiKey){
        this.API_KEY = apiKey;
    }

    /**
     * Triggers an Update for the Weather.
     */
    private void updateWeather(){
        long now = System.currentTimeMillis();
        //Update max. every 5 Minutes:
        if(now < lastSuccess + (1000 * 60 * 5)){
            lastTried = now;
            return;
        }

        //Update the last try:
        this.lastTried = System.currentTimeMillis();

        //No api key, cant do anything!
        if(API_KEY.isEmpty()) return;

        //Find the newest Location in total:
        LocationManager locationManager = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
        Location last = null;
        for(String provider : locationManager.getAllProviders()){
            Location loc = locationManager.getLastKnownLocation(provider);
            if(loc == null) continue;

            if(last == null) last = loc;
            else if (loc.getTime() > last.getTime()) last = loc;
        }

        //No location found!
        if(last == null) return;

        //Now update Async:
        new UpdateWeatherAsync().execute(last.getLatitude(), last.getLongitude(), API_KEY, this);
    }


    private class UpdateWeatherAsync extends AsyncTask<Object,Void,Integer>{

        private CurrentWeatherSensor sensor;

        @Override
        protected Integer doInBackground(Object... params) {
            double lat = (double) params[0];
            double lon = (double) params[1];
            String apiKey = params[2].toString();
            sensor = (CurrentWeatherSensor) params[3];

            String address = WEATHER_ADDRESS_TEMPLATE
                    .replace("{LAT}", String.valueOf(lat))
                    .replace("{LON}", String.valueOf(lon))
                    .replace("{API_KEY}", apiKey);

            //The connections to close later on.
            HttpURLConnection con = null ;
            InputStream is = null;

            try{
                con = (HttpURLConnection) ( new URL(address)).openConnection();
                con.setRequestMethod("GET");
                con.setDoInput(true);
                con.setDoOutput(true);
                con.connect();

                // Let's read the response
                is = con.getInputStream();
                BufferedReader br = new BufferedReader(new InputStreamReader(is));

                JsonObject obj = new JsonParser().parse(br).getAsJsonObject();
                if(!obj.has("weather")) return -1;

                JsonArray array = obj.get("weather").getAsJsonArray();
                if(array.size() <= 0) return -1;

                return array.get(0).getAsJsonObject().get("id").getAsInt();
            }catch (Throwable exp){
                Log.w("Weather", "Error on getting weather", exp);
            }finally {
                try{ if(con != null) con.disconnect(); }catch (Throwable exp){}
                try{ if(is != null) is.close(); }catch (Throwable exp2){}
            }

            return -1;
        }

        @Override
        protected void onPostExecute(Integer result) {
            super.onPostExecute(result);

            //Update to the result:
            if(sensor != null){
                sensor.lastWeather = Weather.parseWeatherByID(result);
                if(sensor.lastWeather != Weather.UNKNOWN) sensor.lastSuccess = System.currentTimeMillis();
            }
        }
    }

    @Override public void start() {}
    @Override public void stop() {}

    @Override public void configure(Map<String, Object> configuration) {}
    @Override public void configure(String key, Object value) {}


    @SensorOutput
    public Weather getLastWeather(){
        if(System.currentTimeMillis() > lastTried + 60_000) updateWeather();
        return lastWeather;
    }

}
