using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    [SerializeField] private Button buttonRestart;
    [SerializeField] private Button buttonRestartEnd;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private TextMeshProUGUI endGameText;
    private void Start()
    {
        buttonRestart.onClick.AddListener(OnCLickRestart);
        buttonRestartEnd.onClick.AddListener(OnCLickRestart);
        gameManager.OnLose += OnLoseGame;
        gameManager.OnWin += OnWinGame;

        Init();
    }

    private void Init()
    {
        endGameUI.SetActive(false);
    }

    private void OnLoseGame()
    {
        endGameText.text = "You Lose!";
        endGameUI.SetActive(true);
    }
    
    private void OnWinGame()
    {
        endGameText.text = "You Win!";
        endGameUI.SetActive(true);
    }
    
    
    private void OnCLickRestart()
    {
        gameManager.InitializeGame();
        endGameUI.SetActive(false);
    }

    private void OnDestroy()
    {
        buttonRestart.onClick.RemoveListener(OnCLickRestart);
        buttonRestartEnd.onClick.RemoveListener(OnCLickRestart);
        
        gameManager.OnLose -= OnLoseGame;
        gameManager.OnWin -= OnWinGame;
    }
}
