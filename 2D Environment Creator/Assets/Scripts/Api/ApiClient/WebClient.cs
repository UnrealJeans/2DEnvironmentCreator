using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Text;

public class WebClient : MonoBehaviour
{
    public string baseUrl;
    private string token;

    public void SetToken(string token)
    {
        this.token = token;
    }

    public async Awaitable<IWebRequestReponse> SendGetRequest(string route)
    {
        string url = baseUrl + route;
        UnityWebRequest webRequest = CreateWebRequest("GET", url, null);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendPostRequest(string route, string data)
    {
        string url = baseUrl + route;
        UnityWebRequest webRequest = CreateWebRequest("POST", url, data);
        return await SendWebRequest(webRequest);
    }

    public async Awaitable<IWebRequestReponse> SendDeleteRequest(string route)
    {
        string url = baseUrl + route;
        UnityWebRequest webRequest = CreateWebRequest("DELETE", url, null);
        return await SendWebRequest(webRequest);
    }

    private UnityWebRequest CreateWebRequest(string type, string url, string data)
    {
        Debug.Log("Creating " + type + " request to " + url + " with data: " + data);

        if (data != null)
        {
            data = RemoveIdFromJson(data); // Backend throws error if it receives empty strings as a GUID value.
        }

        var webRequest = new UnityWebRequest(url, type);
        if (!string.IsNullOrEmpty(data))
        {
            byte[] dataInBytes = new UTF8Encoding().GetBytes(data);
            webRequest.uploadHandler = new UploadHandlerRaw(dataInBytes);
        }
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        AddToken(webRequest);
        return webRequest;
    }

    public async Awaitable<IWebRequestReponse> SendPutRequest(string route, string data)
    {
        string url = $"{baseUrl}{route}";
        UnityWebRequest webRequest = CreateWebRequest("PUT", url, data);
        return await SendWebRequest(webRequest);
    }


    private string RemoveIdFromJson(string json)
    {
        return json?.Replace("\"id\":\"\",", "");
    }

    private async Awaitable<IWebRequestReponse> SendWebRequest(UnityWebRequest webRequest)
    {
        await webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            return new WebRequestData<string>(webRequest.downloadHandler.text);
        }
        else
        {
            return new WebRequestError(webRequest.error);
        }
    }


    private void AddToken(UnityWebRequest webRequest)
    {
        if (!string.IsNullOrEmpty(token))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
        }
    }
}

[Serializable]
public class Token
{
    public string tokenType;
    public string accessToken;
}
