using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;

    // 입력은 Callback 에서 바로 이동하지 않고 요청으로 저장 (1턴에 한번씩 이동하도록 처리)
    private Vector2Int m_MoveRequest;
    private bool m_HasMoveRequest;

    public Vector2Int Cell => m_CellPosition;

    public void Init()
    {
        m_IsGameOver = false;
        m_HasMoveRequest = false;
        m_MoveRequest = Vector2Int.zero;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell);
    }

    public void MoveTo(Vector2Int cell)
    {
        m_CellPosition = cell;
        transform.position = m_Board.CellToWorld(m_CellPosition);
    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed)
        {
            return;
        }
        
        //Debug.Log("Move performed: " + ctx.ReadValue<Vector2>());
        Vector2 v = ctx.ReadValue<Vector2>();
        if (v == Vector2.zero)
        {
            return;
        }

        // 대각 입력은 무시, 정규화
        if(Mathf.Abs(v.x) > Mathf.Abs(v.y))
        {
            m_MoveRequest = new Vector2Int(v.x > 0 ? 1 : -1, 0);
        }
        else
        {
            m_MoveRequest = new Vector2Int(0, v.y > 0 ? 1 : -1);
        }

        m_HasMoveRequest = true;
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed)
        {
            return;
        }

        if(m_IsGameOver)
        {
            GameManager.Instance.StartNewGame();
        }
    }

    private void Update()
    {
        if (m_IsGameOver)
        {
            return;
        }

        if(!m_HasMoveRequest)
        {
            return;
        }

        m_HasMoveRequest = false;

        Vector2Int newCellTarget = m_CellPosition + m_MoveRequest;

        BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
        if (cellData == null || !cellData.Passable)
        {
            return;
        }

        GameManager.Instance.TurnManager.Tick();

        if (cellData.ContainedObject == null)
        {
            MoveTo(newCellTarget);
        }
        else if(cellData.ContainedObject.PlayerWantsToEnter())
        {
            MoveTo(newCellTarget);
            cellData.ContainedObject.PlayerEntered();
        }
    }
}