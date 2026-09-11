using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action OnWin;
    public event Action OnGameOver;
    public event Action OnPause;
    public event Action<float> OnUpdateTime;
    public event Action OnCloseMenu;

    [Header("Settings")]
    [SerializeField] private float surviveTime;
    [SerializeField] private int currentLevel;

    public bool IsPlaying { get; private set; }
    private float _countdown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        Time.timeScale = 1;
        IsPlaying = true;
        _countdown = surviveTime;
    }

    private void ChangeGameState(bool play)
    {
        if (play)
        {
            Time.timeScale = 1;
            IsPlaying = true;
        }
        else
        {
            Time.timeScale = 0;
            IsPlaying = false;
        }
    }

    private void Update()
    {
        if (surviveTime == -1)
        {
            return;
        }

        if (_countdown <= 0 && IsPlaying)
        {
            Win();
            return;
        }

        UpdateCountdowntime();
    }

    private void UpdateCountdowntime()
    {
        if (IsPlaying)
            _countdown = Mathf.Max(_countdown - Time.deltaTime, 0);

        OnUpdateTime?.Invoke(_countdown);
    }

    public void Win()
    {
        Debug.Log("Win");
        OnCloseMenu?.Invoke();
        OnWin?.Invoke();
        ChangeGameState(false);
    }

    public void NextLevel()
    {
        ++currentLevel;
        LoadLevelScene(currentLevel);
    }

    public void LoadLevelScene(int Scene)
    {
        SceneManager.LoadScene($"Level{Scene}Scene");
        Setup();
    }

    public void Gameover()
    {
        ChangeGameState(false);
        OnCloseMenu?.Invoke();
        OnGameOver?.Invoke();
    }

    public void Pause()
    {
        OnCloseMenu?.Invoke();
        OnPause?.Invoke();
        ChangeGameState(false);
    }

    public void Retry()
    {
        OnCloseMenu?.Invoke();
        ChangeGameState(true);
        SceneManager.LoadScene(currentLevel);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Return()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Resume()
    {
        OnCloseMenu?.Invoke();
        ChangeGameState(true);
    }

    public bool IsInCameraView(Camera cam, Transform target)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);

        bool isInView = viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;

        return isInView;
    }
}
