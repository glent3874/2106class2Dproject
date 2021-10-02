using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    #region ���
    [Header("���ʳt��"), Range(0, 500)]
    public float moveSpeed;
    [Header("���D����"), Range(0, 5000)]
    public int jumpHeight = 3000;
    [Header("�Ĩ�Z��"), Range(0, 15)]
    public int RushDistance;
    private float rushdis;
    [Header("��q"), Range(0, 20000)]
    public float HP = 100;
    [Header("�����O"), Range(0, 1000)]
    public float attack = 20;
    [Header("�O�_�b�a�O�W")]
    public bool onFloor = false;

    private AudioSource aud;                    //���Ĩӷ�
    private Rigidbody2D rig;                    //����
    private Animator ani;                       //�ʵe
    /// <summary>
    /// ���a������J�ƭ�
    /// </summary>
    private float hValue;

    [Header("�����ϰ쪺�첾�P�j�p")]
    public Vector2 checkAttackOffset;
    public Vector2 checkAttackSize;

    [Header("�����N�o"), Range(0, 5)]
    public float cd = 2;
    /// <summary>
    /// �����p�ɾ�
    /// </summary>
    public float timer;
    /// <summary>
    /// �O�_������
    /// </summary>
    private bool isAttack;

    private Text textHP;
    private Image imgHP;

    private float hpMax;

    private Transform weapon;

    [Header("�ˬd�a�O�ϰ�: �첾�P�b�|")]
    public Vector3 groundOffset;
    [Range(0, 2)]
    public float groundRadius = 0.5f;

    [Header("���O"), Range(0.01f, 3)]
    public float gravity = 3;

    public bool isRush;
    private float rushtimer;
    [Header("�Ĩ�N�o"), Range(0, 2)]
    public float rushtime;

    [Header("�������鰻��")]
    public RaycastHit2D RaycastHit;
    #endregion

    #region �ƥ�
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();      //���o����
        ani = GetComponent<Animator>();         //���o�ʵe

        hpMax = HP;

        textHP = GameObject.Find("��q").GetComponent<Text>();
        imgHP = GameObject.Find("���").GetComponent<Image>();

        weapon = GameObject.Find("������X�I").GetComponent<Transform>();

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
    /// �e�X�����d��
    /// </summary>
    private void OnDrawGizmos()
    {
        // ���M�w�C��bø�s�ϥ�
        Gizmos.color = new Color(1, 0, 0, 0.3f);    //�����a�O
        Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);

        Gizmos.color = new Color(0.5f, 0.3f, 0.1f, 0.3f);   //�����d��
        Gizmos.DrawCube(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize);

        Gizmos.color = new Color(0, 1, 0, 0.3f);    //�Ĩ�Z��
        Gizmos.DrawRay(transform.position, -transform.right * RushDistance);
    }
    #endregion

    #region �禡
    /// <summary>
    /// ���o���a��J�����b�V��: ���P�k A, D, ��, �k
    /// </summary>
    private void GetPlayerInputHorizontal()
    {
        // ������ = ��J.���o�b�V(�b�V�W��)
        // �@��: ���o���a���U�������䪺��, ���k�� 1 , ������-1, �S���� 0 
        hValue = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="horizontal">���k�ƭ�</param>
    private void Move(float horizontal)
    {
        /** �Ĥ@�ز��ʤ覡: �ۭq���O
        // �ϰ��ܼ�: �b��k�������, ���ϰ��, �ȭ��󦹤�k���s��
        // ²�g: transform ������ Transform �ܧΤ���
        // posMove = �����e�y�� + ���a��J��������
        // Time.fixedDeltaTime �� 1/50 ��
        Vector2 posMove = transform.position + new Vector3(horizontal, -gravity, 0) * moveSpeed * Time.fixedDeltaTime;
        // ����.���ʮy��(�n�e�����y��);
        rig.MovePosition(posMove);
        */

        /* �ĤG�ز��ʤ覡: �ϥαM�פ������O - ���w�C */
        rig.velocity = new Vector2(horizontal + moveSpeed * Time.fixedDeltaTime, rig.velocity.y);

        // ������ʵe: ������0�ɤĿ�
        ani.SetBool("����", horizontal != 0);
    }

    /// <summary>
    /// �����V: �B�z���⭱�V���D, ���k����0, ��������180
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
    /// ���D
    /// </summary>
    private void Jump()
    {
        // Vector2 �Ѽƥi�H�ϥ� Vector3 �N�J, �{���|�۰ʧ� z �b����
        // << �첾�B��l
        // ���w�ϼh�y�k: 1 << �ϼh�s��
        Collider2D hit = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, 1 << 6);

        if (hit) onFloor = true;
        else onFloor = false;

        //�]�w�ʵe�Ѽ� �P �O�_�b�a�O �ۤ�
        ani.SetBool("���D", !onFloor);

        // �p�G���a���U�ť��䨤��N���W���D
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
            ani.SetTrigger("��i");
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
    /// ����
    /// </summary>
    private void Attack()
    {
        // �p�G���O������ �åB���U����~�i�H���� �Ұ�Ĳ�o�Ѽ�
        if (!isAttack && Input.GetKeyDown(KeyCode.X))
        {
            isAttack = true;
            ani.SetTrigger("����");

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
    /// ����
    /// </summary>
    /// <param name="damage">�y�����ˮ`</param>
    public void Hurt(float damage)
    {
        HP -= damage;                       //��q�����ˮ`��

        if (HP <= 0) Dead();                //�p�G ��q <= 0 �N���`

        textHP.text = "HP " + HP;
        imgHP.fillAmount = HP / hpMax;
    }

    /// <summary>
    /// ���`
    /// </summary>
    private void Dead()
    {
        HP = 0;                             //��q�k�s
        ani.SetBool("���`", true);          //���`�ʵe
        enabled = false;                    //�������}��
    }

    /// <summary>
    /// �߹D��
    /// </summary>
    /// <param name="propName">�D��W��</param>
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
