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
    [SerializeField] private float moveAnimTime = 0.15f;

    private static readonly int MovingHash = Animator.StringToHash("Moving"); // bool
    private static readonly int AttackHash = Animator.StringToHash("Attack"); // trigger
    private static readonly int HitHash = Animator.StringToHash("Hit");    // trigger

    private Coroutine animRoutine;

    public Vector2Int Cell => m_CellPosition;

    public void Init()
    {
        m_IsGameOver = false;
        m_HasMoveRequest = false;
        m_MoveRequest = Vector2Int.zero;

        if (animator == null) animator = GetComponent<Animator>();
        animator.SetBool(MovingHash, false);
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

        Vector2Int targetCell = m_CellPosition + m_MoveRequest;
        var cellData = m_Board.GetCellData(targetCell);

        if (cellData == null || !cellData.Passable) return;

        GameManager.Instance.TurnManager.Tick();

        // 빈 칸: 이동
        if (cellData.ContainedObject == null)
        {
            MoveTo(targetCell);
            PlayMoveAnimOnce();
            return;
        }

        // 오브젝트가 있으면: 먼저 들어갈 수 있는지 판단
        bool canEnter = cellData.ContainedObject.PlayerWantsToEnter();

        if (canEnter)
        {
            // Food / Exit: 이동 + 처리
            MoveTo(targetCell);
            PlayMoveAnimOnce();
            cellData.ContainedObject.PlayerEntered();
        }
        else
        {
            // Enemy: 절대 이동하지 않음(자리 바뀜 방지 핵심)
            animator.SetTrigger(AttackHash);

            cellData.ContainedObject.OnPlayerAttack();

            int counter = cellData.ContainedObject.GetCounterDamageToPlayer();
            if (counter > 0)
            {
                GameManager.Instance.ChangeFood(-counter);
                animator.SetTrigger(HitHash);
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
        animator.SetBool(MovingHash, true);
        yield return new WaitForSeconds(moveAnimTime);
        animator.SetBool(MovingHash, false);
    }
}
