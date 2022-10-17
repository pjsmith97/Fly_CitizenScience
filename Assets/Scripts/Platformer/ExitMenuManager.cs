using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ExitMenuManager : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("UI GameObjects")]
    public GameObject exitUI;
    public GameObject playUI;
    public GameObject lensUI;
    public GameObject seerUI;
    public Transform buttons;
    public List<GameObject> buttonObjects;
    public int buttonIndex;
    private int buttonCount;

    public float incrementTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        buttonIndex = 1;

        buttonCount = buttons.childCount;

        foreach (Transform t in buttons)
        {
            buttonObjects.Add(t.gameObject);
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonObjects[buttonIndex]);

        exitUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("Exit"))
        {
            if (!exitUI.activeInHierarchy)
            {
                player.controllers.maps.SetMapsEnabled(false, "Default");
                GetComponent<CameraTimeManager>().NormalizeTime();
                lensUI.SetActive(false);
                seerUI.SetActive(false);
                exitUI.SetActive(true);
                //playUI.SetActive(false);
            }       
        }

        else if (player.GetButtonDown("ExitMenu"))
        {
            exitUI.SetActive(false);
            GetComponent<CameraTimeManager>().newScene = false;
            //playUI.SetActive(true);
            player.controllers.maps.SetMapsEnabled(true, "Default");
        }

        if (player.GetButtonDown("MenuSelect") && exitUI.activeInHierarchy)
        {
            ButtonAction();
        }
    }

    public void FixedUpdate()
    {
        if (exitUI.activeInHierarchy)
        {
            float Del = Time.deltaTime;
            float leftStickVal = player.GetAxis("ButtonHIncrement");

            if (leftStickVal > 0.1 || leftStickVal < -0.1)
            {
                if (incrementTimer == 1)
                {
                    // If this a positive or negative increment
                    bool posIncrement;

                    // Increment button index appropriately to the input
                    if (leftStickVal > 0)
                    {
                        posIncrement = true;
                    }
                    else
                    {
                        posIncrement = false;
                    }

                    IncrementButton(posIncrement);
                }

                incrementTimer += Del;
            }

            else
            {
                incrementTimer = 1;
            }
        }

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
        EventSystem.current.SetSelectedGameObject(buttonObjects[buttonIndex]);
    }

    public void ButtonAction()
    {
        if (buttonIndex == 0)
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
        else if (buttonIndex == 1)
        {
            exitUI.SetActive(false);
            GetComponent<CameraTimeManager>().newScene = false;
            player.controllers.maps.SetMapsEnabled(true, "Default");
        }
    }
}
