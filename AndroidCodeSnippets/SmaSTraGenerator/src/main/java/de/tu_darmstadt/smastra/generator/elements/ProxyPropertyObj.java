package de.tu_darmstadt.smastra.generator.elements;

import java.lang.reflect.Method;

import de.tu_darmstadt.smastra.markers.elements.proxyproperties.ProxyProperty;

/**
 * This is a holder for the Proxy-Property.
 * @author Tobias Welther
 */

public class ProxyPropertyObj {

    /**
     * The Method this contains.
     */
    private final Method method;

    /**
     * The Proxy class needed.
     */
    private final Class<?> proxyClass;

    /**
     * The Property object.
     */
    private final ProxyProperty property;


    public ProxyPropertyObj(Method method, ProxyProperty property) throws IllegalStateException {
        this.method = method;
        this.property = property;

        Class<?>[] args = method.getParameterTypes();
        if(args.length != 1) throw new IllegalStateException("Can not take Proxy method " + method.getName() + ". Need Exactly 1 Arg!");

        this.proxyClass = args[0];
    }


    /**
     * Gets the Method to use
     * @return the method to use.
     */
    public Method getMethod() {
        return method;
    }

    /**
     * Gets the Property annotation.
     * @return the method to use.
     */
    public ProxyProperty getProperty() {
        return property;
    }

    /**
     * Gets the proxy class.
     * @return the proxy class.
     */
    public Class<?> getProxyClass() {
        return proxyClass;
    }
}
