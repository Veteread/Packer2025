using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button checkButton;

    private void Start()
    {
        checkButton.onClick.AddListener(OnCheckButtonClicked);
    }

    public async UniTask CheckLevelCompletionAsync()
    {
        try
        {
            await UniTask.Delay(500);
            var isComplete = EvaluateLevel();
            Debug.Log($"������� ��������: {isComplete}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ ��������: {e}");
        }
    }

    private void OnCheckButtonClicked()
    {
        CheckLevelCompletionAsync()
            .AttachExternalCancellation(this.GetCancellationTokenOnDestroy())
            .Forget();
    }

    private bool EvaluateLevel() => /* ���� ������ */ true;
}