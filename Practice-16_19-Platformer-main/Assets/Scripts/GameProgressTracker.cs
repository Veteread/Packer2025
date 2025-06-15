using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameProgressTracker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Collider2D playArea;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject finishButton;
    [SerializeField][Range(0, 1)] private float completionThreshold = 0.7f;
    [SerializeField] private LayerMask blockLayer;

    [Header("Save Settings")]
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float saveInterval = 5f;

    private float lastSaveTime;
    private float currentProgress;

    private void Start()
    {
        LoadProgress();
        UpdateProgressUI(currentProgress);
    }

    private void Update()
    {
        if (Time.time - lastSaveTime >= saveInterval && autoSave)
        {
            SaveProgress();
            lastSaveTime = Time.time;
        }
    }

    public void CheckCompletion()
    {
        if (!ValidateComponents()) return;
        
        currentProgress = CalculateFilledPercentage();
        UpdateProgressUI(currentProgress);

        if (currentProgress >= completionThreshold)
        {
            OnCompletionReached();
        }
    }

    private bool ValidateComponents()
    {
        if (playArea == null)
        {
            Debug.LogError("PlayArea not assigned!");
            return false;
        }
        return true;
    }

    private float CalculateFilledPercentage()
    {
        Bounds bounds = playArea.bounds;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(bounds.min, bounds.max, blockLayer);
        
        float totalArea = CalculateColliderArea(playArea);
        float filledArea = 0f;

        foreach (var collider in colliders)
        {
            if (collider == playArea) continue;

            filledArea += CalculateColliderArea(collider);
        }

        return Mathf.Clamp01(filledArea / totalArea);
    }

    private float CalculateColliderArea(Collider2D collider)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            Vector2 scaledSize = boxCollider.size * new Vector2(
                collider.transform.lossyScale.x,
                collider.transform.lossyScale.y
            );
            return scaledSize.x * scaledSize.y;
        }
        else if (collider is PolygonCollider2D polyCollider)
        {
            return CalculatePolygonArea(polyCollider.points, collider.transform);
        }
        
        return 0f;
    }

    private float CalculatePolygonArea(Vector2[] points, Transform transform)
    {
        float area = 0f;
        int length = points.Length;

        Vector2[] transformedPoints = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            transformedPoints[i] = transform.TransformPoint(points[i]);
        }

        for (int i = 0; i < length; i++)
        {
            Vector2 current = transformedPoints[i];
            Vector2 next = transformedPoints[(i + 1) % length];
            area += (current.x * next.y) - (next.x * current.y);
        }

        return Mathf.Abs(area * 0.5f);
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

    private void OnCompletionReached()
    {
        Debug.Log("Level Completed!");
        SaveProgress();
    }

    #region Save/Load Integration
    [System.Serializable]
    private class ProgressData
    {
        public float progress;
    }

    public void SaveProgress()
    {
        var data = new ProgressData { progress = currentProgress };
        SaveLoadManager.Save(data, "progress");
    }

    public void LoadProgress()
    {
        ProgressData data = SaveLoadManager.Load<ProgressData>("progress");
        currentProgress = data?.progress ?? 0f;
    }
    #endregion

    public void PrintSavedProgress()
    {
        ProgressData data = SaveLoadManager.Load<ProgressData>("progress");
        
        if(data != null)
        {
            Debug.Log($"Saved progress: {data.progress * 100}%");
        }
        else
        {
            Debug.Log("No saved data found");
        }
    }
}