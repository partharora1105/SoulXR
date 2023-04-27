using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] ASR _ASRRef;

    [SerializeField] bool IsSpeaking = false;
    [SerializeField] EmoEnum EmotionalState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateMic()
    {
        _ASRRef.SetActivation(true);
    }

    public void DeactivateMic()
    {
        _ASRRef.SetActivation(false);
    }

    public bool IsObjSpeaking()
    {
        return IsSpeaking;
    }

    public EmoEnum EmoState()
    {
        EmotionalState = _ASRRef.GetEmoState();
        return _ASRRef.GetEmoState();
    }

    public void SetSpeakingState(bool val)
    {
        IsSpeaking = val;
    }

    public void SetEmoState()
    {
        _ASRRef.SetEmoState();
    }

    public void EmoStateNeutral()
    {
        _ASRRef.EmoStateNeutral();
    }
}
