using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TableButton : MonoBehaviour {

    [SerializeField]
    Button btn;
	[SerializeField]
	Image btnBkg;
	[SerializeField]
	Image btnLabel;
	[SerializeField]
	Sprite disableBtnBkg;
	[SerializeField]
	Sprite disableBtnLabel;
	[SerializeField]
	Sprite normalBtnBkg;
	[SerializeField]
	Sprite normalBtnLabel;
//	void Start(){
//		normalBtnBkg = btnBkg.sprite;
//		normalBtnLabel = btnLabel.sprite;
//	}

	public void ChangeSprite (bool isDisable) {
		if (isDisable) {
			btnBkg.sprite=disableBtnBkg;
			btnLabel.sprite=disableBtnLabel;
            btn.enabled = false;
		} else {
			btnBkg.sprite=normalBtnBkg;
			btnLabel.sprite=normalBtnLabel;
            btn.enabled = true;
		}
	}
}
