using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuChooseStatsHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("UI GameObjects")]
    public GameObject statsUI;
    public TextMeshProUGUI classCountText;
    public Transform buttons;
    public List<GameObject> buttonObjects;
    public TextMeshProUGUI soonText;
    public int buttonIndex;
    private int buttonCount;
    //public bool select;
    public bool back;
    public bool local;
    public bool online;

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
        local = false;
        online = false;
        back = false;
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

        if(buttonIndex == 1)
        {
            soonText.gameObject.SetActive(true);
        }
        else if(soonText.gameObject.activeSelf)
        {
            soonText.gameObject.SetActive(false);
        }

        // Change selected button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonObjects[buttonIndex]);

        // Reset increment timer
        incrementTimer = 0;
    }

    public void ButtonAction()
    {
        if (buttonIndex == 0)
        {
            Debug.Log("Local");
            local = true;
        }
        if (buttonIndex == 1)
        {
            Debug.Log("Online");
            online = true;
        }

        if (buttonIndex == 2)
        {
            Debug.Log("MainMenu");
            back = true;
            //Application.Quit();
        }
    }
}
