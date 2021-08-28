using UnityEngine;
using System.Collections; // �ޥ� �t��.���X - ��P�{��

/// <summary>
/// ��Ԩ�
/// </summary>
public class NearEnemy : BaseEnemy
{
    #region ���
    [Header("�����ϰ쪺�첾�P�j�p")]
    public Vector2 checkAttackOffset;
    public Vector2 checkAttackSize;
    #endregion

    #region �ƥ�
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

    #region �禡
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

        StartCoroutine(DelaySendDamageToPlayer());
    }

    private IEnumerator DelaySendDamageToPlayer()
    {
        yield return new WaitForSeconds(attackDelayFirst);
        player.Hurt(attack);
    }
    #endregion
}
