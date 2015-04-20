using UnityEngine;
using System.Collections;

public class Dialog : MonoBehaviour {

	[SerializeField]
	protected Tweener tweener;

	public virtual void Init()
	{
	}

    public virtual void Show()
    {
		tweener.FromTarget ();
	}

    public virtual void Hide()
    {
		tweener.ToTarget (delegate() {
			gameObject.SetActive(false);
		});
	}
}
