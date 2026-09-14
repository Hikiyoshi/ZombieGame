using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A custom class contains information about Audio Clips.
/// Used by the Audio Manager.
/// </summary>
[Serializable]
public class AudioEntry
{
	public enum AudioType
	{
		Sound,
		Music,
		Ambience
	}

	public string name;
	public AudioType audioType;

	[Space] public AudioClip[] clips;
	[Space] public AudioMixerGroup mixerGroup;

	[Range(0f, 1f), Space] public float volume = 1f;
	[Range(-3f, 3f)] public float pitch = 1f;

	public bool isLooped;
	[HideInInspector] public AudioSource source;

	public AudioClip this[int index] => clips[index];

	public int ClipCount => clips.Length;
}
