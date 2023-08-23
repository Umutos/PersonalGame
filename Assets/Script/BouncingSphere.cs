using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingSphere : MonoBehaviour
{
    public float speed = 5f;
    public Transform quadTransform;

    private Vector3 direction;
    private Vector3 halfBounds;
    private Vector3 initialPosition;
    private float sphereRadius;
    private Renderer sphereRenderer;

    void Start()
    {
        initialPosition = transform.position;
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        sphereRadius = transform.localScale.x * 0.5f;
        sphereRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 nextPosition = transform.position + direction * speed * Time.deltaTime;
        float relativeX = nextPosition.x - initialPosition.x;
        float relativeY = nextPosition.y - initialPosition.y;

        if (Mathf.Abs(relativeX) > (quadTransform.localScale.x*0.78f) )
        {
            direction.x = -direction.x;
            nextPosition.x = initialPosition.x + (Mathf.Sign(relativeX) * (quadTransform.localScale.x * 0.78f)); 
            AddRandomBounceAngle();
            ChangeColor();
        }
        if (Mathf.Abs(relativeY) > (quadTransform.localScale.y * 0.7f))
        {
            direction.y = -direction.y;
            nextPosition.y = initialPosition.y + (Mathf.Sign(relativeY) * (quadTransform.localScale.y * 0.7f)); 
            AddRandomBounceAngle();
            ChangeColor();
        }

        transform.position = nextPosition;
    }

    void AddRandomBounceAngle()
    {
        Vector2 newDirection = new Vector2(direction.x, direction.y);
        newDirection = Quaternion.Euler(0, 0, Random.Range(-15f, 15f)) * newDirection;
        direction.x = newDirection.x;
        direction.y = newDirection.y;
    }

    public void ImpactBounce()
    {
        direction = new Vector3(-direction.x, Random.Range(-1f, 1f), 0).normalized;
        AddRandomBounceAngle();
        ChangeColor();
        speed++;

    }

    void ChangeColor()
    {
        Color newColor = new Color(Random.value, Random.value, Random.value);
        sphereRenderer.material.color = newColor;
    }
}
