using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;

    [SerializeField] 
    private Transform weaponHolder;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject weaponInst = Instantiate(_weapon.graphcs, weaponHolder.position, weaponHolder.rotation);
        weaponInst.transform.SetParent(weaponHolder);

        if(isLocalPlayer)
        {
            SetLayerRecursively(weaponInst, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
