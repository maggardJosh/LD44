using UnityEngine;
using System.Collections;

public abstract class Npc : MonoBehaviour
{
    public abstract void Interact(GameObject player);

    public abstract void StopInteracting();
}
