using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel {

    private int levelNumber;
    private float[][] notePositions;
    private int duration;
    private string[] tags;

    public GameLevel(int levelNumber, float[][] notePositions, int duration, string[] tags)
    {
        this.levelNumber = levelNumber;
        this.notePositions = notePositions;
        this.duration = duration;
        this.tags = tags;
    }

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public float[][] GetNotePositions()
    {
        return notePositions;
    }

    public int GetDuration()
    {
        return duration;
    }

    public string[] GetTags()
    {
        return tags;
    }
}
