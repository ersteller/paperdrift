using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugsprite : MonoBehaviour
{

    public Transform target;
    public Transform debug;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion tempquatrotate = Quaternion.Euler(0, 0, target.rotation.eulerAngles.z); // euler gets degrees and setts degrees
        debug.rotation = tempquatrotate;
    }
}
