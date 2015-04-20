using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignMgr
{
	Sign sign = new Sign ();
	
	public void Load ()
	{
		string data = Resources.Load (COMMON_CONST.PathSign).ToString ();
		//Debug.Log (data);
		sign.XmlLoad (data, "ContinuousSignIn");
		//Debug.Log (sign.INFO.Values.Count);
	}

	public List<INFO_SIGN> GetAllSignData ()
	{
		List<INFO_SIGN> list = new List<INFO_SIGN> ();
		list.AddRange (sign.INFO.Values);
		//Debug.Log (sign.INFO.Values.Count);
		return list;
	}

	public INFO_SIGN GetSignData (int signID)
	{
		foreach (var e in sign.INFO.Values) {
			if (e.Id == signID)
				return e;
		}
		
		return null;
	}
}
