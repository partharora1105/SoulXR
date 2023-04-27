using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{

    public bool IsHovered { get; set; }
    private bool isBuffered;

    [SerializeField] private UnityEvent<GameObject> OnObjectHover;

    [SerializeField] private Material OnHoverActiveMaterial;
    [SerializeField] private Material OnHoverInactiveMaterial;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject ASLR;

    private MeshRenderer meshRenderer;

    public static stickToHand CurrentFocus { get; private set; }

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        isBuffered = false;
    }

    void Update()
    {
        if (IsHovered && !isBuffered)
        {
            anim.SetBool("isLookedAt", true);
            isBuffered = true;
            Invoke("stopAnim", 2.0f);
            OnObjectHover?.Invoke(gameObject);
            var stickToHand = transform.parent.GetComponentInChildren<stickToHand>();
            if (stickToHand.State == stickToHand.PlacementState.Placed)
            {
                var anyTalking = PersonaManager.Instance.Personas.Any(persona => persona.AudioManager.IsObjSpeaking());
                if (!anyTalking) ASLR.GetComponent<AudioManager>().ActivateMic();
            }
            CurrentFocus = stickToHand;
        }
    }

    void stopAnim()
    {
        anim.SetBool("isLookedAt", false);
        isBuffered = false;
    }
}
