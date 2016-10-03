package de.tu_darmstadt.smastra.generator.extras;

import com.google.gson.JsonObject;

import de.tu_darmstadt.smastra.markers.interfaces.SmaSTraExtra;

/**
 * The basic class for Extras.
 */

public abstract class AbstractSmaSTraExtra implements SmaSTraExtra {


    private static final String TYPE_PATH = "type";


    /**
     * The Name of the Type.
     */
    private final String typeName;


    public AbstractSmaSTraExtra(String typeName){
        this.typeName = typeName;
    }


    public JsonObject serialize() {
        JsonObject obj = new JsonObject();
        obj.addProperty(TYPE_PATH, typeName);

        return obj;
    }
}
