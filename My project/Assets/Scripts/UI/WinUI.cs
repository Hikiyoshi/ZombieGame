using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button returnBtn;
    [SerializeField] private Button quitBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton.onClick.AddListener(GameManager.Instance.NextLevel);
        returnBtn.onClick.AddListener(GameManager.Instance.Return);
        quitBtn.onClick.AddListener(GameManager.Instance.Quit);

        GameManager.Instance.OnCloseMenu += GameManager_OncloseMenu;
        GameManager.Instance.OnWin += GameManager_OnWin;

        Hide();
    }

    private void GameManager_OnWin()
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
