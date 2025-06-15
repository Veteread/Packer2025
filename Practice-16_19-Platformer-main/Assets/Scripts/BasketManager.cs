using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketManager : MonoBehaviour
{
    // ... существующий код
    public float FillPercentage;
    public float rotationSpeed;
    
    public void SetRotationSpeed(float speed)
    {
        // Применяем скорость вращения для блоков
        rotationSpeed = speed;
    }
    
    public float GetFillPercentage()
    {
    	FillPercentage = 10;
    	return 10f;
        // Ваша реализация вычисления заполнения корзины
        //return CalculateFill();
    }
    
    // ... остальные методы
}
