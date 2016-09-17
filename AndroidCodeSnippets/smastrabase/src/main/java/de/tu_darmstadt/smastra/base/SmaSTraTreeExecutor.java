package de.tu_darmstadt.smastra.base;

import android.content.Context;
import android.util.Log;

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
    protected ScheduledExecutorService executor;

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
     * If already inited.
     */
    protected boolean inited = false;

    /**
     * If we have an error while initing:
     */
    protected boolean errorOnInit = false;


    /**
     * Creates the SmaSTra Execution Tree.
     * @param context to use.
     * @param maxSteps the maximal amount of steps present.
     */
    public SmaSTraTreeExecutor(int maxSteps, Context context) {
        this.maxSteps = maxSteps;
        this.context = context;
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
                if(!inited) {
                    inited = true;
                    try{
                        init();
                    }catch(Throwable exp){
                        Log.e("SmaSTra", "Error in Init Method. Shutting down! Please fix this!");
                        exp.printStackTrace();
                        errorOnInit = true;
                    }
                }

                try{
                    if(!errorOnInit) step();
                }catch(Throwable exp){
                    exp.printStackTrace();
                }
            }
        }, 50, 50, TimeUnit.MILLISECONDS);
    }


    /**
     * Stops ticking.
     */
    public void stop(){
        if(this.executor != null){
            this.executor.shutdown();
            this.executor = null;
        }
    }


    /**
     * Inits all needed stuff (as sensors, Web-APIs, aso).
     */
    protected void init() {}


    /**
     * Does a simple step.
     * <br>The step is calling each step of the Pipeline.
     */
    protected void step(){
        for(int i = 0; i < maxSteps; i++) {
            try{ transform(i); }catch (Throwable exp){ exp.printStackTrace(); }
        }
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
