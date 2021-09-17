using UnityEngine;

/// <summary>
/// ��v���l�ܥؼ�
/// </summary>
public class CameraControl: MonoBehaviour
{
    #region ���
    [Header("Camera�j�p")]
    public float cameraSize;
    [Header("�l�ܳt��"), Range(0, 100)]
    public float speed = 10;
    [Header("�n�l�ܪ�����W��")]
    public string nameTarget;
    [Header("���k����")]
    public Vector2 limitHorizontal;
    [Header("�W�U����")]
    public Vector2 limitVertical;

    /// <summary>
    /// �n�l�ܪ��ؼ�
    /// </summary>
    private Transform target;

    #endregion

    #region �ƥ�
    private void Start()
    {
        //��ĳ�bstart�ϥ�
        //�ؼ�tranform���� = GameObject.Find(�ؼЦW��).transform 
        target = GameObject.Find(nameTarget).transform;

        Camera.main.orthographicSize = cameraSize;
    }

    private void LateUpdate()
    {
        Track();
    }
    #endregion

    #region �禡
    /// <summary>
    /// �l�ܥؼ�
    /// </summary>
    private void Track()
    {
        Vector3 poscamera = transform.position;    //A�I: ��v���y��
        Vector3 posTarget = target.position;       //B�I: �ؼЪ��y��

        Vector3 posResult = Vector3.Lerp(poscamera, posTarget, speed * Time.deltaTime);
        //2D�C����v���w�]-10
        posResult.z = -10;

        posResult.x = Mathf.Clamp(posResult.x, limitHorizontal.x, limitHorizontal.y);
        posResult.y = Mathf.Clamp(posResult.y, limitVertical.x, limitVertical.y);

        //�����󪺮y�� ���w�� �B��᪺�y��
        transform.position = posResult;
    }
    #endregion
}
