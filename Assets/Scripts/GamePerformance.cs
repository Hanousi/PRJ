using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GamePerformance {

    public int hiHat;
    public int crash;
    public int snareDrum;
    public int hiTom;
    public int midTom;
    public int floorTom;
    public int ride;

    public GamePerformance(int hiHat, int crash, int snareDrum, int hiTom, int midTom, int floorTom, int ride)
    {
        this.hiHat = hiHat;
        this.crash = crash;
        this.snareDrum = snareDrum;
        this.hiTom = hiTom;
        this.midTom = midTom;
        this.floorTom = floorTom;
        this.ride = ride;
    }

    public override string ToString()
    {
        return "HiHat: " + hiHat + ", Crash: " + crash + ", Snare Drum: " + snareDrum + ", HiTom: " + hiTom + ", MidTom: " + midTom + ", FloorTom: " + floorTom + ", Ride: " + ride;
    }
}
