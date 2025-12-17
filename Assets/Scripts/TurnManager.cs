using UnityEngine;
using System;

public class TurnManager
{
    private int m_TurnCount;
    public event Action OnTick;

    public TurnManager()
    {
        m_TurnCount = 1;
    }

    public void Tick()
    {
        m_TurnCount += 1;
        Debug.Log($"Current turn count : {m_TurnCount}");

        if (OnTick != null)
        {
            OnTick.Invoke();
        }
    }
}