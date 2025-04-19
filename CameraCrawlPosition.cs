using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class CameraCrawlPosition : MonoBehaviour
{
	// Token: 0x060000BC RID: 188 RVA: 0x00007453 File Offset: 0x00005653
	private void Start()
	{
		this.Player = PlayerController.instance;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00007460 File Offset: 0x00005660
	private void Update()
	{
		if (this.Player.Crawling || this.Player.Sliding)
		{
			this.Active = true;
		}
		else
		{
			this.Active = false;
		}
		if (this.Active != this.ActivePrev)
		{
			if (this.Active)
			{
				PlayerController.instance.playerAvatarScript.CrouchToCrawl();
			}
			else
			{
				PlayerController.instance.playerAvatarScript.CrawlToCrouch();
			}
			GameDirector.instance.CameraShake.Shake(1f, 0.1f);
			this.ActivePrev = this.Active;
		}
		float num = this.PositionSpeed;
		if (this.Player.Sliding)
		{
			num *= 2f;
		}
		if (this.Active)
		{
			this.Lerp += Time.deltaTime * num;
		}
		else
		{
			this.Lerp -= Time.deltaTime * num;
		}
		this.Lerp = Mathf.Clamp01(this.Lerp);
		base.transform.localPosition = new Vector3(0f, this.AnimationCurve.Evaluate(this.Lerp) * this.Position, 0f);
	}

	// Token: 0x040001ED RID: 493
	public CameraCrouchPosition CrouchPosition;

	// Token: 0x040001EE RID: 494
	[Space]
	public float Position;

	// Token: 0x040001EF RID: 495
	public float PositionSpeed;

	// Token: 0x040001F0 RID: 496
	public AnimationCurve AnimationCurve;

	// Token: 0x040001F1 RID: 497
	private float Lerp;

	// Token: 0x040001F2 RID: 498
	[Space]
	public Sound IntroSound;

	// Token: 0x040001F3 RID: 499
	[Space]
	public Sound OutroSound;

	// Token: 0x040001F4 RID: 500
	[HideInInspector]
	public bool Active;

	// Token: 0x040001F5 RID: 501
	private bool ActivePrev;

	// Token: 0x040001F6 RID: 502
	private PlayerController Player;
}
