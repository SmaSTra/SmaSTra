package de.tu_darmstadt.smastra.generator.extras;

import com.google.gson.JsonObject;

/**
 * This indicates that the Element needs a Service to register.
 * @author Tobias Welther
 */

public class NeedsBroadcast extends AbstractSmaSTraExtra {


    private static final String BROADCAST_CLASS_PATH = "broadcast";
    private static final String EXPORTABLE_PATH = "exportable";

    /**
     * The Class of the Broadcast to register.
     */
    private final String broadcastClassName;

    /**
     * If the broadcast is exportable.
     */
    private final boolean exportable;


    public NeedsBroadcast(String broadcastClassName, boolean exportable){
        super("broadcast");

        this.broadcastClassName = broadcastClassName;
        this.exportable = exportable;
    }


    @Override
    public JsonObject serialize() {
        JsonObject obj = super.serialize();
        obj.addProperty(BROADCAST_CLASS_PATH, broadcastClassName);
        obj.addProperty(EXPORTABLE_PATH, exportable);

        return obj;
    }
}
