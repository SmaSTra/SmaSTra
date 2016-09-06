package de.tu_darmstadt.smastra.buffers;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.BufferAdd;
import de.tu_darmstadt.smastra.markers.elements.BufferGet;
import de.tu_darmstadt.smastra.markers.elements.BufferInfo;
import de.tu_darmstadt.smastra.markers.elements.Configuration;
import de.tu_darmstadt.smastra.markers.elements.ConfigurationElement;
import de.tu_darmstadt.smastra.markers.interfaces.Buffer;
import de.tu_darmstadt.smastra.utils.ConfigParserUtils;

/**
 * This is a window which is used for timed collections.
 * <br>The time is taken when the element is Added!!!
 * <br>The list of data is completely Threadsafe, the data itself however is not!
 *
 * @author Tobias Welther
 */
@Configuration(elements =  {
        @ConfigurationElement(key = TimeWindowBuffer.TIME_PATH, configClass = Integer.class, description = "The time in MS that is buffered.")
})
@BufferInfo(displayName = "TimeWindowBuffer", description = "A buffer capturing over time.")
public class TimeWindowBuffer<T> implements Buffer<T> {

    static final String TIME_PATH = "time";

    /**
     * The Time in MS to keep.
     */
    private long timeForWindow = 2000;

    /**
     * The List of data to use.
     */
    private final List<TimeWrappedObject> data = new LinkedList<>();


    @BufferAdd
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

    @BufferGet
    @Override
    public Collection<T> getData() {
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


    @Override
    public void configure(Map<String, Object> configuration) {
        this.timeForWindow = ConfigParserUtils.parseInt(configuration.get(TIME_PATH), 2000);
    }

    @Override
    public void configure(String key, Object value) {
        if(TIME_PATH.equals(key)){
            this.timeForWindow = ConfigParserUtils.parseInt(value, 2000);
        }
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
