using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어를 보드 위에서 움직이는 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    // 현재 플레이어가 올라가 있는 보드를 기억
    private BoardManager m_Board;
    // 플레이어의 현재 격자 위치
    private Vector2Int m_CellPosition;

    protected Vector2Int m_Cell;

    public Vector2Int Cell
    {
        get { return m_Cell; }
    }

    // 보드 위에 플레이어를 처음 소환할 때 호출
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        m_CellPosition = cell;

        // 보드 셀 위치에 맞는 월드 좌표로 플레이어 이동
        transform.position = m_Board.CellToWorld(cell);
    }

    // 셀 좌표 기준으로 위치를 옮기는 함수
    public void MoveTo(Vector2Int cell)
    {
        m_CellPosition = cell;
        transform.position = m_Board.CellToWorld(m_CellPosition); // 셀 좌표 -> 유니티 화면
    }

    // 매 프레임마다 입력을 받아서 이동 처리
    private void Update()
    {
        // 기본값: 현재 위치에서 시작
        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;     // 이번 프레임에 이동했는지 여부

        // ↑ 키
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;  // 위쪽 셀로 한 칸 이동
            hasMoved = true;
        }
        // ↓ 키
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;  // 아래쪽 셀로 한 칸 이동
            hasMoved = true;
        }
        // → 키
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;    // 오른쪽 셀로 한 칸 이동
            hasMoved = true;
        }
        // ← 키
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;    // 왼쪽 셀로 한 칸 이동
            hasMoved = true;
        }

        // 이번 프레임에 이동 입력이 있었다면
        if (hasMoved)
        {
            // 위치의 셀 정보 가져오기 (벽? 또는 길, 등)
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            // 셀이 존재하고, 통과 가능한 칸일 때만 실제로 이동
            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();
                MoveTo(newCellTarget);
            }
        }
    }
}