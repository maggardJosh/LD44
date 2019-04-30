using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundHandler : MonoBehaviour
{
    public List<SoundManager.Sound> StepSound;

    public void PlayStepSound()
    {
        SoundManager.Instance.PlaySound(StepSound[Random.Range(0, StepSound.Count)]);
    }

}
