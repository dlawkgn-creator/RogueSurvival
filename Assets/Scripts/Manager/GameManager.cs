using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private Slider foodSlider;
    [SerializeField] private int startFood = 100;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject titlePanel;

    [Header("GameOver UI")]
    [SerializeField] private TMP_Text gameOverText;
    
    private enum GameState
    {
        Title, Playing, GameOver
    }
    private GameState m_State;

    public TurnManager TurnManager { get; set; }
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    
    private int m_FoodAmount = 0;
    private int m_CurrentLevel = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        ShowTitle();
    }

    public void ShowTitle()
    {
        m_State = GameState.Title;
        Time.timeScale = 1f;

        if (titlePanel) titlePanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void ShowPlaying()
    {
        m_State = GameState.Playing;
        Time.timeScale = 1f;

        if (titlePanel) titlePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void ShowGameOver(string msg)
    {
        m_State = GameState.GameOver;
        Time.timeScale = 0f;

        if (gameOverText) gameOverText.text = msg;
        if (titlePanel) titlePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void OnClickStart()
    {
        SoundManager.Instance?.PlayUiClick();
        ShowPlaying();
        StartNewGame();
    }

    public void OnClickQuit()
    {
        SoundManager.Instance?.PlayUiClick();
        Application.Quit();
    }

    public void OnClickRestart()
    {
        SoundManager.Instance?.PlayUiClick();
        ShowPlaying();
        StartNewGame();
    }

    public void OnClickBackToTitle()
    {
        SoundManager.Instance?.PlayUiClick();
        BoardManager.Clean();
        ShowTitle();
    }

    public void StartNewGame() // 새 게임 시작
    {
        m_CurrentLevel = 1;
        m_FoodAmount = 40;

        if(foodSlider != null)
        {
            foodSlider.minValue = 0;
            foodSlider.maxValue = startFood;
            foodSlider.value = m_FoodAmount;
        }

        BoardManager.Clean();
        BoardManager.Init();

        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }

    void OnTurnHappen()
    {
        if(m_State != GameState.Playing)
        {
            return;
        }
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        if(m_State != GameState.Playing)
        {
            return;
        }

        if(PlayerController == null)
        {
            return;
        }
        
        m_FoodAmount += amount;
        if(m_FoodAmount < 0)
        {
            m_FoodAmount = 0;
        }
        
        if(foodSlider != null)
        {
            foodSlider.value = m_FoodAmount;
        }
        
        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            ShowGameOver($"Game Over!\n\nSurvived {m_CurrentLevel} days");
        }
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        m_CurrentLevel++;
    }

    void HideGameOver()
    {
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}