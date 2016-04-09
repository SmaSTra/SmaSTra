package de.tu_darmstadt.smastra.collectors;

import org.junit.Test;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertTrue;

/**
 * Test for Timed Window Collectors.
 * @author Tobias Welther
 */
public class TimeWindowCollectorTest {



    @Test
    public void testAddingItemWorks(){
        String element = "TeSt";

        long now = System.currentTimeMillis();
        TimeWindowCollector<String> sut = new TimeWindowCollector<>(1000);
        sut.addData(now, element);

        assertEquals(1, sut.getData().size());
        assertTrue(sut.getData().contains(element));
    }


    @Test
    public void testAddingOldItemsRemovedThemWorks(){
        String element1 = "TeSt1";
        String element2 = "TeSt2";
        String element3 = "TeSt3";

        long now = System.currentTimeMillis();
        TimeWindowCollector<String> sut = new TimeWindowCollector<>(300);
        sut.addData(now - 100, element1);
        sut.addData(now - 200, element2);
        sut.addData(now - 500, element3);

        assertEquals(2, sut.getData().size());
        assertTrue(sut.getData().contains(element1));
        assertTrue(sut.getData().contains(element2));
    }

    @Test
    public void testOrderStaysCorrect(){
        String element1 = "TeSt1";
        String element2 = "TeSt2";
        String element3 = "TeSt3";

        long now = System.currentTimeMillis();
        TimeWindowCollector<String> sut = new TimeWindowCollector<>(300);
        sut.addData(now - 100, element1);
        sut.addData(now - 200, element2);
        sut.addData(now - 500, element3);

        assertEquals(2, sut.getData().size());
        assertEquals(element1, sut.getDataList().get(0));
        assertEquals(element2, sut.getDataList().get(1));
    }


}
