using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "My Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string name   = "Gun";
    public float  damage = 1f;
    public float  range  = 100f;

    public float fireRate = 0f;

    public int magazineSize = 10;

    public float timeReload = 1f;

    public GameObject graphcs;
}
