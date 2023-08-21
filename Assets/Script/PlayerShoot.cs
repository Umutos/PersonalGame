using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]   
    LayerMask mask;


    // Start is called before the first frame update
    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("Camera Missing in PlayerShoot");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
            Debug.DrawRay(cam.transform.position, cam.transform.forward * weapon.range, Color.green);
        }
    }

    [Client]
    private void Shoot()
    {
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            if(hit.collider.tag == "Player")
            {
                CommandPlayerShot(hit.collider.name);
            }
        }
    }

    [Command]
    private void CommandPlayerShot(string playerName)
    {
        Debug.Log(playerName + " get touch");
    }

}
