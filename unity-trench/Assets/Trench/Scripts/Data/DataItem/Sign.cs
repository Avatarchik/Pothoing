using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class xmlSign :BaseXmlReader<Dictionary<int,INFO_SIGN>,INFO_SIGN>
{
	protected override void OnRead (INFO_SIGN v)
	{
		m_Info.Add (v.Id, v);
	}
}

public class Sign : BaseXmlNode<Dictionary<int ,INFO_SIGN>,xmlSign>
{
	
}
