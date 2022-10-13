using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Rewired;

public class MainMenuStateHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("Buttons")]
    public Transform buttons;
    public List<GameObject> buttonObjects;
    public int buttonIndex;
    private int buttonCount;
    public bool select;
    public bool quit;

    [SerializeField] int incrementSpeed;
    private float incrementTimer = 1;

    [Header("Tutorial Serialization")]
    public TutorialSaveManager tutorialManager;
    public bool tutorialInquiry;



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

        select = false;
        tutorialInquiry = false;
        quit = false;

        tutorialManager = GetComponent<TutorialSaveManager>();
    }

    // Update is called once per frame
    public void HelperUpdate()
    {
        if (player.GetButtonDown("MenuSelect"))
        {
            select = true;
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

        // Change selected button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonObjects[buttonIndex]);

        // Reset increment timer
        incrementTimer = 0;
    }

    public void ButtonAction()
    {
        if(buttonIndex == 0)
        {
            if (tutorialManager.tutorialSeen)
            {
                SceneManager.LoadSceneAsync("ValleyLevel");
            }
            else
            {
                tutorialInquiry = true;
            }
            
        }
        if(buttonIndex == 1)
        {
            SceneManager.LoadSceneAsync("TutorialLevel");
        }

        if(buttonIndex == 3)
        {
            Debug.Log("Quit");
            quit = true;
            //Application.Quit();
        }
    }
}
