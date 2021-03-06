using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BossRoom : MonoBehaviour
{
    #region 欄位:公開
    [Header("房間位移與大小")]
    public Vector2 checkRoomOffset;
    public Vector2 checkRoomSize;

    [Header("Camera移動速度與縮放速度")]
    public float movespeed;
    public float zoomspeed;

    [Header("進入且位於Layer7的物件")]
    public Collider2D hit;

    [Header("房間的主人")]
    public GameObject bossName;

    [Header("過關事件")]
    public UnityEvent onPass;
    #endregion

    #region 欄位:私人
    private Transform lockcamera;
    private Camera Cam;
    private SpriteRenderer bossroombackground;

    private bool bossdefeat = false;
    #endregion

    #region 事件
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
        bossroombackground = GameObject.Find("BossRoomBackground").GetComponent<SpriteRenderer>();
        Cam = Camera.main;
        
    }
    private void Update()
    {
        CheckPlayerEnterRoom();
        gameovercheck();
    }
    #endregion

    #region 函式
    private void CheckPlayerEnterRoom()
    {
        Color colorA = new Color(bossroombackground.color.r, bossroombackground.color.g, bossroombackground.color.b, 1);
        Color colorB = new Color(bossroombackground.color.r, bossroombackground.color.g, bossroombackground.color.b, 0);

        hit = Physics2D.OverlapBox(
            transform.position +
            transform.right * checkRoomOffset.x +
            transform.up * checkRoomOffset.y,
            checkRoomSize, 0, 1 << 7);
        if(hit && bossName)
        {
            lockcamera.position = Vector3.Lerp(Cam.transform.position, transform.position, movespeed * Time.deltaTime);
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 13, zoomspeed);
            bossroombackground.color = Color.Lerp(bossroombackground.color, colorA, 10 * Time.deltaTime);
            bossName.GetComponent<BossEnemy>().PlayerEnterRoom = true;
        }
        else
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 7.5f, zoomspeed);
            bossroombackground.color = Color.Lerp(bossroombackground.color, colorB, 10 * Time.deltaTime);
        }
    }

    private void gameovercheck()
    {
        if(!bossName)
        {
            if (bossdefeat == false)
            {
                bossdefeat = true;
                onPass.Invoke();
            }
        }
    }
    #endregion
}
