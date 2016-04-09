package de.tu_darmstadt.smastra.markers;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

/**
 * This is a marker interface for generating snippets.
 * <br>It says that the class is exportable for the SmaSTra System.
 *
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
public @interface Exportable {}
