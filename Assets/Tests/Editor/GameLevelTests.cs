using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelTests {

    GameLevel mockLevel = new GameLevel(10, new float[][] { new float[] { 1 },
            new float[] { 2 },
            new float[] { 3 },
            new float[] { 4 },
            new float[] { 5 },
            new float[] { 6 },
            new float[] { 7 }},
            20,
            new string[] { "snareDrum", "hiTom" });

    [Test]
    public void GameLevelReturnsCorrectLevelNumber()
    {
        int gameLevelNumber = mockLevel.GetLevelNumber();
        Assert.AreEqual(10, gameLevelNumber);
    }

    [Test]
    public void GameLevelReturnsCorrectNotePositions()
    {
        float[][] gameLevelNotePositions = mockLevel.GetNotePositions();
        Assert.AreEqual(new float[][] { new float[] { 1 },
            new float[] { 2 },
            new float[] { 3 },
            new float[] { 4 },
            new float[] { 5 },
            new float[] { 6 },
            new float[] { 7 }}, gameLevelNotePositions);
    }

    [Test]
    public void GameLevelReturnsCorrectDuration()
    {
        int gameLevelDuration = mockLevel.GetDuration();
        Assert.AreEqual(20, gameLevelDuration);
    }

    [Test]
    public void GameLevelReturnsCorrectTags()
    {
        string[] gameLevelTags = mockLevel.GetTags();
        Assert.AreEqual(new string[] {"snareDrum", "hiTom"}, gameLevelTags);
    }
}
