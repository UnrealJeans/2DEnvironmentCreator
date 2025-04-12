

using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfielkeuzeApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadProfielKeuzes()
    {
        string route = "/ProfielKeuze"; // Correcte route
        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseProfielKeuzeListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateProfielKeuze(ProfielKeuze profielKeuze)
    {
        string route = "/ProfielKeuze"; // Correcte route
        string data = JsonUtility.ToJson(profielKeuze);
        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseProfielKeuzeResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteProfielKeuze(string profielKeuzeId)
    {
        string route = "/ProfielKeuze/" + profielKeuzeId; // Correcte route
        return await webClient.SendDeleteRequest(route);
    }

    private IWebRequestReponse ParseProfielKeuzeResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                ProfielKeuze profielKeuze = JsonUtility.FromJson<ProfielKeuze>(data.Data);
                WebRequestData<ProfielKeuze> parsedWebRequestData = new WebRequestData<ProfielKeuze>(profielKeuze);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseProfielKeuzeListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<ProfielKeuze> profielKeuzes = JsonHelper.ParseJsonArray<ProfielKeuze>(data.Data);
                WebRequestData<List<ProfielKeuze>> parsedWebRequestData = new WebRequestData<List<ProfielKeuze>>(profielKeuzes);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public async void GetAllProfielInfo()
    {
        Debug.LogError("Keuzes worden geladen");
        IWebRequestReponse response = await ReadProfielKeuzes();
        if (response is WebRequestData<List<ProfielKeuze>> profielKeuzesData)
        {
            List<ProfielKeuze> profielKeuzes = profielKeuzesData.Data;
            foreach (var profielKeuze in profielKeuzes)
            {
                Debug.Log($"Name: {profielKeuze.name}, Arts: {profielKeuze.arts}, GeboorteDatum: {profielKeuze.geboorteDatum}, Avatar: {profielKeuze.avatar}");
            }
        }
        else
        {
            Debug.LogError("Failed to retrieve profiel keuzes.");
        }
    }
}





