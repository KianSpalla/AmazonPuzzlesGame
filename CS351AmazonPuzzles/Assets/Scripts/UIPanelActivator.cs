using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelActivator : MonoBehaviour
{
    [Header("Assign your UI Panel here")]
    public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (panel == null) return;

        if (IsPlayer(other))
        {
            panel.SetActive(true);
        }
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.CompareTag("Player1") || other.CompareTag("Player2");
    }
}
