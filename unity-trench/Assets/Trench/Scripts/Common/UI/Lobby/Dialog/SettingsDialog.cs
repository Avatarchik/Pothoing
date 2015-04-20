using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsDialog : Dialog
{
	[SerializeField]
	SettingData
		data;
	[SerializeField]
	Slider
		music;
	[SerializeField]
	Slider
		sound;
	[SerializeField]
	Toggle
		vibration;
	[SerializeField]
	Toggle
		muteMode;
    
	public override void Init ()
	{
		data.Init ();
		music.value = data.musicVolume;
		sound.value = data.soundVolume;
		vibration.isOn = data.vibrationToggle;
		muteMode.isOn = data.muteMode;
	}

	public void OnMusicVolumeChange ()
	{
		data.SetMusicVolume (music.value);
	}

	public void OnSoundVolumeChange ()
	{
		data.SetSoundVolume (sound.value);
	}

	public void OnVibrationToggle ()
	{
		data.SetVibration (vibration.isOn);
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}

	public void OnMuteModeToggle ()
	{
		data.SetMuteMode (muteMode.isOn);
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
//		if (muteMode.isOn) {
//			data.SetMusicVolume (0f);
//			data.SetSoundVolume (0f);
//		} else {
//			OnMusicToggle ();
//			OnSoundToggle ();
//		}
	}
}
