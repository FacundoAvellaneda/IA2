using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatColliderMouseFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private float radius = 1f;

    private bool shouldUpdate = false;
    private Camera mainCam;

    void Start()
    {
        FindMainCamera();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        shouldUpdate = false;
    }

    void Update()
    {
        if (!shouldUpdate || player == null)
            return;

        if (mainCam == null)
            FindMainCamera();

        if (mainCam != null)
            UpdatePositionAndRotation();
    }

    private void FindMainCamera()
    {
        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogWarning(" No se encontró una cámara con el tag 'MainCamera'.");
    }

    private void UpdatePositionAndRotation()
    {
        // Verificamos otra vez por seguridad
        if (mainCam == null || player == null) return;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = (mouseWorld - player.position).normalized;
        transform.position = (Vector2)player.position + direction * radius;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void PrepareForAttack()
    {
        shouldUpdate = true;
        UpdatePositionAndRotation();
    }

    public void StopFollowing()
    {
        shouldUpdate = false;
    }
}