using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class P_Controller : NetworkBehaviour
{
    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float mouseSensitivityX = 3f;
    [SerializeField]
    private float mouseSensitivityY = 3f;

    private PlayerMotor motor;
    private Animator anim;

    public NetworkAnimator netAnim;

    private void Start()
    { 
        motor = GetComponent<PlayerMotor>();
        anim = GetComponent<Animator>();    
    }

    private void Update()
    {
        if(PauseMenu.isOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            motor.Move(Vector3.zero);
            motor.Rotation(Vector3.zero);
            motor.CamRotation(0f);

            return;
        }

        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Calcul Velocity
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHoriz = transform.right * xMov;
        Vector3 moveVert = transform.forward * zMov;

        Vector3 velocity = (moveHoriz + moveVert) * speed;

        //Play animation
        netAnim.animator.SetFloat("ForwardVelocity", zMov);

        motor.Move(velocity);

        //Player Rotation
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;

        motor.Rotation(rotation);

        float xRot = Input.GetAxisRaw("Mouse Y");

        float camRotationX = xRot * mouseSensitivityY;

        motor.CamRotation(camRotationX);
    }
}
