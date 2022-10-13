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

            if (menu.tutorialStateHelper.buttonIndex == 0)
            {
                SceneManager.LoadSceneAsync("TutorialLevel");
            }

            else
            {
                SceneManager.LoadSceneAsync("ValleyLevel");
            }
        }

        if (menu.tutorialStateHelper.back)
        {
            menu.tutorialStateHelper.back = false;
            menu.ChangeState(new MainMenuState());
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        menu.tutorialStateHelper.HelperFixedUpdate();
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
