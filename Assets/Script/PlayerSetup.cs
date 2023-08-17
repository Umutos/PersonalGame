using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentToDisable;
    Camera sceneCam;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            //Disable other player component
            for (int i = 0; i < componentToDisable.Length; i++)
            {
                componentToDisable[i].enabled = false;
            }
        }
        else
        {
            sceneCam = Camera.main;
            if(sceneCam != null)
            {
                sceneCam.gameObject.SetActive(false);
            }
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
