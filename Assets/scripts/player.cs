using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 500)]
    public float moveSpeed;
    [Header("跳躍高度"), Range(0, 5000)]
    public int jumpHeight = 3000;
    [Header("衝刺距離"), Range(0, 15)]
    public int RushDistance;
    private float rushdis;
    [Header("血量"), Range(0, 20000)]
    public float HP = 100;
    [Header("攻擊力"), Range(0, 1000)]
    public float attack = 20;
    [Header("是否在地板上")]
    public bool onFloor = false;

    private AudioSource aud;                    //音效來源
    private Rigidbody2D rig;                    //剛體
    private Animator ani;                       //動畫
    /// <summary>
    /// 玩家水平輸入數值
    /// </summary>
    private float hValue;

    [Header("攻擊區域的位移與大小")]
    public Vector2 checkAttackOffset;
    public Vector2 checkAttackSize;

    [Header("攻擊冷卻"), Range(0, 5)]
    public float cd = 2;
    /// <summary>
    /// 攻擊計時器
    /// </summary>
    public float timer;
    /// <summary>
    /// 是否為攻擊
    /// </summary>
    private bool isAttack;

    private Text textHP;
    private Image imgHP;

    private float hpMax;

    private Transform weapon;

    [Header("檢查地板區域: 位移與半徑")]
    public Vector3 groundOffset;
    [Range(0, 2)]
    public float groundRadius = 0.5f;

    [Header("重力"), Range(0.01f, 3)]
    public float gravity = 3;

    public bool isRush;
    private float rushtimer;
    [Header("衝刺冷卻"), Range(0, 2)]
    public float rushtime;

    [Header("水平物體偵測")]
    public RaycastHit2D RaycastHit;
    #endregion

    #region 事件
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();      //取得剛體
        ani = GetComponent<Animator>();         //取得動畫

        hpMax = HP;

        textHP = GameObject.Find("血量").GetComponent<Text>();
        imgHP = GameObject.Find("血條").GetComponent<Image>();

        weapon = GameObject.Find("斬擊輸出點").GetComponent<Transform>();

        textHP.text = "HP " + HP;
        imgHP.fillAmount = HP / hpMax;
    }

    private void Update()
    {
        GetPlayerInputHorizontal();
        Turndirection();
        Jump();
        Attack();
        Rush(hValue);
        walldetect();
    }

    private void FixedUpdate()
    {
        Move(hValue * 5);
    }

    /// <summary>
    /// 畫出攻擊範圍
    /// </summary>
    private void OnDrawGizmos()
    {
        // 先決定顏色在繪製圖示
        Gizmos.color = new Color(1, 0, 0, 0.3f);    //偵測地板
        Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);

        Gizmos.color = new Color(0.5f, 0.3f, 0.1f, 0.3f);   //攻擊範圍
        Gizmos.DrawCube(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize);

        Gizmos.color = new Color(0, 1, 0, 0.3f);    //衝刺距離
        Gizmos.DrawRay(transform.position, -transform.right * RushDistance);
    }
    #endregion

    #region 函式
    /// <summary>
    /// 取得玩家輸入水平軸向值: 左與右 A, D, 左, 右
    /// </summary>
    private void GetPlayerInputHorizontal()
    {
        // 水平值 = 輸入.取得軸向(軸向名稱)
        // 作用: 取得玩家按下水平按鍵的值, 按右為 1 , 按左為-1, 沒按為 0 
        hValue = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="horizontal">左右數值</param>
    private void Move(float horizontal)
    {
        /** 第一種移動方式: 自訂重力
        // 區域變數: 在方法內的欄位, 有區域性, 僅限於此方法內存取
        // 簡寫: transform 此物件的 Transform 變形元件
        // posMove = 角色當前座標 + 玩家輸入的水平值
        // Time.fixedDeltaTime 指 1/50 秒
        Vector2 posMove = transform.position + new Vector3(horizontal, -gravity, 0) * moveSpeed * Time.fixedDeltaTime;
        // 剛體.移動座標(要前往的座標);
        rig.MovePosition(posMove);
        */

        /* 第二種移動方式: 使用專案內的重力 - 較緩慢 */
        rig.velocity = new Vector2(horizontal + moveSpeed * Time.fixedDeltaTime, rig.velocity.y);

        // 控制走路動畫: 不等於0時勾選
        ani.SetBool("走路", horizontal != 0);
    }

    /// <summary>
    /// 旋轉方向: 處理角色面向問題, 按右角度0, 按左角度180
    /// </summary>
    private void Turndirection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.eulerAngles = Vector3.zero;
        }

    }

    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {
        // Vector2 參數可以使用 Vector3 代入, 程式會自動把 z 軸取消
        // << 位移運算子
        // 指定圖層語法: 1 << 圖層編號
        Collider2D hit = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, 1 << 6);

        if (hit) onFloor = true;
        else onFloor = false;

        //設定動畫參數 與 是否在地板 相反
        ani.SetBool("跳躍", !onFloor);

        // 如果玩家按下空白鍵角色就往上跳躍
        if (onFloor && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rig.AddForce(new Vector2(0, jumpHeight));
        }
    }

    private void Rush(float horizontal)
    {
        if (onFloor && Input.GetKeyDown(KeyCode.C) && !isRush)
        {
            isRush = true;
            ani.SetTrigger("突進");
            transform.position = new Vector3(transform.position.x + horizontal * rushdis, transform.position.y);
        }

        if (isRush)
        {
            if (rushtimer < rushtime)
            {
                rushtimer += Time.deltaTime;
            }
            else
            {
                rushtimer = 0;
                isRush = false;
            }
        }
    }

    /// <summary>
    /// 攻擊
    /// </summary>
    private void Attack()
    {
        // 如果不是攻擊中 並且按下左鍵才可以攻擊 啟動觸發參數
        if (!isAttack && Input.GetKeyDown(KeyCode.X))
        {
            isAttack = true;
            ani.SetTrigger("攻擊");

            Collider2D hit = Physics2D.OverlapBox(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize, 0, 1 << 8);

            if (hit)
            {
                hit.GetComponent<BaseEnemy>().enemyHurt(attack);
            }
        }

        if (isAttack)
        {
            if (timer < cd)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                isAttack = false;
            }
        }
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">造成的傷害</param>
    public void Hurt(float damage)
    {
        HP -= damage;                       //血量扣除傷害值

        if (HP <= 0) Dead();                //如果 血量 <= 0 就死亡

        textHP.text = "HP " + HP;
        imgHP.fillAmount = HP / hpMax;
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        HP = 0;                             //血量歸零
        ani.SetBool("死亡", true);          //死亡動畫
        enabled = false;                    //關閉此腳本
    }

    /// <summary>
    /// 撿道具
    /// </summary>
    /// <param name="propName">道具名稱</param>
    private void EatProp(string propName)
    {

    }

    private void walldetect()
    {
        RaycastHit = Physics2D.Raycast(this.gameObject.transform.position, -transform.right, RushDistance, 1 << 6);

        if (RaycastHit.collider != null)
        {
            if (RaycastHit.distance <= RushDistance)
            {
                rushdis = RaycastHit.distance - 1;
            }
            
        }
        else
        {
            rushdis = RushDistance;
        }
    }

    #endregion
}
