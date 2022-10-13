using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotoFinalResultState : PhotoAnalysisState
{
    public override void Enter(PhotoAnalysisController analysis)
    {
        base.Enter(analysis);

        photoAnalysis.finalStateHelper.resultsUI.SetActive(true);

        if (photoAnalysis.finalStateHelper.searchHighScore)
        {
            photoAnalysis.finalStateHelper.searchHighScoreText.SetActive(true);
        }

        if (photoAnalysis.finalStateHelper.classHighScore)
        {
            photoAnalysis.finalStateHelper.classHighScoreText.SetActive(true);
        }

        if (photoAnalysis.correctPhotos == FlyScoreManager.flyPhotos)
        {
            photoAnalysis.finalStateHelper.checkMarkSymbols.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            photoAnalysis.finalStateHelper.checkMarkSymbols.transform.GetChild(1).gameObject.SetActive(true);
        }

        // Set fly score text
        photoAnalysis.finalStateHelper.classCountText.text = 
            "Valid Classifications: " + photoAnalysis.correctPhotos + "/" + FlyScoreManager.flyPhotos;

        //Set Search timer text
        int timerMinuteNum = Mathf.FloorToInt(FlyScoreManager.finalTime / 60);
        int timerSecondNum = Mathf.FloorToInt(FlyScoreManager.finalTime % 60);
        string timerMinute = timerMinuteNum < 10 ? "0" + timerMinuteNum : "" + timerMinuteNum;
        string timerSecond = timerSecondNum < 10 ? "0" + timerSecondNum : "" + timerSecondNum;

        string timerString = timerMinute + ":" + timerSecond;

        photoAnalysis.finalStateHelper.searchTimeText.text = "Search Time: " + timerString;

        // Set Classification timer text
        timerMinuteNum = Mathf.FloorToInt((int)photoAnalysis.totalGuessingTime / 60);
        timerSecondNum = Mathf.FloorToInt((int)photoAnalysis.totalGuessingTime % 60);
        timerMinute = timerMinuteNum < 10 ? "0" + timerMinuteNum : "" + timerMinuteNum;
        timerSecond = timerSecondNum < 10 ? "0" + timerSecondNum : "" + timerSecondNum;

        timerString = timerMinute + ":" + timerSecond;

        photoAnalysis.finalStateHelper.classTimeText.text = "Classification Time: " + timerString;
    }

    public override void Update()
    {
        base.Update();

        photoAnalysis.finalStateHelper.HelperUpdate();

        if (photoAnalysis.finalStateHelper.done)
        {
            photoAnalysis.finalStateHelper.done = false;

            FinishLevel();
        }
    }

    private async void FinishLevel()
    {
        FlyScoreManager.sceneName = "";
        FlyScoreManager.flyPhotos = 0;

        SceneManager.LoadSceneAsync("MainMenu");
    }

    

    public override void Exit()
    {
        base.Exit();

    }
}
