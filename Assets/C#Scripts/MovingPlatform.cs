using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;
    [Tooltip("�ړ����x�i�P�ʁF���j�b�g�^�b�j")]
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
        // Rigidbody2D ���g���ĕ����I�Ɉړ�
        Vector2 next = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        // �ڕW�n�_�ɏ\���߂Â����甽�]
        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            target = (target == pointB.position) ? pointA.position : pointB.position;
        }
    }

}
