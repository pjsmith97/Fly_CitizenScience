using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MenuController : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    public MenuState menuState;

    public MainMenuStateHelper mainMenuHelper;
    public MenuTutorialStateHelper tutorialStateHelper;
    public MenuStatsStateHelper statsHelper;

    public LevelSaveManager saveManager;
    
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        player.controllers.maps.SetMapsEnabled(true, "MainMenu");

        mainMenuHelper = GetComponent<MainMenuStateHelper>();
        tutorialStateHelper = GetComponent<MenuTutorialStateHelper>();
        statsHelper = GetComponent<MenuStatsStateHelper>();

        saveManager = GetComponent<LevelSaveManager>();

        mainMenuHelper.buttons.gameObject.SetActive(false);
        statsHelper.statsUI.SetActive(false);

        ChangeState(new MainMenuState());
    }

    // Update is called once per frame
    void Update()
    {
        menuState.Update();
    }

    private void FixedUpdate()
    {
        menuState.FixedUpdate();
    }

    public void ChangeState(MenuState state)
    {
        //Debug.Log("Changing State!");
        if (menuState != null)
        {
            //Debug.Log("Exiting State!");
            menuState.Exit();
        }
        menuState = state;
        menuState.Enter(this);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; 
        #else
                 Application.Quit();
        #endif
    }
}
