using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuTutorialState : MenuState
{
    public override void Enter(MenuController newMenu)
    {
        base.Enter(newMenu);

        menu.tutorialStateHelper.tutorialConfirmationPanelUI.SetActive(true);

        menu.tutorialStateHelper.ChangeUIAnimationState(new MenuTutorialUIOpenState());
    }

    public override void Update()
    {
        base.Update();

        menu.tutorialStateHelper.HelperUpdate();

        if (menu.tutorialStateHelper.done)
        {
            menu.tutorialStateHelper.done = false;
            menu.tutorialStateHelper.glitching = true;

            /*if (menu.tutorialStateHelper.buttonIndex == 0)
            {
                SceneManager.LoadSceneAsync("TutorialLevel");
            }

            else
            {

                menu.mainMenuHelper.tutorialManager.SaveData(true);
                SceneManager.LoadSceneAsync(menu.levelChooseHelper.levelName);
            }*/
        }

        if (menu.tutorialStateHelper.back)
        {
            menu.tutorialStateHelper.back = false;
            menu.ChangeState(new MenuChooseLevelState());
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        menu.tutorialStateHelper.HelperFixedUpdate();

        if (menu.tutorialStateHelper.anGlitch.scanLineJitter >= menu.tutorialStateHelper.glitchCap &&
                menu.tutorialStateHelper.anGlitch.verticalJump >= menu.tutorialStateHelper.glitchCap &&
                menu.tutorialStateHelper.dgtGlitch.intensity >= menu.tutorialStateHelper.glitchCap)
        {
            menu.tutorialStateHelper.glitching = false;

            if (menu.tutorialStateHelper.buttonIndex == 0)
            {
                SceneManager.LoadScene("TutorialLevel");
            }

            else
            {
                menu.mainMenuHelper.tutorialManager.SaveData(true);
                SceneManager.LoadScene(menu.levelChooseHelper.levelName);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        EventSystem.current.SetSelectedGameObject(null);

        menu.tutorialStateHelper.tutorialConfirmationPanelUI.SetActive(false);
        menu.tutorialStateHelper.buttonsUI.SetActive(false);
        menu.tutorialStateHelper.headerTextUI.gameObject.SetActive(false);
    }
}
