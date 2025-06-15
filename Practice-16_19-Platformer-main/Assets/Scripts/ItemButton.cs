using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void Initialize(Sprite icon, UnityEngine.Events.UnityAction onClick)
    {
    	iconImage.sprite = icon;
    	GetComponent<Button>().onClick.AddListener(onClick);
    }
}
