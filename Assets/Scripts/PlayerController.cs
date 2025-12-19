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
    [SerializeField] private float moveAnimTime = 0.15f;   // 이동 연출 시간(왕복에도 사용)
    [SerializeField] private float attackAnimTime = 0.25f; // 공격 모션 보여줄 시간

    //Animator 파라미터 이름 (사용자 말대로)
    private static readonly int MovingHash = Animator.StringToHash("Moving"); // bool
    private static readonly int AttackHash = Animator.StringToHash("Attack"); // Trigger 권장
    private static readonly int HitHash = Animator.StringToHash("Hit");    // Trigger 권장

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

    // 외부에서 피격 시 호출 가능
    public void PlayHit()
    {
        if (animator == null) return;
        SafeSetTrigger(HitHash);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Vector2 v = ctx.ReadValue<Vector2>();
        if (v == Vector2.zero) return;

        // 대각 무시
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
        BoardManager.CellData cellData = m_Board.GetCellData(targetCell);

        if (cellData == null || !cellData.Passable) return;

        // 턴 진행
        GameManager.Instance.TurnManager.Tick();

        // 1) 빈 칸이면 실제 이동 + Move 애니
        if (cellData.ContainedObject == null)
        {
            MoveTo(targetCell);
            PlayMoveAnimOnce();
            return;
        }

        // 2) 오브젝트(적 등)가 있으면: "가서 때리고 + 반격으로 맞고 + 원위치" 연출
        StartAttackSequence(cellData.ContainedObject, targetCell);
    }

    private void StartAttackSequence(CellObject obj, Vector2Int enemyCell)
    {
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(Co_AttackSequence(obj, enemyCell));
    }

    private IEnumerator Co_AttackSequence(CellObject obj, Vector2Int enemyCell)
    {
        Vector3 originPos = m_Board.CellToWorld(m_CellPosition);
        Vector3 enemyPos = m_Board.CellToWorld(enemyCell);

        // 이동 중 Moving 잠깐 켬
        SafeSetBool(MovingHash, true);

        // 1) 적 칸으로 "연출 이동"
        yield return MoveTransformLerp(originPos, enemyPos, moveAnimTime * 0.5f);

        SafeSetBool(MovingHash, false);

        // 2) 공격 트리거
        SafeSetTrigger(AttackHash);

        // 3) 적이 플레이어 공격을 처리(HP 감소/파괴 등)
        if (obj != null)
            obj.OnPlayerAttack();

        // 4) 반격 데미지 + Hit 트리거(같이 맞으면서 때리는 연출)
        int counter = (obj != null) ? obj.GetCounterDamageToPlayer() : 0;
        if (counter > 0)
        {
            GameManager.Instance.ChangeFood(-counter);
            SafeSetTrigger(HitHash);
        }

        // 공격 모션 보이는 시간
        yield return new WaitForSeconds(attackAnimTime);

        // 5) 원래 위치로 복귀
        SafeSetBool(MovingHash, true);
        yield return MoveTransformLerp(enemyPos, originPos, moveAnimTime * 0.5f);
        SafeSetBool(MovingHash, false);

        // 안전하게 정렬
        transform.position = originPos;
    }

    private IEnumerator MoveTransformLerp(Vector3 from, Vector3 to, float t)
    {
        if (t <= 0f)
        {
            transform.position = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < t)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Clamp01(elapsed / t);
            transform.position = Vector3.Lerp(from, to, a);
            yield return null;
        }
        transform.position = to;
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

    //안전 Set (파라미터 오류 방지용)
    private void SafeSetBool(int hash, bool value)
    {
        if (animator == null) return;
        animator.SetBool(hash, value);
    }

    private void SafeSetTrigger(int hash)
    {
        if (animator == null) return;
        animator.SetTrigger(hash);
    }
}
