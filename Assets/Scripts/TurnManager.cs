using UnityEngine;
using System;

public class TurnManager
{
    public event Action OnTick;
    private int m_TurnCount;

    //현재 턴을 1턴으로 초기화
    public TurnManager()
    {
        m_TurnCount = 1;
    }

    //현재 턴을 출력하는 메서드
    public void Tick()
    {
        m_TurnCount += 1;
        Debug.Log($"현재 Turn : {m_TurnCount}");

        // 턴이 발생했음을 알림
        OnTick?.Invoke();
    }
}
