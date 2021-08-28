using UnityEngine;
using System.Linq;

public class BaseEnemy : MonoBehaviour
{
    #region ���:���}
    [Header("�򥻯�O")]

    [Range(50, 5000)]
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

    private float timerAttack;


    //�N�p�H�����ܦb�ݩʭ��O�W
    [SerializeField]
    protected StateEnemy state;
    #endregion

    #region ���:�p�H
    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;

    /// <summary>
    /// ���ݮɶ�
    /// </summary>
    private float timeIdle;
    /// <summary>
    /// ���ݭp�ɾ�
    /// </summary>
    private float timerIdle;

    /// <summary>
    /// �����ɶ�
    /// </summary>
    private float timeWalk;
    /// <summary>
    /// �����p�ɾ�
    /// </summary>
    private float timerWalk;
    #endregion

    protected player player;
    protected Collider2D hit;

    #region �ƥ�
    private void Start()
    {
        #region ���o����
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        player = GameObject.Find("���a").GetComponent<player>();
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

    

    #region �禡

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
        }
    }

    private void TurnDirection()
    {
        float y = transform.eulerAngles.y;
        if (y == 0) transform.eulerAngles = Vector3.up * 180;
        else transform.eulerAngles = Vector3.zero;
    }

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
                break;
        }
    }

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

    private void Attack()
    {
        if(timerAttack < cdAttack)
        {
            timerAttack += Time.deltaTime;
        }
        else
        {
            AttackMethod();
        }
    }

    protected virtual void AttackMethod()
    {
        timerAttack = 0;
        ani.SetTrigger("����");
    }

    /// <summary>
    /// �N����欰��W�B�z�æb FxiedUpdate �I�s
    /// </summary>
    private void WalkInFixedUpdate()
    {
        if (state == StateEnemy.walk) rig.velocity = -transform.right * speed * Time.fixedDeltaTime + Vector3.up * rig.velocity.y;
    }

    private void RandomDirection()
    {
        int random = Random.Range(0, 2);

        if (random == 0) transform.eulerAngles = Vector2.zero;
        else transform.eulerAngles = new Vector3(0, 180, 0);
    }
    #endregion
}

public enum StateEnemy
{
    idle, walk, track, attack, dead 
}
