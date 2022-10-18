using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuChooseLevelState : MenuState
{
    public override void Enter(MenuController newMenu)
    {
        base.Enter(newMenu);

        menu.levelChooseHelper.buttonIndex = 0;

        EventSystem.current.SetSelectedGameObject(null);

        menu.mainMenuHelper.buttons.gameObject.SetActive(false);

        menu.levelChooseHelper.levelUI.SetActive(true);
        menu.levelChooseHelper.panelUI.SetActive(false);

        EventSystem.current.SetSelectedGameObject(menu.levelChooseHelper.buttonObjects[menu.levelChooseHelper.buttonIndex]);

        LoadLevelData();
    }

    public override void Update()
    {
        base.Update();
        menu.levelChooseHelper.HelperUpdate();

        if (menu.levelChooseHelper.increment)
        {
            menu.levelChooseHelper.increment = false;
            LoadLevelData();
            // Change selected button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menu.levelChooseHelper.buttonObjects[menu.levelChooseHelper.buttonIndex]);
        }

        if (menu.levelChooseHelper.tutorialInquiry)
        {
            menu.levelChooseHelper.tutorialInquiry = false;
            menu.ChangeState(new MenuTutorialState());
        }

        if (menu.levelChooseHelper.levelStart)
        {
            menu.levelChooseHelper.levelStart = false;
            menu.levelChooseHelper.glitching = true;
            menu.levelChooseHelper.parentUI.SetActive(false);
            //SceneManager.LoadSceneAsync(menu.levelChooseHelper.levelName);
        }

        if (menu.levelChooseHelper.back)
        {
            menu.levelChooseHelper.back = false;

            menu.ChangeState(new MainMenuState());
        }

        if (menu.levelChooseHelper.glitching)
        {
            if (menu.levelChooseHelper.anGlitch.scanLineJitter >= menu.levelChooseHelper.glitchCap &&
                menu.levelChooseHelper.anGlitch.verticalJump >= menu.levelChooseHelper.glitchCap &&
                menu.levelChooseHelper.dgtGlitch.intensity >= menu.levelChooseHelper.glitchCap)
            {
                menu.levelChooseHelper.glitching = false;
                SceneManager.LoadSceneAsync(menu.levelChooseHelper.levelName);
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        menu.levelChooseHelper.HelperFixedUpdate();
    }

    public void LoadLevelData()
    {
        if ((menu.levelChooseHelper.buttonObjects[menu.levelChooseHelper.buttonIndex].
            GetComponent<LevelStatButton>() != null))
        {
            menu.levelChooseHelper.levelName = menu.levelChooseHelper.buttonObjects[menu.levelChooseHelper.buttonIndex].
                GetComponent<LevelStatButton>().sceneName;
        }
    }

    public override void Exit()
    {
        base.Exit();

        EventSystem.current.SetSelectedGameObject(null);

        menu.levelChooseHelper.panelUI.SetActive(true);
        menu.levelChooseHelper.levelUI.SetActive(false);
        
    }
}
