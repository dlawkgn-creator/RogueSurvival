using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private Slider foodSlider;
    [SerializeField] private int startFood = 100;

    [Header("GameOver UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    

    public TurnManager TurnManager { get; set; }
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    
    private int m_FoodAmount = 0;
    //private Label m_FoodLabel;
    private int m_CurrentLevel = 1;

    //private VisualElement m_GameOverPanel;
    //private Label m_GameOverMessage;

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
        // Turn 시스템 초기화
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        // UI 참조
        //m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        //m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        //m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        SoundManager.Instance.Play();

        StartNewGame();
    }

    public void StartNewGame() // 새 게임 시작
    {
        //m_GameOverPanel.style.visibility = Visibility.Hidden;
        HideGameOver();

        m_CurrentLevel = 1;
        m_FoodAmount = 40;
        //m_FoodLabel.text = "Food : " + m_FoodAmount;

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
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        if(PlayerController == null)
        {
            return;
        }

        m_FoodAmount += amount;
        if(m_FoodAmount < 0)
        {
            m_FoodAmount = 0;
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

    void ShowGameOver(string msg)
    {
        if(gameOverText != null)
        {
            gameOverText.text = msg;
        }

        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void HideGameOver()
    {
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}