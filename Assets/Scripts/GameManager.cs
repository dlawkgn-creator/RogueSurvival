using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 게임 전체를 총괄하는 매니저 (Singleton)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }

    public BoardManager BoardManager;
    public PlayerController PlayerController;

    public TurnManager TurnManager { get; private set; }
    private int m_FoodAmount = 100;
    public int Food = 100;

    public UIDocument UIDoc;
    private Label m_FoodLabel;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = $"Food : {m_FoodAmount}";

        // 시작할 때 새로운 TurnManager를 생성
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        BoardManager.Init();

        // BoardManager와 2d x축 y축을 매개로 PlayerController 의 Spawn메서드로 캐릭터를 생성
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }
    
    // 턴마다 음식 감소 처리
    private void OnTurnHappen()
    {
        m_FoodAmount -= 1;
        m_FoodLabel.text = $"Food : {m_FoodAmount}";
        Debug.Log($"현재 음식 수량 : {m_FoodAmount}");
    }

    public void ChangeFood(int amount)
    {
        Food += amount;
        Debug.Log($"Food : {Food}");

        if(Food <= 0)
        {
            Debug.Log("Game Over");
            // 게임 오버 처리 되어야함
        }
    }
}
