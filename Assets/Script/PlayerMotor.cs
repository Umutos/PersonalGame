using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity;
    private Vector3 rotation;
    private float camRotX = 0f;
    private float currentCamRotX = 0f;

    [SerializeField]
    private float camAngle = 85f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotation(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    public void CamRotation(float _camRotX)
    {
        camRotX = _camRotX;
    }

    private void FixedUpdate()
    {
        PerfMov();
        PerfRot();
    }
    private void PerfMov()
    { 
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    private void PerfRot()
    {
        //Calcule Rotate Camera
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        currentCamRotX -= camRotX;
        currentCamRotX = Mathf.Clamp(currentCamRotX, -camAngle, camAngle);

        //Apply rotate
        cam.transform.localEulerAngles = new Vector3(currentCamRotX, 0f, 0f);
    }

}
