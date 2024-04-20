using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Slider))]
public class SpeedSlider : MonoBehaviour
{
    Slider slider;
    const string label = "<#e0c067>Ball Speed:</color> ";

    [SerializeField]
    TMPro.TextMeshProUGUI labelText;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = Mathf.RoundToInt(GameProperties.BallSpeed * 100);
        slider.onValueChanged.AddListener(OnSpeedChanged);
    }

    void OnSpeedChanged(float inValue)
    {
        labelText.text = label + inValue.ToString();
        GameProperties.BallSpeed = ((float)inValue / 100F);
        Ball.RecalculateBallVelocity?.Invoke();
    }
}
