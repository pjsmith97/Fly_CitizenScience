using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuState : MenuState
{
    public override void Enter(MenuController newMenu)
    {
        base.Enter(newMenu);

        menu.mainMenuHelper.buttons.gameObject.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menu.mainMenuHelper.buttonObjects[menu.mainMenuHelper.buttonIndex]);
    }

    public override void Update()
    {
        base.Update();
        menu.mainMenuHelper.HelperUpdate();

        if (menu.mainMenuHelper.select)
        {
            menu.mainMenuHelper.select = false;
            menu.mainMenuHelper.ButtonAction();
        }

        if (menu.mainMenuHelper.tutorialInquiry)
        {
            menu.mainMenuHelper.tutorialInquiry = false;
            menu.ChangeState(new MenuTutorialState());
        }

        if (menu.mainMenuHelper.stats)
        {
            menu.mainMenuHelper.stats = false;
            menu.ChangeState(new MenuStatsState());
        }

        if (menu.mainMenuHelper.quit)
        {
            menu.mainMenuHelper.quit = false;

            menu.QuitGame();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        menu.mainMenuHelper.HelperFixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        //menu.mainMenuHelper.buttons.gameObject.SetActive(false);
    }
}
