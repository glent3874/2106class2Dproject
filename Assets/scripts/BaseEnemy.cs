using UnityEngine;
using System.Linq;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    #region 欄位:公開
    [Header("基本能力")]

    [Range(50, 10000)]
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
    [Header("攻擊完成後隔多久回復原本狀態"), Range(0, 5)]
    public float afterAttackRestoreOriginal = 1;
    [Header("受傷硬直時間"), Range(0, 2)]
    public float timeHurt = 0.5f;
    [Header("死亡動畫時間"), Range(0, 10)]
    public float deadAnimationTime;

    [Header("是否轉向")]
    public bool isFlipped = false;


    //將私人欄位顯示在屬性面板上
    [SerializeField]
    [Header("當前狀態")]
    protected StateEnemy state;

    protected player player;
    protected Collider2D hit;
    #endregion

    #region 欄位:私人
    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;

    #region 計時器
    /// <summary>
    /// 攻擊計時器
    /// </summary>
    protected float timerAttack;

    /// <summary>
    /// 死亡動畫計時器
    /// </summary>
    private float deadtimer = 0f;

    /// <summary>
    /// 等待時間
    /// </summary>
    private float timeIdle;
    /// <summary>
    /// 等待計時器
    /// </summary>
    protected float timerIdle;

    /// <summary>
    /// 走路時間
    /// </summary>
    private float timeWalk;
    /// <summary>
    /// 走路計時器
    /// </summary>
    private float timerWalk;

    /// <summary>
    /// 受傷硬直計時器
    /// </summary>
    private float timerHurt;
    #endregion

    /// <summary>
    /// 怪物攻擊, 追蹤的目標名稱
    /// </summary>
    private string attackTargetName = "玩家";
    /// <summary>
    /// 要追蹤的目標
    /// </summary>
    private Transform attackTarget;
    #endregion

    #region 事件
    private void Start()
    {
        #region 取得元件
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        player = GameObject.Find("玩家").GetComponent<player>();

        //取得攻擊目標的transform;
        attackTarget = GameObject.Find(attackTargetName).transform;
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

    #region 函式: 公開
    /// <summary>
    /// 受傷時的血量計算, 動畫啟動, 進入受傷狀態
    /// </summary>
    /// <param name="damage"></param>
    public void enemyHurt(float damage)
    {
        hp -= damage;
        if(state != StateEnemy.attack)          //受傷硬直(小於攻擊)
        {
            ani.SetTrigger("受傷");
            state = StateEnemy.hurt;            //使怪物進入受傷狀態
        }
        if (hp <= 0) state = StateEnemy.dead;
    }
    #endregion

    #region 函式: 私人

    /// <summary>
    /// 檢查前方行走區域
    /// </summary>
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
            if (isFlipped) isFlipped = false;
            else isFlipped = true;
        }
    }

    /// <summary>
    /// 轉向
    /// </summary>
    private void TurnDirection()
    {
        float y = transform.eulerAngles.y;
        if (y == 0) transform.eulerAngles = Vector3.up * 180;
        else transform.eulerAngles = Vector3.zero;
    }

    /// <summary>
    /// 檢查當前狀態
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
    /// 等待
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
    /// 走路
    /// </summary>
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

    /// <summary>
    /// 攻擊
    /// </summary>
    private void Attack()
    {
        if (timerAttack < cdAttack)
        {
            timerAttack += Time.deltaTime;
            ani.SetBool("走路", false);
        }
        else
        {
            AttackMethod();
        }
    }

    /// <summary>
    /// 攻擊方式
    /// </summary>
    protected virtual void AttackMethod()
    {
        timerAttack = 0;
        Vector3 posTarget = attackTarget.position;      //目標座標
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
        ani.SetTrigger("攻擊");
    }

    /// <summary>
    /// 受傷動作
    /// </summary>
    private void Hurt()
    {
        Vector3 posTarget = attackTarget.position;      //目標座標
        if (timerHurt < timeHurt)
        {
            timerHurt += Time.deltaTime;
            ///受傷時轉向玩家
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
            ani.SetBool("走路", false);
            timerIdle = 0;
            state = StateEnemy.idle;
            timerHurt = 0;
            timerWalk = 0;
        }
    }

    /// <summary>
    /// 死亡:死亡動畫, 狀態, 關閉腳本, 碰撞器, 加速度, 剛體凍結
    /// </summary>
    private void Dead()
    {
        hp = 0;

        ani.SetBool("死亡", true);
        GetComponent<CapsuleCollider2D>().enabled = false;      //關閉碰撞器
        rig.velocity = Vector3.zero;                            //加速度歸零
        rig.constraints = RigidbodyConstraints2D.FreezeAll;     //剛體凍結全部

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
    /// 將物件行為單獨處理並在 FxiedUpdate 呼叫
    /// </summary>
    private void WalkInFixedUpdate()
    {
        if (state == StateEnemy.walk) rig.velocity = -transform.right * speed * Time.fixedDeltaTime + Vector3.up * rig.velocity.y;
        
    }

    /// <summary>
    /// 隨機面對方向
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
