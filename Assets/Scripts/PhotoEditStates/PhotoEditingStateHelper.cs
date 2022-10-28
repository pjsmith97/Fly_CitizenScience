using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PhotoEditingStateHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("Fly Photo")]
    public Image flyPhoto;
    public GameObject loadingUI;
    public PhotoManager photoManager;

    [Header("Sliders")]
    private float leftSlideSpeed;
    private float rightSlideSpeed;
    public float maxSlideSpeed;
    public float slideMax;
    public float slideMin;
    public Slider leftSlider;
    public Slider rightSlider;

    [Header("Rotate slider")]
    public Image rotationGauge;

    [Header("Editing UI")]
    public GameObject editingUI;

    [Header("Completion")]
    public bool done = false; // Set to true when player presses the "Finish" button to submit their result

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        photoManager = GetComponent<PhotoManager>();

        // Set Slider Values, Max, and Min
        leftSlider.value = 0;
        leftSlider.maxValue = slideMax;
        leftSlider.minValue = slideMin;

        rightSlider.value = 0;
        rightSlider.maxValue = slideMax;
        rightSlider.minValue = slideMin;

        rotationGauge.fillAmount = 0;
    }

    // Update is called once per frame
    public void HelperUpdate()
    {
        /*if (player.GetButtonDown("RotateRight"))
        {
            if (rotationGauge.fillAmount == 0)
            {
                rotationGauge.fillAmount = 1f;
            }

            rotationGauge.fillAmount -= 0.25f;

            flyPhoto.transform.rotation *= Quaternion.Euler(0, 0, 360 * -0.25f);

        }

        if (player.GetButtonDown("RotateLeft"))
        {
            rotationGauge.fillAmount += 0.25f;

            if (rotationGauge.fillAmount >= 1)
            {
                rotationGauge.fillAmount -= 1;
            }

            flyPhoto.transform.rotation *= Quaternion.Euler(0, 0, 360 * 0.25f);
        }*/

        if (!photoManager.loadingPhoto)
        {
            if (player.GetButtonDown("Finish"))
            {
                //var sendTask = photoManager.SendSpiPollInfo("male", 1000);
                done = true;
            }
        }
    }

    public void HelperFixedUpdate()
    {

        if (!photoManager.loadingPhoto)
        {
            float Del = Time.deltaTime;
            float leftStickVal = player.GetAxis("LeftSlider");
            float rightStickVal = player.GetAxis("RightSlider");

            // Calculate analog stick input and apply it to the sliders
            if (leftStickVal > 0)
            {
                leftSlideSpeed = Mathf.Lerp(0, maxSlideSpeed, leftStickVal);
                leftSlider.value += leftSlideSpeed * Del;
                if (leftSlider.value > slideMax)
                {
                    leftSlider.value = slideMax;
                }
            }
            else
            {
                leftSlideSpeed = Mathf.Lerp(0, -maxSlideSpeed, Mathf.Abs(leftStickVal));
                leftSlider.value += leftSlideSpeed * Del;
                if (leftSlider.value < slideMin)
                {
                    leftSlider.value = slideMin;
                }
            }

            if (rightStickVal > 0)
            {
                rightSlideSpeed = Mathf.Lerp(0, maxSlideSpeed, rightStickVal);
                rightSlider.value += rightSlideSpeed * Del;
                if (rightSlider.value > slideMax)
                {
                    rightSlider.value = slideMax;
                }
            }
            else
            {
                rightSlideSpeed = Mathf.Lerp(0, -maxSlideSpeed, Mathf.Abs(rightStickVal));
                rightSlider.value += rightSlideSpeed * Del;
                if (rightSlider.value < slideMin)
                {
                    rightSlider.value = slideMin;
                }
            }

            // Check for rotator inputs and apply them to progression bar
            if (player.GetButton("RotateRight"))
            {
                if (rotationGauge.fillAmount == 0)
                {
                    rotationGauge.fillAmount = 1f;
                }

                rotationGauge.fillAmount -= maxSlideSpeed * Del;
            }

            if (player.GetButton("RotateLeft"))
            {
                rotationGauge.fillAmount += maxSlideSpeed * Del;

                if (rotationGauge.fillAmount >= 1)
                {
                    rotationGauge.fillAmount -= 1;
                }
            }
        }
    } 
}
