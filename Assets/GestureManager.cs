using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    public static GestureManager Instance { get; private set; }

    private const float EMOTION_SECONDS = 2f;

    private Animator anim => EyeInteractable.CurrentFocus == null ? null : EyeInteractable.CurrentFocus.Anim;

    private void Awake()
    {
        Instance = this;
    }


    public void OnPeaceSign()
    {
        Debug.Log("On Peace Sign");
        PlayEmotionChange(anim, "isEnergetic", EMOTION_SECONDS);
    }

    public void OnMiddleFinger()
    {
        PlayEmotionChange(anim, "isMad", EMOTION_SECONDS);
    }

    public void OnThumbsUp()
    {
        Debug.Log("On Thumbs Up");
        PlayEmotionChange(anim, "isHappy", EMOTION_SECONDS);
    }

    private void PlayEmotionChange(Animator anim, string property, float seconds)
    {
        if (anim == null) return;
        IEnumerator EmotionChange(Animator anim, string property)
        {
            Debug.Log("Starting emotion: " + property);
            anim.SetBool(property, true);
            yield return new WaitForSeconds(seconds);
            Debug.Log("Ending emotion: " + property);
            anim.SetBool(property, false);
        }
        StartCoroutine(EmotionChange(anim, property));
    }
}
