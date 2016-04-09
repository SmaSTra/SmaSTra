package de.tu_darmstadt.smastra.generator.transaction;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;

import de.tu_darmstadt.smastra.generator.ElementGenerationFailedException;
import de.tu_darmstadt.smastra.generator.elements.Input;
import de.tu_darmstadt.smastra.generator.elements.Output;
import de.tu_darmstadt.smastra.markers.NeedsOtherClass;
import de.tu_darmstadt.smastra.markers.Transformation;

/**
 * Parses a class to a bunch of SmaSTra Transactions.
 *
 * @author Tobias Welther
 */
public class SmaSTraClassTransactionParser {


    /**
     * Reads the Transactions from the Class.
     * @param clazz to read from.
     * @return the Transactions
     */
    public static Collection<SmaSTraTransformation> readFromClass(Class<?> clazz){
        Collection<SmaSTraTransformation> transactions = new ArrayList<>();
        if(clazz == null) return transactions;

        //Read all methods:
        for(Method method : clazz.getMethods()){
            if(!hasTransactionAnnotation(method)) continue;

            try{
                SmaSTraTransactionBuilder builder = new SmaSTraTransactionBuilder();
                builder.setClass(clazz);
                builder.setMethodName(method.getName());
                builder.setStatic((method.getModifiers() & 0x8) == 0x8);

                builder.setDescription(readDescription(method));
                builder.setOutput(readOutput(method));
                builder.addInputs(readInput(method));
                builder.addNeededClass(readNeededClasses(clazz));

                SmaSTraTransformation transaction = builder.build();
                if(transaction != null) transactions.add(transaction);
            }catch(ElementGenerationFailedException exp){
                exp.printStackTrace();
            }
        }

        return transactions;
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
        return method.getAnnotation(Transformation.class).desctiption();
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
     * Reads the Needed Classes from the Class passed.
     * @param toReadFrom to use.
     * @return the needed Classes.
     */
    private static Collection<Class<?>> readNeededClasses(Class<?> toReadFrom){
        NeedsOtherClass annotation = toReadFrom.getAnnotation(NeedsOtherClass.class);
        if(annotation == null) return new ArrayList<>();

        return Arrays.asList(annotation.value());
    }


}
