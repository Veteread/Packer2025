using UnityEngine.UI;
using UnityEngine;

public class MobileUIScaler : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;

    private void Start()
    {
        // Автоматическая адаптация под разрешение
        float aspect = (float)Screen.width / Screen.height;

        if (aspect < 0.5f) // Портрет
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
        else // Ландшафт
        {
            canvasScaler.matchWidthOrHeight = 1;
        }

        // Увеличение размеров UI для сенсорного ввода
        foreach (Button btn in FindObjectsOfType<Button>())
        {
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta *= 1.5f;
        }
    }
}