using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerapov : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(-10f, 12f, -10f);

    [Header("Wall Transparency Settings")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Material transparentMaterial;

    // Stores original materials of hidden walls
    private Dictionary<Renderer, Material> hiddenWalls = new Dictionary<Renderer, Material>();
    private List<Renderer> currentWalls = new List<Renderer>();

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Move the camera first
        FollowTarget();

        // 2. Detect blocking walls from the updated camera position
        DetectBlockingWalls();
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    private void DetectBlockingWalls()
    {
        currentWalls.Clear();

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        // Cast a ray from the camera to the target through the "Wall" layer
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, wallLayer);

        // Make blocking walls transparent
        foreach (RaycastHit hit in hits)
        {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
            if (wallRenderer != null)
            {
                currentWalls.Add(wallRenderer);

                if (!hiddenWalls.ContainsKey(wallRenderer))
                {
                    hiddenWalls.Add(wallRenderer, wallRenderer.sharedMaterial);
                    wallRenderer.material = transparentMaterial;
                }
            }
        }

        // Restore walls that are no longer blocking the view
        List<Renderer> wallsToRestore = new List<Renderer>();

        foreach (var pair in hiddenWalls)
        {
            Renderer wallRenderer = pair.Key;
            if (!currentWalls.Contains(wallRenderer))
            {
                if (wallRenderer != null)
                {
                    wallRenderer.material = pair.Value;
                }
                wallsToRestore.Add(wallRenderer);
            }
        }

        // Clean up restored walls from the dictionary
        foreach (Renderer wallRenderer in wallsToRestore)
        {
            hiddenWalls.Remove(wallRenderer);
        }
    }

    // Helper method to automatically calculate the offset in the Editor
    [ContextMenu("Calculate Current Offset")]
    private void CalculateOffset()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }
}