using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    void Activate();
}
public class UniversalButton : MonoBehaviour, IInteractable
{
    public GameObject[] targets;

    public void Interact()
    {
        foreach (var target in targets)
        {
            if (target == null) continue;

            // Find anything on this GameObject that implements IActivatable
            var activatables = target.GetComponents<IActivatable>();
            foreach (var activatable in activatables)
            {
                activatable.Activate();
            }
        }
    }
}
