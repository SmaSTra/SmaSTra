package de.tu_darmstadt.smastra.base;

import android.content.Context;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import de.tu_darmstadt.smastra.utils.ConfigParserUtils;

/**
 * This is an abstract class for Traversing the SmaSTra Tree.
 *
 * @author Tobias Welther
 */
public abstract class SmaSTraTreeExecutor<T> {

    /**
     * The Prefix for the transformation Method.
     */
    private static final String TRANSFORM_METHOD_PREFIX = "transform";

    /**
     * The Executor service to use for ticking!
     */
    private ScheduledExecutorService executor;

    /**
     * The Context to use.
     */
    protected final Context context;

    /**
     * The List of Methods to call.
     */
    private final List<Method> transformMethods = new ArrayList<>();

    /**
     * This is the current data present to give to the user.
     */
    protected T data;


    /**
     * Creates the SmaSTra Execution Tree.
     * @param context to use.
     */
    protected SmaSTraTreeExecutor(Context context) {
        this.context = context;

        //Read the Methods to call:
        Class<? extends  SmaSTraTreeExecutor> clazz = getClass();
        for(Method method : clazz.getDeclaredMethods()){
            if (method.getName().startsWith(TRANSFORM_METHOD_PREFIX)){
                transformMethods.add(method);
            }
        }

        //Sort the method to the index:
        Collections.sort(transformMethods, new Comparator<Method>() {
            @Override
            public int compare(Method o1, Method o2) {
                int nr1 = ConfigParserUtils.parseInt(o1.getName().substring(TRANSFORM_METHOD_PREFIX.length()), 0);
                int nr2 = ConfigParserUtils.parseInt(o2.getName().substring(TRANSFORM_METHOD_PREFIX.length()), 0);
                return Integer.compare(nr1,nr2);
            }
        });
    }

    /**
     * Starts ticking.
     */
    public void start(){
        //If not stopped -> Stop before!
        if(this.executor != null) stop();

        //Start the ticking of the Pipeline:
        this.executor = Executors.newSingleThreadScheduledExecutor();
        this.executor.scheduleWithFixedDelay(new Runnable() {
            @Override
            public void run() {
                try{
                    step();
                }catch(Throwable exp){
                    exp.printStackTrace();
                }
            }
        }, 50, 50, TimeUnit.MILLISECONDS);

        this.startIntern();
    }


    /**
     * Stops ticking.
     */
    public void stop(){
        if(this.executor != null){
            this.executor.shutdown();
            this.executor = null;
            this.stopIntern();
        }
    }


    /**
     * Starts all intern stuff.
     */
    protected abstract void startIntern();


    /**
     * Stops all intern stuff.
     */
    protected abstract void stopIntern();


    /**
     * Does a simple step.
     * <br>The step is calling each step of the Pipeline.
     */
    private void step(){
        for(Method method : transformMethods) {
            try{
                method.invoke(this);
            }catch (Throwable exp){ exp.printStackTrace(); }
        }
    }


    /**
     * Gets the current Data.
     * @return the current data.
     */
    public T getData(){
        return data;
    }

}
