using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;

    public override bool PlayerWantsToEnter()
    {
        return true;
    }

    public override void PlayerEntered()
    {
        GameManager.Instance.ChangeFood(AmountGranted);

        SoundManager.Instance.PlayEatFood();

        ClearFromBoard();
        Destroy(gameObject);
    }
}
