using UnityEngine;

public class Enemy : CellObject
{
    public int Health = 3;
    public int CounterDamageToPlayer = 3; // 플레이어가 공격하면 같이 받는 반격 데미지

    private int m_CurrentHealth;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
        {
            GameManager.Instance.TurnManager.OnTick += TurnHappened;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
        {
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
        }
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;
    }

    // 플레이어가 "공격"했을 때 적이 처리할 것 (HP 감소 / 죽으면 보드에서 제거)
    public override void OnPlayerAttack()
    {
        m_CurrentHealth -= 1;

        if (m_CurrentHealth <= 0)
        {
            ClearSelfFromBoard();
            Destroy(gameObject);
        }
    }

    // 플레이어가 공격할 때 같이 받는 반격 데미지
    public override int GetCounterDamageToPlayer()
    {
        return CounterDamageToPlayer;
    }

    private void ClearSelfFromBoard()
    {
        var board = GameManager.Instance != null ? GameManager.Instance.BoardManager : null;
        if (board == null) return;

        var cell = board.GetCellData(m_Cell);
        if (cell != null && cell.ContainedObject == this)
            cell.ContainedObject = null;
    }

    // 기존: 플레이어가 적 칸에 "들어가려는" 건 허용하지 않음 (제자리 전투 룰 유지)
    public override bool PlayerWantsToEnter()
    {
        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    void TurnHappened()
    {
        var playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        // 인접하면 플레이어 데미지(적 턴 공격)
        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            GameManager.Instance.ChangeFood(-CounterDamageToPlayer);

            // 플레이어 피격 모션
            var pc = GameManager.Instance.PlayerController;
            if (pc != null) pc.PlayHit();

            return;
        }

        // 추적 이동
        if (absXDist > absYDist)
        {
            if (!TryMoveInX(xDist))
                TryMoveInY(yDist);
        }
        else
        {
            if (!TryMoveInY(yDist))
                TryMoveInX(xDist);
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0) return MoveTo(m_Cell + Vector2Int.right);
        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0) return MoveTo(m_Cell + Vector2Int.up);
        return MoveTo(m_Cell + Vector2Int.down);
    }
}
