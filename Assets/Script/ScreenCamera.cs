using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCamera : MonoBehaviour
{
    public Transform quadTransform;
    private Camera _camera; 

    void Start()
    {
        if (!quadTransform)
        {
            Debug.LogError("Quad Transform not assigned!");
            return;
        }

        _camera = GetComponent<Camera>(); 

        float quadAspectRatio = quadTransform.localScale.x / quadTransform.localScale.y;

        _camera.aspect = quadAspectRatio; 
    }

    void Update()
    {
        
    }
}
