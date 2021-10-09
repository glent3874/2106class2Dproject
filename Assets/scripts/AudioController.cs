using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [Header("�V����")]
    public AudioMixer mixer;

    public void SetVolumeBGM(float volume)
    {
        mixer.SetFloat("VolumeBGM", volume);
    }

    public void SetVolumeSFX(float volume)
    {
        mixer.SetFloat("VolumeSFX", volume);
    }
}
