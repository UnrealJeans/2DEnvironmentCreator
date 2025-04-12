using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadEnvironments()
    {
        string route = "/Environment"; // Correct route
        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseEnvironmentListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateEnvironment(Environment environment)
    {
        string route = "/Environment"; // Correct route
        string data = JsonUtility.ToJson(environment);
        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseEnvironmentResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteEnvironment(string environmentId)
    {
        string route = "/Environment/" + environmentId; // Correct route
        return await webClient.SendDeleteRequest(route);
    }

    private IWebRequestReponse ParseEnvironmentResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Environment environment = JsonUtility.FromJson<Environment>(data.Data);
                WebRequestData<Environment> parsedWebRequestData = new WebRequestData<Environment>(environment);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseEnvironmentListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Environment> environments = JsonHelper.ParseJsonArray<Environment>(data.Data);
                WebRequestData<List<Environment>> parsedWebRequestData = new WebRequestData<List<Environment>>(environments);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public async void GetAllEnvironmentInfo()
    {
        Debug.LogError("Environments are being loaded");
        IWebRequestReponse response = await ReadEnvironments();
        if (response is WebRequestData<List<Environment>> environmentsData)
        {
            List<Environment> environments = environmentsData.Data;
            foreach (var environment in environments)
            {
                Debug.Log($"Name: {environment.name}");
            }
        }
        else
        {
            Debug.LogError("Failed to retrieve environments.");
        }
    }
}
