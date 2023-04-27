using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] string UserDisplayName = "Human";

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUserDisplayName(string val)
    {
        Debug.Log("User name is updated to :: " + val);
        UserDisplayName = val;
    }

    public string GetUserDisplayName()
    {
        return UserDisplayName;
    }
}
