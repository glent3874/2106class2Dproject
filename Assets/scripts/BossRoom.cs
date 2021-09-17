using UnityEngine;

public class BossRoom : MonoBehaviour
{
    #region ���
    [Header("�ж��첾�P�j�p")]
    public Vector2 checkRoomOffset;
    public Vector2 checkRoomSize;

    public Collider2D hit;
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

        }
    }
    #endregion
}
