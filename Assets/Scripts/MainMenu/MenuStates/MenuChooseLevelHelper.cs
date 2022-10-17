using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Kino;

public class MenuChooseLevelHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("UI GameObjects")]
    public GameObject levelUI;
    public GameObject panelUI;
    public Transform buttons;
    public List<GameObject> buttonObjects;
    public string levelName;
    public int buttonIndex;
    private int buttonCount;
    public bool increment;
    public bool back;
    public bool levelStart;
    public bool glitching;

    [Header("Glitch Transition")]
    public DigitalGlitch dgtGlitch;
    public AnalogGlitch anGlitch;

    [Header("Tutorial Serialization")]
    public TutorialSaveManager tutorialManager;
    public bool tutorialInquiry;

    [SerializeField] int incrementSpeed;
    private float incrementTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        buttonIndex = 0;

        buttonCount = buttons.childCount;

        foreach (Transform t in buttons)
        {
            buttonObjects.Add(t.gameObject);
        }

        //select = false;
        back = false;
        increment = false;
        levelStart = false;
        tutorialInquiry = false;
        glitching = false;

        tutorialManager = GetComponent<TutorialSaveManager>();
    }

    public void HelperUpdate()
    {
        if (player.GetButtonDown("MenuSelect"))
        {
            ButtonAction();
        }
    }

    public void HelperFixedUpdate()
    {
        float Del = Time.deltaTime;
        float leftStickVal = player.GetAxis("ButtonIncrement");

        if (leftStickVal > 0.1 || leftStickVal < -0.1)
        {
            if (incrementTimer == 1)
            {
                // If this a positive or negative increment
                bool posIncrement;

                // Increment button index appropriately to the input
                if (leftStickVal < 0)
                {
                    posIncrement = true;
                }
                else
                {
                    posIncrement = false;
                }

                IncrementButton(posIncrement);
            }

            incrementTimer += incrementSpeed * Del;
        }

        else
        {
            incrementTimer = 1;
        }

    }

    private void IncrementButton(bool positiveIncrement)
    {
        if (positiveIncrement)
        {
            if (buttonIndex < buttonCount - 1)
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

        increment = true;

        // Reset increment timer
        incrementTimer = 0;
    }

    public void ButtonAction()
    {
        if (buttonIndex == buttonCount - 1)
        {
            Debug.Log("MainMenu");
            back = true;
            //Application.Quit();
        }
        else if(levelName != "")
        {
            if (tutorialManager.tutorialSeen)
            {
                levelStart = true;
            }
            else
            {
                tutorialInquiry = true;
            }
        }
    }
}
