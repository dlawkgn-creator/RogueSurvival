using UnityEngine;

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
        if(Instance != null)            //외부에서 들어온값이 공백이아니면 게임오브젝트를 파괴하고 리턴
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;        //공백이면 인스턴스가 게임매니저를 가리키도록 설정
    }

    void Start()
    {
        m_TurnManager = new TurnManager();      //시작할 때 새로운 턴매니저를 생성

        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));     //보드매니저와,2d x축y축을매개로 플레이어컨트룰러의스폰메소드로 캐릭터를 스폰(생성) 




    }

}
