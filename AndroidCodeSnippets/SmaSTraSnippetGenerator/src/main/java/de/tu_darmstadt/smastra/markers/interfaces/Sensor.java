package de.tu_darmstadt.smastra.markers.interfaces;

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

}
