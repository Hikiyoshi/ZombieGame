using UnityEngine;
using UnityEngine.UI;

public class GameoverUI : MonoBehaviour
{
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button returnBtn;
    [SerializeField] private Button quitBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        retryBtn.onClick.AddListener(GameManager.Instance.Retry);
        returnBtn.onClick.AddListener(GameManager.Instance.Return);
        quitBtn.onClick.AddListener(GameManager.Instance.Quit);

        GameManager.Instance.OnCloseMenu += GameManager_OncloseMenu;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;

        Hide();
    }

    private void GameManager_OnGameOver()
    {
        Show();
    }

    private void GameManager_OncloseMenu()
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
