using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public static bool gameOver;

    public TMP_Text textbox;
    public string winText = "LEVEL COMPLETE!\nPress N to continue";

    public GameObject player1;
    public GameObject player2;

    public AudioClip winSound;
    public string nextSceneName;
    public bool fallbackToBuildOrder = true;

    bool player1InGoal;
    bool player2InGoal;
    AudioSource audioSource;

    void Start()
    {
        gameOver = false;
        player1InGoal = false;
        player2InGoal = false;

        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1f;

        if (textbox) textbox.text = "";
    }

    void Update()
    {
        if (!gameOver)
        {
            if (player1InGoal && player2InGoal)
            {
                if (audioSource && winSound) audioSource.PlayOneShot(winSound);
                gameOver = true;
                Time.timeScale = 0f;
                if (textbox) textbox.text = winText;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.N))
            LoadNextLevel();
    }

    void LoadNextLevel()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        if (!fallbackToBuildOrder) return;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextIndex = currentIndex + 1;
        if (nextIndex >= totalScenes) nextIndex = 0;

        SceneManager.LoadScene(nextIndex);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player1) player1InGoal = true;
        else if (other.gameObject == player2) player2InGoal = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player1) player1InGoal = false;
        else if (other.gameObject == player2) player2InGoal = false;
    }
}
