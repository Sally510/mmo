using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    private Canvas _canvas;

    void Start()
    {
        _canvas = GetComponent<Canvas>();    
    }

    public void SetVisiblity(bool visiblity)
    {
        _canvas.enabled = visiblity;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
