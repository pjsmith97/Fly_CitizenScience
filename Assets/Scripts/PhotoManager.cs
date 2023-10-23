using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Dynamic;
using System.Threading.Tasks;
using MMOS.SDK;

/***************************************************************************************
*    Title: PhotoManager
*    Author: Philip Smith
*    Date: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Description: Manages the MMOS API calls to the developer portal and loads in the fly image 
*
***************************************************************************************/
public class PhotoManager : MonoBehaviour
{
    //private MMOSCalls mmosCaller;

    [SerializeField] private Image flyPhoto; // the fly image
    private string photoURL; 
    private int currentTaskID; // ID from MMOS that gives the current classification task
    private Api sessionApi; // Api game object from MMOS
    private dynamic requestApiBody;
    private dynamic postApiBody;
    private string apiGameName = "dev-pjsmith997-yahoo-com"; // Fly Catcher's name registered with MMOS developer portal
    public string playerCode /*= "player00"*/;
    public string playerGroup = "playergroup"; // name of player group

    private PhotoAnalysisController analysisController; // Photo Analysis Controller object
    public bool loadingPhoto; // is the class loading a photo?

    // Start is called before the first frame update
    void Start()
    {
        /***************************************************************************************
*   Original Author: MassivelyMultiplayerOnlineScience
*
*    Description: Sets Api to receive info from MMOS developer portal
*
***************************************************************************************/
        Api.ApiConfig config = new Api.ApiConfig(
            new Api.ApiKey("dev-pjsmith997-yahoo-com", "f7a540a3a4deef44eac8f77bfcaa1cf9d1fdfa57"), // info specific to Fly Catcher
            apiGameName,
            null,
            "api.depo.mmos.blue",
            null,
            null
            );

        sessionApi = new Api(config);

        requestApiBody = new ExpandoObject();
        requestApiBody.projects = new string[] { "spipoll-fly" }; // the data set
        requestApiBody.player = new ExpandoObject();
        requestApiBody.player.accountCode = playerCode;

/***************************************************************************************
*   End
*
***************************************************************************************/

        analysisController = GetComponent<PhotoAnalysisController>(); // Set analysis controller

        var spiPollTask = GetSpiPollInfo(); // Retrieve MMOS classification tassk
        //photoURL = spiPollTask.Result;

        loadingPhoto = false;
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(GetTexture());
    }

    /***************************************************************************************
*    Title: GetTexture
*    
*    Description: Download fly photo texture from photo URL address 
*
***************************************************************************************/
    private IEnumerator GetTexture()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoURL);
        Debug.Log("getting texture");

        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError
            || request.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("Photo successfully downloaded");

            var texture = DownloadHandlerTexture.GetContent(request);
            flyPhoto.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)); // Turn texture into sprite
        }
    }

    /***************************************************************************************
*    Title: GetSpiPollInfo
*    
*    Description: Asynchronous function used that calls MMOSPortalGetPhoto to retrieve photo URL and GetTexture to load the image.
*                 Monitors when the photo is loading and when it ends.
*
***************************************************************************************/
    public async Task GetSpiPollInfo()
    {
        Debug.Log("Getting Photo!");
        loadingPhoto = true;
        analysisController.editingStateHelper.loadingUI.SetActive(true); // Sets photo loading UI to active

        await MMOSPortalGetPhoto(); // retrieve URL
        //await mmosTask;
        StartCoroutine(GetTexture()); // load photo image
        //return currentImageURL;

        Debug.Log("Stop loading");
        loadingPhoto = false;
        analysisController.editingStateHelper.loadingUI.SetActive(false); // Sets photo loading UI to inactive
    }

    /***************************************************************************************
*    Title: MMOSPortalGetPhoto
*    
*    Description: API call to MMOS portal to retrieve photo URL
*
***************************************************************************************/
    private async Task MMOSPortalGetPhoto()
    {
        dynamic response = null;

        Debug.Log("try start");

        try
        {
            response = await sessionApi.V2.Players.CreateTask(playerCode, requestApiBody); // retrieve photo URL
            var responseBody = response["body"];
            
            /*if(responseBody["task"]["difficulty"] != null)
            {
                while ((int)responseBody["task"]["difficulty"] > 3 || currentTaskID == (int)responseBody["task"]["id"])
                {
                    response = await sessionApi.V2.Players.CreateTask(playerCode, requestApiBody);
                    responseBody = response["body"];
                }
            }

            else
            {
                Debug.Log("No Difficulty");
                while (currentTaskID == (int)responseBody["task"]["id"])
                {
                    response = await sessionApi.V2.Players.CreateTask(playerCode, requestApiBody);
                    responseBody = response["body"];
                }
            }*/

            if (responseBody["task"]["difficulty"] == null) // Check if the task has a difficulty
            {
                Debug.Log("No Difficulty");
            }

            while (currentTaskID == (int)responseBody["task"]["id"]) // Don't retrieve the same photo twice in a row
            {
                response = await sessionApi.V2.Players.CreateTask(playerCode, requestApiBody);
                responseBody = response["body"];
            }

            Debug.Log("response body assets");
            photoURL = (string)responseBody["task"]["assets"]["url"];
            currentTaskID = (int)responseBody["task"]["id"];
            Debug.Log(photoURL);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /***************************************************************************************
*    Title: SendSpiPollInfo
*    
*    Description: Asynchronous function. Calls MMOSPortalPostClassification to post the classification answer from the player.
*                 Keeps track of when the task starts being posted and when it ends. playerResult is the answer they chose and 
*                 time is the amount of time it took them.
*
***************************************************************************************/
    public async Task SendSpiPollInfo(string playerResult, int time)
    {
        Debug.Log("Posting Task!");
        await MMOSPortalPostClassification(playerResult, time);
        //await mmosTask;
        Debug.Log("Done Awaiting Post Task");
    }

    /***************************************************************************************
*    Title: SendAndGetSpiPollInfo
*    
*    Description: Asynchronous function. Calls functions to send MMOS classification info, and after it's done
*                 retrieve a new fly photo task. playerResult is the answer they chose and 
*                 time is the amount of time it took them.
*
***************************************************************************************/
    public async Task SendAndGetSpiPollInfo(string playerResult, int time)
    {
        await SendSpiPollInfo(playerResult, time);
        await GetSpiPollInfo();
    }

    /***************************************************************************************
*    Title: MMOSPortalPostClassification
*    
*    Description: Asynchronous function. Posts the player result to MMOS. playerResult is the answer they chose and 
*                 time is the amount of time it took them.
*
***************************************************************************************/
    private async Task MMOSPortalPostClassification(string playerResult, int time)
    {
        /*
         {
              "task": {
                "id": 20000000,
                "result": {
                  "gender": "female"
                }
              },
              "circumstances": {
                "t": 1000
              },
              "player": "test-sc050-cc0",
              "playergroup": "playergroup",
              "game": "mmos"
            }
         */

        // Set up API body with the player result and time
        postApiBody = new ExpandoObject();
        postApiBody.task = new ExpandoObject();
        postApiBody.task.id = currentTaskID;
        postApiBody.task.result = new ExpandoObject();
        postApiBody.task.result.gender = playerResult;
        postApiBody.circumstances = new ExpandoObject();
        postApiBody.circumstances.t = time;
        postApiBody.game = apiGameName;
        postApiBody.player = playerCode;
        postApiBody.playergroup = playerGroup;


        dynamic response = null;

        try
        {
            response = await sessionApi.V2.Classifications.Create(postApiBody); // send result info
            var responseBody = response["body"];

            //currentTaskID = (int)responseBody["task"]["id"];
            //Debug.Log("uid: " + (string)responseBody["uid"]);
            //Debug.Log("Score: " + (string)responseBody["score"]);
            //Debug.Log("Result Reliability: " + (string)responseBody["reliability"]);

            if(responseBody["task"]["solution"] != null)
            {

                if (int.Parse((string)responseBody["score"]) == 1) // Player answered right on test data. Award point
                {
                    analysisController.correctPhotos += 1;
                    analysisController.solutionStateHelper.correct = true;
                }
                else
                {
                    analysisController.solutionStateHelper.correct = false; // Player answered wrong on test data
                }

                Debug.Log("Solution: " + (string)responseBody["task"]["solution"]["gender"]);
                analysisController.solutionStateHelper.solution = (string)responseBody["task"]["solution"]["gender"]; // send controller the solution

                // Set solution string to capitalized
                char[] solutionArray = analysisController.solutionStateHelper.solution.ToCharArray();
                solutionArray[0] = char.ToUpper(solutionArray[0]);
                analysisController.solutionStateHelper.solution = new string(solutionArray);

                // Grammar check for Can't See solution
                if(analysisController.solutionStateHelper.solution == "CantSee")
                {
                    analysisController.solutionStateHelper.solution = "Can\'t See";
                }
            }

            // If the photo wasn't a test photo, award player point
            else
            {
                analysisController.correctPhotos += 1;
                analysisController.solutionStateHelper.correct = true;

                Debug.Log("Solution: No Solution");
                analysisController.solutionStateHelper.solution = "No Solution";
            }

            Debug.Log("Correct Photo Count: " + analysisController.correctPhotos);
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

    }
}
