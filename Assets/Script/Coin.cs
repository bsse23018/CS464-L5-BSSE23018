using UnityEngine;

public enum CoinType { Normal, Red, Blue }

public class Coin : MonoBehaviour
{
    [SerializeField] CoinType type = CoinType.Normal;
    [SerializeField] int value = 1;
    [SerializeField] float spinSpeed = 180f;

    // Called by GameManager right after spawning, so no prefab setup is needed
    public void Setup(CoinType newType, Color color)
    {
        type = newType;
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.material.color = color;
    }

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("No GameManager in the scene!");
            return;
        }

        Debug.Log($"Collected {type} coin");
        Destroy(gameObject);

        switch (type)
        {
            case CoinType.Normal: gm.AddScore(value); break;
            case CoinType.Red: gm.CollectRedCoin(); break;
            case CoinType.Blue: gm.CollectBlueCoin(); break;
        }
    }
}