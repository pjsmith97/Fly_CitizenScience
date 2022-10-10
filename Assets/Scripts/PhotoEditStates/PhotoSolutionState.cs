using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotoSolutionState : PhotoAnalysisState
{
    public override void Enter(PhotoAnalysisController analysis)
    {
        base.Enter(analysis);

        photoAnalysis.solutionStateHelper.solutionUI.SetActive(true);

        photoAnalysis.SetPanelTitle("Result");

        foreach (GameObject obj in photoAnalysis.solutionStateHelper.imageList)
        {
            obj.SetActive(false);
        }

        if(photoAnalysis.solutionStateHelper.solution == "No Solution")
        {
            photoAnalysis.solutionStateHelper.solutionTextUI.text = "Thank you for your submission!";
        }
        else
        {
            photoAnalysis.solutionStateHelper.solutionTextUI.text = "Solution: " +
                photoAnalysis.solutionStateHelper.solution;
        }

        if (photoAnalysis.solutionStateHelper.correct)
        {
            if(photoAnalysis.solutionStateHelper.solution == "No Solution")
            {
                photoAnalysis.solutionStateHelper.imageList[2].SetActive(true);
            }
            else
            {
                photoAnalysis.solutionStateHelper.imageList[0].SetActive(true);
            }
        }

        else
        {
            photoAnalysis.solutionStateHelper.imageList[1].SetActive(true);
        }
    }

    public override void Update()
    {
        base.Update();
        photoAnalysis.solutionStateHelper.HelperUpdate();

        if (photoAnalysis.solutionStateHelper.done)
        {
            photoAnalysis.solutionStateHelper.done = false;

            if (FlyScoreManager.flyPhotos == photoAnalysis.completedPhotos)
            {
                FinishLevel();
            }

            else
            {
                //Debug.Log("New Total Time: " + photoAnalysis.totalGuessingTime);
                NextPhoto();
            }
        }
    }

    private async void FinishLevel()
    {
        FlyScoreManager.sceneName = "";
        FlyScoreManager.flyPhotos = 0;

        SceneManager.LoadSceneAsync(photoAnalysis.nextSceneName);
    }

    private async void NextPhoto()
    {
        await photoAnalysis.confirmationStateHelper.photoManager.GetSpiPollInfo();

        photoAnalysis.confirmationStateHelper.distortionManager.CreateBalancers();

        photoAnalysis.editingStateHelper.leftSlider.value = 0;
        photoAnalysis.editingStateHelper.rightSlider.value = 0;
        photoAnalysis.editingStateHelper.rotationGauge.fillAmount = 0;

        photoAnalysis.editingStateHelper.player.controllers.maps.SetMapsEnabled(false, "PhotoDecision");

        photoAnalysis.ChangeState(new PhotoEditingState());
    }

    public override void Exit()
    {
        base.Exit();
        photoAnalysis.solutionStateHelper.solutionUI.SetActive(false);
    }
}
