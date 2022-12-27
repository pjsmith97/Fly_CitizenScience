using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
*    Title: PlatformerTutorialUIState
*    Original Title: TurretState
*    Author: inScope Studios
*    Date: Mar 17, 2021
*    Edit: Aug 29, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtu.be/VnfD5wGEXFw
*    Description: Abstract finite state machine class for Tutorial level info card UI
*
***************************************************************************************/

public abstract class PlatformerTutorialUIState 
{
    protected PlatformerTutorialManager tutorialManager; // EDIT
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
