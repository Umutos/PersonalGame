using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.Rendering;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private WeaponData currentWeapon;
    private WeaponManager weaponManager;

    [SerializeField]
    private Camera cam;

    [SerializeField]   
    LayerMask mask;

    public Material lineMaterial;
    public float lineWidth = 0.1f;
    public float lineDuration = 1.0f;
    private LineRenderer lineRendererInstance;


    // Start is called before the first frame update
    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("Camera Missing in PlayerShoot");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize < currentWeapon.magazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    [Command]
    void CommandOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphcs().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    //When Someone shoot, this funtion is call on the server
    [Command]
    void CommandOnShoot()
    {
        RpcShootEffect();
    }

    //Particuls effect visible for all players
    [ClientRpc]
    void RpcShootEffect()
    {
        weaponManager.GetCurrentGraphcs().muzzleFlash.Play();
    }

    [Client]
    private void Shoot()
    {
        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if(weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        weaponManager.currentMagazineSize--;

        Debug.Log("Rest : " + weaponManager.currentMagazineSize);

        CommandOnShoot();

        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            if(hit.collider.tag == "Player")
            {
                CommandPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            if (hit.collider.tag == "Screen")
            {
                CommandScreenShot(hit);
            }

            CommandOnHit(hit.point, hit.normal);
        }
    }

    [Command]
    private void CommandPlayerShot(string playerId, float damage, string sourceID)
    {
        Debug.Log(playerId + " get touch");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage, sourceID);
    }

    private void CommandScreenShot(RaycastHit hit)
    {
      Camera screenCamera = hit.collider.GetComponent<Screen>().screenCamera;
      Vector3 relativeHitPoint = hit.point - hit.transform.position;

      float quadHalfWidth = hit.transform.localScale.x * 0.5f;
      float quadHalfHeight = hit.transform.localScale.y * 0.5f;
       
      float normalizedU = relativeHitPoint.x / hit.transform.localScale.x;
      float normalizedV = relativeHitPoint.y / hit.transform.localScale.y;

      float u = (normalizedU ) + 0.5f; 
      float v = normalizedV + 0.5f;

      Ray ray = screenCamera.ViewportPointToRay(new Vector3(u, v, 0));
      RaycastHit screenHit;

     if (Physics.Raycast(ray, out screenHit, Mathf.Infinity, mask))
        {
         if (screenHit.collider.tag == "Sphere")
            {
                Debug.Log("Interacted with sphere on screen");
                screenHit.collider.GetComponent<BouncingSphere>().ImpactBounce();
            }
        }
        //Debug raycast
        //DrawRaycastLine(ray.origin, ray.origin + ray.direction * 10f, lineDuration);
        
    }

    private void DrawRaycastLine(Vector3 start, Vector3 end, float duration)
    {

        GameObject lineObject = new GameObject("RaycastLine");
        lineRendererInstance = lineObject.AddComponent<LineRenderer>();

        lineRendererInstance.material = lineMaterial;
        lineRendererInstance.startWidth = lineWidth;
        lineRendererInstance.endWidth = lineWidth;
        lineRendererInstance.positionCount = 2;
        lineRendererInstance.SetPosition(0, start);
        lineRendererInstance.SetPosition(1, end);

        StartCoroutine(HideLineAfterDuration(duration, lineObject));
    }

    private IEnumerator HideLineAfterDuration(float duration, GameObject lineObject)
    {
        yield return new WaitForSeconds(duration);
        if (lineRendererInstance != null)
        {
            Destroy(lineObject);
        }
    }

}
