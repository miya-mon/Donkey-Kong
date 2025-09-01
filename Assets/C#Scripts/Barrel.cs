// Assets/Scripts/Barrel.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Barrel : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("�������̓]���葬�x")]
    public float rollSpeed = 3f;

    [Header("Spawn Direction")]
    [Tooltip("true: �����_���ɍ��E�ǂ��炩�Afalse: ��ɉE����")]
    public bool isRandom = true;

    [Header("Drop Chance")]
    [Range(0f, 1f), Tooltip("�n�V�S��ŗ������J�n����m��")]
    public float dropChanceOnLadder = 0.3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // �X�|�[�����̏����x�ݒ�
        float dir = isRandom
            ? (Random.value > 0.5f ? 1f : -1f)
            : 1f;  // false �̂Ƃ��͕K���E����

        rb.velocity = new Vector2(dir * rollSpeed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // �ǁiGround �^�O�̐����ʁj�ɂԂ������甽�]
        if (col.collider.CompareTag("Ground") && Mathf.Abs(col.relativeVelocity.x) > 0.1f)
        {
            // �ڐG�ʂ̖@�������āA�����������傫�����u�ǁv�ɂԂ����Ă��邩�ǂ������m�F�ł��܂�
            ContactPoint2D contact = col.GetContact(0);
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // �n�V�S��Ń����_���ɗ���
        if (col.CompareTag("Ladder") && Random.value < dropChanceOnLadder)
        {
            rb.velocity = Vector2.down * 5f;
            rb.gravityScale = 1f;
        }
        // ����ɐG�ꂽ��t�@�C���[�{�[������
        else if (col.CompareTag("OilPot"))
        {
            GameController.Instance.SpawnFireball(transform.position);
            Destroy(gameObject);
        }
    }
}
