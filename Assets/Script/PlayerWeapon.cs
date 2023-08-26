using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name   = "Gun";
    public float  damage = 1f;
    public float  range  = 100f;

    public float fireRate = 0f;

    public GameObject graphcs;
}
