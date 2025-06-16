using System.Collections.Generic;
using UnityEngine;

public class BodySegmentManager : MonoBehaviour
{
    public GameObject bodyPrefab;
    public float segmentDistance = 1f;
    public float followSpeed = 8f;
    public LayerMask groundLayer;
    
    private List<GameObject> bodySegments = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    private List<float> segmentHeights = new List<float>();

    void Update()
    {
        // Record player's grounded position
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            positionHistory.Insert(0, hit.point);
        }
        else
        {
            positionHistory.Insert(0, transform.position);
        }

        // Move body segments
        for (int i = 0; i < bodySegments.Count; i++)
        {
            if (bodySegments[i] == null) continue;

            // Calculate target position (with ground snapping)
            int targetIndex = Mathf.Min(i * 3, positionHistory.Count - 1);
            Vector3 targetPos = positionHistory[targetIndex] + Vector3.up * segmentHeights[i];

            // Ground check for this segment
            if (Physics.Raycast(targetPos + Vector3.up * 1f, Vector3.down, out RaycastHit segmentHit, 2f, groundLayer))
            {
                targetPos.y = segmentHit.point.y + segmentHeights[i];
            }

            // Smooth movement
            bodySegments[i].transform.position = Vector3.Lerp(
                bodySegments[i].transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );

            // Face movement direction
            if (targetIndex > 0)
            {
                Vector3 lookDir = positionHistory[targetIndex-1] - bodySegments[i].transform.position;
                if (lookDir != Vector3.zero)
                {
                    bodySegments[i].transform.rotation = Quaternion.LookRotation(lookDir);
                }
            }
        }

        // Trim position history
        if (positionHistory.Count > 100)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }

    public void AddSegment()
    {
        GameObject newSegment = Instantiate(bodyPrefab);
        
        // Add ground collider
        var collider = newSegment.GetComponent<Collider>() ?? newSegment.AddComponent<BoxCollider>();
        collider.isTrigger = false;
        
        // Set initial position
        Vector3 spawnPos;
        if (bodySegments.Count > 0)
        {
            Transform last = bodySegments[bodySegments.Count-1].transform;
            spawnPos = last.position - last.forward * segmentDistance;
        }
        else
        {
            spawnPos = transform.position - transform.forward * segmentDistance;
        }

        // Snap to ground
        if (Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 3f, groundLayer))
        {
            spawnPos.y = hit.point.y;
        }

        newSegment.transform.position = spawnPos;
        bodySegments.Add(newSegment);
        segmentHeights.Add(0.5f); // Half unit above ground
    }
}