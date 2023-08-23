using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerShoot : NetworkBehaviour
{
    public PlayerWeapon weapon;

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
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
            //Debug raycast
            //Debug.DrawRay(cam.transform.position, cam.transform.forward * weapon.range, Color.green);
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
                CommandPlayerShot(hit.collider.name, weapon.damage);
            }

            if (hit.collider.tag == "Screen")
            {
                CommandScreenShot(hit);
            }
        }
        //Debug raycast
        //DrawRaycastLine(cam.transform.position, cam.transform.position+ cam.transform.forward* weapon.range, lineDuration);
    }

    [Command]
    private void CommandPlayerShot(string playerId, float damage)
    {
        Debug.Log(playerId + " get touch");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
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
