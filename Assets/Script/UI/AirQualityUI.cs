using UnityEngine;
using TMPro;

public class AirQualityUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI airQualityText;

    void Start()
    {
        if (AirQualityManager.Instance != null)
        {
            // 订阅空气质量等级变化事件
            AirQualityManager.Instance.OnAirQualityChanged += UpdateAirQualityText;
        }
    }

    private void OnDestroy()
    {
        if (AirQualityManager.Instance != null)
        {
            AirQualityManager.Instance.OnAirQualityChanged -= UpdateAirQualityText;
        }
    }

    private void UpdateAirQualityText(AirQualityLevel newLevel)
    {
        // 根据不同的等级，显示不同的文本和颜色
        switch (newLevel)
        {
            case AirQualityLevel.Excellent:
                airQualityText.text = "Air Quality: Excellent";
                airQualityText.color = Color.green;
                break;
            case AirQualityLevel.Good:
                airQualityText.text = "Air Quality: Good";
                airQualityText.color = Color.yellow;
                break;
            case AirQualityLevel.Moderate:
                airQualityText.text = "Air Quality: Moderate";
                airQualityText.color = new Color(1.0f, 0.5f, 0.0f); // Orange
                break;
            case AirQualityLevel.Poor:
                airQualityText.text = "Air Quality: Poor";
                airQualityText.color = Color.red;
                break;
            case AirQualityLevel.Hazardous:
                airQualityText.text = "Air Quality: Hazardous";
                airQualityText.color = new Color(0.5f, 0, 0.5f); // Purple
                break;
        }
    }
}