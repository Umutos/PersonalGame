using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentToDisable;

    [SerializeField]
    private string EnemiLayerName = "EnemiPlayer";

    Camera sceneCam;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignEnemiPlayer();
        }
        else
        {
            sceneCam = Camera.main;
            if(sceneCam != null)
            {
                sceneCam.gameObject.SetActive(false);
            }
        }

        GetComponent<Player>().Setup();
    }

    //Start when someone connect server
    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netID, player);
    }

    private void AssignEnemiPlayer() 
    {
        gameObject.layer = LayerMask.NameToLayer(EnemiLayerName);
    }

    private void DisableComponents()
    {
        //Disable other player component
        for (int i = 0; i < componentToDisable.Length; i++)
        {
            componentToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        if(sceneCam != null)
        {
            sceneCam.gameObject.SetActive(true);   
        }

        GameManager.UnregisterPlayer(transform.name);
    }
}
