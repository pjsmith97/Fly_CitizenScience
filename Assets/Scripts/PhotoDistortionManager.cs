using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class PhotoDistortionManager : MonoBehaviour
{
    [SerializeField] private Slider leftSlider;
    [SerializeField] private Slider rightSlider;
    [SerializeField] private Image flyPhoto;
    public float leftBalancer;
    public float rightBalancer;

    private void Start()
    {
        // photoRend = flyPhoto.GetComponent<Renderer>();
        CreateBalancers();

    }

    private void Update()
    {
        flyPhoto.material.SetFloat("_Magnitude", Mathf.Abs(leftSlider.value - leftBalancer) * 0.1f);
        flyPhoto.material.SetFloat("_Magnitude2", Mathf.Abs(rightSlider.value - rightBalancer) * 0.1f);
    }

    public void CreateBalancers()
    {
        leftBalancer = Random.Range(1, 100) * 0.01f;
        rightBalancer = Random.Range(1, 100) * 0.01f;


    }
}
