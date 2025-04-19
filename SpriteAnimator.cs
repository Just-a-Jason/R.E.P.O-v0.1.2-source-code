using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class SpriteAnimator : MonoBehaviour
{
	// Token: 0x060011FE RID: 4606 RVA: 0x0009F5B5 File Offset: 0x0009D7B5
	private void Start()
	{
		if (this.spriteRenderer == null)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
		this.secondsPerFrame = 1f / (float)this.framesPerSecond;
		base.StartCoroutine(this.AnimateSprite());
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x0009F5F1 File Offset: 0x0009D7F1
	private IEnumerator AnimateSprite()
	{
		while (this.isAnimating)
		{
			this.spriteRenderer.sprite = this.animationSprites[this.currentSpriteIndex];
			this.currentSpriteIndex = (this.currentSpriteIndex + 1) % this.animationSprites.Count;
			yield return new WaitForSeconds(this.secondsPerFrame);
		}
		yield break;
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x0009F600 File Offset: 0x0009D800
	private void OnDisable()
	{
		this.isAnimating = false;
	}

	// Token: 0x04001E53 RID: 7763
	public SpriteRenderer spriteRenderer;

	// Token: 0x04001E54 RID: 7764
	public List<Sprite> animationSprites;

	// Token: 0x04001E55 RID: 7765
	public int framesPerSecond = 12;

	// Token: 0x04001E56 RID: 7766
	private int currentSpriteIndex;

	// Token: 0x04001E57 RID: 7767
	private bool isAnimating = true;

	// Token: 0x04001E58 RID: 7768
	private float secondsPerFrame;
}
