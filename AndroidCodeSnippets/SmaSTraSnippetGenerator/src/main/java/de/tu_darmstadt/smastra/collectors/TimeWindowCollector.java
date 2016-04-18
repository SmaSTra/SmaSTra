package de.tu_darmstadt.smastra.collectors;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

import de.tu_darmstadt.smastra.markers.NeedsOtherClass;

/**
 * This is a window which is used for timed collections.
 * <br>The time is taken when the element is Added!!!
 * <br>The list of data is completely Threadsafe, the data itself however is not!
 *
 * @author Tobias Welther
 */
@NeedsOtherClass(WindowCollection.class)
public class TimeWindowCollector <T> implements WindowCollection<T> {


    /**
     * The Time in MS to keep.
     */
    private final long timeForWindow;

    /**
     * The List of data to use.
     */
    private final List<TimeWrappedObject> data = new LinkedList<>();


    public TimeWindowCollector(long timeForWindow) {
        this.timeForWindow = timeForWindow;
    }


    @Override
    public void addData(T element) {
        addData(System.currentTimeMillis(), element);
    }

    /**
     * Adds data with a defined time.
     * @param time to set
     * @param element to set.
     */
    public void addData(long time, T element) {
        long now = System.currentTimeMillis();
        long expired = time + timeForWindow;

        //Already expired -> Do not need to add!
        if(now > expired) return;

        //To make sure we are Thread-safe.
        synchronized (this.data) {
            this.data.add(new TimeWrappedObject(expired, element));
        }
    }


    @Override
    public Collection<T> getData() {
        return getDataList();
    }


    @Override
    public List<T> getDataList() {
        List<T> data = new ArrayList<>();
        long now = System.currentTimeMillis();
        synchronized (this.data){
            Iterator<TimeWrappedObject> it = this.data.iterator();

            //Remove all that are expired.
            while(it.hasNext()){
                if(it.next().getExpired() < now) {
                    it.remove();
                }
            }

            //Copy the rest:
            for(TimeWrappedObject obj : this.data) {
                data.add(obj.getObject());
            }
        }

        return data;
    }


    /**
     * Small inner class that wraps an Object with a timestamp.
     *
     * @author Tobias Welther
     */
    private class TimeWrappedObject {
        private final long expired;
        private final T object;

        public TimeWrappedObject(long expired, T object) {
            this.expired = expired;
            this.object = object;
        }

        public long getExpired() {
            return expired;
        }

        public T getObject() {
            return object;
        }
    }


}
