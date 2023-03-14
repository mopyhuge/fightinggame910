using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScript : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderValue;
    // Start is called before the first frame update
    void Start()
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("roundTimer", 100));
        sliderValue.text = "100";

        slider.onValueChanged.AddListener((value) =>
        {
            sliderValue.text = value.ToString();
            PlayerPrefs.SetFloat("roundTimer", value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
