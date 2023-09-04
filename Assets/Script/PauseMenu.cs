using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PauseMenu : NetworkBehaviour
{
    public static bool isOn = false;

    private NetworkManager networkManager;

    public void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoomButton()
    {
        if(isClientOnly)
        {
            networkManager.StopClient();
        }
        else
        {
            networkManager.StopHost();
        }
    }
}
