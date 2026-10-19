using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Main game")]
    [SerializeField] int targetScore = 10;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject winPanel;

    [Header("Special coins")]
    [SerializeField] GameObject coinPrefab;      // your ORIGINAL Coin prefab
    [SerializeField] Transform player;
    [SerializeField] float redSpawnArea = 7f;
    [SerializeField] Color redColor = new Color(0.90f, 0.22f, 0.21f);
    [SerializeField] Color blueColor = new Color(0.12f, 0.53f, 0.90f);

    [Header("Blue bonus coins")]
    [SerializeField] int bonusCoinCount = 50;
    [SerializeField] float bonusMinRadius = 2f;
    [SerializeField] float bonusRadius = 6f;
    [SerializeField] float arenaLimit = 9f;
    [SerializeField] TMP_Text bonusText;

    int score;
    int bonusCollected;
    int blueRemaining;
    int redTriggerScore;
    bool redSpawned;
    bool redPending;

    void Awake() => Instance = this;

    void Start()
    {
        redTriggerScore = Random.Range(1, targetScore);   // 1..9
        Debug.Log($"Red coin will spawn when score reaches {redTriggerScore}");

        if (bonusText != null) bonusText.gameObject.SetActive(false);
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();

        if (!redSpawned && score >= redTriggerScore) SpawnRedCoin();
        TryWin();
    }

    void SpawnRedCoin()
    {
        redSpawned = true;    // only one red coin, ever
        redPending = true;

        Vector3 pos;
        int tries = 0;
        do
        {
            pos = new Vector3(Random.Range(-redSpawnArea, redSpawnArea), 0.5f,
                              Random.Range(-redSpawnArea, redSpawnArea));
        }
        while (Vector3.Distance(pos, player.position) < 3f && ++tries < 20);

        SpawnSpecial(pos, CoinType.Red, redColor);
        Debug.Log("RED coin spawned");
    }

    public void CollectRedCoin()
    {
        redPending = false;
        Vector3 c = player.position;
        float minSq = bonusMinRadius * bonusMinRadius;
        float maxSq = bonusRadius * bonusRadius;

        blueRemaining = bonusCoinCount;
        for (int i = 0; i < bonusCoinCount; i++)
        {
            float t = (i + 0.5f) / bonusCoinCount;
            float r = Mathf.Sqrt(Mathf.Lerp(minSq, maxSq, t));
            float a = i * 2.39996f;   // golden angle: even spread, no overlap

            float x = Mathf.Clamp(c.x + r * Mathf.Cos(a), -arenaLimit, arenaLimit);
            float z = Mathf.Clamp(c.z + r * Mathf.Sin(a), -arenaLimit, arenaLimit);

            SpawnSpecial(new Vector3(x, 0.5f, z), CoinType.Blue, blueColor);
        }
        Debug.Log($"Spawned {bonusCoinCount} BLUE coins");
    }

    public void CollectBlueCoin()
    {
        bonusCollected++;
        blueRemaining--;

        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(true);
            bonusText.text = $"BonusCoins: {bonusCollected}/{bonusCoinCount}";
        }
        TryWin();
    }

    void SpawnSpecial(Vector3 pos, CoinType type, Color color)
    {
        var go = Instantiate(coinPrefab, pos, Quaternion.identity, transform);
        go.GetComponent<Coin>().Setup(type, color);
    }

    void TryWin()
    {
        // No win while the red coin is waiting or blue coins are left
        if (score >= targetScore && !redPending && blueRemaining <= 0) Win();
    }

    void UpdateUI() => scoreText.text = $"Score: {score} / {targetScore}";

    void Win() { winPanel.SetActive(true); Time.timeScale = 0f; }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}