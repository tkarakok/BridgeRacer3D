using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive = false;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;
    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startMoneyText, gameOverMoneyText, finishMoneyText;
    int currentLevel;
    public int score = 0;
    private AudioSource gameMusicAudioSource;
    public AudioClip victorySound, gameOverSound;
    public DailyReward dailyReward;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)
        {
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            dailyReward.InitializeDailyReward();
            int money = PlayerPrefs.GetInt("money");
            startMoneyText.text = money.ToString();
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
        }
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();
        UpdateMoneyText();
    }
    private void Update()
    {
        if (gameActive)
        {
            PlayerController player = PlayerController.Current;
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }
    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running", true);
        gameActive = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1));
    }

    public void GameOver()
    {
        UpdateMoneyText();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverSound);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()
    {
        GiveMoneyToPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victorySound);
        PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;
    }

    public void ChangeScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }

    public void UpdateMoneyText()
    {
        int money = PlayerPrefs.GetInt("money");
        startMoneyText.text = money.ToString();
        gameOverMoneyText.text = money.ToString();
        finishMoneyText.text = money.ToString();
    }

    public void GiveMoneyToPlayer(int increment)
    {
        int money = PlayerPrefs.GetInt("money");
        money += Mathf.Max(0,money + increment);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyText();
    }

}
