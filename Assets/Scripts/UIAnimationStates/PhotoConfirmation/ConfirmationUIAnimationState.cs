using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
*    Title: ConfirmationUIAnimationState
*    Original Title: TurretState
*    Author: inScope Studios
*    Date: Mar 17, 2021
*    Edit: Aug 29, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtu.be/VnfD5wGEXFw
*    Description: Abstract finite state machine class for confirmation panel in Photo Analysis mode
*
***************************************************************************************/

public abstract class ConfirmationUIAnimationState
{
    protected PhotoConfirmationStateHelper confirmationHelper;

    public virtual void Enter(PhotoConfirmationStateHelper helper)
    {
        confirmationHelper = helper;
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