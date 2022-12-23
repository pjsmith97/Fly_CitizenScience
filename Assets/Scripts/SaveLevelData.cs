using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***************************************************************************************
*    Title: SaveLevelData
*    Author: Philip Smith
*    Date: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Description: Serializable class that stores Dictionary that contains the current high score of a given level 
*
***************************************************************************************/
[System.Serializable]
public class SaveLevelData
{
    public Dictionary<string, float> levelData;
}
