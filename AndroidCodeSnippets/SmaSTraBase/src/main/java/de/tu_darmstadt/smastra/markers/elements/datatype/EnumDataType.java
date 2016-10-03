package de.tu_darmstadt.smastra.markers.elements.datatype;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * This is a annotation to extract an Enum as DataType.
 * @author Tobias Welther
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface EnumDataType {}
