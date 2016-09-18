package de.tu_darmstadt.smastra.base;

import android.content.Context;

import org.junit.Test;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;
import static junit.framework.Assert.fail;

/**
 * This is the test for the Tree Executor.
 *
 * @author Tobias Welther
 */
public class SmaSTraTreeExecutorTest {


    @Test
    public void testAllLevelsGetCalledInTheCorrectOrder() throws InterruptedException {
        int max = 2;
        TestTree1 sut = new TestTree1(null);
        long start = System.currentTimeMillis();

        sut.start();
        while(sut.call != max){
            Thread.sleep(2);

            //Something went wrong!
            if(System.currentTimeMillis() - start > 1000){
                sut.stop();
                fail("Took too long!");
                return;
            }
        }

        //Assert all steps were taken.
        sut.stop();
        assertEquals(max, sut.call);
    }


    private class TestTree1 extends SmaSTraTreeExecutor<String> {

        int call = 0;

        TestTree1(Context context) {
            super(context);
        }

        @Override protected void startIntern() {}
        @Override protected void stopIntern() {}

        protected void transform0() {
            assertEquals(0, call);
            call++;
        }

        protected void transform1() {
            assertEquals(1, call);
            call++;
        }

    }


    @Test
    public void testStopStopsTheTicking() throws InterruptedException {
        TestTree2 sut = new TestTree2(null);
        sut.start();

        //Sleep a bit.
        Thread.sleep(200);

        //Assert all steps were taken.
        sut.stop();
        int current = sut.call;

        //sleep a bit to be sure!
        Thread.sleep(200);
        assertEquals(current, sut.call);
    }


    private class TestTree2 extends SmaSTraTreeExecutor<String> {

        int call = 0;

        TestTree2(Context context) {
            super(context);
        }

        @Override protected void startIntern() {}
        @Override protected void stopIntern() {}

        protected void transform0() {
            call++;
        }
    }


    @Test
    public void testNotStartingDoesNotTick() throws InterruptedException {
        TestTree2 sut = new TestTree2( null);

        //Sleep a bit.
        Thread.sleep(200);
        assertEquals(0, sut.call);
    }


    @Test
    public void testCallingStartIfAlreadyStartedStillTicks() throws InterruptedException {
        TestTree2 sut = new TestTree2(null);
        sut.start();
        Thread.sleep(100);

        int current = sut.call;
        sut.start();
        Thread.sleep(100);

        sut.stop();
        assertTrue(sut.call > current);
    }


}
