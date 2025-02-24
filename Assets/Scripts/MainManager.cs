using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO; // Added for file I/O

public class MainManager : MonoBehaviour
{

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public InputField PlayerNameInput;
    public Text ScoreText;
    public GameObject GameOverText;
    public Text TopScoreText; // Added to display top score on start menu

    private bool m_Started = false;
    private int m_Points;
    private string m_PlayerName;
    private bool m_GameOver = false;
    private BestScoreData bestScoreData; // Added to store best score data

    void Start()
    {
        InitializeBricks();
        InitializePlayerName();
        LoadBestScore();
        UpdateScoreText();
        UpdateTopScoreText(); // Added to display top score on start menu
    }

    private void UpdateTopScoreText()
    {
        if (TopScoreText != null)
        {
            TopScoreText.text = $"Best Score: {bestScoreData.playerName} : {bestScoreData.score}";
        }
    }

    private void LoadBestScore()
    {
        string path = Application.persistentDataPath + "/bestscore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            bestScoreData = JsonUtility.FromJson<BestScoreData>(json);
        }
        else
        {
            bestScoreData = new BestScoreData { playerName = "None", score = 0 };
        }
    }

    private void InitializeBricks()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void InitializePlayerName()
    {
        if (PlayerData.Instance != null)
        {
            m_PlayerName = PlayerData.Instance.playerName;
            Debug.Log($"Player name initialized: {m_PlayerName}");
        }
        else
        {
            m_PlayerName = "Unknown";
            Debug.LogWarning("PlayerData.Instance is null. Player name set to 'Unknown'.");
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartBallMovement();
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartGame();
            }
        }
    }

    private void StartBallMovement()
    {
        m_Started = true;
        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDir = new Vector3(randomDirection, 1, 0);
        forceDir.Normalize();

        Ball.transform.SetParent(null);
        Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void AddPoint(int point)
    {
        m_Points += point;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        ScoreText.text = $"{m_PlayerName} Score: {m_Points}";

    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveBestScore();
    }

    private void SaveBestScore()
    {
        if (m_Points > bestScoreData.score)
        {
            bestScoreData.playerName = m_PlayerName;
            bestScoreData.score = m_Points;

            string json = JsonUtility.ToJson(bestScoreData);
            string path = Application.persistentDataPath + "/bestscore.json";
            File.WriteAllText(path, json);
        }
    }
}
