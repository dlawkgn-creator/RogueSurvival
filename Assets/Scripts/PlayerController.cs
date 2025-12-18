using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;

    private Vector2Int m_MoveRequest;
    private bool m_HasMoveRequest;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float moveAnimTime = 0.15f;   // 이동 애니메이션 보이는 시간(턴제용)
    [SerializeField] private float attackAnimTime = 0.25f; // 공격 모션 잠깐 보이게

    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");

    private Coroutine animRoutine;

    public Vector2Int Cell => m_CellPosition;

    public void Init()
    {
        m_IsGameOver = false;
        m_HasMoveRequest = false;
        m_MoveRequest = Vector2Int.zero;

        if (animator == null) animator = GetComponent<Animator>();
        SafeSetBool(MovingHash, false);
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

    //외부(데미지 처리 코드)에서 호출할 피격 함수
    public void PlayHit()
    {
        if (animator == null) return;
        animator.SetTrigger(HitHash);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 v = ctx.ReadValue<Vector2>();
        if (v == Vector2.zero) return;

        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            m_MoveRequest = new Vector2Int(v.x > 0 ? 1 : -1, 0);
        else
            m_MoveRequest = new Vector2Int(0, v.y > 0 ? 1 : -1);

        m_HasMoveRequest = true;
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (m_IsGameOver)
            GameManager.Instance.StartNewGame();
    }

    private void Update()
    {
        if (m_IsGameOver) return;
        if (!m_HasMoveRequest) return;

        m_HasMoveRequest = false;

        Vector2Int newCellTarget = m_CellPosition + m_MoveRequest;

        BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
        if (cellData == null || !cellData.Passable) return;

        GameManager.Instance.TurnManager.Tick();

        // 1) 빈 칸이면 이동 + 이동 애니메이션 잠깐
        if (cellData.ContainedObject == null)
        {
            MoveTo(newCellTarget);
            PlayMoveAnimOnce();
        }
        // 2) 오브젝트가 있으면: (예: 적) 공격 트리거를 먼저
        else
        {
            PlayAttackAnimOnce();

            // 기존 로직 유지 (팀원이 만든 규칙에 따라 들어갈지 말지)
            if (cellData.ContainedObject.PlayerWantsToEnter())
            {
                MoveTo(newCellTarget);
                cellData.ContainedObject.PlayerEntered();
            }
        }
    }

    private void PlayMoveAnimOnce()
    {
        if (animator == null) return;
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(Co_MoveAnim());
    }

    private IEnumerator Co_MoveAnim()
    {
        SafeSetBool(MovingHash, true);
        yield return new WaitForSeconds(moveAnimTime);
        SafeSetBool(MovingHash, false);
    }

    private void PlayAttackAnimOnce()
    {
        if (animator == null) return;
        animator.SetTrigger(AttackHash);
    }

    private void SafeSetBool(int hash, bool value)
    {
        if (animator == null) return;
        animator.SetBool(hash, value);
    }
}
