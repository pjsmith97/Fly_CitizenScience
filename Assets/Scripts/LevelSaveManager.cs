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
    public int totalCurrentClassifications;

    [Header ("HighScore")]
    public bool classificationHighScore;
    public bool searchHighScore;

    private string levelSavePath;
    private string correctCounterSavePath;

    // Start is called before the first frame update
    void Start()
    {
        levelSavePath = Path.Combine(Application.persistentDataPath, "LevelScoreSaves");
        correctCounterSavePath = Path.Combine(Application.persistentDataPath, "ClassificationCounterSave");


        if (!Directory.Exists(levelSavePath))
        {
            Directory.CreateDirectory(levelSavePath);
        }
        if (!Directory.Exists(correctCounterSavePath))
        {
            Directory.CreateDirectory(correctCounterSavePath);
        }

        sceneName = FlyScoreManager.sceneName;
        levelSavePath = Path.Combine(levelSavePath, sceneName + ".save");
        correctCounterSavePath = Path.Combine(correctCounterSavePath, "ClassificationCounter.save");

        searchHighScore = false;
        classificationHighScore = false;

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData()
    {
        Dictionary<string, float> newData = new Dictionary<string, float>();

        if(finalSearchingTime > FlyScoreManager.finalTime)
        {
            newData["Best Search Time"] = FlyScoreManager.finalTime;
            searchHighScore = true;
        }
        else
        {
            newData["Best Search Time"] = finalSearchingTime;
        }

        float classificationTimer = GetComponent<PhotoAnalysisController>().totalGuessingTime;
        int correctPhotos = GetComponent<PhotoAnalysisController>().correctPhotos;
        

        // Classification time only matters if photo score is at least as good as the current high score
        if (analysisScore <= correctPhotos)
        {
            newData["Best Photo Score"] = correctPhotos;

            if (finalClassificationTime > classificationTimer)
            {
                newData["Best Classification Time"] = classificationTimer;
                classificationHighScore = true;

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

        totalCurrentClassifications += correctPhotos;

        var totalClassSave = new SaveClassificationCounter()
        {
            totalClassifications = totalCurrentClassifications
        };

        var binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(levelSavePath))
        {
            binaryFormatter.Serialize(fileStream, save);
        }

        using (var fileStream = File.Create(correctCounterSavePath))
        {
            binaryFormatter.Serialize(fileStream, totalClassSave);
        }

        Debug.Log("Data Saved");
    }

    public void LoadData()
    {
        SaveLevelData save;
        SaveClassificationCounter counterSave;

        Debug.Log("Loading " + levelSavePath);

        var binaryFormatter = new BinaryFormatter();
        if (File.Exists(levelSavePath))
        {
            using (var fileStream = File.Open(levelSavePath, FileMode.Open))
            {
                save = (SaveLevelData)binaryFormatter.Deserialize(fileStream);
            }

            finalSearchingTime = save.levelData["Best Search Time"];
            analysisScore = (int)save.levelData["Best Photo Score"];
            finalClassificationTime = save.levelData["Best Classification Time"];
        }

        else
        {
            finalSearchingTime = 60*60;
            analysisScore = 0;
            finalClassificationTime = 60*60;
            Debug.LogWarning(levelSavePath + " Data doesn't exist");
        }

        if (File.Exists(correctCounterSavePath))
        {
            using (var fileStream = File.Open(correctCounterSavePath, FileMode.Open))
            {
                counterSave = (SaveClassificationCounter)binaryFormatter.Deserialize(fileStream);
            }

            totalCurrentClassifications = counterSave.totalClassifications;
        }

        else
        {
            totalCurrentClassifications = 0;
        }

        Debug.Log("Best Search Time: " + finalSearchingTime);
        Debug.Log("Best Photo Score: " + analysisScore);
        Debug.Log("Best Classification Time: " + finalClassificationTime);
        Debug.Log("Total Classifications: " + totalCurrentClassifications);
    }
}
