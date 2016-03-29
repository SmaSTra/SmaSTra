package de.tu_darmstadt.smastra.collectors;

import java.util.Collection;
import java.util.LinkedList;
import java.util.List;

import de.tu_darmstadt.smastra.markers.Exportable;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;

/**
 * This is a collection of a fixed sized Buffer for data.
 * <br>The list of data is completely Threadsafe, the data itself however is not!
 *
 * @author Tobias Welther
 */
@Exportable
@NeedsOtherClass(WindowCollection.class)
public class FixedWindowCollector <T> implements WindowCollection<T> {

    /**
     * The fixed list to use.
     */
    private final List<T> data = new LinkedList<>();

    /**
     * The size to use.
     */
    private final int size;


    public FixedWindowCollector(int size) {
        this.size = size;
    }


    /**
     * Adds a new Element to the data.
     * @param element to add.
     */
    public void addData(T element){
        this.data.add(element);
        if(data.size() >= size) data.remove(0);
    }


    /**
     * Returns the data in this window.
     */
    public Collection<T> getData(){
        return data;
    }


    /**
     * Returns the data in this window as List.
     */
    public List<T> getDataList(){
        return data;
    }

}
