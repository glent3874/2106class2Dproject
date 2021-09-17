using UnityEngine;
using UnityEngine.UI;

public class BossRoom : MonoBehaviour
{
    #region ���:���}
    [Header("�ж��첾�P�j�p")]
    public Vector2 checkRoomOffset;
    public Vector2 checkRoomSize;

    [Header("Camera���ʳt�׻P�Y��t��")]
    public float movespeed;
    public float zoomspeed;


    [Header("�i�J�B���Layer7������")]
    public Collider2D hit;
    #endregion

    #region ���:�p�H
    private Transform lockcamera;
    private Camera Cam;
    #endregion

    #region �ƥ�
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(130,255,255,0.3f);
        Gizmos.DrawCube(
            transform.position +
            transform.right * checkRoomOffset.x +
            transform.up * checkRoomOffset.y,
            checkRoomSize);
    }
    private void Start()
    {
        lockcamera = GameObject.Find("Main Camera").GetComponent<Transform>();
        Cam = Camera.main;
    }
    private void Update()
    {
        CheckPlayerEnterRoom();
    }
    #endregion

    #region �禡
    private void CheckPlayerEnterRoom()
    {
        hit = Physics2D.OverlapBox(
            transform.position +
            transform.right * checkRoomOffset.x +
            transform.up * checkRoomOffset.y,
            checkRoomSize, 0, 1 << 7);
        if(hit)
        {
            lockcamera.position = Vector3.Lerp(Cam.transform.position, transform.position, movespeed * Time.deltaTime);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 13, zoomspeed);
        }
        else
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 7.5f, zoomspeed);
        }
    }
    #endregion
}
