package de.tu_darmstadt.smastra.buffers;

import org.junit.Test;

import java.util.List;

import static junit.framework.Assert.*;

/**
 * Test for Fixed Window Collectors.
 * @author Tobias Welther
 */
public class FixedWindowSizeBufferTest {



    @Test
    public void testAddingItemWorks(){
        String element = "TeSt";

        FixedWindowSizeBuffer<String> sut = new FixedWindowSizeBuffer<>();
        sut.addData(element);

        assertEquals(1, sut.getData().size());
        assertTrue(sut.getData().contains(element));
    }


    @Test
    public void testAddingMoreItemsKillsFirstWorks(){
        String element1 = "TeSt1";
        String element2 = "TeSt2";
        String element3 = "TeSt3";

        FixedWindowSizeBuffer<String> sut = new FixedWindowSizeBuffer<>();
        sut.configure("size",2);
        sut.addData(element1);
        sut.addData(element2);
        sut.addData(element3);

        assertEquals(2, sut.getData().size());
        assertTrue(sut.getData().contains(element2));
        assertTrue(sut.getData().contains(element3));
    }

    @Test
    public void testOrderStaysCorrect(){
        String element1 = "TeSt1";
        String element2 = "TeSt2";
        String element3 = "TeSt3";

        FixedWindowSizeBuffer<String> sut = new FixedWindowSizeBuffer<>();
        sut.configure("size",2);
        sut.addData(element1);
        sut.addData(element2);
        sut.addData(element3);

        assertEquals(2, sut.getData().size());
        assertEquals(element2, ((List<String>)sut.getData()).get(0));
        assertEquals(element3, ((List<String>)sut.getData()).get(1));
    }


}
