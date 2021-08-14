using UnityEngine;

public class player : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 500)]
    public float moveSpeed;
    [Header("跳躍高度"), Range(0, 10000)]
    public int jumpHeight = 3000;
    [Header("血量"), Range(0, 20000)]
    public float HP = 100;
    [Header("是否在地板上")]
    public bool onFloor = false;

    private AudioSource aud;
    private Rigidbody2D rig;
    private Animator ani;
    /// <summary>
    /// 玩家水平輸入數值
    /// </summary>
    private float hValue;
    #endregion

    #region 事件
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }
    private void Update()
    {
        GetPlayerInputHorizontal();
        Turndirection();
        Jump();
        Attack();
    }
    private void FixedUpdate()
    {
        Move(hValue * 5);
    }

    [Header("檢查地板區域: 位移與半徑")]
    public Vector3 groundOffset;
    [Range(0, 2)]
    public float groundRadius = 0.5f;

    private void OnDrawGizmos()
    {
        // 先決定顏色在繪製圖示
        Gizmos.color = new Color(1, 0, 0, 0.3f);    // 半透明紅色
        Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);   // 繪製球體(中心點, 半徑)
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
        //print("玩家水平值: " + hValue);
    }
    [Header("重力"), Range(0.01f, 1)]
    public float gravity = 1;

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

        /** 第二種移動方式: 使用專案內的重力 - 較緩慢 */
        rig.velocity = new Vector2(horizontal + moveSpeed * Time.fixedDeltaTime, rig.velocity.y);

        // 控制走路動畫: 不等於0時勾選
        ani.SetBool("走路", horizontal != 0);
    }

    /// <summary>
    /// 旋轉方向: 處理角色面向問題, 按右角度0, 按左角度180
    /// </summary>
    private void Turndirection()
    {
        // 如果玩家按D就將角度設定為(0,0,0)
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        // 如果玩家按A就將角度設定為(0,180,0)
        if (Input.GetKeyDown(KeyCode.A))
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
        if (onFloor && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(new Vector2(0, jumpHeight));
        }
    }

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

    /// <summary>
    /// 攻擊
    /// </summary>
    private void Attack()
    {
        // 如果不是攻擊中 並且按下左鍵才可以攻擊 啟動觸發參數
        if (!isAttack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttack = true;
            ani.SetTrigger("攻擊");
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

    }
    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {

    }
    /// <summary>
    /// 撿道具
    /// </summary>
    /// <param name="propName">道具名稱</param>
    private void EatProp(string propName)
    {

    }

    #endregion
}
