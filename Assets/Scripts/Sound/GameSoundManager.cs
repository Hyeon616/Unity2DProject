using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameSoundManager : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider MasterSlider;
    public Slider bgmSlider;
    public Slider seSlider;

    public void MasterControl()
    {
        float sound = MasterSlider.value;

        if (sound == -40f)
            audioMixer.SetFloat("Master", -80);
        else
            audioMixer.SetFloat("Master", sound);

    }

    public void BgmControl()
    {
        float sound = bgmSlider.value;

        if (sound == -40f)
            audioMixer.SetFloat("BGM", -80);
        else
            audioMixer.SetFloat("BGM", sound);

    }

    public void SEControl()
    {
        float sound = seSlider.value;

        if (sound == -40f)
            audioMixer.SetFloat("SE", -80);
        else
            audioMixer.SetFloat("SE", sound);

    }



}
