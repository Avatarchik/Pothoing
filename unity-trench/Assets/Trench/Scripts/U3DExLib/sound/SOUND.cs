using UnityEngine;
using System.Collections;

public class SOUND : MonoBehaviour
{
	private static SOUND instance;
	
	public static SOUND Instance { 
		get { 
			if (null == instance)
				instance = FindObjectOfType (typeof(SOUND)) as SOUND; 
			if (null == instance) { 
				GameObject go = Instantiate (Resources.Load ("Prefabs/Sound/SoundManager")) as GameObject;
				go.name = "SoundManager";
				instance = go.GetComponent<SOUND> ();
			} 
			return instance; 
		}
	}
	
	void Awake ()
	{
		
		if (instance == null) { 
			instance = this.GetComponent<SOUND> (); 
		} else { 
			Destroy (gameObject); 
		} 
		
		DontDestroyOnLoad (gameObject); 
		Init ();
	}

	//public AudioClip BGM;
	public AudioSource MusicPlayer;
	public AudioSource SoundPlayer;
	public bool vibration;
	public bool muteMode;

	void Init ()
	{
		MusicPlayer.volume = 1.0f - PlayerPrefs.GetFloat ("RTS_MUSIC_VOLUME");
		SoundPlayer.volume = 1.0f - PlayerPrefs.GetFloat ("RTS_SOUND_VOLUME");
		vibration = PlayerPrefs.GetInt ("VIBRATION", 0) == 1 ? true : false;
		muteMode = PlayerPrefs.GetInt ("MUTEMODE", 0) == 1 ? true : false;

	}

	public void SetMusicVolume (float fVolume)
	{
		MusicPlayer.volume = fVolume;
		PlayerPrefs.SetFloat ("RTS_MUSIC_VOLUME", 1.0f - fVolume);
	}

	public void SetSoundVolume (float fVolume)
	{
		SoundPlayer.volume = fVolume;
		PlayerPrefs.SetFloat ("RTS_SOUND_VOLUME", 1.0f - fVolume);
	}

	public void SetVibration (bool isOn)
	{
		vibration = isOn;
		PlayerPrefs.SetInt ("VIBRATION", vibration ? 1 : 0);
	}

	public void SetMuteMode (bool isOn)
	{
		muteMode = isOn;
		if (isOn) {
			MusicPlayer.Stop ();
			SoundPlayer.Stop ();
		} else {
			MusicPlayer.Stop ();
			MusicPlayer.Play ();
		}
		PlayerPrefs.SetInt ("MUTEMODE", muteMode ? 1 : 0);
	}

	public void PlayBGM (AudioClip BGM, bool bLoop = true)
	{
		if (muteMode) {
			return;
		}
		if (BGM == null) {
			MusicPlayer.Stop ();
			return;
		}

		MusicPlayer.clip = BGM;
		MusicPlayer.loop = bLoop;
		MusicPlayer.Play ();
	}

	public void StopBGM ()
	{
		MusicPlayer.Stop ();
	}

	public void OneShotSound (AudioClip EffSound, float TempPitch = 1.0f)
	{
		if (muteMode) {
			return;
		}
		TempPitch = 1.0f;
		if (EffSound == null)
			return;
		SoundPlayer.pitch = TempPitch;
		SoundPlayer.PlayOneShot (EffSound);
		if (TempPitch != 1.0f) {
			Debug.Log ("OneShotSound :" + TempPitch.ToString ());
		}
	}

//	public void OneShotVoice (params string[] strName)
//	{
//		if (muteMode) {
//			return;
//		}
//		string strNa = strName [Random.Range (0, strName.Length)];
//		OneShotSound (Resources.Load ("Sound/Voice/" + strNa) as AudioClip);
//	}

}
