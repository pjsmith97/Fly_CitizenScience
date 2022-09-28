using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelSaveManager : MonoBehaviour
{
    public static string sceneName;
    public float finalSearchingTime;
    public float finalClassificationTime;
    public int analysisScore;

    private string savePath;

    // Start is called before the first frame update
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "LevelScoreSaves");

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        sceneName = FlyScoreManager.sceneName;
        savePath = Path.Combine(savePath, sceneName + ".save");

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData()
    {
        Dictionary<string, float> newData = new Dictionary<string, float>();

        if(finalSearchingTime < FlyScoreManager.finalTime)
        {
            newData["Best Search Time"] = FlyScoreManager.finalTime;
        }
        else
        {
            newData["Best Search Time"] = finalSearchingTime;
        }

        float classificationTimer = GetComponent<PhotoAnalysisController>().confirmationStateHelper.timerVal;
        int correctPhotos = GetComponent<PhotoAnalysisController>().correctPhotos;
        
        if (analysisScore <= correctPhotos)
        {
            newData["Best Photo Score"] = correctPhotos;

            if (finalClassificationTime < classificationTimer)
            {
                newData["Best Classification Time"] = classificationTimer;

            }
            else
            {
                newData["Best Classification Time"] = finalClassificationTime;
            }
        }
        else
        {
            newData["Best Photo Score"] = analysisScore;
            newData["Best Classification Time"] = finalClassificationTime;
        }


        var save = new SaveLevelData()
        {
            levelData = newData
        };

        var binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(savePath))
        {
            binaryFormatter.Serialize(fileStream, save);
        }

        Debug.Log("Data Saved");
    }

    public void LoadData()
    {
        SaveLevelData save;

        Debug.Log("Loading " + savePath);

        var binaryFormatter = new BinaryFormatter();
        if (File.Exists(savePath))
        {
            using (var fileStream = File.Open(savePath, FileMode.Open))
            {
                save = (SaveLevelData)binaryFormatter.Deserialize(fileStream);
            }

            finalSearchingTime = save.levelData["Best Search Time"];
            analysisScore = (int)save.levelData["Best Photo Score"];
            finalClassificationTime = save.levelData["Best Classification Time"];
        }

        else
        {
            finalSearchingTime = 0;
            analysisScore = 0;
            finalClassificationTime = 0;
            Debug.LogWarning(savePath + " Data doesn't exist");
        }

        Debug.Log("Best Search Time: " + finalSearchingTime);
        Debug.Log("Best Photo Score: " + analysisScore);
        Debug.Log("Best Classification Time: " + finalClassificationTime);

    }
}
