using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class PhotoConfirmationState : PhotoAnalysisState
{
    public override void Enter(PhotoAnalysisController analysis)
    {
        base.Enter(analysis);

        photoAnalysis.decisionStateHelper.player.controllers.maps.SetMapsEnabled(true, "PhotoDecision");

        photoAnalysis.confirmationStateHelper.confirmationPanelUI.SetActive(true);

        /*EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            photoAnalysis.confirmationStateHelper.uiButtonGameObjects[photoAnalysis.confirmationStateHelper.buttonIndex]);*/

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

            photoAnalysis.completedPhotos += 1;

            photoAnalysis.totalGuessingTime += photoAnalysis.confirmationStateHelper.timerVal;

            if (FlyScoreManager.flyPhotos == photoAnalysis.completedPhotos)
            {
                FinishLevel();
            }

            else
            {
                Debug.Log("New Total Time: " + photoAnalysis.totalGuessingTime);
                NextPhoto();
            }
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

        EventSystem.current.SetSelectedGameObject(null);

        photoAnalysis.confirmationStateHelper.confirmationPanelUI.SetActive(false);
        photoAnalysis.confirmationStateHelper.confirmationButtonUI.SetActive(false);
        photoAnalysis.confirmationStateHelper.decisionTextUI.gameObject.SetActive(false);
        photoAnalysis.confirmationStateHelper.headerTextUI.gameObject.SetActive(false);
    }

    private async void FinishLevel()
    {
         await photoAnalysis.confirmationStateHelper.photoManager.SendSpiPollInfo(
                photoAnalysis.decisionStateHelper.buttonIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex],
                (int)photoAnalysis.confirmationStateHelper.timerVal);

        photoAnalysis.FinishLevel();

        /*FlyScoreManager.sceneName = "";
        FlyScoreManager.flyPhotos = 0;

        SceneManager.LoadSceneAsync(photoAnalysis.nextSceneName);*/

        photoAnalysis.ChangeState(new PhotoSolutionState());
    }

    private async void NextPhoto()
    {
        await photoAnalysis.confirmationStateHelper.photoManager.SendSpiPollInfo(
               photoAnalysis.decisionStateHelper.buttonIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex],
               (int)photoAnalysis.confirmationStateHelper.timerVal);

        photoAnalysis.ChangeState(new PhotoSolutionState());
    }
}
