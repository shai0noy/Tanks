using UnityEngine;
using System.Collections;

public abstract class TakesDamage : MonoBehaviour {

	public Vector2 position {
		get { return transform.position; }
	}

	public abstract void hit (float damage);

	public abstract void takeBlastPush(Vector2 force);

	public abstract void takeBlastDamage(float damage);


	public void blasted(Vector2 origin, float blastRadius, float blastDamage, float blastPush) {
		Vector2 delta = ((Vector2)transform.position - origin);
		float dist = delta.magnitude;
		if (dist < blastRadius) {
			Vector2 dir = delta.normalized;
			takeBlastDamage((blastRadius - dist)/blastRadius * blastDamage);
			takeBlastPush(dir * (blastRadius - dist)/blastRadius * blastPush);
		}
	}

}
