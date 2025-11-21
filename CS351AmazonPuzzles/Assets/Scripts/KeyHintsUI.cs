using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KeyIcon
{
    public string name;             // Just for organizing in inspector (e.g. "Move Up")
    public KeyCode key;             // The key to listen for (e.g. KeyCode.W)
    public Image image;             // UI Image showing the key

    [Header("Sprites (optional)")]
    public Sprite idleSprite;       // normal / greyed-out key
    public Sprite pressedSprite;    // highlighted key

    [Header("Colors (optional)")]
    public Color idleColor = Color.gray;     // color when not pressed
    public Color pressedColor = Color.white; // color when pressed
}
public class KeyHintsUI : MonoBehaviour
{
    public KeyIcon[] keyIcons;
    void Start()
    {
        // Initialize images to idle state
        foreach (var k in keyIcons)
        {
            if (k.image == null) continue;

            if (k.idleSprite != null)
                k.image.sprite = k.idleSprite;

            k.image.color = k.idleColor;
        }
    }

    void Update()
    {
        foreach (var k in keyIcons)
        {
            if (k.image == null) continue;

            bool isPressed = Input.GetKey(k.key);

            // Sprite swap
            if (k.idleSprite != null && k.pressedSprite != null)
            {
                k.image.sprite = isPressed ? k.pressedSprite : k.idleSprite;
            }

            // Color tint (on top of sprite swap or alone)
            k.image.color = isPressed ? k.pressedColor : k.idleColor;
        }
    }
}
