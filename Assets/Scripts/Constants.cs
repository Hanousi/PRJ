using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants {

    public const float HIHATSPEED = 3;
    public const float CRASHSPEED = 3;
    public const float SNAREDRUMSPEED = 3;
    public const float HITOMSPEED = 3;
    public const float MIDTOMSPEED = 3;
    public const float FLOORTOMSPEED = 3;
    public const float RIDESPEED = 3;
    public const int GHOSTHITMAXTHRESHOLD = 20;
    public const int GHOSTHITMIDTHRESHOLD = 15;
    public const int GHOSTHITMINTHRESHOLD = 10;
    public const int GHOSTHITMAXHEURISTIC = 3;
    public const int GHOSTHITMIDHEURISTIC = 2;
    public const int GHOSTHITMINHEURISTIC = 1;
    public const double HITRATEMAXTHRESHOLD = 0.7;
    public const double HITRATEMIDTHRESHOLD = 0.8;
    public const double HITRATEMINTHRESHOLD = 0.9;
    public const int HITRATEMAXHEURISTIC = 3;
    public const int HITRATEMIDHEURISTIC = 2;
    public const int HITRATEMINHEURISTIC = 1;
    public const int PERFORMANCERECORDSIZE = 5;

    public const float HHX = 0;
    public const float HHXOFFSET = -0.59f;
    public const float HHXMODIFIER = -0.106f;
    public const float HHY = 0.8f;
    public const float HHYOFFSET = 0;
    public const float HHYMODIFIER = 0;
    public const float HHZOFFSET = 0;

    public const float CX = 0;
    public const float CXOFFSET = -0.18f;
    public const float CXMODIFIER = -0.106f;
    public const float CY = 1.12f;
    public const float CYOFFSET = 0;
    public const float CYMODIFIER = 0;
    public const float CZOFFSET = 0.92f;

    public const float SDX = -0.07f;
    public const float SDXOFFSET = 0;
    public const float SDXMODIFIER = 0;
    public const float SDY = 0.6f;
    public const float SDYOFFSET = 0;
    public const float SDYMODIFIER = 0;
    public const float SDZOFFSET = 0.28f;

    public const float HTX = 0.05f;
    public const float HTXOFFSET = 0;
    public const float HTXMODIFIER = 0;
    public const float HTY = 0;
    public const float HTYOFFSET = 0.85f;
    public const float HTYMODIFIER = 0.18f;
    public const float HTZOFFSET = 0.95f;

    public const float MTX = 0.47f;
    public const float MTXOFFSET = 0;
    public const float MTXMODIFIER = 0;
    public const float MTY = 0;
    public const float MTYOFFSET = 0.85f;
    public const float MTYMODIFIER = 0.18f;
    public const float MTZOFFSET = 0.95f;

    public const float FTX = 0.63f;
    public const float FTXOFFSET = 0;
    public const float FTXMODIFIER = 0;
    public const float FTY = 0.6f;
    public const float FTYOFFSET = 0;
    public const float FTYMODIFIER = 0;
    public const float FTZOFFSET = 0.28f;

    public const float RX = 0;
    public const float RXOFFSET = 0.87f;
    public const float RXMODIFIER = 0.106f;
    public const float RY = 1;
    public const float RYOFFSET = 0;
    public const float RYMODIFIER = 0;
    public const float RZOFFSET = 0.6f;

    public static readonly string[] drumstickInteractables = { "HiHat", "SnareDrum", "Crash", "Ride", "HiTom", "MiddleTom", "FloorTom", "XylophoneKey", "SteelDrumkey" };

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
