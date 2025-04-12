using System;
using System.Collections.Generic;
using UnityEngine;

public class AgendaApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadAgendas()
    {
        string route = "/api/agenda"; // Correct route
        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseAgendaListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateAgenda(Agenda agenda)
    {
        string route = "/api/agenda"; // Correct route
        string data = JsonUtility.ToJson(agenda);
        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseAgendaResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteAgenda(string agendaId)
    {
        string route = "/api/agenda/" + agendaId; // Correct route
        return await webClient.SendDeleteRequest(route);
    }

    public async Awaitable<IWebRequestReponse> UpdateAgenda(string agendaId, Agenda agenda)
    {
        string route = "/api/agenda/" + agendaId; // Correct route
        string data = JsonUtility.ToJson(agenda);
        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseAgendaResponse(webRequestResponse);
    }

    private IWebRequestReponse ParseAgendaResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Agenda agenda = JsonUtility.FromJson<Agenda>(data.Data);
                WebRequestData<Agenda> parsedWebRequestData = new WebRequestData<Agenda>(agenda);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseAgendaListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Agenda> agendas = JsonHelper.ParseJsonArray<Agenda>(data.Data);
                WebRequestData<List<Agenda>> parsedWebRequestData = new WebRequestData<List<Agenda>>(agendas);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public async void GetAllAgendaInfo()
    {
        Debug.LogError("Agendas are being loaded");
        IWebRequestReponse response = await ReadAgendas();
        if (response is WebRequestData<List<Agenda>> agendasData)
        {
            List<Agenda> agendas = agendasData.Data;
            foreach (var agenda in agendas)
            {
                Debug.Log($"Date1: {agenda.date1}, Location1: {agenda.location1}, Date2: {agenda.date2}, Location2: {agenda.location2}, Date3: {agenda.date3}, Location3: {agenda.location3}");
            }
        }
        else
        {
            Debug.LogError("Failed to retrieve agendas.");
        }
    }
}
