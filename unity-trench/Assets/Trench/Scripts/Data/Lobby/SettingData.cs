using UnityEngine;
using System.Collections;

public class SettingData : MonoBehaviour
{
	[Range(0f,1f)]
	public float
		soundVolume;
	[Range(0f,1f)]
	public float
		musicVolume;
	public bool vibrationToggle;
	public bool muteMode;

	public void Init ()
	{
		soundVolume = SOUND.Instance.SoundPlayer.volume;
		musicVolume = SOUND.Instance.MusicPlayer.volume;
		vibrationToggle = SOUND.Instance.vibration;
		muteMode = SOUND.Instance.muteMode;
	}

	public void SetSoundVolume (float volume)
	{
		SOUND.Instance.SetSoundVolume (volume);
		soundVolume = volume;
	}

	public void SetMusicVolume (float volume)
	{
		SOUND.Instance.SetMusicVolume (volume);
		musicVolume = volume;
	}

	public void SetVibration (bool isVibrationOn)
	{
		SOUND.Instance.SetVibration (isVibrationOn);
		vibrationToggle = isVibrationOn;
	}

	public void SetMuteMode (bool isOn)
	{
		SOUND.Instance.SetMuteMode (isOn);
		muteMode = isOn;
	}
}
