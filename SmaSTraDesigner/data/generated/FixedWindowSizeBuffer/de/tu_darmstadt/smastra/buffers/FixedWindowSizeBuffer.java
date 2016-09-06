package de.tu_darmstadt.smastra.buffers;

import java.util.Collection;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import de.tu_darmstadt.smastra.markers.elements.BufferAdd;
import de.tu_darmstadt.smastra.markers.elements.BufferGet;
import de.tu_darmstadt.smastra.markers.elements.BufferInfo;
import de.tu_darmstadt.smastra.markers.elements.Configuration;
import de.tu_darmstadt.smastra.markers.elements.ConfigurationElement;
import de.tu_darmstadt.smastra.markers.interfaces.Buffer;
import de.tu_darmstadt.smastra.sensors.Abstract3dAndroidSensor;
import de.tu_darmstadt.smastra.utils.ConfigParserUtils;

/**
 * This is a collection of a fixed sized Buffer for data.
 * <br>The list of data is completely Threadsafe, the data itself however is not!
 *
 * @author Tobias Welther
 */
@Configuration(elements =  {
        @ConfigurationElement(key = FixedWindowSizeBuffer.SIZE_PATH, configClass = Integer.class, description = "How many elements can be in this buffer.")
})
@BufferInfo(displayName = "FixedWindowSizeBuffer", description = "A buffer with a fixed Window size.")
public class FixedWindowSizeBuffer<T> implements Buffer<T> {

    /**
     * The Path for the Size of the Window.
     */
    static final String SIZE_PATH = "size";


    /**
     * The fixed list to use.
     */
    private final List<T> data = new LinkedList<>();

    /**
     * The size to use.
     */
    private int size = 10;


    /**
     * Adds a new Element to the data.
     * @param element to add.
     */
    @BufferAdd
    @Override
    public void addData(T element){
        this.data.add(element);
        while(data.size() > size) data.remove(0);
    }


    /**
     * Returns the data in this window.
     */
    @BufferGet
    @Override
    public Collection<T> getData(){
        while(data.size() > size) data.remove(0);
        return data;
    }

    @Override
    public void configure(Map<String, Object> configuration) {
        this.size = ConfigParserUtils.parseInt(configuration.get(SIZE_PATH), 10);
    }

    @Override
    public void configure(String key, Object value) {
        if(SIZE_PATH.equals(key)){
            this.size = ConfigParserUtils.parseInt(value, 10);
        }
    }
}
