using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlyScoreManager : MonoBehaviour
{
    [SerializeField] GameObject fliesListObject;
    private List<FlyDetectionManager> flies;
    public static int flyPhotos;
    public static float finalTime;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(flies.Count == flyPhotos)
        {
            GetComponent<CameraTimeManager>().NormalizeTime();
            finalTime = timerManager.timer;
            SceneManager.LoadSceneAsync("PhotoAnalysisTutorial");
        }
    }
}
