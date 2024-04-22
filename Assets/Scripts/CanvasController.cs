using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject Camera;
    private float startingHeight;
    // Start is called before the first frame update
    void Start()
    {
        startingHeight = Camera.transform.position.y;

        // set the UI canvas to the same height as the camera
        transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // if press the space key, reset the height of the UI canvas
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startingHeight = Camera.transform.position.y;
            transform.position = new Vector3(transform.position.x, startingHeight, transform.position.z);
        }
    }
}
