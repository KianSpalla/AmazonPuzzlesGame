using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public static bool gameOver;
    public static string winnerName;

    [Header("UI")]
    public TMP_Text textbox;  // assign in Inspector

    [Header("Co-op Goal Settings")]
    public GameObject player1;        // assign Player 1 in Inspector
    public GameObject player2;        // assign Player 2 in Inspector

    // Are the players standing in the goal area?
    private bool player1InGoal;
    private bool player2InGoal;

    private AudioSource audioSource;
    public AudioClip WinSound;

    void Start()
    {
        gameOver = false;
        winnerName = "";
        player1InGoal = false;
        player2InGoal = false;
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1f; // ensure unfrozen when scene starts
    }

    void Update()
    {
        // If level not finished yet, check if both players are in the goal
        if (!gameOver)
        {
            if (player1InGoal && player2InGoal)
            {
                // Both players reached the goal → level complete
                audioSource.PlayOneShot(WinSound);
                DeclareWinner("Both Players");
            }
            return;
        }

        // show winner / level complete message
        if (textbox)
        {
            textbox.text = $"{winnerName} WINS!\nPress N to Load Next Level";
        }

        // press N to load next level
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }
    }

    public static void DeclareWinner(string name)
    {
        if (gameOver) return;   // safety guard

        winnerName = name;
        gameOver = true;
        Time.timeScale = 0f; // freeze gameplay
    }

    private void LoadNextLevel()
    {
        Time.timeScale = 1f; // Unfreeze before loading next level

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextIndex = currentIndex + 1;

        if (nextIndex >= totalScenes)
        {
            nextIndex = 0; // Wrap back to the first level if at the end
        }

        SceneManager.LoadScene(nextIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player1)
        {
            player1InGoal = true;
        }
        else if (other.gameObject == player2)
        {
            player2InGoal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player1)
        {
            player1InGoal = false;
        }
        else if (other.gameObject == player2)
        {
            player2InGoal = false;
        }
    }
}
