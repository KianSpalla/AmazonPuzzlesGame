using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    void Activate();
}
public class UniversalButton : MonoBehaviour, IInteractable
{
    [Tooltip("Objects that should react when this button is pressed.")]
    public GameObject[] targets;

    [Tooltip("If true, this button can only be used once.")]
    public bool oneShot = false;

    private bool used = false;

    public void Interact()
    {
        if (oneShot && used) return;
        used = true;

        foreach (var target in targets)
        {
            if (target == null) continue;

            var activatables = target.GetComponents<IActivatable>();
            foreach (var activatable in activatables)
            {
                activatable.Activate();
            }
        }

        Debug.Log("Button pressed!");
    }
}
