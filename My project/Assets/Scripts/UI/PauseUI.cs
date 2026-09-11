using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button quitBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resumeBtn.onClick.AddListener(GameManager.Instance.Resume);
        quitBtn.onClick.AddListener(GameManager.Instance.Quit);

        GameManager.Instance.OnCloseMenu += GameManager_OncloseMenu;
        GameManager.Instance.OnPause += GameManager_OnPause;

        Hide();
    }

    private void GameManager_OnPause()
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
