using UnityEngine;
using System.Linq;

public class BaseEnemy : MonoBehaviour
{
    #region 欄位:公開
    [Header("基本能力")]

    [Range(50, 5000)]
    public float hp = 100;

    [Range(5, 1000)]
    public float attack = 20;

    [Range(1, 5000)]
    public float speed = 1.5f;

    /// <summary>
    /// 隨機等待範圍
    /// </summary>
    public Vector2 v2RandomIdle = new Vector2(1, 5);
    /// <summary>
    /// 隨機走路範圍
    /// </summary>
    public Vector2 v2RandomWalk = new Vector2(3, 6);

    public Collider2D[] hits;

    public Collider2D[] hitGround;

    public Collider2D[] hitResult;

    [Header("檢查前方是否有障礙物或地板球體")]
    public Vector3 checkForwardOffset;

    [Range(0, 1)]
    public float checkForWardRadius = 0.3f;

    /// <summary>
    /// 攻擊冷卻
    /// </summary>
    [Range(0.5f, 5)]
    public float cdAttack = 3;
    [Header("第一次攻擊延遲"), Range(0.5f, 5)]
    public float attackDelayFirst = 0.5f;

    private float timerAttack;


    //將私人欄位顯示在屬性面板上
    [SerializeField]
    protected StateEnemy state;
    #endregion

    #region 欄位:私人
    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;

    /// <summary>
    /// 等待時間
    /// </summary>
    private float timeIdle;
    /// <summary>
    /// 等待計時器
    /// </summary>
    private float timerIdle;

    /// <summary>
    /// 走路時間
    /// </summary>
    private float timeWalk;
    /// <summary>
    /// 走路計時器
    /// </summary>
    private float timerWalk;
    #endregion

    protected player player;
    protected Collider2D hit;

    #region 事件
    private void Start()
    {
        #region 取得元件
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        player = GameObject.Find("玩家").GetComponent<player>();
        #endregion

        #region 初始值設定
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

    

    #region 函式

    private void CheckForward()
    {
        hits = Physics2D.OverlapCircleAll(
            transform.position +
            transform.right * checkForwardOffset.x +
            transform.up * checkForwardOffset.y,
            checkForWardRadius
            );

        hitResult = hits.Where(x => x.name != "地板" && x.name != "跳台" && x.name != "玩家").ToArray();

        //前方的物體沒有地板就轉向
        hitGround = hits.Where(x => x.name == "地板").ToArray();

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
            ani.SetBool("走路", true);
        }
        else
        {
            ani.SetBool("走路", false);
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
        ani.SetTrigger("攻擊");
    }

    /// <summary>
    /// 將物件行為單獨處理並在 FxiedUpdate 呼叫
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
