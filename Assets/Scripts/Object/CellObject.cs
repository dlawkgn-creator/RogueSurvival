using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int m_Cell;

    public virtual void Init(Vector2Int cell)
    {
        m_Cell = cell;
    }

    public virtual bool PlayerWantsToEnter()
    {
        return true;
    }

    public virtual void PlayerEntered()
    {
    }

    public virtual void OnPlayerAttack()
    {
    }

    public virtual int GetCounterDamageToPlayer()
    {
        return 0;
    }

    // 보드에서 자기 자신 제거(음식/적 죽음 등에서 사용)
    protected void ClearFromBoard()
    {
        if (GameManager.Instance == null || GameManager.Instance.BoardManager == null) return;

        var cell = GameManager.Instance.BoardManager.GetCellData(m_Cell);
        if (cell != null && cell.ContainedObject == this)
            cell.ContainedObject = null;
    }
}
