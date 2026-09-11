using TMPro;
using UnityEngine;

public class DisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeTMP;

    private void Start()
    {
        GameManager.Instance.OnUpdateTime += GameManager_OnUpdateTime;
    }

    private void GameManager_OnUpdateTime(float time)
    {
        int minute = (int)time / 60;
        int second = (int)time % 60;
        string timeText = $"{minute:D2}:{second:D2}";
        timeTMP.text = timeText;
    }
}
