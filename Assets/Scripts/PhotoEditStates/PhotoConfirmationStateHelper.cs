using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using TMPro;

public class PhotoConfirmationStateHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    public GameObject confirmationPanelUI;
    public GameObject confirmationButtonUI;
    public List<GameObject> uiButtonGameObjects;

    public ConfirmationUIAnimationState uiAnimationState;

    public float panelGrowthTimer;
    public float uiAlphaTimer;

    public PhotoManager photoManager;
    public PhotoDistortionManager distortionManager;

    public string decisionText;
    public TextMeshProUGUI decisionTextUI;
    public TextMeshProUGUI headerTextUI;

    public float timerVal;

    public float incrementTimer = 1;

    public int buttonIndex;

    public bool done = false;
    public bool back = false;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        ChangeUIAnimationState(new ConfirmationPanelOpenState());

        uiButtonGameObjects = new List<GameObject>();
        int i = 0;
        foreach (Transform child in confirmationButtonUI.transform)
        {
            uiButtonGameObjects.Add(child.gameObject);
            //uiButtonGameObjects[i] = child.gameObject;
            //child.gameObject.SetActive(false);
            i++;
        }

        buttonIndex = 1;

        confirmationButtonUI.SetActive(false);
        decisionTextUI.gameObject.SetActive(false);
        headerTextUI.gameObject.SetActive(false);
        confirmationPanelUI.SetActive(false);

        photoManager = GetComponent<PhotoManager>();
    }

    // Update is called once per frame
    public void HelperUpdate()
    {
        uiAnimationState.Update();
    }

    public void HelperFixedUpdate()
    {
        uiAnimationState.FixedUpdate();
    }

    public void SetDecisionText(string newText)
    {
        decisionText = newText;
        decisionTextUI.text = decisionText;
    }

    public void SetTimer(float newTimer)
    {
        timerVal = newTimer;
    }

    public void IncrementButton(bool positiveIncrement)
    {
        if (positiveIncrement)
        {
            if (buttonIndex < 1)
            {
                buttonIndex++;
            }
        }
        else
        {
            if (buttonIndex > 0)
            {
                buttonIndex--;
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(uiButtonGameObjects[buttonIndex]);
    }

    public void ChangeUIAnimationState(ConfirmationUIAnimationState state)
    {
        //Debug.Log("Changing State!");
        if (uiAnimationState != null)
        {
            //Debug.Log("Exiting State!");
            uiAnimationState.Exit();
        }
        uiAnimationState = state;
        uiAnimationState.Enter(this);
    }
}
