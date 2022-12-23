using UnityEngine.Audio;
using UnityEngine;

/***************************************************************************************
*    Title: Sound
*    Author: Brackeys
*    Date: May 31, 2017 
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtube.com/watch?v=6OT43pvUyfY&si=EnSIkaIECMiOmarE
*    Description: Sound class that manages Audio properties
*
***************************************************************************************/

[System.Serializable]
public class Sound {

	public string name;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = .75f;
	[Range(0f, 1f)]
	public float volumeVariance = .1f;

	[Range(.1f, 3f)]
	public float pitch = 1f;
	[Range(0f, 1f)]
	public float pitchVariance = .1f;

	public bool loop = false;

	public AudioMixerGroup mixerGroup;

	[HideInInspector]
	public AudioSource source;

}
