using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class object2DApiClient : MonoBehaviour
{
    public WebClient webClient;
    public EnvironmentApiClient environmentApiClient;

    public async Awaitable<IWebRequestReponse> ReadObject2Ds(string environmentId)
    {
        string route = "/api/environment/" + environmentId + "/objects";
        Debug.Log($"Sending GET request to: {webClient.baseUrl}{route}");

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);

        if (webRequestResponse is WebRequestData<string> data)
        {
            Debug.Log($"Raw response data: {data.Data}");
        }
        else
        {
            Debug.LogError("Failed to retrieve object2D data.");
        }

        return ParseObject2DListResponse(webRequestResponse);
    }





    public async Awaitable<IWebRequestReponse> CreateObject2D(object2D object2D)
    {
        string route = $"/api/environment/{object2D.environmentId}/objects";
        string data = JsonUtility.ToJson(object2D);

        Debug.Log($"POST Request URL: {webClient.baseUrl}{route}");
        Debug.Log($"POST Request Body: {data}");

        var response = await webClient.SendPostRequest(route, data);
        if (response is WebRequestError error)
        {
            Debug.LogError($"CreateObject2D failed: {error.ErrorMessage}");
        }
        return response;
    }



    public async Awaitable<IWebRequestReponse> UpdateObject2D(object2D object2D)
    {
        string route = "/environment/" + object2D.environmentId + "/objects/" + object2D.id;
        string data = JsonUtility.ToJson(object2D);

        return await webClient.SendPutRequest(route, data);
    }

    private IWebRequestReponse ParseObject2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Object2D object2D = JsonUtility.FromJson<Object2D>(data.Data);
                WebRequestData<Object2D> parsedWebRequestData = new WebRequestData<Object2D>(object2D);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseObject2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);

                if (string.IsNullOrEmpty(data.Data))
                {
                    Debug.LogError("API response is empty.");
                    return new WebRequestError("Empty response from API.");
                }

                try
                {
                    List<object2D> objects = JsonHelper.ParseJsonArray<object2D>(data.Data);
                    return new WebRequestData<List<object2D>>(objects);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse API response: {ex.Message}");
                    return new WebRequestError("Failed to parse API response.");
                }

            default:
                Debug.LogError("Unexpected response type.");
                return webRequestResponse;
        }
    }

}