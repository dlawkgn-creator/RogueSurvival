using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int m_Cell;

    public virtual void Init(Vector2Int cell)
    {
        m_Cell = cell;
    }

    // 플레이어가 이 칸에 "들어가도 되는지"
    public virtual bool PlayerWantsToEnter()
    {
        return true;
    }

    // 플레이어가 이 칸에 "실제로 들어갔을 때" 실행
    public virtual void PlayerEntered()
    {
    }

    //추가: 플레이어가 공격했을 때 오브젝트가 처리할 것(HP 감소, 파괴 등)
    public virtual void OnPlayerAttack()
    {
    }

    //추가: 플레이어가 공격할 때 같이 받는 반격 데미지(없으면 0)
    public virtual int GetCounterDamageToPlayer()
    {
        return 0;
    }
}
