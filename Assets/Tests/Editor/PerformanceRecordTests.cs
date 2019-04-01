using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceRecordTests {

    [Test]
    public void PerformanceRecordAllowsQueuingOfPerformances()
    {
        PerformanceRecord mockRecord = new PerformanceRecord();
        Queue<GamePerformance> recordQueue = mockRecord.GetQueue();

        Assert.AreEqual(0, recordQueue.Count);

        GamePerformance mockPerformance = new GamePerformance(0, 0, 0, 0, 0, 0, 0);
        mockRecord.Add(mockPerformance);

        Assert.AreEqual(1, recordQueue.Count);
    }

    [Test]
    public void PerformanceRecordDoesNotIncreasePastQueueLimit()
    {
        PerformanceRecord mockRecord = new PerformanceRecord();
        Queue<GamePerformance> recordQueue = mockRecord.GetQueue();

        for(int i = 0; i < Constants.PERFORMANCERECORDSIZE; i++)
        {
            GamePerformance mockPerformance = new GamePerformance(0, 0, 0, 0, 0, 0, 0);

            mockRecord.Add(mockPerformance);
        }

        Assert.AreEqual(Constants.PERFORMANCERECORDSIZE, recordQueue.Count);

        GamePerformance mockPerformance1 = new GamePerformance(0, 0, 0, 0, 0, 0, 0);
        mockRecord.Add(mockPerformance1);

        Assert.AreEqual(Constants.PERFORMANCERECORDSIZE, recordQueue.Count);
    }

    [Test]
    public void PerformanceRecordEnqueuesCorrectly()
    {
        PerformanceRecord mockRecord = new PerformanceRecord();
        Queue<GamePerformance> recordQueue = mockRecord.GetQueue();

        for (int i = 0; i < Constants.PERFORMANCERECORDSIZE; i++)
        {
            GamePerformance mockPerformance;

            if(i == 1)
            {
                mockPerformance = new GamePerformance(1, 0, 0, 0, 0, 0, 0);
            } else
            {
                mockPerformance = new GamePerformance(0, 0, 0, 0, 0, 0, 0);
            }

            mockRecord.Add(mockPerformance);
        }

        recordQueue.Dequeue();
        Assert.AreEqual(1, recordQueue.Dequeue().hiHat);
    }

}
