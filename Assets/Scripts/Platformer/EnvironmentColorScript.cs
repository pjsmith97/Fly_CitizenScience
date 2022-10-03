using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentColorScript : MonoBehaviour
{
    [SerializeField] bool identify;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component from the new cube
        var thisRenderer = GetComponent<Renderer>();

        // Create a new RGBA color using the Color constructor and store it in a variable
        //Color customColor = new Color(0.4f, 0.9f, 0.7f, 1.0f);

        // Call SetColor using the shader property name "_Color" and setting the color
        int thisLayer = gameObject.layer;

        if (identify)
        {
            if (thisLayer == LayerMask.NameToLayer("WallRunning"))
            {
                thisRenderer.material.SetColor("_Color", new Vector4(1, 0.5f, 0.5f, 1));
            }

            else if (thisLayer == LayerMask.NameToLayer("Ledge"))
            {
                thisRenderer.material.SetColor("_Color", new Vector4(0.5f, 1, 0.5f, 1));
            }

            else if (thisLayer == LayerMask.NameToLayer("Bouncy"))
            {
                thisRenderer.material.SetColor("_Color", Color.blue);
            }
        }
    }

}
