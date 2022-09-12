using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Dynamic;
using System.Threading.Tasks;
using MMOS.SDK;

public class PhotoManager : MonoBehaviour
{
    //private MMOSCalls mmosCaller;

    [SerializeField] private Image flyPhoto;
    private string photoURL;
    private int currentTaskID;
    private Api sessionApi;
    private dynamic requestApiBody;
    private dynamic postApiBody;
    private string apiGameName = "dev-pjsmith997-yahoo-com";
    public string playerCode = "player00";
    public string playerGroup = "playergroup";

    // Start is called before the first frame update
    void Start()
    {
        Api.ApiConfig config = new Api.ApiConfig(
            new Api.ApiKey("dev-pjsmith997-yahoo-com", "f7a540a3a4deef44eac8f77bfcaa1cf9d1fdfa57"),
            apiGameName,
            null,
            "api.depo.mmos.blue",
            null,
            null
            );

        sessionApi = new Api(config);

        requestApiBody = new ExpandoObject();
        requestApiBody.projects = new string[] { "spipoll-fly" };
        requestApiBody.player = new ExpandoObject();
        requestApiBody.player.accountCode = playerCode;

        var spiPollTask = GetSpiPollInfo();
        //photoURL = spiPollTask.Result;
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(GetTexture());
    }

    private IEnumerator GetTexture()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoURL);

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
            flyPhoto.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public async Task GetSpiPollInfo()
    {
        Debug.Log("Getting Photo!");
        await MMOSPortalGetPhoto();
        //await mmosTask;
        StartCoroutine(GetTexture());
        //return currentImageURL;
    }

    private async Task MMOSPortalGetPhoto()
    {
        dynamic response = null;

        try
        {
            response = await sessionApi.V2.Players.CreateTask(playerCode, requestApiBody);
            var responseBody = response["body"];
            
            if(responseBody["task"]["difficulty"] != null)
            {
                while ((int)responseBody["task"]["difficulty"] > 3 || currentTaskID == (int)responseBody["task"]["id"])
                {
                    response = await sessionApi.V2.Players.CreateTask("player00", requestApiBody);
                    responseBody = response["body"];
                }
            }

            else
            {
                Debug.Log("No Difficulty");
                while (currentTaskID == (int)responseBody["task"]["id"])
                {
                    response = await sessionApi.V2.Players.CreateTask("player00", requestApiBody);
                    responseBody = response["body"];
                }
            }
            


            photoURL = (string)responseBody["task"]["assets"]["url"];
            currentTaskID = (int)responseBody["task"]["id"];
            Debug.Log(photoURL);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public async Task SendSpiPollInfo(string playerResult, int time)
    {
        Debug.Log("Posting Task!");
        await MMOSPortalPostClassification(playerResult, time);
        //await mmosTask;
        Debug.Log("Done Awaiting Post Task");
    }

    public async Task SendAndGetSpiPollInfo(string playerResult, int time)
    {
        await SendSpiPollInfo(playerResult, time);
        await GetSpiPollInfo();
    }

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
            response = await sessionApi.V2.Classifications.Create(postApiBody);
            var responseBody = response["body"];

            //currentTaskID = (int)responseBody["task"]["id"];
            Debug.Log("uid: " + (string)responseBody["uid"]);
            Debug.Log("Score: " + (string)responseBody["score"]);
            Debug.Log("Result Reliability: " + (string)responseBody["reliability"]);

            if(responseBody["task"]["solution"] != null)
            {
                Debug.Log("Solution: " + (string)responseBody["task"]["solution"]["gender"]);
            }
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }

    }
}
