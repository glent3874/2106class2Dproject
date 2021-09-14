using UnityEngine;
using System.Collections; // 引用 系統.集合 - 協同程序
using System.Linq;

/// <summary>
/// 近戰角
/// </summary>
//類別: 父類別
//: 冒號後面第一個代表的是要繼承的類別
public class NearEnemy : BaseEnemy
{
    #region 欄位
    [Header("攻擊區域的位移與大小")]
    public Vector2 checkAttackOffset;
    public Vector2 checkAttackSize;
    #endregion

    #region 事件
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = new Color(0.5f, 0.3f, 0.1f, 0.3f);
        Gizmos.DrawCube(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize);
    }

    protected override void Update()
    {
        base.Update();

        CheckPlayerInAttackArea();
    }
    #endregion

    #region 函式
    /// <summary>
    /// 檢查玩家是否進入攻擊區域
    /// </summary>
    private void CheckPlayerInAttackArea()
    {
        hit = Physics2D.OverlapBox(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize, 0, 1 << 7);

        if (hit) state = StateEnemy.attack;
    }

    protected override void AttackMethod()
    {
        base.AttackMethod();

        StartCoroutine(DelaySendDamageToPlayer());  //啟動協同程序
    }

    //協同程序用法:
    //1.引用 System.collections API
    //2.傳回方法, 傳回類型為 IEnumerator
    //3.使用 StartCoroutine() 啟動協同程序
    /// <summary>
    /// 延遲將傷害傳給玩家
    /// </summary>
    private IEnumerator DelaySendDamageToPlayer()
    {
        yield return new WaitForSeconds(attackDelayFirst);
        if (hit) player.Hurt(attack);

        //等待攻擊後回復原本狀態時間 - 攻擊動畫最後的時間
        yield return new WaitForSeconds(afterAttackRestoreOriginal);
        //如果玩家還在攻擊區域內就攻擊否則就走路
        if (hit) state = StateEnemy.attack;
        else state = StateEnemy.walk;
    }
    #endregion
}
