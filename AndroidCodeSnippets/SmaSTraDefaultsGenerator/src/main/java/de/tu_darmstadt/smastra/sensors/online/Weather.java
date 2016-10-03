package de.tu_darmstadt.smastra.sensors.online;

import de.tu_darmstadt.smastra.markers.elements.datatype.DataType;
import de.tu_darmstadt.smastra.markers.elements.datatype.EnumDataType;

/**
 * @author Tobias Welther
 */
@EnumDataType
public enum Weather {

    THUNDER_STORM,
    DRIZZLE,
    RAINY,
    SNOW,
    ATMOSPHERE,
    CLEAR,
    CLOUDY,
    EXTREME,
    UNKNOWN;


    /**
     * Parses the Weather by ID.
     * @param id to parse
     * @return the parsed Weather.
     */
    public static Weather parseWeatherByID(int id){
        if (isBetween(id, 200, 299 ))
            return THUNDER_STORM;
        else if (isBetween(id, 300, 399))
            return DRIZZLE;
        else if (isBetween(id, 500, 599))
            return RAINY;
        else if (isBetween(id, 600, 699))
            return SNOW;
        else if (isBetween(id, 700, 799))
            return ATMOSPHERE;
        else if (isBetween(id, 800, 800))
            return CLEAR;
        else if (isBetween(id, 801, 809))
            return CLOUDY;
        else if (isBetween(id, 900, 909))
            return EXTREME;
        else
            return UNKNOWN;
    }


    private static boolean isBetween( int x, int startInclusive, int endInclusive ) {
        return startInclusive <= x && x <= endInclusive;
    }


}
