using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class CameraBob : MonoBehaviour
{
	// Token: 0x06000085 RID: 133 RVA: 0x000056E6 File Offset: 0x000038E6
	private void Awake()
	{
		CameraBob.Instance = this;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000056EE File Offset: 0x000038EE
	private void Start()
	{
		this.bobUpLerpStrengthCurrent = this.bobUpLerpStrength;
		this.bobSideLerpStrengthCurrent = this.bobSideLerpStrength;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00005708 File Offset: 0x00003908
	public void SetMultiplier(float multiplier, float time)
	{
		this.MultiplierTarget = multiplier;
		this.MultiplierTimer = time;
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00005718 File Offset: 0x00003918
	private void Update()
	{
		float overrideSpeedMultiplier = PlayerController.instance.overrideSpeedMultiplier;
		float num = Time.deltaTime * overrideSpeedMultiplier;
		if (this.MultiplierTimer > 0f)
		{
			this.MultiplierTimer -= 1f * num;
		}
		else
		{
			this.MultiplierTarget = 1f;
		}
		this.Multiplier = Mathf.Lerp(this.Multiplier, this.MultiplierTarget, 5f * num);
		if (GameDirector.instance.currentState == GameDirector.gameState.Main && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			float num2 = 1f;
			float num3 = 1f;
			if (this.playerController.sprinting)
			{
				float b = this.SprintSpeedMultiplier + (float)StatsManager.instance.playerUpgradeSpeed[this.playerController.playerAvatarScript.steamID] * 0.1f;
				num2 = Mathf.Lerp(1f, b, this.playerController.SprintSpeedLerp);
			}
			else if (this.playerController.Crouching)
			{
				num2 = this.CrouchSpeedMultiplier;
				num3 = this.CrouchAmountMultiplier;
			}
			this.bobUpLerpStrengthCurrent = Mathf.Lerp(this.bobUpLerpStrengthCurrent, this.bobUpLerpStrength * num3, 5f * num);
			float b2 = Mathf.LerpUnclamped(0f, this.bobUpLerpStrengthCurrent, this.bobUpCurve.Evaluate(this.bobUpLerpAmount));
			this.bobUpLerpAmount += this.bobUpLerpSpeed * this.bobUpActiveLerp * num2 * num;
			if (this.bobUpLerpAmount > 1f)
			{
				if (this.playerController.CollisionController.Grounded && !CameraJump.instance.jumpActive)
				{
					if (this.playerController.sprinting)
					{
						this.playerController.playerAvatarScript.Footstep(Materials.SoundType.Heavy);
					}
					else if (this.bobUpActiveLerp > 0.75f && !this.playerController.Crouching)
					{
						this.playerController.playerAvatarScript.Footstep(Materials.SoundType.Medium);
					}
					else
					{
						this.playerController.playerAvatarScript.Footstep(Materials.SoundType.Light);
					}
				}
				this.bobUpLerpAmount = 0f;
			}
			if (this.playerController.moving && !this.camController.targetActive && this.playerController.CollisionController.Grounded)
			{
				this.bobUpActiveLerp = Mathf.Clamp01(this.bobUpActiveLerp + this.bobUpActiveLerpSpeedIn * num);
			}
			else
			{
				this.bobUpActiveLerp = Mathf.Clamp01(this.bobUpActiveLerp - this.bobUpActiveLerpSpeedOut * num);
			}
			this.bobSideLerpStrengthCurrent = Mathf.Lerp(this.bobSideLerpStrengthCurrent, this.bobSideLerpStrength * num3, 5f * num);
			float num4 = Mathf.LerpUnclamped(-this.bobSideLerpStrengthCurrent, this.bobSideLerpStrengthCurrent, this.bobSideCurve.Evaluate(this.bobSideLerpAmount));
			if (this.bobSideRev)
			{
				this.bobSideLerpAmount += this.bobSideLerpSpeed * this.bobSideActiveLerp * num2 * num;
				if (this.bobSideLerpAmount > 1f)
				{
					this.bobSideRev = false;
				}
			}
			else
			{
				this.bobSideLerpAmount -= this.bobSideLerpSpeed * this.bobSideActiveLerp * num2 * num;
				if (this.bobSideLerpAmount < 0f)
				{
					this.bobSideRev = true;
				}
			}
			if (this.playerController.moving && !this.camController.targetActive)
			{
				this.bobSideActiveLerp = Mathf.Clamp01(this.bobSideActiveLerp + this.bobSideActiveLerpSpeedIn * num);
			}
			else
			{
				this.bobSideActiveLerp = Mathf.Clamp01(this.bobSideActiveLerp - this.bobSideActiveLerpSpeedOut * num);
			}
			this.positionResult = new Vector3(0f, Mathf.LerpUnclamped(0f, b2, this.bobUpActiveCurve.Evaluate(this.bobUpActiveLerp)) * this.Multiplier, 0f);
			this.rotationResult = Quaternion.Euler(0f, Mathf.LerpUnclamped(0f, num4 * 10f, this.bobSideActiveCurve.Evaluate(this.bobSideActiveLerp)) * this.Multiplier, Mathf.LerpUnclamped(0f, num4 * 5f, this.bobSideActiveCurve.Evaluate(this.bobSideActiveLerp)) * this.Multiplier);
		}
		else
		{
			this.bobSideActiveLerp = 0f;
			this.bobUpActiveLerp = 0f;
			this.positionResult = Vector3.Lerp(this.positionResult, Vector3.zero, 5f * num);
			this.rotationResult = Quaternion.Slerp(this.rotationResult, Quaternion.identity, 5f * num);
		}
		base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.positionResult, GameplayManager.instance.cameraAnimation);
		base.transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.rotationResult, GameplayManager.instance.cameraAnimation);
	}

	// Token: 0x04000152 RID: 338
	public static CameraBob Instance;

	// Token: 0x04000153 RID: 339
	public CameraTarget camController;

	// Token: 0x04000154 RID: 340
	public PlayerController playerController;

	// Token: 0x04000155 RID: 341
	public AudioPlay footstepAudio;

	// Token: 0x04000156 RID: 342
	[Header("Bob Up")]
	public AnimationCurve bobUpCurve;

	// Token: 0x04000157 RID: 343
	public float bobUpLerpSpeed;

	// Token: 0x04000158 RID: 344
	public float bobUpLerpStrength;

	// Token: 0x04000159 RID: 345
	private float bobUpLerpStrengthCurrent;

	// Token: 0x0400015A RID: 346
	private float bobUpLerpAmount;

	// Token: 0x0400015B RID: 347
	public float bobUpActiveLerpSpeedIn = 1f;

	// Token: 0x0400015C RID: 348
	public float bobUpActiveLerpSpeedOut = 1f;

	// Token: 0x0400015D RID: 349
	private float bobUpActiveLerp;

	// Token: 0x0400015E RID: 350
	public AnimationCurve bobUpActiveCurve;

	// Token: 0x0400015F RID: 351
	[Header("Bob Side")]
	public AnimationCurve bobSideCurve;

	// Token: 0x04000160 RID: 352
	public float bobSideLerpSpeed;

	// Token: 0x04000161 RID: 353
	public float bobSideLerpStrength;

	// Token: 0x04000162 RID: 354
	private float bobSideLerpStrengthCurrent;

	// Token: 0x04000163 RID: 355
	private float bobSideLerpAmount;

	// Token: 0x04000164 RID: 356
	private bool bobSideRev;

	// Token: 0x04000165 RID: 357
	public float bobSideActiveLerpSpeedIn = 1f;

	// Token: 0x04000166 RID: 358
	public float bobSideActiveLerpSpeedOut = 1f;

	// Token: 0x04000167 RID: 359
	private float bobSideActiveLerp;

	// Token: 0x04000168 RID: 360
	public AnimationCurve bobSideActiveCurve;

	// Token: 0x04000169 RID: 361
	[Header("Other")]
	public float SprintSpeedMultiplier = 1f;

	// Token: 0x0400016A RID: 362
	public float CrouchSpeedMultiplier = 1f;

	// Token: 0x0400016B RID: 363
	public float CrouchAmountMultiplier = 1f;

	// Token: 0x0400016C RID: 364
	private float Multiplier;

	// Token: 0x0400016D RID: 365
	private float MultiplierTarget;

	// Token: 0x0400016E RID: 366
	private float MultiplierTimer;

	// Token: 0x0400016F RID: 367
	internal Vector3 positionResult;

	// Token: 0x04000170 RID: 368
	internal Quaternion rotationResult;
}
