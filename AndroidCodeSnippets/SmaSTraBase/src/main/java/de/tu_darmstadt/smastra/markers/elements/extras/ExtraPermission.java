package de.tu_darmstadt.smastra.markers.elements.extras;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

/**
 * This is an encapsulation for the Extras Annotation.
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
public @interface ExtraPermission {
    String permission();
}
