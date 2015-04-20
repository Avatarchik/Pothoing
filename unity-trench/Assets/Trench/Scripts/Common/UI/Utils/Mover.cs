using UnityEngine;
using System.Collections;

public enum Direction
{
	Left,
	Right,
	Top,
	Down,
	Center,

	Count
}

public class Mover : MonoBehaviour
{	
	[Tooltip("0:left ,1:right ,2:top,3:down ,4:center ,x/-x: max horizontal,y/-y: max vertical")]
	[SerializeField]
	Vector3[]
		moveTarget;
	[SerializeField]
	iTween.EaseType easeType;
	float moveTime=1f;

	/// <summary>
	/// Moves to.
	/// </summary>
	/// <param name="dir">0:left ,1:right ,2:top,3:down ,4:center</param>
	public void MoveTo ( int dir){
		MoveTo ((Direction)dir);
	}

	public void MoveTo (Direction dir)
	{
		iTween.MoveTo (gameObject,iTween.Hash("position",moveTarget [(int)dir],"easetype",easeType,"time",moveTime));
	}
}
