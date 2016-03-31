package de.tu_darmstadt.smastra.base;

import android.content.Context;

import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

/**
 * This is an abstract class for Traversing the SmaSTra Tree.
 *
 * @author Tobias Welther
 */
public abstract class SmaSTraTreeExecutor<T> {

    /**
     * The Executor service to use for ticking!
     */
    protected final ScheduledExecutorService executor;

    /**
     * The Context to use.
     */
    protected final Context context;

    /**
     * The maximal amount of steps in the Pipeline.
     */
    protected final int maxSteps;

    /**
     * This is the current data present to give to the user.
     */
    protected T data;


    /**
     * Creates the SmaSTra Execution Tree.
     * @param context to use.
     * @param maxSteps the maximal amount of steps present.
     */
    public SmaSTraTreeExecutor(int maxSteps, Context context) {
        this.maxSteps = maxSteps;
        this.context = context;

        //Start the ticking of the Pipeline:
        this.executor = Executors.newSingleThreadScheduledExecutor();
        this.executor.scheduleWithFixedDelay(new Runnable() {
            @Override
            public void run() {
                step();
            }
        }, 50, 50, TimeUnit.MILLISECONDS);

        initSensors();
    }

    /**
     * Inits all sensors.
     */
    protected void initSensors() {}


    /**
     * Does a simple step.
     * <br>The step is calling each step of the Pipeline.
     */
    public void step(){
        for(int i = 0; i < maxSteps; i++) transform(i);
    }


    /**
     * Does a Transform for the level passed.
     * @param level to use.
     */
    protected abstract void transform(int level);


    /**
     * Gets the current Data.
     * @return the current data.
     */
    public T getData(){
        return data;
    }

}
