// Assets/Scripts/Barrel.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Barrel : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("横方向の転がり速度")]
    public float rollSpeed = 3f;

    [Header("Spawn Direction")]
    [Tooltip("true: ランダムに左右どちらか、false: 常に右向き")]
    public bool isRandom = true;

    [Header("Drop Chance")]
    [Range(0f, 1f), Tooltip("ハシゴ上で落下を開始する確率")]
    public float dropChanceOnLadder = 0.3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // スポーン時の初速度設定
        float dir = isRandom
            ? (Random.value > 0.5f ? 1f : -1f)
            : 1f;  // false のときは必ず右向き

        rb.velocity = new Vector2(dir * rollSpeed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // 壁（Ground タグの垂直面）にぶつかったら反転
        if (col.collider.CompareTag("Ground") && Mathf.Abs(col.relativeVelocity.x) > 0.1f)
        {
            // 接触面の法線を見て、水平成分が大きい＝「壁」にぶつかっているかどうかも確認できます
            ContactPoint2D contact = col.GetContact(0);
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // ハシゴ上でランダムに落下
        if (col.CompareTag("Ladder") && Random.value < dropChanceOnLadder)
        {
            rb.velocity = Vector2.down * 5f;
            rb.gravityScale = 1f;
        }
        // 油壺に触れたらファイヤーボール生成
        else if (col.CompareTag("OilPot"))
        {
            GameController.Instance.SpawnFireball(transform.position);
            Destroy(gameObject);
        }
    }
}
