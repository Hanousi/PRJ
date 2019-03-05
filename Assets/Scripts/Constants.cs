using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants {

    public const float HiHatSpeed = 3;
    public const float CrashSpeed = 3;
    public const float SnareDrumSpeed = 3;
    public const float HiTomSpeed = 3;
    public const float MidTomSpeed = 3;
    public const float FloorTomSpeed = 3;
    public const float RideSpeed = 3;

    public static int drumKey(string drumName)
    {
        switch (drumName)
        {
            case "hiHat": return 0;
            case "crash": return 1;
            case "snareDrum": return 2;
            case "hiTom": return 3;
            case "midTom": return 4;
            case "floorTom": return 5;
            case "ride": return 6;

            default: return -1;
        }
    }
}
