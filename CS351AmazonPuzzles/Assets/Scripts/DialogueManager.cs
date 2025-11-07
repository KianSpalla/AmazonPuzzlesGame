using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text textbox;
    public string[] sentences;
    public float typingSpeed = 0.03f;

    public GameObject continueButton;
    public GameObject dialoguePanel;

    int index = 0;
    bool isTyping = false;

    // Keep track of shown scenes to avoid repeating dialogues
    static HashSet<string> shownScenes = new HashSet<string>();

    private void Update()
    {
        //If panel is active freeze time
        if (dialoguePanel.activeSelf)
        {
            Time.timeScale = 0f; //freeze game
        }
        else
        {
            Time.timeScale = 1f; //unfreeze game
        }
    }

    private void Awake()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (shownScenes.Contains(sceneName))
        {
            dialoguePanel.SetActive(false);
            enabled = false;
        }
        else
        {
            shownScenes.Add(sceneName);
        }


    }
    void OnEnable()
    {
        // Reset state every time dialogue opens
        StopAllCoroutines();
        isTyping = false;
        index = 0;
        if (continueButton) continueButton.SetActive(false);

        // Basic safety checks
        if (textbox == null)
        {
            Debug.LogError("DialogueManager: 'textbox' is not assigned.");
            return;
        }
        if (sentences == null || sentences.Length == 0)
        {
            Debug.LogWarning("DialogueManager: 'sentences' is empty. Closing dialogue.");
            if (dialoguePanel) dialoguePanel.SetActive(false);
            return;
        }

        StartCoroutine(TypeVisible());
    }

    IEnumerator TypeVisible()
    {
        isTyping = true;

        // Bounds check just in case
        if (index < 0 || index >= sentences.Length)
        {
            Debug.LogWarning($"DialogueManager: index {index} out of range for sentences[{sentences.Length}].");
            yield break;
        }

        // Set full rich-text sentence at once, then reveal characters
        textbox.text = sentences[index];
        textbox.ForceMeshUpdate();

        int totalChars = textbox.textInfo.characterCount;
        textbox.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalChars; i++)
        {
            textbox.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typingSpeed); // works even if Time.timeScale==0
        }

        isTyping = false;
        if (continueButton) continueButton.SetActive(true);
    }

    public void NextSentence()
    {
        // If user clicks mid-typing, finish instantly
        if (isTyping)
        {
            textbox.maxVisibleCharacters = textbox.textInfo.characterCount;
            isTyping = false;
            if (continueButton) continueButton.SetActive(true);
            return;
        }

        if (continueButton) continueButton.SetActive(false);

        // Advance
        index++;

        if (index < sentences.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeVisible());
        }
        else
        {
            // End of dialogue
            textbox.text = "";
            if (dialoguePanel) dialoguePanel.SetActive(false);
            Time.timeScale = 1f; // unfreeze game
        }
    }
    //start function
    void Start()
    {
        //freeze game
        Time.timeScale = 0f;
    }
}
