package de.tu_darmstadt.smastra.generator.extras;

import com.google.gson.JsonObject;

/**
 * Created by Toby on 03.10.2016.
 */

public class NeedsLibrary extends AbstractSmaSTraExtra {


    private static final String LIB_NAME_PATH = "lib";

    /**
     * The Library to add.
     */
    private final String libName;


    public NeedsLibrary(String libName) {
        super("lib");

        this.libName = libName;
    }


    @Override
    public JsonObject serialize() {
        JsonObject obj = super.serialize();
        obj.addProperty(LIB_NAME_PATH, libName);

        return obj;
    }
}
