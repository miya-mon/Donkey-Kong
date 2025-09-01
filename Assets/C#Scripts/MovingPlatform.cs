using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;
    [Tooltip("移動速度（単位：ユニット／秒）")]
    public float speed = 2f;

    private Vector3 target;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = pointB.position;
    }

    void FixedUpdate()
    {
        // Rigidbody2D を使って物理的に移動
        Vector2 next = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        // 目標地点に十分近づいたら反転
        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            target = (target == pointB.position) ? pointA.position : pointB.position;
        }
    }

}
