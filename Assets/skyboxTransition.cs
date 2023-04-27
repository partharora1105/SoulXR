using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skyboxTransition : MonoBehaviour
{
    [SerializeField] private OVRSkeleton skeleton;
    [SerializeField] private OVRHand hand;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject obj;
    [SerializeField] private float distThreshold;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hand.IsTracked)
        {
            float dist = Vector3.Distance(skeleton.Bones[(int)OVRPlugin.BoneId.Hand_ThumbTip].Transform.position, cam.transform.position);
            if (dist > distThreshold)
            {
                GetComponent<Renderer>().enabled = false;  
            }
            else
            {
                GetComponent<Renderer>().enabled = true;
            } 
        }
        
    }
}
