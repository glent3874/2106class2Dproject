using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform caster;
    public GameObject slashPrefab;

    [Header("攻擊冷卻"), Range(0, 5)]
    public float cd = 2;
    /// <summary>
    /// 攻擊計時器
    /// </summary>
    public float timer;
    /// <summary>
    /// 是否為攻擊
    /// </summary>
    public bool isAttack;

    private void Update()
    {

        if (!isAttack && Input.GetKeyDown(KeyCode.X))
        {
            isAttack = true;
            cast();
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

    private void cast()
    {
        Instantiate(slashPrefab, caster.position, caster.rotation);
    }
}
