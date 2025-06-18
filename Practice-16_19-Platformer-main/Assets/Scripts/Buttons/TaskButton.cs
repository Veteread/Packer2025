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
            Debug.Log($"Уровень завершен: {isComplete}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка проверки: {e}");
        }
    }

    private void OnCheckButtonClicked()
    {
        CheckLevelCompletionAsync()
            .AttachExternalCancellation(this.GetCancellationTokenOnDestroy())
            .Forget();
    }

    private bool EvaluateLevel() => /* ваша логика */ true;
}