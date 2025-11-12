using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCountdown : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text label;
    public GameObject panel;

    [Header("Start After This Panel Closes")]
    public GameObject waitForPanel;

    [Header("Timing")]
    public int seconds = 3;
    public float stepDelay = 1f;

    [Header("Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip beepClip;
    public AudioClip goClip;

    float prevTimeScale = 1f;

    void Start()
    {
        StartCoroutine(WaitThenRun());
    }

    IEnumerator WaitThenRun()
    {
        // 1) If a dialogue (or any UI) is open, wait for it to close first.
        if (waitForPanel != null)
        {
            // Wait until the panel exists AND is inactive in hierarchy
            yield return new WaitWhile(() => waitForPanel != null && waitForPanel.activeInHierarchy);
            // give UI one frame to settle (e.g., DialogueManager restoring timeScale)
            yield return null;
        }

        // 2) Now run the countdown (using unscaled time so it works while paused)
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        label.text = "";
        yield return null;

        int n = Mathf.Max(1, seconds);
        for (int i = n; i >= 1; i--)
        {
            label.text = i.ToString();
            Play(beepClip);
            yield return new WaitForSecondsRealtime(stepDelay);
        }

        label.text = "GO!";
        Play(goClip);
        yield return new WaitForSecondsRealtime(0.4f);

        // 3) Unpause and hide
        Time.timeScale = prevTimeScale;
        if (panel) panel.SetActive(false);
        else gameObject.SetActive(false);
    }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}

