using UnityEngine;

public class SmoothOrbit : MonoBehaviour
{
    [SerializeField] private float targetOrbitRadius = 5f;
    [SerializeField] private float transitionSpeed = 1f;

    private float currentRadius;
    private OrbitController orbitController;

    private void Start()
    {
        orbitController = GetComponent<OrbitController>();
        currentRadius = orbitController.orbitRadius;
    }

    private void Update()
    {
        if (Mathf.Abs(currentRadius - targetOrbitRadius) > 0.1f)
        {
            currentRadius = Mathf.Lerp(
                currentRadius,
                targetOrbitRadius,
                Time.deltaTime * transitionSpeed
            );

            orbitController.orbitRadius = currentRadius;
        }
    }

    public void ChangeOrbit(float newRadius)
    {
        targetOrbitRadius = newRadius;
    }
}