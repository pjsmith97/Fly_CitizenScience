using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;


public class PhotoAnalysisController : MonoBehaviour
{

    public PhotoAnalysisState photoAnalysisState;
    public PhotoEditingStateHelper editingStateHelper;
    public PhotoDecisionStateHelper decisionStateHelper;
    public PhotoConfirmationStateHelper confirmationStateHelper;

    [SerializeField] private TextMeshProUGUI panelTitle;

    public int completedPhotos;
    public int correctPhotos;

    public float totalGuessingTime;

    public string nextSceneName;

    // Start is called before the first frame update
    void Start()
    {
        /*player.controllers.maps.SetMapsEnabled(true, "PhotoAnalysis");
        player.controllers.maps.SetMapsEnabled(false, "Default");*/

        completedPhotos = 0;
        correctPhotos = 0;

        totalGuessingTime = 0;

        editingStateHelper = GetComponent<PhotoEditingStateHelper>();

        decisionStateHelper = GetComponent<PhotoDecisionStateHelper>();

        confirmationStateHelper = GetComponent<PhotoConfirmationStateHelper>();

        decisionStateHelper.decisionUI.SetActive(false);

        nextSceneName = FlyScoreManager.sceneName;

        ChangeState(new PhotoEditingState());

        Debug.Log("Final Search Timer: " + FlyScoreManager.finalTime);
    }

    // Update is called once per frame
    void Update()
    {
        photoAnalysisState.Update();
    }

    private void FixedUpdate()
    {
        photoAnalysisState.FixedUpdate();
    }

    public void ChangeState(PhotoAnalysisState state)
    {
        //Debug.Log("Changing State!");
        if (photoAnalysisState != null)
        {
            //Debug.Log("Exiting State!");
            photoAnalysisState.Exit();
        }
        photoAnalysisState = state;
        photoAnalysisState.Enter(this);
    }

    public void SetPanelTitle(string newTitle)
    {
        panelTitle.text = newTitle;
    }

    public void FinishLevel()
    {
        GetComponent<LevelSaveManager>().SaveData();
        GetComponent<LevelSaveManager>().LoadData();
    }
}
