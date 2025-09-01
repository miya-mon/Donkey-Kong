// Assets/Scripts/GameController.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("UI")]
    public Text scoreText;   // StartScene では null OK
    public Text livesText;   // 同上
    public Text timerText;   // 同上

    [Header("Prefabs")]
    public GameObject barrelPrefab;
    public GameObject fireballPrefab;

    [Header("Barrel Spawn Positions")]
    [Tooltip("ステージごとの樽生成位置を、StageList.Names の順で設定")]
    public Vector2[] barrelSpawnPositions;

    [Header("Barrel Spawn Direction")]
    [Tooltip("ステージごとに、樽の初期移動向きをランダムにするか (true) 右固定にするか (false) を設定")]
    public bool[] barrelRandomFlags;

    [Header("Barrel Spawn Interval")]
    [Tooltip("ステージごとの樽発射間隔（秒）を、StageList.Names の順で設定")]
    public float[] barrelSpawnIntervals;

    [Header("Settings")]
    public int timeBonusPerSecond = 50;
    public int initialLives = 3;
    public float initialStageTime = 99f;

    private bool isPlaying;
    private int score;
    private int lives;
    private float stageTimer;
    private float barrelTimer;
    private float currentBarrelInterval;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // シーン切り替え時に UI と BarrelInterval を自動バインド
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Loaded scene: {scene.name}");

        scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        livesText = GameObject.Find("LivesText")?.GetComponent<Text>();
        timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
        UpdateUI();

        StartCoroutine(ActivateStageRootsNextFrame(scene));

        // 現在ステージの樽間隔をセット
        string current = scene.name;
        int idx = Array.IndexOf(StageList.Names, current);
        if (idx >= 0 && idx < barrelSpawnIntervals.Length)
            currentBarrelInterval = barrelSpawnIntervals[idx];
        else
            currentBarrelInterval = 3f;  // フォールバック
        barrelTimer = 0f;
    }


    private IEnumerator ActivateStageRootsNextFrame(Scene scene)
    {
        yield return null; // 次フレームまで待つ
        ActivateStageRoots(scene);
    }

    /// <summary>
    /// 読み込んだシーンのルートオブジェクトを走査し、
    /// 条件に合うものを active にする。
    /// 条件：StageRootMarker コンポーネント、または名前が "StageRoot" で始まる、またはタグが "StageRoot"
    /// </summary>
    private void ActivateStageRoots(Scene scene)
    {
        // 例：ステージ名配列 StageList.Names に含まれるシーンだけ処理する（誤活性化防止）
        if (Array.IndexOf(StageList.Names, scene.name) < 0)
        {
            Debug.Log($"[GameController] Scene '{scene.name}' is not a stage. Skip ActivateStageRoots.");
            return;
        }

        var rootObjects = scene.GetRootGameObjects();
        int activatedCount = 0;

        foreach (var go in rootObjects)
        {
            bool isMarker = go.GetComponent<StageRootMarker>() != null;
            bool nameMatches = go.name.StartsWith("StageRoot");
            bool tagMatches = false;
            try { tagMatches = go.tag == "StageRoot"; }
            catch { tagMatches = false; }

            if (isMarker || nameMatches || tagMatches)
            {
                if (!go.activeSelf)
                {
                    go.SetActive(true);
                    activatedCount++;
                }
            }
        }

        Debug.Log($"[GameController] Activated {activatedCount} StageRoot(s) in scene '{scene.name}'.");
    }


    void Update()
    {
        if (!isPlaying) return;

        // タイマー更新
        stageTimer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(stageTimer).ToString();

        if (stageTimer <= 0f)
            PlayerDied();

        // Barrel スポーン
        barrelTimer += Time.deltaTime;
        if (barrelTimer >= currentBarrelInterval)
        {
            barrelTimer = 0f;
            SpawnBarrel();
        }
    }

    public void StartGame()
    {
        score = 0;
        lives = initialLives;
        stageTimer = initialStageTime;
        isPlaying = true;
        UpdateUI();
        SceneManager.LoadScene(StageList.Names[0]);
    }

    private void SpawnBarrel()
    {
        string current = SceneManager.GetActiveScene().name;
        int idx = Array.IndexOf(StageList.Names, current);

        // 生成位置
        Vector3 spawnPos = Vector3.zero;
        if (idx >= 0 && idx < barrelSpawnPositions.Length)
            spawnPos = new Vector3(barrelSpawnPositions[idx].x, barrelSpawnPositions[idx].y, 0f);

        var go = Instantiate(barrelPrefab, spawnPos, Quaternion.identity);

        // 向きフラグを設定
        if (idx >= 0 && idx < barrelRandomFlags.Length)
        {
            var barrel = go.GetComponent<Barrel>();
            if (barrel != null)
                barrel.isRandom = barrelRandomFlags[idx];
        }
    }

    public void SpawnFireball(Vector3 pos)
    {
        Instantiate(fireballPrefab, pos, Quaternion.identity);
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (isPlaying) UpdateUI();
    }

    public void PlayerDied()
    {
        lives--;
        UpdateUI();
        if (lives <= 0)
            SceneManager.LoadScene("GameOver");
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StageClear()
    {
        isPlaying = false;
        int bonus = Mathf.CeilToInt(stageTimer) * timeBonusPerSecond;
        AddScore(bonus);
       // AudioSource.PlayClipAtPoint(
       //     Resources.Load<AudioClip>("Sounds/StageClear"),
       //     Camera.main.transform.position
       // );
        StartCoroutine(ClearAndLoadNext());
    }

    private IEnumerator ClearAndLoadNext()
    {
        yield return new WaitForSeconds(1f);
        LoadNextStageByName();
        isPlaying = true;
    }

    private void LoadNextStageByName()
    {
        string current = SceneManager.GetActiveScene().name;
        int idx = Array.IndexOf(StageList.Names, current);
        if (idx < 0) idx = 0;
        int next = idx + 1;
        if (next < StageList.Names.Length)
            SceneManager.LoadScene(StageList.Names[next]);
        else
            SceneManager.LoadScene(StageList.Names[StageList.Names.Length - 1]);
    }

    public void ReloadStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isPlaying = true;
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString("D6");
        if (livesText != null) livesText.text = "×" + lives;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
