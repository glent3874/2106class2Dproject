using UnityEngine;

public class player : MonoBehaviour
{
    #region ���
    [Header("���ʳt��"), Range(0, 500)]
    public float moveSpeed;
    [Header("���D����"), Range(0, 10000)]
    public int jumpHeight = 3000;
    [Header("��q"), Range(0, 20000)]
    public float HP = 100;
    [Header("�O�_�b�a�O�W")]
    public bool onFloor = false;

    private AudioSource aud;
    private Rigidbody2D rig;
    private Animator ani;
    /// <summary>
    /// ���a������J�ƭ�
    /// </summary>
    private float hValue;
    #endregion

    #region �ƥ�
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

    [Header("�ˬd�a�O�ϰ�: �첾�P�b�|")]
    public Vector3 groundOffset;
    [Range(0, 2)]
    public float groundRadius = 0.5f;

    private void OnDrawGizmos()
    {
        // ���M�w�C��bø�s�ϥ�
        Gizmos.color = new Color(1, 0, 0, 0.3f);    // �b�z������
        Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);   // ø�s�y��(�����I, �b�|)
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
        //print("���a������: " + hValue);
    }
    [Header("���O"), Range(0.01f, 1)]
    public float gravity = 1;

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

        /** �ĤG�ز��ʤ覡: �ϥαM�פ������O - ���w�C */
        rig.velocity = new Vector2(horizontal + moveSpeed * Time.fixedDeltaTime, rig.velocity.y);

        // ������ʵe: ������0�ɤĿ�
        ani.SetBool("����", horizontal != 0);
    }

    /// <summary>
    /// �����V: �B�z���⭱�V���D, ���k����0, ��������180
    /// </summary>
    private void Turndirection()
    {
        // �p�G���a��D�N�N���׳]�w��(0,0,0)
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        // �p�G���a��A�N�N���׳]�w��(0,180,0)
        if (Input.GetKeyDown(KeyCode.A))
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
        if (onFloor && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(new Vector2(0, jumpHeight));
        }
    }

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

    /// <summary>
    /// ����
    /// </summary>
    private void Attack()
    {
        // �p�G���O������ �åB���U����~�i�H���� �Ұ�Ĳ�o�Ѽ�
        if (!isAttack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttack = true;
            ani.SetTrigger("����");
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

    }
    /// <summary>
    /// ���`
    /// </summary>
    private void Dead()
    {

    }
    /// <summary>
    /// �߹D��
    /// </summary>
    /// <param name="propName">�D��W��</param>
    private void EatProp(string propName)
    {

    }

    #endregion
}
