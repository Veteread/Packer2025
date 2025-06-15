
using UnityEngine;

public class Vibro : MonoBehaviour
{
    [SerializeField] private GameObject basket;
    [SerializeField] private float rotate = 5;
    
    public void VibroBasket()
    {
        basket.transform.localRotation = Quaternion.Euler(0f, 0f, rotate);
        Invoke("ReturnBasket", 0.5f);
    }

    private void ReturnBasket()
    {
    	basket.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
