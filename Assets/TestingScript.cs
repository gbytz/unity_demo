using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    
    public GameObject detectedObject;
    public GameObject Bubble;

    private GameObject myBubble;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pressed()
    {
        //this.boundingBox.transform.rotation = Quaternion.Euler(0.0f, (float)(detectedObject.Orientation * 180.0f / Math.PI), 0.0f);

        myBubble = Instantiate(Bubble);
        myBubble.GetComponent<BubblePhysics>().Setup(detectedObject);

        myBubble.transform.position = detectedObject.transform.position + new Vector3(0f, detectedObject.transform.localScale.y / 2.0f, 0f);
    }
}
