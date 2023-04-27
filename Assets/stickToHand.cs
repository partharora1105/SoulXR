using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stickToHand : MonoBehaviour
{
    private enum finger
    {
        Thumb, Index, Middle, Ring, Little
    };

    public enum PlacementState
    {
        Finger, Held, Placed
    };

    [Header("Hand Objects")]
    [SerializeField] private OVRSkeleton skeleton;
    [Header("Finger to Stick On")]
    [SerializeField] private finger CurrFinger;
    [Header("Finger")]
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject personaFingerObject;

    [HideInInspector] public OVRSpatialAnchor SpatialAnchor;
    [HideInInspector] public PlacementState State;

    [Header("SFX")]
    [SerializeField] private AudioSource grab;
    [SerializeField] private AudioSource release;
    [SerializeField] private AudioSource laugh;


    private string talkingEmotion;

    public Animator Anim => anim;

    private const float FINGER_DISTANCE_THRESHOLD = 0.1f;
    bool isStateChanged = false;

    public AudioManager AudioManager { get; private set; }
    private void Awake()
    {
        AudioManager = GetComponentInChildren<AudioManager>();
    }

    void Update()
    {
        if (State == PlacementState.Finger)
        {
            transform.position = GetFingerPosition();
            personaFingerObject.transform.localScale = new Vector3(1.2f, 1.2f, 0);
            transform.LookAt(Camera.main.transform);
            anim.SetBool("isGrabbed", false);

        }
        else if (State == PlacementState.Held)
        {
            anim.SetBool("isLookedAt", true);
            anim.SetBool("isGrabbed", true);
            personaFingerObject.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        }
        else if (State == PlacementState.Placed)
        {
            anim.SetBool("isLookedAt", true);
            anim.SetBool("isGrabbed", false);
            anim.SetBool("isGrowing", true);
            float scale;
            if (CurrFinger == finger.Thumb || CurrFinger == finger.Little)
            {
                scale = 4;
            }
            else if (CurrFinger == finger.Index || CurrFinger == finger.Ring)
            {
                scale = 8;
            } else
            {
                scale = 30;
            }
            personaFingerObject.transform.localScale = new Vector3(scale, scale, 0);
            Invoke("changeFace", 2.0f);

            // Set animation speaking and emotion states
            string EmoStateToAnim(EmoEnum emo)
            {
                switch (emo)
                {
                    case EmoEnum.NEUTRAL:
                        return string.Empty;
                    case EmoEnum.HAPPY:
                        return "isHappy";
                    case EmoEnum.SAD:
                        return "isSad";
                    case EmoEnum.MAD:
                        return string.Empty;
                    case EmoEnum.ENERGETIC:
                        return "isEnergetic";
                }
                return string.Empty;
            }
            if (anim.GetBool("isTalking") != AudioManager.IsObjSpeaking())
            {
                anim.SetBool("isTalking", AudioManager.IsObjSpeaking());
                if (AudioManager.IsObjSpeaking())
                {
                    talkingEmotion = EmoStateToAnim(AudioManager.EmoState());
                    anim.SetBool(talkingEmotion, true);
                }
                else
                {
                    anim.SetBool(talkingEmotion, false);
                }
            }
        }

        if (isStateChanged)
        {
            if (State == PlacementState.Finger)
            {
                laugh.Play();
            }
            else if (State == PlacementState.Held)
            {
                grab.Play();
            }
            else if (State == PlacementState.Placed)
            {
                release.Play();
            }
            isStateChanged = false;
        }
    }

    private Vector3 GetFingerPosition()
    {
        OVRPlugin.BoneId FingerToBoneId(finger finger)
        {
            switch (finger)
            {
                case finger.Thumb: return OVRPlugin.BoneId.Hand_Thumb3;
                case finger.Index: return OVRPlugin.BoneId.Hand_Index3;
                case finger.Middle: return OVRPlugin.BoneId.Hand_Middle3;
                case finger.Ring: return OVRPlugin.BoneId.Hand_Ring3;
                case finger.Little: return OVRPlugin.BoneId.Hand_Pinky3;
            }
            return OVRPlugin.BoneId.Invalid;
        }
        return skeleton.Bones[(int)FingerToBoneId(CurrFinger)].Transform.position;
    }

    public void ObjectToggleGrab()
    {
        if (State == PlacementState.Finger)
        {

        }
        else if (State == PlacementState.Placed)
        {
            SpatialAnchor.Erase();
            Destroy(SpatialAnchor);
        }

        State = PlacementState.Held;
        isStateChanged = true;
    }

    public void ObjectRelease()
    {
        // Place back on finger
        if (Vector3.Distance(GetFingerPosition(), transform.position) < FINGER_DISTANCE_THRESHOLD)
        {
            State = PlacementState.Finger;
            isStateChanged = true;
            anim.SetTrigger("shrink");
        }
        // Place in the environment
        else
        {
            if (SpatialAnchor == null)
            {
                SpatialAnchor = gameObject.AddComponent<OVRSpatialAnchor>();
                Save();
            }
            State = PlacementState.Placed;
            isStateChanged = true;
        }
    }

    void Save()
    {
        IEnumerator SaveNextFrame()
        {
            yield return null;
            if (SpatialAnchor != null) SpatialAnchor.Save();
        }

        StartCoroutine(SaveNextFrame());
    }
	
	public void animateObject()
    {
        gameObject.transform.localScale = new Vector3(2, 2, 2);
    }

    public void changeFace()
    {
        anim.SetBool("isGrowing", false);
    }


}
