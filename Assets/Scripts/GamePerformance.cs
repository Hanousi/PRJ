using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class GamePerformance {

    public float hiHat;
    public float crash;
    public float snareDrum;
    public float hiTom;
    public float midTom;
    public float floorTom;
    public float ride;

    public GamePerformance(float hiHat, float crash, float snareDrum, float hiTom, float midTom, float floorTom, float ride)
    {
        this.hiHat = hiHat;
        this.crash = crash;
        this.snareDrum = snareDrum;
        this.hiTom = hiTom;
        this.midTom = midTom;
        this.floorTom = floorTom;
        this.ride = ride;
    }

    public void averageScores(float averageFactor)
    {
        FieldInfo[] fields = typeof(GamePerformance).GetFields();

        foreach(FieldInfo field in fields)
        {
            float currentScore = (float)field.GetValue(this);
            field.SetValue(this, currentScore / averageFactor);
        }
    }

    public string getMaxScore()
    {
        FieldInfo[] fields = typeof(GamePerformance).GetFields();

        FieldInfo largestScore = fields.Aggregate((largest, next) => (float)next.GetValue(this) > (float)largest.GetValue(this) ? next : largest);

        return largestScore.Name;
    }

    public override string ToString()
    {
        return "HiHat: " + hiHat + ", Crash: " + crash + ", Snare Drum: " + snareDrum + ", HiTom: " + hiTom + ", MidTom: " + midTom + ", FloorTom: " + floorTom + ", Ride: " + ride;
    }
}
