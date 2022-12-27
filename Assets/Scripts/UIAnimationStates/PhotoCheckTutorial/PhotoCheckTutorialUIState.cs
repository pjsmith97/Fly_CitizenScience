using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
*    Title: PhotoCheckTutorialUIState
*    Original Title: TurretState
*    Author: inScope Studios
*    Date: Mar 17, 2021
*    Edit: Aug 29, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtu.be/VnfD5wGEXFw
*    Description: Abstract finite state machine class for photo analysis Tutorial slide show
*
***************************************************************************************/

public abstract class PhotoCheckTutorialUIState
{
    protected TutorialPanelManager tutorialManager;

    public virtual void Enter(TutorialPanelManager tutorial)
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
