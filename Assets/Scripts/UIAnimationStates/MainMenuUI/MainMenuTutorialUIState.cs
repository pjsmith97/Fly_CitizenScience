using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
*    Title: MainMenuTutorialState
*    Original Title: TurretState
*    Author: inScope Studios
*    Date: Mar 17, 2021
*    Edit: Aug 29, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtu.be/VnfD5wGEXFw
*    Description: Abstract finite state machine class for Main Menu Tutorial prompt UI
*
***************************************************************************************/

public abstract class MainMenuTutorialUIState
{
    protected MenuTutorialStateHelper menuHelper;

    public virtual void Enter(MenuTutorialStateHelper helper)
    {
        menuHelper = helper;
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
