using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanelOpenState : ConfirmationUIAnimationState
{
    public override void Enter(PhotoConfirmationStateHelper helper)
    {
        base.Enter(helper);

        confirmationHelper.panelGrowthTimer = 0;
        confirmationHelper.uiAlphaTimer = 0;

        var tempColor = confirmationHelper.confirmationPanelUI.GetComponent<Image>().color;
        tempColor.a = 0;
        confirmationHelper.confirmationPanelUI.GetComponent<Image>().color = tempColor;

        confirmationHelper.confirmationPanelUI.GetComponent<Transform>().localScale = new Vector3(1, 1, 0);
    }

    public override void Update()
    {
        base.Update();

        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float Del = Time.deltaTime;

        // Gradually appear panel with growing alpha
        if (confirmationHelper.confirmationPanelUI.GetComponent<Image>().color.a < 1)
        {
            var tempColor = confirmationHelper.confirmationPanelUI.GetComponent<Image>().color;
            tempColor.a = Mathf.Lerp(0, 1, confirmationHelper.uiAlphaTimer);
            confirmationHelper.confirmationPanelUI.GetComponent<Image>().color = tempColor;

            confirmationHelper.uiAlphaTimer += Del;
        }

        if (confirmationHelper.confirmationPanelUI.GetComponent<Transform>().localScale.x < 2 &&
            confirmationHelper.confirmationPanelUI.GetComponent<Transform>().localScale.y < 2)
        {
            confirmationHelper.confirmationPanelUI.GetComponent<Transform>().localScale = new Vector3(Mathf.Lerp(0, 2f, confirmationHelper.panelGrowthTimer),
                Mathf.Lerp(0, 2f, confirmationHelper.panelGrowthTimer), 0);

            confirmationHelper.panelGrowthTimer += Del;
        }

        if(confirmationHelper.confirmationPanelUI.GetComponent<Transform>().localScale.x >= 2 &&
            confirmationHelper.confirmationPanelUI.GetComponent<Transform>().localScale.y >= 2 &&
            confirmationHelper.confirmationPanelUI.GetComponent<Image>().color.a >= 1)
        {            
            confirmationHelper.ChangeUIAnimationState(new ConfirmationChoosingState());
        }
    }
}
