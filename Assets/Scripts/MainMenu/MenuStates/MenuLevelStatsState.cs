using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuLevelStatsState : MenuState
{
    public override void Enter(MenuController newMenu)
    {
        base.Enter(newMenu);

        menu.levelStatsHelper.buttonIndex = 0;

        EventSystem.current.SetSelectedGameObject(null);

        menu.levelStatsHelper.statsUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(menu.levelStatsHelper.buttonObjects[menu.levelStatsHelper.buttonIndex]);
        
        LoadLevelData();
    }

    public override void Update()
    {
        base.Update();
        menu.levelStatsHelper.HelperUpdate();

        if (menu.levelStatsHelper.increment)
        {
            menu.levelStatsHelper.increment = false;
            LoadLevelData();
            // Change selected button
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menu.levelStatsHelper.buttonObjects[menu.levelStatsHelper.buttonIndex]);
        }

        if (menu.levelStatsHelper.back)
        {
            menu.levelStatsHelper.back = false;

            menu.ChangeState(new MenuChooseStatsState());
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        menu.levelStatsHelper.HelperFixedUpdate();
    }

    public void LoadLevelData()
    {
        if((menu.levelStatsHelper.buttonObjects[menu.levelStatsHelper.buttonIndex].
            GetComponent<LevelStatButton>() != null))
        {
            menu.saveManager.UpdateLevelPath(menu.levelStatsHelper.buttonObjects[menu.levelStatsHelper.buttonIndex].
            GetComponent<LevelStatButton>().sceneName);

            if (!menu.saveManager.noData)
            {
                //Set Search timer text
                int timerMinuteNum = Mathf.FloorToInt(menu.saveManager.finalSearchingTime / 60);
                int timerSecondNum = Mathf.FloorToInt(menu.saveManager.finalSearchingTime % 60);
                string timerMinute = timerMinuteNum < 10 ? "0" + timerMinuteNum : "" + timerMinuteNum;
                string timerSecond = timerSecondNum < 10 ? "0" + timerSecondNum : "" + timerSecondNum;

                string timerString = timerMinute + ":" + timerSecond;

                menu.levelStatsHelper.searchText.text = "Search Time: " + timerString;

                // Set Classification timer text
                timerMinuteNum = Mathf.FloorToInt(menu.saveManager.finalClassificationTime / 60);
                timerSecondNum = Mathf.FloorToInt(menu.saveManager.finalClassificationTime % 60);
                timerMinute = timerMinuteNum < 10 ? "0" + timerMinuteNum : "" + timerMinuteNum;
                timerSecond = timerSecondNum < 10 ? "0" + timerSecondNum : "" + timerSecondNum;

                timerString = timerMinute + ":" + timerSecond;

                menu.levelStatsHelper.classTimeText.text = "Classification Time: " + timerString;
                menu.levelStatsHelper.classCountText.text = "Classification Count: " + menu.saveManager.analysisScore;
            }

            else
            {
                menu.levelStatsHelper.searchText.text = "Search Time: ???";
                menu.levelStatsHelper.classCountText.text = "Classification Count: ???";
                menu.levelStatsHelper.classTimeText.text = "Classification Time: ???";
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        EventSystem.current.SetSelectedGameObject(null);

        menu.levelStatsHelper.statsUI.SetActive(false);
    }
}
