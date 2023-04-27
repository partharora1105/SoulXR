using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startAnim : MonoBehaviour
{
    private bool activated;
    private bool isTimeUp;
    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        isTimeUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleAnim()
    {
        if (isTimeUp)
        {
            activated = true;
            if (activated)
            {
                //gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                //gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
        }
    }

}
