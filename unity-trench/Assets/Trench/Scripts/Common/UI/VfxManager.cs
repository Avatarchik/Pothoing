using UnityEngine;
using System.Collections;

public enum VFX{
	Dust,
	Straight,
	Bomb,
	Win,
	Lose
}

public class VfxManager : MonoBehaviour {
	
	[SerializeField]
	VfxPlayer straightVfx;
	[SerializeField]
	ParticleSystem dustVfx;
	[SerializeField]
	ParticleSystem bombExploVfx;
	[SerializeField]
	ParticleSystem winVfx;
	[SerializeField]
	ParticleSystem loseVfx;
	
public void Play(VFX vfx){
		switch(vfx){
		case VFX.Dust:
			dustVfx.Play();
			break;
		case VFX.Bomb:
			bombExploVfx.Play();
			break;
		case VFX.Straight:
			straightVfx.gameObject.SetActive(true);
			straightVfx.Play();
			break;
		case VFX.Win:
			winVfx.Play();
			break;
		case VFX.Lose:
			loseVfx.Play();
			break;
		}
	}
}
