package de.tu_darmstadt.smastra.base;

import android.content.Context;

import org.junit.Test;

import de.tu_darmstadt.smastra.base.SmaSTraTreeExecutor;

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
        int max = 10;
        TestTree1 sut = new TestTree1(max, null);
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

        public TestTree1(int max, Context context) {
            super(max, context);
        }

        @Override
        protected void transform(int level) {
            assertEquals(call, level);
            call++;
        }

    }


    @Test
    public void testStopStopsTheTicking() throws InterruptedException {
        int max = 10;
        TestTree2 sut = new TestTree2(max, null);
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

        public TestTree2(int max, Context context) {
            super(max, context);
        }

        @Override
        protected void transform(int level) {
            call++;
        }
    }


    @Test
    public void testNotStartingDoesNotTick() throws InterruptedException {
        int max = 10;
        TestTree2 sut = new TestTree2(max, null);

        //Sleep a bit.
        Thread.sleep(200);
        assertEquals(0, sut.call);
    }


    @Test
    public void testCallingStartIfAlreadyStartedStillTicks() throws InterruptedException {
        int max = 10;
        TestTree2 sut = new TestTree2(max, null);
        sut.start();
        Thread.sleep(100);

        int current = sut.call;
        sut.start();
        Thread.sleep(100);

        sut.stop();
        assertTrue(sut.call > current);
    }

    @Test
    public void testInitIsOnlyCalledOnce() throws InterruptedException {
        int max = 10;
        TestTree3 sut = new TestTree3(max, null);
        sut.start();
        Thread.sleep(100);
        sut.stop();

        assertEquals(1, sut.call);
    }

    private class TestTree3 extends SmaSTraTreeExecutor<String> {

        int call = 0;

        public TestTree3(int max, Context context) {
            super(max, context);
        }

        @Override
        protected void init() {
            super.init();
            call++;
        }

        @Override
        protected void transform(int level) {}
    }


}
