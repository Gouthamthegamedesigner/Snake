using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AppleCollectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if collision is with player
        if (!other.TryGetComponent<CharacterController>(out _)) 
            return;

        // Make player grow
        if (other.TryGetComponent<BodySegmentManager>(out var bodyManager))
        {
            bodyManager.AddSegment();
        }

        // Destroy apple
        Destroy(gameObject);
        
        // Tell spawner to make new apple
        FindObjectOfType<AppleSpawner>()?.OnAppleCollected();
    }
}