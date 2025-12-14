using UnityEngine;

public class FoodObject : CellObject
{
    public override bool PlayerWantsToEnter()
    {
        Destroy(gameObject);
        return true; // 이동 허용
    }
}
