package de.tu_darmstadt.smastra.generator.elements;

import java.util.ArrayList;
import java.util.List;

/**
 * This is an input parameter.
 *
 * @author Tobias Welther
 */
public class Input {

    /**
     * The list of input parameters.
     */
    private final List<Class<?>> parameters = new ArrayList<>();

    /**
     * The Name of the method.
     */
    private final String name;


    public Input(String name, List<Class<?>> parameters) {
        this.name = name;
        this.parameters.addAll(parameters);
    }


    /**
     * Returns the name of the method.
     * @return the name.
     */
    public String getName() {
        return name;
    }


    /**
     * Returns the Parameters of the Input.
     * @return parameters.
     */
    public List<Class<?>> getParameters() {
        return parameters;
    }
}
