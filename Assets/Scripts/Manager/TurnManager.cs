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

        if (OnTick != null)
        {
            OnTick.Invoke();
        }
    }
}