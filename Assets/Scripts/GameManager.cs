using UnityEngine;

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

    private TurnManager m_TurnManager;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // 시작할 때 새로운 TurnManager를 생성
        m_TurnManager = new TurnManager();      

        BoardManager.Init();

        // BoardManager와 2d x축 y축을 매개로 PlayerController 의 Spawn메서드로 캐릭터를 생성 
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));     
    }
}
