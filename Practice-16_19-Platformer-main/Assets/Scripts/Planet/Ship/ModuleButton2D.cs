using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModuleButton2D : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;

    private ModuleData2D moduleData;
    private System.Action onClick;

    public void Initialize(ModuleData2D data, System.Action callback)
    {
        moduleData = data;
        onClick = callback;

        nameText.text = data.displayName;
        iconImage.sprite = data.sprite;
        iconImage.color = data.color;
    }

    public void OnClick()
    {
        onClick?.Invoke();
    }
}