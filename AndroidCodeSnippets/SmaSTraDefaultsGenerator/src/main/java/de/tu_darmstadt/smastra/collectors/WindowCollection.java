package de.tu_darmstadt.smastra.collectors;

import java.util.Collection;
import java.util.List;

/**
 * An interface for a simple collection of data.
 * <br>Similar to a java collection.
 *
 * @author Tobias Welther
 */
public interface WindowCollection <T> extends de.tu_darmstadt.smastra.markers.interfaces.Buffer {

    /**
     * Adds a new Element to the data.
     * @param element to add.
     */
    void addData(T element);


    /**
     * Returns the data in this window.
     */
    Collection<T> getData();


    /**
     * Returns the data in this window as List.
     */
    List<T> getDataList();

}
