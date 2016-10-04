package de.tu_darmstadt.smastra.generator.extras;

import com.google.gson.JsonObject;

/**
 * This is an Extra standing for a Permission needed.
 * @author Tobias Welther
 */

public class NeedsPermission extends AbstractSmaSTraExtra {

    private static final String PERMISSION_PATH = "permission";


    /**
     * The Permission requested.
     */
    private final String permission;


    public NeedsPermission(String permission) {
        super("permission");

        this.permission = permission;
    }


    public String getPermission() {
        return permission;
    }

    @Override
    public JsonObject serialize() {
        JsonObject obj = super.serialize();
        obj.addProperty(PERMISSION_PATH, permission);

        return obj;
    }


    @Override
    public boolean equals(Object obj) {
        if(obj instanceof  NeedsPermission){
            return ((NeedsPermission) obj).permission.equals(permission);
        }

        return super.equals(obj);
    }

    @Override
    public int hashCode() {
        return ("permission" + permission).hashCode();
    }
}
