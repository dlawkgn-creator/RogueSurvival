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

    [Header("Direction")]
    [SerializeField] private bool facingRight = true;

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
            UpdateFacing(m_MoveRequest);

            MoveTo(targetCell);
            PlayMoveAnimOnce();

            SoundManager.Instance?.PlayMove();

            return;
        }

        // 오브젝트 참조를 먼저 잡아둠
        var obj = cellData.ContainedObject;
        if (obj == null) return;

        // 먼저 들어갈 수 있는지 판단
        bool canEnter = obj.PlayerWantsToEnter();

        if (canEnter)
        {
            UpdateFacing(m_MoveRequest);
            // Food / Exit: 이동 + 처리
            MoveTo(targetCell);
            PlayMoveAnimOnce();

            SoundManager.Instance?.PlayMove();
            obj.PlayerEntered(); // obj로 호출 (cellData.ContainedObject 다시 안 씀)
        }
        else
        {
            // Enemy: 절대 이동하지 않음(자리 바뀜 방지 핵심)

            // 반격 데미지는 적이 죽어서 보드에서 빠지기 전에 먼저 받아둠
            int counter = obj.GetCounterDamageToPlayer();

            // 공격 모션
            if (animator != null) animator.SetTrigger(AttackHash);

            // 적에게 공격 적용 (여기서 적이 죽으면 cellData.ContainedObject가 null 될 수 있음)
            obj.OnPlayerAttack();

            // 반격 데미지 + 피격 모션
            if (counter > 0)
            {
                GameManager.Instance.ChangeFood(-counter);
                if (animator != null) animator.SetTrigger(HitHash);
            }
        }
    }

    private void UpdateFacing(Vector2Int moveDir)
    {
        if(moveDir.x == 0)
        {
            return;
        }

        bool FaceRight = moveDir.x > 0;

        if(FaceRight == facingRight)
        {
            return;
        }

        facingRight = FaceRight;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
        transform.localScale = scale;
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
