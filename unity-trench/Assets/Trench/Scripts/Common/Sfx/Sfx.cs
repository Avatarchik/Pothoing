using UnityEngine;
using System.Collections;

public enum VoiceType
{
	Single,
	Pair,
	Triple,
	Quad,
	Straight,

	Warning,
	Bigger,
	CallScore,
	Trench,
	Pass
}

public class Sfx : MonoBehaviour
{
	public static Sfx inst;
//	[SerializeField]
//	UserData
//		user;
	public AudioClip btn;
	public AudioClip clock;
	public AudioClip buy;
	public AudioClip reward;
	public AudioClip win;
	public AudioClip chooseCard;
	public AudioClip outCard;
	public AudioClip sendCard;
	public AudioClip show;
	public AudioClip hide;
	public AudioClip hitTable;

	public AudioClip boyStraight;
	public AudioClip girlStraight;
	public AudioClip[] boySingle;
	public AudioClip[] girlSingle;
	public AudioClip[] boyPair;
	public AudioClip[] girlPair;
	public AudioClip[] boyTriple;
	public AudioClip[] girlTriple;
	public AudioClip[] boyQuad;
	public AudioClip[] girlQuad;
	public AudioClip[] boyWarning;
	public AudioClip[] girlWarning;
	public AudioClip[] boyBigger;
	public AudioClip[] girlBigger;
	public AudioClip[] boyCallScore;
	public AudioClip[] girlCallScore;
	public AudioClip[] boyTrench;
	public AudioClip[] girlTrench;
	public AudioClip[] boyPass;
	public AudioClip[] girlPass;

	void Awake ()
	{
		inst = this;
	}

	public AudioClip GetVoice (VoiceType voice,int param=0,bool isFemale=false)
	{

		if (isFemale) {
			switch (voice) {
			case VoiceType.Single:
				return girlSingle [param];
				break;
			case VoiceType.Pair:
				return girlPair [param];
				break;
			case VoiceType.Triple:
				return girlTriple [param];
				break;
			case VoiceType.Quad:
				return girlQuad [param];
				break;
			case VoiceType.Straight:
				return girlStraight;
				break;
			case VoiceType.Warning:
				return girlWarning [param];
				break;
			case VoiceType.Bigger:
				return girlBigger [param];
				break;
			case VoiceType.CallScore:
				return girlCallScore [param];
				break;
			case VoiceType.Trench:
				return girlTrench [param];
				break;
			case VoiceType.Pass:
				return girlPass [param];
				break;
			default:
				return null;
				break;
			}
		} else {
			switch (voice) {
			case VoiceType.Single:
				return boySingle [param];
				break;
			case VoiceType.Pair:
				return boyPair [param];
				break;
			case VoiceType.Triple:
				return boyTriple [param];
				break;
			case VoiceType.Quad:
				return boyQuad [param];
				break;
			case VoiceType.Straight:
				return boyStraight;
				break;
			case VoiceType.Warning:
				return boyWarning [param];
				break;
			case VoiceType.Bigger:
				return boyBigger [param];
				break;
			case VoiceType.CallScore:
				return boyCallScore [param];
				break;
			case VoiceType.Trench:
				return boyTrench [param];
				break;
			case VoiceType.Pass:
				return boyPass [param];
				break;
			default:
				return null;
				break;
			}
		}
	}
}
