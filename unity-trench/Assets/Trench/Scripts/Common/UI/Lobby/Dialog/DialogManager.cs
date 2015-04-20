using UnityEngine;
using System.Collections;

public class DialogManager : MonoBehaviour {

    [SerializeField]
    protected GameObject coverObject;
	[SerializeField]
	protected Obstacle obstacle;
	[SerializeField]
	protected Dialog[] dialogs;

	protected Dialog currentDialog;

    public void ShowCover(bool isShow=true)
    {
        coverObject.SetActive(isShow);
    }

	public void ShowDialog(int dialog){
		obstacle.gameObject.SetActive (true);
		obstacle.Show ();
		dialogs [dialog].gameObject.SetActive (true);
		dialogs [dialog].Init ();
		dialogs [dialog].Show ();
		currentDialog = dialogs [dialog];
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}

	public void HideDialog(int dialog){
		obstacle.Hide ();
		dialogs [dialog].Hide ();
		currentDialog = null;
		SOUND.Instance.OneShotSound (Sfx.inst.outCard);
	}

	public void HideCurrentDialog(){
		obstacle.Hide ();
		if (currentDialog != null) {
			currentDialog.Hide ();
			currentDialog = null;
		}
	}
}
