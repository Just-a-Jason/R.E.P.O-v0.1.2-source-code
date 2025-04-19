using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class CameraCrouchPosition : MonoBehaviour
{
	// Token: 0x060000C2 RID: 194 RVA: 0x00007694 File Offset: 0x00005894
	private void Awake()
	{
		CameraCrouchPosition.instance = this;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x0000769C File Offset: 0x0000589C
	private void Start()
	{
		this.Player = PlayerController.instance;
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x000076AC File Offset: 0x000058AC
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.Player.Crouching)
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
				PlayerController.instance.playerAvatarScript.StandToCrouch();
			}
			else
			{
				PlayerController.instance.playerAvatarScript.CrouchToStand();
			}
			GameDirector.instance.CameraShake.Shake(2f, 0.1f);
			this.CameraCrouchRotation.RotationLerp = 0f;
			this.ActivePrev = this.Active;
		}
		float num = this.PositionSpeed * PlayerController.instance.playerAvatarScript.playerAvatarVisuals.animationSpeedMultiplier;
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

	// Token: 0x040001FB RID: 507
	public static CameraCrouchPosition instance;

	// Token: 0x040001FC RID: 508
	public CameraCrouchRotation CameraCrouchRotation;

	// Token: 0x040001FD RID: 509
	[Space]
	public float Position;

	// Token: 0x040001FE RID: 510
	public float PositionSpeed;

	// Token: 0x040001FF RID: 511
	public AnimationCurve AnimationCurve;

	// Token: 0x04000200 RID: 512
	internal float Lerp;

	// Token: 0x04000201 RID: 513
	[HideInInspector]
	public bool Active;

	// Token: 0x04000202 RID: 514
	internal bool ActivePrev;

	// Token: 0x04000203 RID: 515
	private PlayerController Player;
}
