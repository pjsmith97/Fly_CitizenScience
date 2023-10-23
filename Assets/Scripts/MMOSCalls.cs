using MMOS.SDK;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MMOSCalls : MonoBehaviour
{
    private string currentImageURL;

    private void Start()
    {
        var test = GetSpiPollInfo();
    }

    public async Task<string> GetSpiPollInfo()
    {
        var mmosTask = CallMMOSPortal();
        await mmosTask;
        return currentImageURL;
    }

    private async Task CallMMOSPortal()
    {
        Api.ApiConfig config = new Api.ApiConfig(
            new Api.ApiKey("dev-pjsmith997-yahoo-com", "f7a540a3a4deef44eac8f77bfcaa1cf9d1fdfa57"),
            "dev-pjsmith997-yahoo-com",
            null,
            "api.depo.mmos.blue",
            null,
            null
            );

        Api api = new Api(config);

        dynamic body = new ExpandoObject();
        body.projects = new string[] { "spipoll-fly" };
        body.player = new ExpandoObject();
        body.player.accountCode = "player00";

        dynamic response = null;

        try
        {
            response = await api.V2.Players.CreateTask("player00", body);
            var responseBody = response["body"];

            currentImageURL = (string)responseBody["task"]["assets"]["url"];
            Debug.Log(currentImageURL);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
