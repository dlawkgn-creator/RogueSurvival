using UnityEngine;

public class Enemy : CellObject
{
    [Header("Status")]
    [SerializeField][Range(1,10)] private int Health = 3; // 최대 체력

    private int m_CurrentHealth; // 현재 체력

    private void Awake()
    {
        // 턴 시스템의 Tick 이벤트에 등록
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDestroy()
    {
        // 오브젝트 제거 시 이벤트 해제
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord) // 부모 클래스 초기화
    {
        base.Init(coord);
        m_CurrentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1; // 플레이어가 적이 있는 칸으로 이도하려 할때 체력 감소

        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }

        return false; // 적이 있는 칸으로 이동 불가
    }

    private bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        // 이동 불가능한 경우
        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;
        }

        // 현재 셀에서 적 제거
        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        // 목표 셀에 적 배치
        targetCell.ContainedObject = this;
        m_Cell = coord;

        // 월드 좌표 이동
        transform.position = board.CellToWorld(coord);

        return true;
    }

    private void TurnHappened() // 적 AI 패턴
    {
        // 플레이어가 현재 위치한 셀 좌표
        var playerCell = GameManager.Instance.PlayerController.Cell;

        // 플레이어와의 거리 계산
        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        // 플레이어와 상하좌우로 인접해 있으면 공격
        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            // 플레이어의 자원(음식) 감소
            GameManager.Instance.ChangeFood(3);
        }
        else
        {
            // 플레이어와의 거리가 더 먼 축을 우선으로 이동
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    // X 방향 이동 실패 시 Y 방향 시도
                    TryMoveInY(yDist);
                }
            }
            else
            {
                // Y 방향 이동 실패 시 X 방향 시도
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        // 플레이어가 오른쪽에 있는 경우
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        // 플레이어가 왼쪽에 있는 경우
        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        // 플레이어가 위에 있는 경우
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        // 플레이어가 아래에 있는 경우
        return MoveTo(m_Cell + Vector2Int.down);
    }
}
