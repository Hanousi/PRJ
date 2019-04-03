using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelTemplateTests
{
    AILevelTemplate mockTemplate = new AILevelTemplate(new float[][] {
            new float[] { 1, 2, 3, 4 },
            new float[] { 5, 6, 7, 8 }}, 20);

    [Test]
    public void AILevelTemplateReturnsCorrectNoteTemplates()
    {
        float[][] mockNoteTemplates = mockTemplate.GetNoteTemplates();
        Assert.AreEqual(new float[][] {
            new float[] { 1, 2, 3, 4 },
            new float[] { 5, 6, 7, 8 }
        }, mockNoteTemplates);
    }

    [Test]
    public void AILevelTemplateReturnsCorrectDuration()
    {
        int mockDuration = mockTemplate.GetDuration();
        Assert.AreEqual(20, mockDuration);
    }
}
