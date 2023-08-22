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

        RegisterPlayer();
    }

    private void RegisterPlayer()
    {
        //Add player id
        string playerName = "Player" + GetComponent<NetworkIdentity>().netId;
        transform.name = playerName;
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
    }
}
