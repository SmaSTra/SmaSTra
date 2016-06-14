package de.tu_darmstadt.smastra.markers.interfaces;

import java.util.Map;

/**
 * A marker interface to know that this is a Sensor for SmaSTra.
 * @author Tobias Welther
 */
public interface Sensor {

    /**
     * Starts the Sensor.
     */
    void start();


    /**
     * Stops the Sensor.
     */
    void stop();

    /**
     * Sets the configuration wanted.
     * @param configuration to use.
     */
    void configure(Map<String,Object> configuration);
}
