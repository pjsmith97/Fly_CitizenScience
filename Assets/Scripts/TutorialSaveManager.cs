using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class TutorialSaveManager : MonoBehaviour
{
    private string savePath;

    public bool tutorialSeen;

    // Start is called before the first frame update
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Tutorial");

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        savePath = Path.Combine(savePath, "Tutorial.save");

        LoadData();
    }

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

    public void LoadData()
    {
        SaveTutorialData save;

        Debug.Log("Loading " + savePath);

        var binaryFormatter = new BinaryFormatter();
        if (File.Exists(savePath))
        {
            using (var fileStream = File.Open(savePath, FileMode.Open))
            {
                save = (SaveTutorialData)binaryFormatter.Deserialize(fileStream);
            }

            tutorialSeen = save.tutorialSeen;
        }

        else
        {
            tutorialSeen = false;
            Debug.LogWarning(savePath + " Data doesn't exist");
        }

    }
}
