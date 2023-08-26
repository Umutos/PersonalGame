using UnityEngine;
using Mirror;

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
    private GameObject playerUIInstance;

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

            //Desable our player graphics for camera
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Create local player UI 
            playerUIInstance = Instantiate(playerUIPrefab);

        }

        GetComponent<Player>().Setup();
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
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

        if(sceneCam != null)
        {
            sceneCam.gameObject.SetActive(true);   
        }

        GameManager.UnregisterPlayer(transform.name);
    }
}
