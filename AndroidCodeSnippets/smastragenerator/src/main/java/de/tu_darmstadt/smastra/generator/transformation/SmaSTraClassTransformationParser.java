package de.tu_darmstadt.smastra.generator.transformation;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.generator.elements.ProxyPropertyObj;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.elements.NeedsAndroidPermissions;
import de.tu_darmstadt.smastra.markers.elements.Transformation;

/**
 * Parses a class to a bunch of SmaSTra Transactions.
 *
 * @author Tobias Welther
 */
public class SmaSTraClassTransformationParser {


    /**
     * Reads the Transformation from the Class.
     * @param clazz to read from.
     * @return the Transformations.
     */
    public static Collection<SmaSTraTransformation> readFromClass(Class<?> clazz){
        Collection<SmaSTraTransformation> transformations = new ArrayList<>();

        //First check if the Class is a Transformation:
        if(clazz == null || !de.tu_darmstadt.smastra.markers.interfaces.Transformation.class.isAssignableFrom(clazz)) {
            return transformations;
        }

        //Read all methods:
        for(Method method : clazz.getMethods()){
            if(!hasTransactionAnnotation(method)) continue;

            try{
                SmaSTraTransformationBuilder builder = new SmaSTraTransformationBuilder();
                builder.setClass(clazz);
                builder.setMethodName(method.getName());
                builder.setStatic((method.getModifiers() & 0x8) == 0x8);

                builder.setDisplayName(readDisplayName(method));
                builder.setDescription(readDescription(method));
                builder.setOutput(readOutput(method));
                builder.addInputs(readInput(method));
                builder.setAndroidPermissions(readNeededPermissions(clazz));
                builder.addNeededClass(readNeededClasses(clazz));
                builder.addProxyProperties(readProxyProperties(method));

                SmaSTraTransformation transaction = builder.build();
                if(transaction != null) transformations.add(transaction);
            }catch(ElementGenerationFailedException exp){
                exp.printStackTrace();
            }
        }

        return transformations;
    }


    /**
     * Reads the needed Permissions from the Class.
     * @param clazz to use.
     * @return the needed Permissions. Empty Array if none present.
     */
    private static String[] readNeededPermissions(Class<?> clazz) {
        NeedsAndroidPermissions permsAnnotation = clazz.getAnnotation(NeedsAndroidPermissions.class);
        return permsAnnotation == null ? new String[0] : permsAnnotation.value();
    }

    /**
     * Reads the DisplayName from the Method.
     * @param method th read from.
     * @return the displayName.
     */
    private static String readDisplayName(Method method) {
        Transformation transformation = method.getAnnotation(Transformation.class);
        return transformation == null ? null : transformation.displayName();
    }

    /**
     * Reads an Output from the Method passed.
     * @param method to read from.
     * @return the read Output.
     */
    private static Output readOutput(Method method) {
        Class<?> returnType = method.getReturnType();
        return returnType == null || returnType == void.class ? Output.VOID_OUTPUT : new Output(returnType);
    }


    /**
     * Reads the Description from the Transaction.
     * @return the read Description or 'None' if none given.
     */
    private static String readDescription(Method method) {
        return method.getAnnotation(Transformation.class).description();
    }


    /**
     * Checks if the Transaction annotation is present.
     * @param method to observ.
     * @return true if present.
     */
    private static boolean hasTransactionAnnotation(Method method) {
        return method.getAnnotation(Transformation.class) != null;
    }


    /**
     * Reads the Input from a Method.
     * @param method to read from.
     * @return the inputs.
     */
    private static List<Input> readInput(Method method){
        List<Input> inputList = new ArrayList<>();
        int argNr = 0;

        for(Class<?> parameter : method.getParameterTypes()){
            inputList.add(new Input("arg"+argNr, parameter));
            argNr++;
        }

        return inputList;
    }

    /**
     * Reads the proxy properties from the Methos
     * @return the
     */
    private static Collection<ProxyPropertyObj> readProxyProperties(Method method){
        //at the moment not for Transforamtions.
        return new ArrayList<>();
    }


    /**
     * Reads the Needed Classes from the Class passed.
     * <br>This does a iterative Lookup!
     * @param toReadFrom to use.
     * @return the needed Classes.
     */
    private static Collection<Class<?>> readNeededClasses(Class<?> toReadFrom){
        Set<Class<?>> neededClasses = new HashSet<>();

        NeedsOtherClass annotation = toReadFrom.getAnnotation(NeedsOtherClass.class);
        if(annotation == null) return neededClasses;

        //Do a iterative lookup!
        neededClasses.addAll(Arrays.asList(annotation.value()));
        int size = 0;
        while(size != neededClasses.size()){
            size = neededClasses.size();
            for(Class<?> clazz : new HashSet<>(neededClasses)){
                annotation = clazz.getAnnotation(NeedsOtherClass.class);
                if(annotation == null) continue;

                neededClasses.addAll(Arrays.asList(annotation.value()));
            }
        }

        return neededClasses;
    }


}
