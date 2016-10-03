package de.tu_darmstadt.smastra.generator.extras;

import com.google.gson.JsonObject;

/**
 * This indicates that the Element needs a Service to register.
 * @author Tobias Welther
 */

public class NeedsService extends AbstractSmaSTraExtra {


    private static final String SERVICE_CLASS_PATH = "service";
    private static final String EXPORTABLE_PATH = "exportable";

    /**
     * The Class of the Service to register.
     */
    private final String serviceClassName;

    /**
     * If the service is exportable.
     */
    private final boolean exportable;


    public NeedsService(String serviceClassName, boolean exportable){
        super("service");

        this.serviceClassName = serviceClassName;
        this.exportable = exportable;
    }


    @Override
    public JsonObject serialize() {
        JsonObject obj = super.serialize();
        obj.addProperty(SERVICE_CLASS_PATH, serviceClassName);
        obj.addProperty(EXPORTABLE_PATH, exportable);

        return obj;
    }
}
