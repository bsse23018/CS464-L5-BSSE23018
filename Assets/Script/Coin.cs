using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] float spinSpeed = 180f;

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Coin touched by {other.name}");
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("No GameManager in the scene!");
            return;
        }

        GameManager.Instance.AddScore(value);
        Destroy(gameObject);
    }
}