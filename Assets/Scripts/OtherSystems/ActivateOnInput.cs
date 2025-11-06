using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnInput : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject targetObject;   
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab; 
    [SerializeField] private bool startActive = false; 

    private void Start()
    {
        if (targetObject != null)
            targetObject.SetActive(startActive);
    }

    private void Update()
    {
        if (targetObject == null)
            return;

        if (Input.GetKeyDown(toggleKey))
        {
            bool isActive = targetObject.activeSelf;
            targetObject.SetActive(!isActive);
        }
    }
}