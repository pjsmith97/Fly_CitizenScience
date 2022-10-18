using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuTutorialUIChoosingState : MainMenuTutorialUIState
{
    public override void Enter(MenuTutorialStateHelper helper)
    {
        base.Enter(helper);

        menuHelper.buttonsUI.SetActive(true);
        menuHelper.headerTextUI.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuHelper.uiButtonGameObjects[menuHelper.buttonIndex]);
    }

    public override void Update()
    {
        base.Update();

        if (menuHelper.player.GetButtonDown("MenuSelect"))
        {
            menuHelper.done = true;
        }

        else if (menuHelper.player.GetButtonDown("MenuBack"))
        {
            menuHelper.back = true;
        }

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float Del = Time.deltaTime;

        float leftStickVal = menuHelper.player.GetAxis("ButtonHIncrement");

        if (leftStickVal > 0.1 || leftStickVal < -0.1)
        {
            if (menuHelper.incrementTimer == 1)
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

                menuHelper.IncrementButton(posIncrement);
            }

            menuHelper.incrementTimer += Del;
        }

        else
        {
            menuHelper.incrementTimer = 1;
        }

        if (menuHelper.glitching)
        {
            menuHelper.anGlitch.scanLineJitter += Del;
            menuHelper.anGlitch.verticalJump += Del;
            menuHelper.dgtGlitch.intensity += Del;

        }
    }
}
