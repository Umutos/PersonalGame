using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
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

    public int kills;
    public int deaths;

    [SyncVar]
    private float currentLife;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    private bool[] wasEnableOnStart;

    [SerializeField]
    private GameObject deathParticules;

    public NetworkAnimator netAnim;

    private PlayerMotor motor;

    private bool firstSetup = true;

    public void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    public void Setup()
    {
        if(isLocalPlayer)
        {
            //Change camera
            GameManager.instance.SetSceneCamActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        CommandBroadcastNewPlayerSetup();
    }

    [Command(requiresAuthority = false)]
    private void CommandBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnableOnStart = new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnableOnStart[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;

        }
        
        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;
        currentLife = maxLife;

        //Enable Component when player revive
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnableOnStart[i];
        }

        //Enable GameObject when player revive
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        //Reactive player collider
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }

        
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        Setup();

        netAnim.animator.SetBool("Death", false);
    }

    private void Update()
    {
        if(!isLocalPlayer) 
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(999, "Player");
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount, string sourceID)
    {
        if(isDead) 
        {
            return;
        }

        currentLife -= amount;
        Debug.Log(transform.name + " Life : " + currentLife);
        if (currentLife <= 0) 
        {
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(transform.name, sourcePlayer.name);
        }


        deaths++;

        motor.Move(Vector3.zero);
        motor.Rotation(Vector3.zero);
        motor.CamRotation(0f);

        //Desable component when player die
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Desable GameObject when player die
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }

        //Desable player collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        netAnim.animator.SetBool("Death", true);
            
        //Death particules
        GameObject gfxIns = Instantiate(deathParticules, transform.position, Quaternion.identity);
        Destroy(gfxIns, 2f);

        //Change camera
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCamActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is dead");

        StartCoroutine(Respawn());
    }
}
