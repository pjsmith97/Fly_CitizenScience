using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
*    Title: UIBouncing
*    Author: Philip Smith
*    Date: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Description: Class that governs arrow bouncing during Classification Tutorial slideshow
*
***************************************************************************************/

public class UIBouncing : MonoBehaviour
{
    private float startingPosX; // the starting x coordinate of the object
    public bool closingIn; //is the object moving towards arrow direction
    [SerializeField] float bounceSpeed; // speed that the object bounces
    [SerializeField] float bouncelimit; // distance the object moves before changing direction


    // Start is called before the first frame update
    void Start()
    {
        startingPosX = this.transform.position.x;

        closingIn = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float Del = Time.deltaTime;

        float change;

        // If arrow is closing in, set change to move in positive x axis. 
        if (closingIn)
        {
            change = bounceSpeed * Del;

            // If it reaches the limit, change direction
            if (this.transform.position.x + change > startingPosX + bouncelimit)
            {
                change = -change;
                closingIn = false;
            }
        }

        // If not closing in, move in negative x axis
        else
        {
            change = -(bounceSpeed * Del);

            // If it reaches the limit, change direction
            if (this.transform.position.x + change < startingPosX - bouncelimit)
            {
                change = -change;
                closingIn = true;
            }
        }

        // Set object's new position
        Vector3 newPos = new Vector3(this.transform.position.x + change, this.transform.position.y, this.transform.position.z);
        transform.position = newPos;
    }
}
