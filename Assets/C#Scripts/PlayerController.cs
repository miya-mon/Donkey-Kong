using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 6f;

    [Header("Climb")]
    public float climbSpeed = 2.5f;

    [Header("Hammer")]
    public float hammerDuration = 11f;

    [Header("Ground Check")]
    [Tooltip("キャラの中心からレイの発射位置までのオフセット")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    [Tooltip("地面判定用レイの長さ")]
    public float groundCheckDistance = 0.1f;

    // コンポーネント・状態管理
    private Rigidbody2D rb;
    private Animator anim;

    // 入力・タイミング
    private float horizontal;
    private bool jumpPressed;

    // フラグ
    private bool isClimbing = false;
    private int ladderCount = 0;
    private bool hasHammer;
    private float hammerTimer;

    // Ladder 接触判定
    private bool onLadder => ladderCount > 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 横移動入力
        horizontal = Input.GetAxisRaw("Horizontal");
        // ジャンプ入力検知（1フレーム）
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        // 左右向き反転
        if (horizontal != 0f)
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1f, 1f);

        // ハンマーモードタイマー更新
        if (hasHammer)
        {
            hammerTimer -= Time.deltaTime;
            if (hammerTimer <= 0f)
                hasHammer = false;
        }

        // アニメーションパラメータ更新
        bool running = horizontal != 0f && !isClimbing && !hasHammer;
        anim.SetBool("isRunning", running);
        anim.SetBool("isClimbing", isClimbing);
        anim.SetBool("hasHammer", hasHammer);

        // ジャンプ用トリガー
        if (jumpPressed && IsGrounded() && !hasHammer)
            anim.SetTrigger("jump");
    }

    void FixedUpdate()
    {
        // はしご登攀処理
        if (onLadder && !hasHammer)
        {
            float v = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(v) > 0.1f)
            {
                isClimbing = true;
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(0f, v * climbSpeed);
                return;
            }
        }

        // はしご離脱時に重力を戻す
        if (isClimbing)
        {
            isClimbing = false;
            rb.gravityScale = 1f;
        }

        // 横移動
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        // ジャンプ力付与（FixedUpdate で）
        if (jumpPressed && IsGrounded() && !hasHammer)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpPressed = false;  // 消費
        }
    }

    /// <summary>
    /// 接地判定用レイを Scene ビューに可視化
    /// </summary>
    void OnDrawGizmos()
    {
        Vector3 origin = (Vector3)transform.position + (Vector3)groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }

    /// <summary>
    /// 地面に接地しているか判定
    /// </summary>
    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        LayerMask mask = LayerMask.GetMask("Ground", "Platform");
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, mask);
        return hit.collider != null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
        {
            ladderCount++;
        }
        else if (col.CompareTag("Hammer"))
        {
            hasHammer = true;
            hammerTimer = hammerDuration;
            Destroy(col.gameObject);
        }
        else if ((col.CompareTag("Enemy") || col.CompareTag("Barrel") || col.CompareTag("Fireball")) && !hasHammer)
        {
            anim.SetTrigger("die");
            GameController.Instance.PlayerDied();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
        {
            ladderCount = Mathf.Max(0, ladderCount - 1);
        }
    }
}
