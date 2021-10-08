using UnityEngine;
using System.Linq;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    #region ���:���}
    [Header("�򥻯�O")]

    [Range(50, 10000)]
    public float hp = 100;

    [Range(5, 1000)]
    public float attack = 20;

    [Range(1, 5000)]
    public float speed = 1.5f;

    /// <summary>
    /// �H�����ݽd��
    /// </summary>
    public Vector2 v2RandomIdle = new Vector2(1, 5);
    /// <summary>
    /// �H�������d��
    /// </summary>
    public Vector2 v2RandomWalk = new Vector2(3, 6);

    public Collider2D[] hits;

    public Collider2D[] hitGround;

    public Collider2D[] hitResult;

    [Header("�ˬd�e��O�_����ê���Φa�O�y��")]
    public Vector3 checkForwardOffset;

    [Range(0, 1)]
    public float checkForWardRadius = 0.3f;

    /// <summary>
    /// �����N�o
    /// </summary>
    [Range(0.5f, 5)]
    public float cdAttack = 3;
    [Header("�Ĥ@����������"), Range(0.5f, 5)]
    public float attackDelayFirst = 0.5f;
    [Header("����������j�h�[�^�_�쥻���A"), Range(0, 5)]
    public float afterAttackRestoreOriginal = 1;
    [Header("���˵w���ɶ�"), Range(0, 2)]
    public float timeHurt = 0.5f;
    [Header("���`�ʵe�ɶ�"), Range(0, 10)]
    public float deadAnimationTime;

    [Header("�O�_��V")]
    public bool isFlipped = false;


    //�N�p�H�����ܦb�ݩʭ��O�W
    [SerializeField]
    [Header("��e���A")]
    protected StateEnemy state;

    protected player player;
    protected Collider2D hit;
    #endregion

    #region ���:�p�H
    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;

    #region �p�ɾ�
    /// <summary>
    /// �����p�ɾ�
    /// </summary>
    protected float timerAttack;

    /// <summary>
    /// ���`�ʵe�p�ɾ�
    /// </summary>
    private float deadtimer = 0f;

    /// <summary>
    /// ���ݮɶ�
    /// </summary>
    private float timeIdle;
    /// <summary>
    /// ���ݭp�ɾ�
    /// </summary>
    protected float timerIdle;

    /// <summary>
    /// �����ɶ�
    /// </summary>
    private float timeWalk;
    /// <summary>
    /// �����p�ɾ�
    /// </summary>
    private float timerWalk;

    /// <summary>
    /// ���˵w���p�ɾ�
    /// </summary>
    private float timerHurt;
    #endregion

    /// <summary>
    /// �Ǫ�����, �l�ܪ��ؼЦW��
    /// </summary>
    private string attackTargetName = "���a";
    /// <summary>
    /// �n�l�ܪ��ؼ�
    /// </summary>
    private Transform attackTarget;
    #endregion

    #region �ƥ�
    private void Start()
    {
        #region ���o����
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        player = GameObject.Find("���a").GetComponent<player>();

        //���o�����ؼЪ�transform;
        attackTarget = GameObject.Find(attackTargetName).transform;
        #endregion

        #region ��l�ȳ]�w
        timeIdle = Random.Range(v2RandomIdle.x, v2RandomIdle.y);
        #endregion
    }

    private void FixedUpdate()
    {
        WalkInFixedUpdate();
    }

    protected virtual void Update()
    {
        CheckForward();
        CheckState();
        if (hp <= 0) state = StateEnemy.dead;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.3f);
        Gizmos.DrawSphere(
            transform.position + 
            transform.right * checkForwardOffset.x + 
            transform.up * checkForwardOffset.y, 
            checkForWardRadius);
    }
    #endregion

    #region �禡: ���}
    /// <summary>
    /// ���ˮɪ���q�p��, �ʵe�Ұ�, �i�J���˪��A
    /// </summary>
    /// <param name="damage"></param>
    public void enemyHurt(float damage)
    {
        hp -= damage;
        if(state != StateEnemy.attack)          //���˵w��(�p�����)
        {
            ani.SetTrigger("����");
            state = StateEnemy.hurt;            //�ϩǪ��i�J���˪��A
        }
        if (hp <= 0) state = StateEnemy.dead;
    }
    #endregion

    #region �禡: �p�H

    /// <summary>
    /// �ˬd�e��樫�ϰ�
    /// </summary>
    private void CheckForward()
    {
        hits = Physics2D.OverlapCircleAll(
            transform.position +
            transform.right * checkForwardOffset.x +
            transform.up * checkForwardOffset.y,
            checkForWardRadius
            );

        hitResult = hits.Where(x => x.name != "�a�O" && x.name != "���x" && x.name != "���a").ToArray();

        //�e�誺����S���a�O�N��V
        hitGround = hits.Where(x => x.name == "�a�O").ToArray();
        if (hitGround.Length == 0 || hitResult.Length > 0)
        {
            TurnDirection();
            if (isFlipped) isFlipped = false;
            else isFlipped = true;
        }
    }

    /// <summary>
    /// ��V
    /// </summary>
    private void TurnDirection()
    {
        float y = transform.eulerAngles.y;
        if (y == 0) transform.eulerAngles = Vector3.up * 180;
        else transform.eulerAngles = Vector3.zero;
    }

    /// <summary>
    /// �ˬd��e���A
    /// </summary>
    private void CheckState() 
    {
        switch (state)
        {
            case StateEnemy.idle:
                Idle();
                break;
            case StateEnemy.walk:
                Walk();
                break;
            case StateEnemy.track:
                break;
            case StateEnemy.attack:
                Attack();
                break;
            case StateEnemy.dead:
                Dead();
                break;
            case StateEnemy.hurt:
                Hurt();
                break;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    private void Idle()
    {
        if(timerIdle < timeIdle)
        {
            timerIdle += Time.deltaTime;
        }
        else
        {
            RandomDirection();
            state = StateEnemy.walk;
            timeWalk = Random.Range(v2RandomWalk.x, v2RandomWalk.y);
            timerIdle = 0;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    private void Walk()
    {
        if (timerWalk < timeWalk)
        {
            timerWalk += Time.deltaTime;
            ani.SetBool("����", true);
        }
        else
        {
            ani.SetBool("����", false);
            state = StateEnemy.idle;
            timeIdle = Random.Range(v2RandomIdle.x, v2RandomIdle.y);
            timerWalk = 0;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    private void Attack()
    {
        if (timerAttack < cdAttack)
        {
            timerAttack += Time.deltaTime;
            ani.SetBool("����", false);
        }
        else
        {
            AttackMethod();
        }
    }

    /// <summary>
    /// �����覡
    /// </summary>
    protected virtual void AttackMethod()
    {
        timerAttack = 0;
        Vector3 posTarget = attackTarget.position;      //�ؼЮy��
        if (transform.position.x > posTarget.x && isFlipped)
        {
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < posTarget.x && !isFlipped)
        {
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
        ani.SetTrigger("����");
    }

    /// <summary>
    /// ���˰ʧ@
    /// </summary>
    private void Hurt()
    {
        Vector3 posTarget = attackTarget.position;      //�ؼЮy��
        if (timerHurt < timeHurt)
        {
            timerHurt += Time.deltaTime;
            ///���ˮ���V���a
            if(transform.position.x > posTarget.x && isFlipped)
            {
                transform.Rotate(0f, 180f, 0f);
                isFlipped = false;
            }
            else if (transform.position.x < posTarget.x && !isFlipped)
            {
                transform.Rotate(0f, 180f, 0f);
                isFlipped = true;
            }
        }
        else
        {
            ani.SetBool("����", false);
            timerIdle = 0;
            state = StateEnemy.idle;
            timerHurt = 0;
            timerWalk = 0;
        }
    }

    /// <summary>
    /// ���`:���`�ʵe, ���A, �����}��, �I����, �[�t��, ����ᵲ
    /// </summary>
    private void Dead()
    {
        hp = 0;

        ani.SetBool("���`", true);
        GetComponent<CapsuleCollider2D>().enabled = false;      //�����I����
        rig.velocity = Vector3.zero;                            //�[�t���k�s
        rig.constraints = RigidbodyConstraints2D.FreezeAll;     //����ᵲ����

        if(deadtimer < deadAnimationTime)
        {
            deadtimer += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
            enabled = false;
        }
    }

    /// <summary>
    /// �N����欰��W�B�z�æb FxiedUpdate �I�s
    /// </summary>
    private void WalkInFixedUpdate()
    {
        if (state == StateEnemy.walk) rig.velocity = -transform.right * speed * Time.fixedDeltaTime + Vector3.up * rig.velocity.y;
        
    }

    /// <summary>
    /// �H�������V
    /// </summary>
    private void RandomDirection()
    {
        int random = Random.Range(0, 2);

        if (random == 0) transform.eulerAngles = Vector2.zero;
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            if (isFlipped) isFlipped = false;
            else isFlipped = true;
        }
    }
    #endregion
}

public enum StateEnemy
{
    idle, walk, track, attack, dead, hurt
}
