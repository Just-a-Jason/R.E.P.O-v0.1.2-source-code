using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class MapBacktrackPoint : MonoBehaviour
{
	// Token: 0x06000CB4 RID: 3252 RVA: 0x0006FF8E File Offset: 0x0006E18E
	private void Awake()
	{
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0006FFA0 File Offset: 0x0006E1A0
	public void Show(bool _sameLayer)
	{
		Color color = this.spriteRenderer.color;
		if (_sameLayer)
		{
			color.a = 1f;
		}
		else
		{
			color.a = 0.2f;
		}
		this.spriteRenderer.color = color;
		base.StopCoroutine(this.Animate());
		base.StartCoroutine(this.Animate());
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0006FFFB File Offset: 0x0006E1FB
	private IEnumerator Animate()
	{
		this.animating = true;
		this.lerp = 0f;
		for (;;)
		{
			this.lerp += Time.deltaTime * this.speed;
			base.transform.localScale = Vector3.one * this.curve.Evaluate(this.lerp);
			if (this.lerp >= 1f)
			{
				break;
			}
			yield return new WaitForSeconds(0.05f);
		}
		this.animating = false;
		yield break;
	}

	// Token: 0x0400144A RID: 5194
	public SpriteRenderer spriteRenderer;

	// Token: 0x0400144B RID: 5195
	public AnimationCurve curve;

	// Token: 0x0400144C RID: 5196
	public float speed;

	// Token: 0x0400144D RID: 5197
	private float lerp;

	// Token: 0x0400144E RID: 5198
	public bool animating;
}
