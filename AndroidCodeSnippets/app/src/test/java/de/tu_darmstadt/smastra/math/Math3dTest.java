package de.tu_darmstadt.smastra.math;

import org.junit.Before;
import org.junit.Test;

import java.util.ArrayList;
import java.util.List;

import de.tu_darmstadt.smastra.sensors.Vector3d;

import static org.junit.Assert.*;

/**
 * Created by Tobias Welther on 29.03.2016
 */
public class Math3dTest {


    /**
     * The list to use for data.
     */
    private List<Vector3d> sameList = new ArrayList<>();

    /**
     * The list to use for data.
     */
    private List<Vector3d> diffList = new ArrayList<>();


    @Before
    public void generateList(){
        sameList.clear();

        sameList.add(new Vector3d(1,1,1));
        sameList.add(new Vector3d(1,1,1));
        sameList.add(new Vector3d(1,1,1));

        diffList.clear();
        diffList.add(new Vector3d(0,0,0));
        diffList.add(new Vector3d(1,1,1));
        diffList.add(new Vector3d(2,2,2));
    }


    @Test
    public void testMeanSameValueWorks(){
        Vector3d mean = Math3d.mean(sameList);

        assertEquals(1, mean.getX(), 0.001);
        assertEquals(1, mean.getY(), 0.001);
        assertEquals(1, mean.getZ(), 0.001);
    }

    @Test
    public void testMeanDifferentValueWorks(){
        Vector3d mean = Math3d.mean(diffList);

        assertEquals(1, mean.getX(), 0.001);
        assertEquals(1, mean.getY(), 0.001);
        assertEquals(1, mean.getZ(), 0.001);
    }


    @Test
    public void testVarianceSameValueWorks(){
        Vector3d variance = Math3d.variance(sameList);

        assertEquals(0, variance.getX(), 0.001);
        assertEquals(0, variance.getY(), 0.001);
        assertEquals(0, variance.getZ(), 0.001);
    }

    @Test
    public void testVarianceDifferentValueWorks(){
        Vector3d variance = Math3d.variance(diffList);

        double wanted = 2d / (diffList.size()-1);
        assertEquals(wanted, variance.getX(), 0.001);
        assertEquals(wanted, variance.getY(), 0.001);
        assertEquals(wanted, variance.getZ(), 0.001);
    }


}
