package de.tu_darmstadt.smastra.markers.elements.extras;

import android.content.BroadcastReceiver;

/**
 * Created by Toby on 03.10.2016.
 */

public @interface ExtraBroadcast {

    /**
     * The class used.
     * @return the class used.
     */
    Class<? extends BroadcastReceiver> clazz();

    /**
     * If the Broadcast is exported to others.
     * @return if the Class is exported.
     */
    boolean exported() default false;

}
