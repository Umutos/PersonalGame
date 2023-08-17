using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float mouseSensitivityX = 3f;
    [SerializeField]
    private float mouseSensitivityY = 3f;

    private PlayerMotor motor;

    private void Start()
    { 
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        //Calcul Velocity
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHoriz = transform.right * xMov;
        Vector3 moveVert = transform.forward * zMov;

        Vector3 velocity = (moveHoriz + moveVert).normalized * speed;

        motor.Move(velocity);

        //Player Rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;
        Vector3 camRotation = new Vector3(xRot, 0, 0) * mouseSensitivityY;

        motor.Rotation(rotation);
        motor.CamRotation(camRotation);
    }
}
