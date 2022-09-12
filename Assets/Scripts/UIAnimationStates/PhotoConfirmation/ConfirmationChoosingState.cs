using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmationChoosingState : ConfirmationUIAnimationState
{
    public override void Enter(PhotoConfirmationStateHelper helper)
    {
        base.Enter(helper);

        confirmationHelper.confirmationButtonUI.SetActive(true);
        confirmationHelper.decisionTextUI.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(confirmationHelper.uiButtonGameObjects[confirmationHelper.buttonIndex]);
    }

    public override void Update()
    {
        base.Update();

        if (confirmationHelper.player.GetButtonDown("Select"))
        {
            if (confirmationHelper.buttonIndex == 0)
            {
                confirmationHelper.done = true;
                /*var sendTask = confirmationHelper.photoManager.SendSpiPollInfo(confirmationHelper.decisionText, (int)confirmationHelper.timerVal);
                var newTask = confirmationHelper.photoManager.GetSpiPollInfo();
                confirmationHelper.distortionManager.CreateBalancers();*/
            }
            else
            {
                confirmationHelper.back = true;
            }
        }

        else if (confirmationHelper.player.GetButtonDown("Back"))
        {
            confirmationHelper.back = true;
        }

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float Del = Time.deltaTime;

        float leftStickVal = confirmationHelper.player.GetAxis("ButtonSelection");

        if (leftStickVal > 0.1 || leftStickVal < -0.1)
        {
            if (confirmationHelper.incrementTimer == 1)
            {
                // If this a positive or negative increment
                bool posIncrement;

                // Increment button index appropriately to the input
                if (leftStickVal > 0)
                {
                    posIncrement = true;
                }
                else
                {
                    posIncrement = false;
                }

                confirmationHelper.IncrementButton(posIncrement);
            }

            confirmationHelper.incrementTimer += Del;
        }

        else
        {
            confirmationHelper.incrementTimer = 1;
        }
    }
}
