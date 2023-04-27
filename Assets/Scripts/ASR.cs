using System.Collections;
using System.Collections.Generic;
using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Voice;
using UnityEngine;

public class ASR : MonoBehaviour
{
    [Header("Voice")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    [SerializeField] private API _APIRef;

    public bool IsActive => _active;
    private bool _active = false;

    // Start is called before the first frame update
    void Start()
    {
        _APIRef = this.GetComponent<API>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener(OnRequestStarted);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnStartListening.AddListener(OnListenStart);
        appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnListenStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.AddListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.AddListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnRequestResponse);
        appVoiceExperience.VoiceEvents.OnError.AddListener(OnRequestError);
    }

    private void OnDisable()
    {
        appVoiceExperience.VoiceEvents.OnRequestCreated.RemoveListener(OnRequestStarted);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnStartListening.RemoveListener(OnListenStart);
        appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(OnListenStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.RemoveListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.RemoveListener(OnListenForcedStop);
        appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnRequestResponse);
        appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnRequestError);
    }

    // Request began
    private void OnRequestStarted(WitRequest r)
    {
        _active = true;
        Debug.Log("ASR request started");
    }

    // Request transcript
    private void OnRequestTranscript(string transcript)
    {
        Debug.Log("ASR request transcript :: " + transcript);
    }

    // Listen start
    private void OnListenStart()
    {
        Debug.Log("ASR is listening");
    }

    // Listen stop
    private void OnListenStop()
    {
        Debug.Log("ASR has stopped and is processing");
        OnRequestComplete();
    }

    // Force listen stop
    private void OnListenForcedStop()
    {
        Debug.Log("ASR is forced to stop listening");
        OnRequestComplete();
    }

    // Request response
    private void OnRequestResponse(WitResponseNode response)
    {
        if (!string.IsNullOrEmpty(response["text"]))
        {
            Debug.Log("ASR request response :: " + response["text"]);
            _APIRef.CallAPI(response["text"]);
        }
        else
        {
            Debug.Log("ASR processed response is null or empty");
        }
        OnRequestComplete();
    }

    // Request error
    private void OnRequestError(string error, string message)
    {
        Debug.LogError("Error - " + error + " :: " + message);
        OnRequestComplete();
    }

    // Deactivate
    private void OnRequestComplete()
    {
        _active = false;
        Debug.Log("STT request is now complete");
    }

    // Toggle activation
    public void ToggleActivation()
    {
        SetActivation(!_active);
    }

    // Set activation
    public void SetActivation(bool toActivated)
    {
        if (_active != toActivated)
        {
            _active = toActivated;
            if (_active)
            {
                appVoiceExperience.Activate();
            }
            else
            {
                appVoiceExperience.Deactivate();
            }
        }
    }

    public EmoEnum GetEmoState()
    {
        return _APIRef.GetEmoState();
    }

    public void SetEmoState()
    {
        _APIRef.SetEmoState();
    }

    public void EmoStateNeutral()
    {
        _APIRef.EmoStateNeutral();
    }
}
