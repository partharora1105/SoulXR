using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonaManager : MonoBehaviour
{
    public static PersonaManager Instance;

    public stickToHand[] Personas;

    void Awake()
    {
        Instance = this;
    }
}
