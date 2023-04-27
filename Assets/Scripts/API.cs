using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    [SerializeField] GameObject _ManagerRef;

    [SerializeField] GameObject _SpeakerRef;

    private EmoEnum EmoState = EmoEnum.NEUTRAL;
    private string EmoStateTemp;

    [SerializeField] string APIURL = "https://llm-backend.fattyalchemist.com/api/v1.0/mpph/";
    private string AuthToken = "83|IoTuZYmUsyOM6XTnSW2leUqVYh3NO61yMTeR9EWW";

    // Start is called before the first frame update
    void Start()
    {
        _ManagerRef = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallAPI(string con)
    {
        StartCoroutine(CallAPIFn(con));
    }

    IEnumerator CallAPIFn(string con)
    {
        Debug.Log("API call triggered");
        APIReq reqJson = new APIReq
        {
            content = con,
            userName = _ManagerRef.GetComponent<Manager>().GetUserDisplayName()
        };

        yield return null;

        string JsonData = JsonUtility.ToJson(reqJson);

        UnityWebRequest req = new UnityWebRequest(APIURL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", "Bearer " + AuthToken);

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            APIRes res = JsonUtility.FromJson<APIRes>(req.downloadHandler.text);
            Debug.Log(res.ToString());
            Debug.Log(res.data[0].reply);
            Debug.Log(res.data[0].response_emotion);

            //Set this based on mood
            //_SpeakerRef.GetComponent<TTSSpeaker>().presetVoiceID = "";

            EmoStateTemp = res.data[0].response_emotion;

            switch (res.data[0].response_emotion)
            {
                case "neutral":
                    EmoState = EmoEnum.NEUTRAL;
                    break;
                case "happy":
                    EmoState = EmoEnum.HAPPY;
                    break;
                case "sad":
                    EmoState = EmoEnum.SAD;
                    break;
                case "energetic":
                    EmoState = EmoEnum.ENERGETIC;
                    break;
                case "mad":
                    EmoState = EmoEnum.MAD;
                    break;
                default:
                    EmoState = EmoEnum.NEUTRAL;
                    break;
            }

            _SpeakerRef.GetComponent<TTSSpeaker>().Speak(res.data[0].reply);
        }

        yield return null;

        req.Dispose();


    }

    public void SetEmoState()
    {
        switch (EmoStateTemp)
        {
            case "neutral":
                EmoState = EmoEnum.NEUTRAL;
                break;
            case "happy":
                EmoState = EmoEnum.HAPPY;
                break;
            case "sad":
                EmoState = EmoEnum.SAD;
                break;
            case "energetic":
                EmoState = EmoEnum.ENERGETIC;
                break;
            case "mad":
                EmoState = EmoEnum.MAD;
                break;
            default:
                EmoState = EmoEnum.NEUTRAL;
                break;
        }
    }

    public void EmoStateNeutral()
    {
        EmoState = EmoEnum.NEUTRAL;
    }

    public EmoEnum GetEmoState()
    {
        return EmoState;
    }
}

[System.Serializable]
public class APIReq
{
    public string userName;
    public string content;
}

[System.Serializable]
public class APIRes
{
    public DataRes[] data;
}

[System.Serializable]
public class DataRes
{
    public string identified_emotion;
    public string reply;
    public string response_emotion;
}

