// Assets/Scripts/Fireball.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("�ǔ��ړ��̑���")]
    public float speed = 5f;
    [Tooltip("������Ɏ����ŏ�����܂ł̎���")]
    public float lifetime = 10f;

    private Rigidbody2D rb;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // �d�͂𖳌���
        rb.gravityScale = 0f;

        // Collider �� Trigger �ɂ��Ă��蔲��
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // �v���C���[�Q�Ƃ��L���b�V��
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // ���C�t�^�C����ɏ���
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // �v���C���[��������ɍX�V
            Vector2 toPlayer = (player.position - transform.position);
            Vector2 dir = toPlayer.normalized;
            rb.velocity = dir * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // �v���C���[�ɓ��������Ƃ��̏�����
            // �Ⴆ�΃_���[�W��^����Ȃ�
            // GameController.Instance.PlayerHit();

            Destroy(gameObject);
        }
        // ����ȊO�͖������Ă��蔲��
    }
}
