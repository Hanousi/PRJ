using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper Class which stores the note positions for a potential AI generated level.
/// </summary>
public class AILevelTemplate {

    /// <summary>
    /// Note positions of that can be used by the AI to generate a random but coherent beat.
    /// The first array placed in this variable is the focus drum and the other note positions are 
    /// for the random support drums.
    /// </summary>
    private float[][] noteTemplates;
    /// <summary>
    /// Duration of the AI drum piece template.
    /// </summary>
    private int duration;

    public AILevelTemplate(float[][] noteTemplates, int duration)
    {
        this.noteTemplates = noteTemplates;
        this.duration = duration;
    }

    public float[][] GetNoteTemplates()
    {
        return noteTemplates;
    }

    public int GetDuration()
    {
        return duration;
    }
}
