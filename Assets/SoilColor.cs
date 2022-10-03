using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component from the new cube
        var thisRenderer = GetComponent<Renderer>();

        thisRenderer.material.SetColor("_Color", new Vector4(79f/255f, 47f/255f, 3f/255f, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
