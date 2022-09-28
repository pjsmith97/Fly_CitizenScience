using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FlyScoreManager : MonoBehaviour
{
    [SerializeField] GameObject fliesListObject;
    [SerializeField] TextMeshProUGUI flyCounterText;
    private List<FlyDetectionManager> flies;
    public static bool changeCounterUI;
    public static int flyPhotos;
    public static float finalTime;
    public static string sceneName;

    private TimerManager timerManager;

    // Start is called before the first frame update
    void Start()
    {
        flyPhotos = 0;

        flies = new List<FlyDetectionManager>();
        foreach(Transform fly in fliesListObject.transform)
        {
            flies.Add(fly.gameObject.GetComponent <FlyDetectionManager>());
        }

        timerManager = GetComponent<TimerManager>();

        flyCounterText.text = flyPhotos + "/" + flies.Count;

        changeCounterUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (changeCounterUI)
        {
            flyCounterText.text = flyPhotos + "/" + flies.Count;
            changeCounterUI = false;
        }
        
        if(flies.Count == flyPhotos)
        {
            GetComponent<CameraTimeManager>().NormalizeTime();
            finalTime = timerManager.timer;
            sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadSceneAsync("PhotoAnalysisTutorial");
        }
    }
}
