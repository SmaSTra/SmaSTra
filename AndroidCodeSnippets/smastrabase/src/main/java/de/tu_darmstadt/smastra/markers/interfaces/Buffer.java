package de.tu_darmstadt.smastra.markers.interfaces;

import java.util.Collection;
import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.BufferAdd;
import de.tu_darmstadt.smastra.markers.elements.BufferGet;

/**
 * A marker interface to know that this is a Buffer for SmaSTra.
 * @author Tobias Welther
 */
public interface Buffer <T> {


    /**
     * Adds a new Element to the data.
     * @param element to add.
     */
    @BufferAdd
    void addData(T element);


    /**
     * Returns the data in this window.
     */
    @BufferGet
    Collection<T> getData();


    /**
     * Sets the configuration wanted.
     * @param configuration to use.
     */
    void configure(Map<String,Object> configuration);

    /**
     * Sets the configuration wanted.
     * @param key to use.
     * @param value to set
     */
    void configure(String key, Object value);

}
