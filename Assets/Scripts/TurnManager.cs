using UnityEngine;
using System;

public class TurnManager
{
    public event Action OnTick;
    private int m_TurnCount;

    public TurnManager()        //현재 턴을 1턴으로 초기화
    {
        m_TurnCount = 1;
    }

    public void Tick()      //현재 턴을 출력하는 메서드
    {
        m_TurnCount += 1;
        Debug.Log($"현재 Turn : {m_TurnCount}");

        // 턴이 발생했음을 알림
        OnTick?.Invoke();
    }
}
