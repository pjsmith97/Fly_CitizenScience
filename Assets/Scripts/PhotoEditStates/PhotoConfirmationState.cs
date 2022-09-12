using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PhotoConfirmationState : PhotoAnalysisState
{
    public override void Enter(PhotoAnalysisController analysis)
    {
        base.Enter(analysis);

        photoAnalysis.decisionStateHelper.player.controllers.maps.SetMapsEnabled(true, "PhotoDecision");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            photoAnalysis.confirmationStateHelper.uiButtonGameObjects[photoAnalysis.confirmationStateHelper.buttonIndex]);

        photoAnalysis.confirmationStateHelper.confirmationPanelUI.SetActive(true);

        photoAnalysis.confirmationStateHelper.SetDecisionText(
            photoAnalysis.decisionStateHelper.textIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex].GetComponent<TextMeshProUGUI>().text);

        photoAnalysis.confirmationStateHelper.SetTimer(photoAnalysis.decisionStateHelper.guessingTimer);

       photoAnalysis.confirmationStateHelper.ChangeUIAnimationState(new ConfirmationPanelOpenState());
    }

    public override void Update()
    {
        base.Update();
        photoAnalysis.confirmationStateHelper.HelperUpdate();

        if (photoAnalysis.confirmationStateHelper.done)
        {
            photoAnalysis.confirmationStateHelper.done = false;
            
            var sendTask = photoAnalysis.confirmationStateHelper.photoManager.SendAndGetSpiPollInfo(
                photoAnalysis.decisionStateHelper.buttonIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex], 
                (int)photoAnalysis.confirmationStateHelper.timerVal);
            //var newTask = photoAnalysis.confirmationStateHelper.photoManager.GetSpiPollInfo();
            
            photoAnalysis.confirmationStateHelper.distortionManager.CreateBalancers();
            
            photoAnalysis.editingStateHelper.leftSlider.value = 0;
            photoAnalysis.editingStateHelper.rightSlider.value = 0;

            photoAnalysis.editingStateHelper.player.controllers.maps.SetMapsEnabled(false, "PhotoDecision");

            photoAnalysis.ChangeState(new PhotoEditingState());
        }

        if (photoAnalysis.confirmationStateHelper.back)
        {
            photoAnalysis.confirmationStateHelper.back = false;
            photoAnalysis.ChangeState(new PhotoDecisionState());
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        photoAnalysis.confirmationStateHelper.HelperFixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        photoAnalysis.confirmationStateHelper.confirmationPanelUI.SetActive(false);
        photoAnalysis.confirmationStateHelper.confirmationButtonUI.SetActive(false);
        photoAnalysis.confirmationStateHelper.decisionTextUI.gameObject.SetActive(false);
    }
}
