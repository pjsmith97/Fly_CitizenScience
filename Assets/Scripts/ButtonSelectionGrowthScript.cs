using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelectionGrowthScript : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float sizeMultiplier;
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was selected");
        this.transform.localScale = new Vector3(this.transform.localScale.x * sizeMultiplier, this.transform.localScale.y * sizeMultiplier, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was deselected");
        this.transform.localScale = new Vector3(this.transform.localScale.x / sizeMultiplier, this.transform.localScale.y / sizeMultiplier, 0);
    }
}
