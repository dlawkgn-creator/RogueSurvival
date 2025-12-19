using UnityEngine;

public class Enemy : CellObject
{
    [Header("Stats")]
    public int Health = 3;

    [Header("Damage")]
    public int CounterDamageToPlayer = 3;     // 플레이어가 공격할 때 즉시 받는 반격
    public int EnemyTurnDamageToPlayer = 3;   // 적 턴에서 인접 공격 데미지

    private int m_CurrentHealth;

    // 같은 턴에 "플레이어가 공격 + 반격"을 했으면
    // TurnHappened에서 인접 공격을 또 하지 않도록 중복 방지
    private bool m_SkipThisTurnAttack;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
            GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;
        m_SkipThisTurnAttack = false;
    }

    // 적 칸은 절대 들어갈 수 없음(제자리 전투)
    public override bool PlayerWantsToEnter()
    {
        return false;
    }

    // 플레이어가 적을 공격했을 때 적 HP 감소
    public override void OnPlayerAttack()
    {
        m_CurrentHealth -= 1;

        if (m_CurrentHealth <= 0)
        {
            ClearFromBoard();
            Destroy(gameObject);
            return;
        }

        // 이 턴에는 적 턴 인접 공격을 한 번 스킵(중복 방지)
        m_SkipThisTurnAttack = true;
    }

    // 반격 데미지
    public override int GetCounterDamageToPlayer()
    {
        return CounterDamageToPlayer;
    }

    private bool MoveTo(Vector2Int coord)
    {
        var gm = GameManager.Instance;
        if (gm == null) return false;

        // 플레이어 칸으로는 절대 이동하지 않음
        if (gm.PlayerController != null && coord == gm.PlayerController.Cell)
            return false;

        var board = gm.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
            return false;

        var currentCell = board.GetCellData(m_Cell);
        if (currentCell != null) currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    private void TurnHappened()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.PlayerController == null) return;

        var pc = gm.PlayerController;
        var playerCell = pc.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        bool isAdjacent =
            (xDist == 0 && absYDist == 1) ||
            (yDist == 0 && absXDist == 1);

        // 중복 방지
        if (m_SkipThisTurnAttack)
        {
            m_SkipThisTurnAttack = false;
        }
        else if (isAdjacent)
        {
            gm.ChangeFood(-EnemyTurnDamageToPlayer);
            pc.PlayHit();
            return;
        }

        // 추적 이동
        if (absXDist > absYDist)
        {
            if (!TryMoveInX(xDist)) TryMoveInY(yDist);
        }
        else
        {
            if (!TryMoveInY(yDist)) TryMoveInX(xDist);
        }
    }

    private bool TryMoveInX(int xDist)
    {
        if (xDist > 0) return MoveTo(m_Cell + Vector2Int.right);
        return MoveTo(m_Cell + Vector2Int.left);
    }

    private bool TryMoveInY(int yDist)
    {
        if (yDist > 0) return MoveTo(m_Cell + Vector2Int.up);
        return MoveTo(m_Cell + Vector2Int.down);
    }
}
