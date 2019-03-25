using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Helper class which stores all the heuristic values for a level performance.
/// </summary>
[Serializable]
public class GamePerformance {

    /// <summary>
    /// Heuristic value for each drum in a drum piece.
    /// </summary>
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

    /// <summary>
    /// Used when calculating the worst drum across a set of Performance objects.
    /// </summary>
    public void averageScores()
    {
        FieldInfo[] fields = typeof(GamePerformance).GetFields();

        foreach(FieldInfo field in fields)
        {
            float currentScore = (float)field.GetValue(this);
            field.SetValue(this, currentScore / Constants.PERFORMANCERECORDSIZE);
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
