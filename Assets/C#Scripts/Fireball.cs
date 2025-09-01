// Assets/Scripts/Fireball.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("追尾移動の速さ")]
    public float speed = 5f;
    [Tooltip("生成後に自動で消えるまでの時間")]
    public float lifetime = 10f;

    private Rigidbody2D rb;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 重力を無効化
        rb.gravityScale = 0f;

        // Collider を Trigger にしてすり抜け
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // プレイヤー参照をキャッシュ
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // ライフタイム後に消滅
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // プレイヤー方向を常に更新
            Vector2 toPlayer = (player.position - transform.position);
            Vector2 dir = toPlayer.normalized;
            rb.velocity = dir * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // プレイヤーに当たったときの処理例
            // 例えばダメージを与えるなど
            // GameController.Instance.PlayerHit();

            Destroy(gameObject);
        }
        // それ以外は無視してすり抜け
    }
}
