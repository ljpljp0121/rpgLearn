using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region ���
    public Animator animator { get; private set; }
    public Rigidbody2D rigidbody2 { get; private set; }
    public EntityFX fx { get; private set; }

    public SpriteRenderer sr { get; private set; }

    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDir;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    [Header("Collison info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        fx = GetComponent<EntityFX>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }
    //������˸
    public virtual void Damage()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnocked");
    }
    //�ܻ�����
    protected virtual IEnumerator HitKnocked()
    {
        isKnocked = true;

        rigidbody2.velocity = new Vector2(knockbackDir.x * -facingDir, knockbackDir.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    #region Flip
    //��ת
    public void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped != null)
        {
            onFlipped();
        }
    }
    //��ת������
    public void FlipController(float x)
    {
        if (x > 0 && !facingRight)
        {
            Flip();
        }
        else if (x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion

    #region Collison
    //������
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    //����
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Velocity
    //�ٶ�����
    public void ZeroVelocity() => SetVelocity(0, 0);


    //�����ٶ�
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
        {
            return;
        }

        rigidbody2.velocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
    }
    #endregion
    //���˺����˸
    public void MakeTransparent(bool transparent)
    {
        if (transparent)
        {
            sr.color = Color.clear;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    public virtual void Die()
    {

    }

}
