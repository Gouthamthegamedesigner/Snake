using UnityEngine;

public class AppleSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject applePrefab;
    public float spawnRadius = 10f; 
    public float appleHeight = 0.5f; // Height above ground
    public LayerMask groundLayer; // Assign your ground layer in Inspector

    private GameObject _currentApple;

    void Start() => SpawnApple();

    public void SpawnApple()
    {
        // Get random position within circle (XZ plane)
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(randomCircle.x, appleHeight, randomCircle.y);

        // Optional: Raycast to ground for precise placement
        if (Physics.Raycast(spawnPos + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            spawnPos.y = hit.point.y + appleHeight;
        }

        _currentApple = Instantiate(applePrefab, spawnPos, Quaternion.identity);
        
        // Ensure trigger exists
        if (!_currentApple.TryGetComponent(out Collider col))
        {
            col = _currentApple.AddComponent<SphereCollider>();
            col.isTrigger = true;
        }
    }

    public void OnAppleCollected() => SpawnApple();
}