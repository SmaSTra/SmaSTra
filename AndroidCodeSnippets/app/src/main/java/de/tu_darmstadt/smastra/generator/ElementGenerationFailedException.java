package de.tu_darmstadt.smastra.generator;

/**
 * An error thrown, when the configuration of an Element is not valid.
 *
 * @author Tobias Welther
 */
public class ElementGenerationFailedException extends Throwable {

    public ElementGenerationFailedException(String detailMessage) {
        super(detailMessage);
    }

}
