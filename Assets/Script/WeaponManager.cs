using UnityEngine;
using Mirror;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private WeaponData primaryWeapon;

    private WeaponData currentWeapon;
    private WeaponGraphcs currentGraphcs;

    [SerializeField] 
    private Transform weaponHolder;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [HideInInspector]
    public int currentMagazineSize;

    public bool isReloading = false;

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphcs GetCurrentGraphcs()
    {
        return currentGraphcs;
    }

    void EquipWeapon(WeaponData _weapon)
    {
        currentWeapon = _weapon;
        currentMagazineSize = _weapon.magazineSize;

        GameObject weaponInst = Instantiate(_weapon.graphcs, weaponHolder.position, weaponHolder.rotation);
        weaponInst.transform.SetParent(weaponHolder);

        currentGraphcs = weaponInst.GetComponent<WeaponGraphcs>();

        if (currentGraphcs == null)
        {
            Debug.LogError("No WeaponGraphcs scrips in this weapon : " + weaponInst.name);
        }

        if(isLocalPlayer)
        {
            Utils.SetLayerRecursively(weaponInst, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public IEnumerator Reload()
    {
        if(isReloading)
        {
            yield break;
        }

        Debug.Log("reloading...");

        isReloading = true;

        CommandOnReload();
        yield return new WaitForSeconds(currentWeapon.timeReload);
        currentMagazineSize = currentWeapon.magazineSize;

        isReloading = false;

        Debug.Log("reload finish");
    }

    [Command]
    void CommandOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator animator = currentGraphcs.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }
    }


}
