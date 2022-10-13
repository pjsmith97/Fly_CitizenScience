using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuTutorialUIOpenState : MainMenuTutorialUIState
{
    public override void Enter(MenuTutorialStateHelper helper)
    {
        base.Enter(helper);

        menuHelper.panelGrowthTimer = 0;
        menuHelper.uiAlphaTimer = 0;

        menuHelper.buttonIndex = 1;

        var tempColor = menuHelper.tutorialConfirmationPanelUI.GetComponent<Image>().color;
        tempColor.a = 0;
        menuHelper.tutorialConfirmationPanelUI.GetComponent<Image>().color = tempColor;

        menuHelper.tutorialConfirmationPanelUI.GetComponent<Transform>().localScale = new Vector3(1, 1, 0);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float Del = Time.deltaTime;

        // Gradually appear panel with growing alpha
        if (menuHelper.tutorialConfirmationPanelUI.GetComponent<Image>().color.a < 1)
        {
            var tempColor = menuHelper.tutorialConfirmationPanelUI.GetComponent<Image>().color;
            tempColor.a = Mathf.Lerp(0, 1, menuHelper.uiAlphaTimer);
            menuHelper.tutorialConfirmationPanelUI.GetComponent<Image>().color = tempColor;

            menuHelper.uiAlphaTimer += Del;
        }

        if (menuHelper.tutorialConfirmationPanelUI.GetComponent<Transform>().localScale.x < 2 &&
            menuHelper.tutorialConfirmationPanelUI.GetComponent<Transform>().localScale.y < 2)
        {
            menuHelper.tutorialConfirmationPanelUI.GetComponent<Transform>().localScale = 
                new Vector3(Mathf.Lerp(0, 2f, menuHelper.panelGrowthTimer), Mathf.Lerp(0, 2f, menuHelper.panelGrowthTimer), 0);

            menuHelper.panelGrowthTimer += Del;
        }

        if (menuHelper.tutorialConfirmationPanelUI.GetComponent<Transform>().localScale.x >= 2 &&
            menuHelper.tutorialConfirmationPanelUI.GetComponent<Transform>().localScale.y >= 2 &&
            menuHelper.tutorialConfirmationPanelUI.GetComponent<Image>().color.a >= 1)
        {
            menuHelper.ChangeUIAnimationState(new MenuTutorialUIChoosingState());
        }
    }
}
