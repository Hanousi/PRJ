using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelTemplate {

    private float[][] noteTemplates;
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
