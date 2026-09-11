using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TopDown3DController playerController;
    [SerializeField] private TextMeshProUGUI bombTMPUI;
    [SerializeField] private Button pauseBtn;

    private void Start()
    {
        // playerController.PlantBombEvent += UpdateBombButton;

        pauseBtn.onClick.AddListener(GameManager.Instance.Pause);
    }

    private void UpdateBombButton(int amount)
    {
        bombTMPUI.text = amount.ToString();
    }
}
