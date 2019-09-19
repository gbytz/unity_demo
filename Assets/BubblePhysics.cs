using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePhysics : MonoBehaviour
{
    public GameObject detectedObject;


    // Start is called before the first frame update
    void Start()
    {

        var rigidBody = this.GetComponent<Rigidbody>();
        rigidBody.AddForce(new Vector3(3.0f, 3.0f, -3.0f));
        Debug.Log("Apply force");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(GameObject detectedObject)
    {
        this.detectedObject = detectedObject;

        transform.parent = detectedObject.transform;

        
        //initial position
        //initial velocity
        
    }
}
