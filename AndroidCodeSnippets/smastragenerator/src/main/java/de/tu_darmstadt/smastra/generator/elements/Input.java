package de.tu_darmstadt.smastra.generator.elements;

/**
 * This is an input parameter.
 *
 * @author Tobias Welther
 */
public class Input {

    /**
     * The list of input parameters.
     */
    private final Class<?> parameter;

    /**
     * The Name of the method.
     */
    private final String name;


    public Input(String name, Class<?> parameter) {
        this.name = name;
        this.parameter = parameter;
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
    public Class<?> getParameter() {
        return parameter;
    }
}
