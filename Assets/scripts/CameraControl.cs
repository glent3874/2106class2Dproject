using UnityEngine;

/// <summary>
/// 攝影機追蹤目標
/// </summary>
public class CameraControl: MonoBehaviour
{
    #region 欄位
    [Header("Camera大小")]
    public float cameraSize;
    [Header("追蹤速度"), Range(0, 100)]
    public float speed = 10;
    [Header("要追蹤的物件名稱")]
    public string nameTarget;
    [Header("左右限制")]
    public Vector2 limitHorizontal;
    [Header("上下限制")]
    public Vector2 limitVertical;

    /// <summary>
    /// 要追蹤的目標
    /// </summary>
    private Transform target;

    #endregion

    #region 事件
    private void Start()
    {
        //建議在start使用
        //目標tranform元件 = GameObject.Find(目標名稱).transform 
        target = GameObject.Find(nameTarget).transform;

        Camera.main.orthographicSize = cameraSize;
    }

    private void LateUpdate()
    {
        Track();
    }
    #endregion

    #region 函式
    /// <summary>
    /// 追蹤目標
    /// </summary>
    private void Track()
    {
        Vector3 poscamera = transform.position;    //A點: 攝影機座標
        Vector3 posTarget = target.position;       //B點: 目標物座標

        Vector3 posResult = Vector3.Lerp(poscamera, posTarget, speed * Time.deltaTime);
        //2D遊戲攝影機預設-10
        posResult.z = -10;

        posResult.x = Mathf.Clamp(posResult.x, limitHorizontal.x, limitHorizontal.y);
        posResult.y = Mathf.Clamp(posResult.y, limitVertical.x, limitVertical.y);

        //此物件的座標 指定為 運算後的座標
        transform.position = posResult;
    }
    #endregion
}
