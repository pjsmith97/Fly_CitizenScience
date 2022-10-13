using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using TMPro;
using UnityEngine.EventSystems;

public class MenuTutorialStateHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("Ui Objects")]
    public GameObject tutorialConfirmationPanelUI;
    public GameObject buttonsUI;
    public TextMeshProUGUI headerTextUI;
    public List<GameObject> uiButtonGameObjects;

    [Header ("Ui State")]
    public MainMenuTutorialUIState tutorialUIState;

    public float panelGrowthTimer;
    public float uiAlphaTimer;

    public int buttonIndex;

    public float incrementTimer = 1;

    [Header("Progress Variable")]
    public bool done = false;
    public bool back = false;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        ChangeUIAnimationState(new MenuTutorialUIOpenState());

        uiButtonGameObjects = new List<GameObject>();
        int i = 0;
        foreach (Transform child in buttonsUI.transform)
        {
            uiButtonGameObjects.Add(child.gameObject);
            //uiButtonGameObjects[i] = child.gameObject;
            //child.gameObject.SetActive(false);
            i++;
        }

        buttonIndex = 1;

        buttonsUI.SetActive(false);
        headerTextUI.gameObject.SetActive(false);
        tutorialConfirmationPanelUI.SetActive(false);
    }

    public void HelperUpdate()
    {
        tutorialUIState.Update();
    }

    public void HelperFixedUpdate()
    {
        tutorialUIState.FixedUpdate();
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

    public void ChangeUIAnimationState(MainMenuTutorialUIState state)
    {
        //Debug.Log("Changing State!");
        if (tutorialUIState != null)
        {
            //Debug.Log("Exiting State!");
            tutorialUIState.Exit();
        }
        tutorialUIState = state;
        tutorialUIState.Enter(this);
    }
}
