using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class which encapsulates the queue class so that when the Add function is called
/// and the size limit of the queue is reached, the oldest entry is dequeued and new one is
/// queued.
/// </summary>
[Serializable]
public class PerformanceRecord {

    private Queue<GamePerformance> record;

    public PerformanceRecord()
    {
        record = new Queue<GamePerformance>() { };
    }

    public void Add(GamePerformance performance)
    {
        if(record.Count == Constants.PERFORMANCERECORDSIZE)
        {
            record.Dequeue();
        }

        record.Enqueue(performance);
    }

    public Queue<GamePerformance> GetQueue()
    {
        return record;
    }
}
