using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform caster;
    public GameObject slashPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            cast();
        }
    }

    private void cast()
    {
        Instantiate(slashPrefab, caster.position, caster.rotation);
    }
}
