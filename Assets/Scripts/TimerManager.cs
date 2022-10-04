using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float timer;
    [SerializeField] private TextMeshProUGUI timerUI;

    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        int timerMinuteNum = Mathf.FloorToInt(timer / 60);
        int timerSecondNum = Mathf.FloorToInt(timer % 60);
        string timerMinute = timerMinuteNum < 10 ? "0" + timerMinuteNum : "" + timerMinuteNum;
        string timerSecond = timerSecondNum < 10 ? "0" + timerSecondNum : "" + timerSecondNum;

        string timerString = timerMinute + ":" + timerSecond;

        timerUI.text = timerString;
    }
}
