using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderBar : MonoBehaviour
{
    public Color sliderColor = Color.white;

    public int minValue = 0, maxValue = 100, currentValue = 50;

    public string title = "Slider Bar";

    public bool showText = true, showTitle = true, showAsFraction = true;

    public Image slider;
    public TextMeshProUGUI sliderText;

    void Start()
    {
        slider.color = sliderColor;
    }

    void Update()
    {
        slider.fillAmount = Slider();

        if (showText)
        {
            sliderText.SetText(SliderText());
        }
        else
        {
            sliderText.SetText("");
        }
    }

    float Slider()
    {
        float sliderValue = 0;

        int max = maxValue - minValue;
        int current = currentValue - minValue;

        sliderValue = (float)current / (float)max;

        return sliderValue;
    }

    string SliderText()
    {
        string text = "";

        if (showTitle)
            text += title + ": ";

        text += currentValue.ToString();

        if (showAsFraction)
            text += "/" + maxValue.ToString();

        return text;
    }

    public void SetSliderValues(string slTitle, int min, int max, int current, bool sText, bool sTitle, bool sFraction)
    {
        title = slTitle;
        minValue = min;
        maxValue = max;
        currentValue = current;
        showText = sText;
        showTitle = sTitle;
        showAsFraction = sFraction;
    }

    public void SetSliderValues(string slTitle, int min, int max, int current)
    {
        title = slTitle;
        minValue = min;
        maxValue = max;
        currentValue = current;
    }

    public void SetSliderValues(int min, int max, int current)
    {
        minValue = min;
        maxValue = max;
        currentValue = current;
    }
}
