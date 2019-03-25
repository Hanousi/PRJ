using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class used to contain all the information required for an exercise.
/// </summary>
public class GameLevel {

    /// <summary>
    /// Integer which acts as the identifier of the exercise.
    /// </summary>
    private int levelNumber;
    /// <summary>
    /// Two dimensional float array to store all the note positions for all the drums.
    /// </summary>
    private float[][] notePositions;
    /// <summary>
    /// Duration of the drum piece.
    /// </summary>
    private int duration;
    /// <summary>
    /// Tags which describe which drum the piece focuses on.
    /// </summary>
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
