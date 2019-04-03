using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePerformanceTests
{
    [Test]
    public void GamePerformanceReturnsCorrectToStringOutput()
    {
        GamePerformance mockGamePerformance = new GamePerformance(5, 10, 15, 20, 25, 30, 35);

        string gamePerformanceString = mockGamePerformance.ToString();
        Assert.AreEqual("HiHat: 5, Crash: 10, Snare Drum: 15, HiTom: 20, MidTom: 25, FloorTom: 30, Ride: 35", gamePerformanceString);
    }

    [Test]
    public void GamePerformanceCanAverageScoresCorrectly()
    {
        GamePerformance mockGamePerformance = new GamePerformance(5, 10, 15, 20, 25, 30, 35);
        mockGamePerformance.averageScores();

        Assert.AreEqual(1, mockGamePerformance.hiHat);
        Assert.AreEqual(2, mockGamePerformance.crash);
        Assert.AreEqual(3, mockGamePerformance.snareDrum);
        Assert.AreEqual(4, mockGamePerformance.hiTom);
        Assert.AreEqual(5, mockGamePerformance.midTom);
        Assert.AreEqual(6, mockGamePerformance.floorTom);
        Assert.AreEqual(7, mockGamePerformance.ride);
    }

    [Test]
    public void GamePerformanceCanAverageUnfavourableScoresCorrectly()
    {
        GamePerformance mockGamePerformance = new GamePerformance(6, 7, 8, 9, 11, 12, 13);
        mockGamePerformance.averageScores();

        Assert.AreEqual(1.2f, mockGamePerformance.hiHat);
        Assert.AreEqual(1.4f, mockGamePerformance.crash);
        Assert.AreEqual(1.6f, mockGamePerformance.snareDrum);
        Assert.AreEqual(1.8f, mockGamePerformance.hiTom);
        Assert.AreEqual(2.2f, mockGamePerformance.midTom);
        Assert.AreEqual(2.4f, mockGamePerformance.floorTom);
        Assert.AreEqual(2.6f, mockGamePerformance.ride);
    }


    [Test]
    public void GamePerformanceCanReturnDrumNameWithHighestScore()
    {
        GamePerformance mockGamePerformance = new GamePerformance(5, 10, 15, 20, 25, 30, 35);
        string drumName = mockGamePerformance.getMaxScore();

        Assert.AreEqual("ride", drumName);
    }

    [Test]
    public void GamePerformanceCanReturnOneDrumNameWithHighestScore()
    {
        GamePerformance mockGamePerformance = new GamePerformance(5, 10, 15, 20, 25, 35, 35);
        string drumName = mockGamePerformance.getMaxScore();

        Assert.AreEqual("floorTom", drumName);
    }
}