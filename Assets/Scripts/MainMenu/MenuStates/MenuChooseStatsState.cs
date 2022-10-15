using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuChooseStatsState : MenuState
{
    public override void Enter(MenuController newMenu)
    {
        base.Enter(newMenu);

        menu.statsHelper.buttonIndex = 0;

        EventSystem.current.SetSelectedGameObject(null);

        menu.statsHelper.statsUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(menu.statsHelper.buttonObjects[menu.statsHelper.buttonIndex]);
        
        menu.mainMenuHelper.buttons.gameObject.SetActive(false);

        menu.statsHelper.classCountText.text = "" + menu.saveManager.totalCurrentClassifications;
    }

    public override void Update()
    {
        base.Update();
        menu.statsHelper.HelperUpdate();

        if (menu.statsHelper.local)
        {
            menu.statsHelper.local = false;
            menu.ChangeState(new MenuLevelStatsState());
        }

        if (menu.statsHelper.online)
        {
            menu.statsHelper.online = false;
        }

        if (menu.statsHelper.back)
        {
            menu.statsHelper.back = false;

            menu.ChangeState(new MainMenuState());
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        menu.statsHelper.HelperFixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        EventSystem.current.SetSelectedGameObject(null);

        menu.statsHelper.statsUI.SetActive(false);
    }
}
