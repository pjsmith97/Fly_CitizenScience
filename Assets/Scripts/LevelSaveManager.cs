using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

/***************************************************************************************
*    Title: LevelSaveManager
*    Previous Title: SaveScript
*    Author: Reso Coder
*    Date: July 27, 2018
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://www.youtube.com/watch?v=iAgbh_Gb4wE
*    Description: Manages serialization of data that tracks best results in individual levels
*
***************************************************************************************/
public class LevelSaveManager : MonoBehaviour
{
    public static string sceneName; // name of the scene in Unity
    public float finalSearchingTime; // the final result of the player's search time in the 3D level
    public float finalClassificationTime; // final time of the classification section of the game
    public int analysisScore; // final score 
    public int totalCurrentClassifications;
    public bool noData;

    [Header ("HighScore")]
    public bool classificationHighScore;
    public bool searchHighScore;

    private string levelSavePath;
    private string levelDirectoryPath;
    private string correctCounterSavePath;

    // Start is called before the first frame update
    void Start()
    {
        // Creates expected file path to save directory
        levelDirectoryPath = Path.Combine(Application.persistentDataPath, "LevelScoreSaves");

        // Creates expected file path to classification save directory
        correctCounterSavePath = Path.Combine(Application.persistentDataPath, "ClassificationCounterSave");

        //If any save paths don't exist, create a new directory for the save file
        if (!Directory.Exists(levelDirectoryPath))
        {
            Directory.CreateDirectory(levelDirectoryPath);
        }
        if (!Directory.Exists(correctCounterSavePath))
        {
            Directory.CreateDirectory(correctCounterSavePath);
        }


        sceneName = FlyScoreManager.sceneName;
        levelSavePath = Path.Combine(levelDirectoryPath, sceneName + ".save");
        correctCounterSavePath = Path.Combine(correctCounterSavePath, "ClassificationCounter.save");

        searchHighScore = false;
        classificationHighScore = false;
        noData = false;

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /***************************************************************************************
*    Title: UpdatLevelPath
*    
*    Description: Updates the game to know what level the player is currently on. Changes the 
*                 sceneName variables for both FlyScoreManager and this class. Updates the save path
*                 and then reloads the level data.
*
***************************************************************************************/
    public void UpdateLevelPath(string newLevel)
    {
        FlyScoreManager.sceneName = newLevel;
        sceneName = FlyScoreManager.sceneName;
        levelSavePath = Path.Combine(levelDirectoryPath, sceneName + ".save");
        LoadData();
    }

    /***************************************************************************************
*    Title: SaveData
*    
*    Description: Saves new SaveLevelData and SaveClassificationData to file save path. For SaveLevelData 
*               checks if each level stat is an improvement over the previous record.
*
***************************************************************************************/
    public void SaveData()
    {
        Dictionary<string, float> newData = new Dictionary<string, float>();

        // Compare old and new search times
        if(finalSearchingTime > FlyScoreManager.finalTime)
        {
            newData["Best Search Time"] = FlyScoreManager.finalTime;
            searchHighScore = true;
        }
        else
        {
            newData["Best Search Time"] = finalSearchingTime;
        }

        // Grab newest level stats for classification 
        float classificationTimer = GetComponent<PhotoAnalysisController>().totalGuessingTime;
        int correctPhotos = GetComponent<PhotoAnalysisController>().correctPhotos;
        

        // Compare old and new photo scores.
        if (analysisScore <= correctPhotos)
        {
            newData["Best Photo Score"] = correctPhotos;

            // Classification time only matters if photo score is at least as good as the current high score
            
            // Compare old and new classification times
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
        // If photo score doesn't beat the record, use old stats.
        else
        {
            newData["Best Photo Score"] = analysisScore;
            newData["Best Classification Time"] = finalClassificationTime;
        }


        var save = new SaveLevelData()
        {
            levelData = newData
        };

        // increment serialized classification counter
        totalCurrentClassifications += correctPhotos;

        var totalClassSave = new SaveClassificationCounter()
        {
            totalClassifications = totalCurrentClassifications
        };

        // save level data
        var binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(levelSavePath))
        {
            binaryFormatter.Serialize(fileStream, save);
        }

        // save total classification data
        using (var fileStream = File.Create(correctCounterSavePath))
        {
            binaryFormatter.Serialize(fileStream, totalClassSave);
        }

        Debug.Log("Data Saved");
        noData = false;
    }

    /***************************************************************************************
*    Title: LoadData
*    
*    Description: Loads the data from the file save path. If file doesn't exist, create new one with the same file path
*
***************************************************************************************/
    public void LoadData()
    {

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Loads in both SaveLevelData and SaveClassificationData.
*
***************************************************************************************/
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

            // Store saved values
            finalSearchingTime = save.levelData["Best Search Time"];
            analysisScore = (int)save.levelData["Best Photo Score"];
            finalClassificationTime = save.levelData["Best Classification Time"];
            Debug.Log(levelSavePath + " Data Exists");
            noData = false;
        }

        // If the save path doesn't exist, set preset stats and indicate that no data exists
        else
        {
            finalSearchingTime = 60*60;
            analysisScore = 0;
            finalClassificationTime = 60*60;
            Debug.LogWarning(levelSavePath + " Data doesn't exist");
            noData = true;
        }

        if (File.Exists(correctCounterSavePath))
        {
            using (var fileStream = File.Open(correctCounterSavePath, FileMode.Open))
            {
                counterSave = (SaveClassificationCounter)binaryFormatter.Deserialize(fileStream);
            }

            totalCurrentClassifications = counterSave.totalClassifications;
        }

        // If path doesn't exist, set total classifications to 0
        else
        {
            totalCurrentClassifications = 0;
        }

        Debug.Log("Best Search Time: " + finalSearchingTime);
        Debug.Log("Best Photo Score: " + analysisScore);
        Debug.Log("Best Classification Time: " + finalClassificationTime);
        Debug.Log("Total Classifications: " + totalCurrentClassifications);
    }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/
}
