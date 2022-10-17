using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformerTutorialUIState 
{
    protected PlatformerTutorialManager tutorialManager;
    public virtual void Enter(PlatformerTutorialManager tutorial)
    {
        tutorialManager = tutorial;
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {



    }

    public virtual void Exit()
    {

    }
}
