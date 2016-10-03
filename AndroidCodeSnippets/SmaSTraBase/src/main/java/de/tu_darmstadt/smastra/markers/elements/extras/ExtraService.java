package de.tu_darmstadt.smastra.markers.elements.extras;

import android.app.Service;

/**
 * This adds a Service to register.
 * @author Tobias Welther
 */

public @interface ExtraService {

    /**
     * The class used.
     * @return the class used.
     */
    Class<? extends Service> clazz();

    /**
     * If the Service is exported to others.
     * @return if the Class is exported.
     */
    boolean exported() default false;

}
