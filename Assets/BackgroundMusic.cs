using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusic : MonoBehaviour
{

    public AudioClip[] Clips;

    private AudioSource player;

	// Use this for initialization
	void Start ()
    {
        player = GetComponent<AudioSource>();

        if (Clips != null && Clips.Length > 0)
        {
            // Get random index
            int index = Random.Range(0, Clips.Length);

            // Use as clip
            player.clip = Clips[index];

            player.Play();
        }
	}

    public void SliderChanged(Slider slider)
    {
        player.volume = slider.value;
    }

    public void Mute(Toggle toggle)
    {
        player.mute = !toggle.isOn;
    }
}
