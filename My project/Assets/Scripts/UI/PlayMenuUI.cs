using UnityEngine;
using UnityEngine.UI;

public class PlayMenuUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;

    private void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevelScene(1);
        });

        quitBtn.onClick.AddListener(GameManager.Instance.Quit);
    }
}
