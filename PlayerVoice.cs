using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class PlayerVoice : MonoBehaviour
{
	// Token: 0x06000F30 RID: 3888 RVA: 0x0008A67D File Offset: 0x0008887D
	private void Awake()
	{
		PlayerVoice.Instance = this;
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0008A685 File Offset: 0x00088885
	private void Start()
	{
		base.StartCoroutine(this.EnemySetup());
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0008A694 File Offset: 0x00088894
	private IEnumerator EnemySetup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0008A69C File Offset: 0x0008889C
	public void PlayCrouchHush()
	{
		if (this.CurrentVoiceSource != null && this.CurrentVoiceSource.isPlaying)
		{
			this.VoicesToFade.Add(this.CurrentVoiceSource);
			this.CurrentVoiceSource = null;
		}
		this.SprintingTimer = 0f;
		this.SprintLoop.PlayLoop(false, 1f, 50f, 1f);
		this.CurrentVoiceSource = this.CrouchHush.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.VoicePauseTimer = this.CurrentVoiceSource.clip.length * 1.2f;
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0008A750 File Offset: 0x00088950
	private void Update()
	{
		if (this.Player.Crouching && this.VoicePauseTimer <= 0f)
		{
			float num = Mathf.Clamp(this.LevelEnemy.PlayerDistance.PlayerDistanceLocal, this.CrouchLoopDistanceMin, this.CrouchLoopDistanceMax);
			float b = Mathf.Lerp(this.CrouchLoopVolumeMin, this.CrouchLoopVolumeMax, 1f - (num - this.CrouchLoopDistanceMin) / (this.CrouchLoopDistanceMax - this.CrouchLoopDistanceMin));
			this.CrouchLoopVolume = Mathf.Lerp(this.CrouchLoopVolume, b, Time.deltaTime * 5f);
			this.CrouchLoop.LoopVolume = this.CrouchLoopVolume;
			this.CrouchLoop.PlayLoop(true, 1f, 1f, 1f);
		}
		else
		{
			this.CrouchLoop.PlayLoop(false, 1f, 1f, 1f);
		}
		if (this.Player.SprintSpeedLerp >= 1f)
		{
			this.SprintingTimer = 0.5f;
		}
		else if (this.SprintingTimer > 0f)
		{
			this.SprintingTimer -= Time.deltaTime;
		}
		if (this.SprintingTimer > 0f)
		{
			this.SprintLoop.PlayLoop(true, 1f, 5f, 1f);
			if (!this.SprintLoopPlaying)
			{
				if (this.CurrentVoiceSource != null && this.CurrentVoiceSource.isPlaying)
				{
					this.VoicesToFade.Add(this.CurrentVoiceSource);
					this.CurrentVoiceSource = null;
				}
				this.SprintLoop.LoopVolume = 0f;
				this.SprintLoopPlaying = true;
			}
			this.SprintLoopLerp += Time.deltaTime * this.SprintVolumeSpeed;
			this.SprintLoopLerp = Mathf.Clamp01(this.SprintLoopLerp);
			this.SprintLoop.LoopVolume = Mathf.Lerp(0f, this.SprintVolume, this.SprintLoopLerp);
		}
		else
		{
			this.SprintLoop.PlayLoop(false, 1f, 5f, 1f);
			if (this.SprintLoopPlaying)
			{
				if (!this.Player.Crouching)
				{
					if (this.CurrentVoiceSource != null && this.CurrentVoiceSource.isPlaying)
					{
						this.VoicesToFade.Add(this.CurrentVoiceSource);
						this.CurrentVoiceSource = null;
					}
					this.CurrentVoiceSource = this.SprintStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.VoicePauseTimer = this.CurrentVoiceSource.clip.length * 1.2f;
				}
				this.SprintLoopLerp = 0f;
				this.SprintLoopPlaying = false;
			}
		}
		if (this.VoicePauseTimer > 0f)
		{
			this.VoicePauseTimer -= Time.deltaTime;
		}
		foreach (AudioSource audioSource in this.VoicesToFade)
		{
			if (audioSource == null)
			{
				this.VoicesToFade.Remove(audioSource);
				break;
			}
			if (audioSource.volume <= 0.01f)
			{
				audioSource.Stop();
				this.VoicesToFade.Remove(audioSource);
				Object.Destroy(audioSource.gameObject);
				break;
			}
			audioSource.volume -= 2f * Time.deltaTime;
		}
	}

	// Token: 0x0400197C RID: 6524
	public static PlayerVoice Instance;

	// Token: 0x0400197D RID: 6525
	public PlayerController Player;

	// Token: 0x0400197E RID: 6526
	internal AudioSource CurrentVoiceSource;

	// Token: 0x0400197F RID: 6527
	private List<AudioSource> VoicesToFade = new List<AudioSource>();

	// Token: 0x04001980 RID: 6528
	[Space]
	public Sound CrouchLoop;

	// Token: 0x04001981 RID: 6529
	public float CrouchLoopDistanceMax = 10f;

	// Token: 0x04001982 RID: 6530
	public float CrouchLoopDistanceMin = 1f;

	// Token: 0x04001983 RID: 6531
	public float CrouchLoopVolumeMax = 1f;

	// Token: 0x04001984 RID: 6532
	public float CrouchLoopVolumeMin = 1f;

	// Token: 0x04001985 RID: 6533
	private float CrouchLoopVolume;

	// Token: 0x04001986 RID: 6534
	[Space]
	public Sound CrouchHush;

	// Token: 0x04001987 RID: 6535
	[Space]
	private float VoicePauseTimer;

	// Token: 0x04001988 RID: 6536
	private Enemy LevelEnemy;

	// Token: 0x04001989 RID: 6537
	[Space]
	public Sound SprintStop;

	// Token: 0x0400198A RID: 6538
	public Sound SprintLoop;

	// Token: 0x0400198B RID: 6539
	public float SprintVolume;

	// Token: 0x0400198C RID: 6540
	public float SprintVolumeSpeed;

	// Token: 0x0400198D RID: 6541
	private bool SprintLoopPlaying;

	// Token: 0x0400198E RID: 6542
	private float SprintingTimer;

	// Token: 0x0400198F RID: 6543
	private float SprintLoopLerp;
}
