using UnityEngine;
using Mirror;
using Mirror.Examples.AdditiveLevels;

[RequireComponent(typeof(Player))]
[RequireComponent (typeof(P_Controller))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentToDisable;

    [SerializeField]
    private string enemiLayerName = "EnemiPlayer";

    [SerializeField]
    private string dontDrawLayerName = "DontDraw";

    [SerializeField]
    private GameObject playerGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignEnemiPlayer();
        }
        else
        {
            //Desable our player graphics for camera
            Utils.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Create local player UI 
            playerUIInstance = Instantiate(playerUIPrefab);

            GetComponent<Player>().Setup();

        }
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
        gameObject.layer = LayerMask.NameToLayer(enemiLayerName);
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
        Destroy(playerUIInstance);

        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCamActive(true);
        }

        GameManager.UnregisterPlayer(transform.name);
    }
}
