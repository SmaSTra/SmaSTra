package de.tu_darmstadt.smastra.sensors.online;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.transformation.Transformation;

/**
 * @author Tobias Welther
 */
@NeedsOtherClass(Weather.class)
public class WeatherTransformations implements de.tu_darmstadt.smastra.markers.interfaces.Transformation {


    /**
     * Transformats the Weather enum to a readable string.
     * @param weather to convert
     * @return the readable name.
     */
    @Transformation(displayName = "Weather to string", description = "Converts a Weather to a string")
    public static String WeatherToString(Weather weather){
        return weather == null ? "Unknown" : weather.name();
    }


    /**
     * Checks if the Weather is the weather wanted.
     * @param weather to check
     * @param toTestAgainst the string to test the weather against.
     * @return the readable name.
     */
    @Transformation(displayName = "Is Weather", description = "Checks if the Weather is the weather wanted.")
    public static boolean IsWeather(Weather weather, String toTestAgainst){
        if(weather == null || toTestAgainst == null) return false;
        return weather.name().equalsIgnoreCase(toTestAgainst);
    }


}
