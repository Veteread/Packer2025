using UnityEngine.UI;
using UnityEngine;

public class MobileUIScaler : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;

    private void Start()
    {
        // �������������� ��������� ��� ����������
        float aspect = (float)Screen.width / Screen.height;

        if (aspect < 0.5f) // �������
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
        else // ��������
        {
            canvasScaler.matchWidthOrHeight = 1;
        }

        // ���������� �������� UI ��� ���������� �����
        foreach (Button btn in FindObjectsOfType<Button>())
        {
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta *= 1.5f;
        }
    }
}