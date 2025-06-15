using UnityEngine;
using UnityEngine.UI;

public class GameProgressTracker : MonoBehaviour
{
    [SerializeField] private Collider2D playArea;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject finishButton;
    [SerializeField][Range(0, 1)] private float completionThreshold = 0.7f;

    public void CheckCompletion()
    {
        if (playArea == null)
        {
            Debug.LogError("PlayArea not assigned!");
            return;
        }

        Bounds bounds = playArea.bounds;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(bounds.min, bounds.max);
        float totalArea = bounds.size.x * bounds.size.y;
        float filledArea = 0f;

        foreach (var collider in colliders)
        {
            if (collider != null && collider.CompareTag("Block"))
            {
                if (collider is BoxCollider2D box)
                {
                    filledArea += box.size.x * box.size.y;
                }
            }
        }

        float percentage = Mathf.Clamp01(filledArea / totalArea);
        UpdateProgressUI(percentage);
    }

    private void UpdateProgressUI(float percentage)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = percentage;
        }

        if (finishButton != null)
        {
            finishButton.SetActive(percentage >= completionThreshold);
        }
    }

    private float CalculateFilledPercentage()
    {
        Bounds bounds = playArea.bounds;
        float totalArea = bounds.size.x * bounds.size.y;
        float filledArea = 0f;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(bounds.min, bounds.max);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Block") && collider != playArea)
            {
                if (collider is BoxCollider2D box)
                {
                    filledArea += box.size.x * box.size.y;
                }
            }
        }

        return Mathf.Clamp01(filledArea / totalArea);
    }
}