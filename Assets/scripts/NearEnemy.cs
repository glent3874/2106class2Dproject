using UnityEngine;
using System.Collections; // �ޥ� �t��.���X - ��P�{��
using System.Linq;

/// <summary>
/// ��Ԩ�
/// </summary>
//���O: �����O
//: �_���᭱�Ĥ@�ӥN���O�n�~�Ӫ����O
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
    /// <summary>
    /// �ˬd���a�O�_�i�J�����ϰ�
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

        StartCoroutine(DelaySendDamageToPlayer());  //�Ұʨ�P�{��
    }

    //��P�{�ǥΪk:
    //1.�ޥ� System.collections API
    //2.�Ǧ^��k, �Ǧ^������ IEnumerator
    //3.�ϥ� StartCoroutine() �Ұʨ�P�{��
    /// <summary>
    /// ����N�ˮ`�ǵ����a
    /// </summary>
    private IEnumerator DelaySendDamageToPlayer()
    {
        yield return new WaitForSeconds(attackDelayFirst);
        if (hit) player.Hurt(attack);

        //���ݧ�����^�_�쥻���A�ɶ� - �����ʵe�̫᪺�ɶ�
        yield return new WaitForSeconds(afterAttackRestoreOriginal);
        //�p�G���a�٦b�����ϰ줺�N�����_�h�N����
        if (hit) state = StateEnemy.attack;
        else state = StateEnemy.walk;
    }
    #endregion
}
