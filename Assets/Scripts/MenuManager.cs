using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class MenuManager : MonoBehaviour
{
    public InputField PlayerNameInput;
    public TMP_Text TopScoreText;

    private BestScoreData bestScoreData;

    private void Start()
    {
        if (PlayerNameInput != null && PlayerData.Instance != null)
        {
            PlayerNameInput.text = PlayerData.Instance.playerName;
            PlayerNameInput.onEndEdit.AddListener(OnPlayerNameInputEdit);
        }
        LoadBestScore();
        UpdateTopScoreText();
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

    private void OnPlayerNameInputEdit(string playerName)
    {
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.playerName = playerName;
            Debug.Log($"Player name set to: {playerName}");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main"); // Replace "main" with the name of your gameplay scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (PlayerNameInput != null)
        {
            PlayerNameInput.onEndEdit.RemoveListener(OnPlayerNameInputEdit);
        }
    }
    [System.Serializable]
    public class BestScoreData
    {
        public string playerName;
        public int score;
    }
}