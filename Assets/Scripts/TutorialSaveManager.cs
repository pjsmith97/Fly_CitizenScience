using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/***************************************************************************************
*    Title: TutorialSaveManager
*    Previous Title: SaveScript
*    Author: Reso Coder
*    Date: July 27, 2018
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://www.youtube.com/watch?v=iAgbh_Gb4wE
*    Description: Manages serialization of data that tracks whether player has accessed or been asked to play the tutorial
*
***************************************************************************************/
public class TutorialSaveManager : MonoBehaviour
{
    private string savePath; // the file save path of the data

    public bool tutorialSeen; // has the tutorial been offered or played though

    // Start is called before the first frame update
    void Start()
    {
        // Creates expected file path to Tutorial save directory
        savePath = Path.Combine(Application.persistentDataPath, "Tutorial");

        //If save path doesn't exist, create a new directory for the save file
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // Create new file path
        savePath = Path.Combine(savePath, "Tutorial.save");

        // Loads the tutorial save data using the new save path
        LoadData();
    }

/***************************************************************************************
*    Title: SaveData
*    
*    Description: Saves new SaveTutorialData to file save path, with boolean 'seen' determining if tutorial has been seen
*
***************************************************************************************/
    public void SaveData(bool seen)
    {

        var save = new SaveTutorialData()
        {
            tutorialSeen = seen
        };

        var binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(savePath))
        {
            binaryFormatter.Serialize(fileStream, save);
        }

        Debug.Log("Tutorial Data Saved");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/***************************************************************************************
*    Title: LoadData
*    
*    Description: Loads the data from the file save path. If file doesn't exist, create new one with the same file path
*
***************************************************************************************/
    public void LoadData()
    {
        SaveTutorialData save;

        Debug.Log("Loading " + savePath);

        var binaryFormatter = new BinaryFormatter();

        // If the file exists, deserialize and load in the data
        if (File.Exists(savePath))
        {
            using (var fileStream = File.Open(savePath, FileMode.Open))
            {
                save = (SaveTutorialData)binaryFormatter.Deserialize(fileStream);
            }

            tutorialSeen = save.tutorialSeen; //load saved data about whether tutorial has been seen
        }

        // If file path doesn't exist, set it to that tutorial hasn't been seen and return log warning
        else
        {
            tutorialSeen = false;
            Debug.LogWarning(savePath + " Data doesn't exist");
        }

    }
}
