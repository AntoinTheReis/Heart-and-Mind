using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBinder : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider mySlider;

    void Start()
    {
        GameObject obj = GameObject.FindWithTag("AudioManager");
        if (obj != null)
        {
            //mySlider.onValueChanged.AddListener(obj.Instance.OnSliderValueChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
