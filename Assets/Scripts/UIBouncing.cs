using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBouncing : MonoBehaviour
{
    private float startingPosX;
    //private float bouncingTimer;
    public bool closingIn;
    [SerializeField] float bounceSpeed;
    [SerializeField] float bouncelimit;


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

        if (closingIn)
        {
            change = bounceSpeed * Del;

            if(this.transform.position.x + change > startingPosX + bouncelimit)
            {
                change = -change;
                closingIn = false;
            }
        }

        else
        {
            change = -(bounceSpeed * Del);

            if (this.transform.position.x + change < startingPosX - bouncelimit)
            {
                change = -change;
                closingIn = true;
            }
        }

        Vector3 newPos = new Vector3(this.transform.position.x + change, this.transform.position.y, this.transform.position.z);
        transform.position = newPos;
    }
}
