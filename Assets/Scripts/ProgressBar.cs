using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    private Canvas _canvas;

    void Start()
    {
        _canvas = GetComponent<Canvas>();    
    }

    public void SetVisibility(bool visibility)
    {
        _canvas.enabled = visibility;
    }

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetValue(int value)
    {
        slider.value = value;
    }
}
