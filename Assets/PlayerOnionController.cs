using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOnionController : MonoBehaviour
{
    public GameObject _OnionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuild(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnionManager.CreateOnion(transform.position + transform.forward * 2.5f, _OnionPrefab);
        }
    }
}
