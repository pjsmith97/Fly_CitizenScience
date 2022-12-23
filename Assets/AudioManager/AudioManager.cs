using UnityEngine.Audio;
using System;
using UnityEngine;
using Rewired;

/***************************************************************************************
*    Title: AudioManager
*    Author: Brackeys
*    Date: May 31, 2017 
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtube.com/watch?v=6OT43pvUyfY&si=EnSIkaIECMiOmarE
*    Description: Manages 
*
***************************************************************************************/
public class AudioManager : MonoBehaviour
{
	public Player player; // ReWired Player object
	[SerializeField] private int playerID = 0;

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

    private void Start()
    {
		Play("Theme");
		player = ReInput.players.GetPlayer(playerID);
	}

    private void Update()
    {
		if (player.GetButtonDown("ToggleMusic"))
		{
			ToggleMusic();
		}
	}

    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	/***************************************************************************************
*    Title: Stop
*    Original Title: StopPlaying
*    Author: Nightcore Motion
*    Date: May 31, 2017 
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtube.com/watch?v=6OT43pvUyfY&si=EnSIkaIECMiOmarE
*    Description: Stops playing the sound specified
*
***************************************************************************************/
	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Stop();
	}

	/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/

	/***************************************************************************************
*    Title: ToggleMusic
*    
*    Description: Toggles the audio file "Theme" on and off
*
***************************************************************************************/
	private void ToggleMusic()
    {
		Sound s = Array.Find(sounds, item => item.name == "Theme");
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

        if (s.source.isPlaying)
        {
			Stop("Theme");
        }
        else
        {
			Play("Theme");
		}
	}

/***************************************************************************************
*   Edit end
*
***************************************************************************************/
}
