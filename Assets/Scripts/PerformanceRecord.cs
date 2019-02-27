using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PerformanceRecord {

    private Queue<GamePerformance> record;

    public PerformanceRecord()
    {
        record = new Queue<GamePerformance>() { };
    }

    public void Add(GamePerformance performance)
    {
        if(record.Count == 5)
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
