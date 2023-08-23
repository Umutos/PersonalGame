using UnityEngine;
using Mirror;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private float maxLife = 3f;

    [SyncVar]
    private float currentLife;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnableOnStart;

    public void Setup()
    { 
        wasEnableOnStart = new bool[disableOnDeath.Length];
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnableOnStart[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;
        currentLife = maxLife;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnableOnStart[i];
        }

        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    private void Update()
    {
        if(!isLocalPlayer) 
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(10);
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount)
    {
        if(isDead) 
        {
            return;
        }

        currentLife -= amount;
        Debug.Log(transform.name + " Life : " + currentLife);
        if (currentLife <= 0) 
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log(transform.name + " is dead");

        StartCoroutine(Respawn());
    }
}
