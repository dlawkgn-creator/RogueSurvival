using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int m_Cell;

    public virtual void Init(Vector2Int cell)
    {
        m_Cell = cell;
    }

    // 플레이어가 이 오브젝트가 있는 cell 에 들어왔을 때 호출
    public virtual bool PlayerWantsToEnter()
    {
        return true;
    }
}
