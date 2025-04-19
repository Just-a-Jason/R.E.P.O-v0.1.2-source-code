using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class VacuumCleaner : MonoBehaviour
{
	// Token: 0x06000743 RID: 1859 RVA: 0x00044EDC File Offset: 0x000430DC
	private void Start()
	{
		this.IntroSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 0.25f);
		this.SuckingTimer = 1f;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00044F38 File Offset: 0x00043138
	private void Update()
	{
		if (ToolController.instance.Interact)
		{
			Interaction activeInteraction = ToolController.instance.ActiveInteraction;
			if (activeInteraction)
			{
				VacuumSpotInteraction component = activeInteraction.GetComponent<VacuumSpotInteraction>();
				if (component && !this.DebugNoSuck)
				{
					component.VacuumSpot.cleanInput = true;
					this.LoopSuckSoundTimer = 0.1f;
					if (component.VacuumSpot.Amount > 0.2f)
					{
						this.SuckingTimer = Mathf.Max(this.SuckingTimer, this.SuckingTime);
					}
					else if (!component.VacuumSpot.CleanDone)
					{
						component.VacuumSpot.CleanDone = true;
					}
				}
			}
		}
		if (this.LoopSuckSoundTimer > 0f)
		{
			this.LoopSuckSoundTimer -= Time.deltaTime;
			this.LoopSuckSound.PlayLoop(true, 5f, 5f, 1f);
		}
		else
		{
			this.LoopSuckSound.PlayLoop(false, 5f, 5f, 1f);
		}
		if (this.SuckingTimer > 0f)
		{
			this.SuckingTimer -= Time.deltaTime;
			if (!this.Sucking)
			{
				GameDirector.instance.CameraShake.Shake(3f, 0.25f);
				this.Sucking = true;
				this.VacuumCleanerBag.Active = true;
				this.SuckingOffset.Active = true;
				if (this.OutroAudioPlay)
				{
					this.ParticleSystem.Play();
				}
				this.LoopStartSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			}
			GameDirector.instance.CameraShake.Shake(1f, 0.25f);
			this.SuckNoise.noiseStrengthDefault = Mathf.Lerp(this.SuckNoise.noiseStrengthDefault, this.SuckNoiseAmount, 5f * Time.deltaTime);
		}
		else
		{
			if (this.Sucking)
			{
				GameDirector.instance.CameraShake.Shake(3f, 0.25f);
				this.LoopStopSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
				this.VacuumCleanerBag.Active = false;
				this.SuckingOffset.Active = false;
				if (this.OutroAudioPlay)
				{
					this.ParticleSystem.Stop();
				}
				this.Sucking = false;
			}
			this.SuckNoise.noiseStrengthDefault = Mathf.Lerp(this.SuckNoise.noiseStrengthDefault, 0f, 5f * Time.deltaTime);
		}
		this.LoopSound.PlayLoop(this.Sucking, 0.5f, 0.5f, 1f);
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			if (this.ParticleSystem != null && this.ParticleSystem.isPlaying)
			{
				this.ParticleSystem.gameObject.transform.parent = null;
				this.ParticleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
				this.ParticleSystem.Stop();
				this.ParticleSystem = null;
			}
			this.OutroSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(3f, 0.25f);
		}
		this.FollowTransform.position = ToolController.instance.ToolFollow.transform.position;
		this.FollowTransform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		this.FollowTransform.localScale = ToolController.instance.ToolHide.transform.localScale;
		this.ParentTransform.transform.position = ToolController.instance.ToolTargetParent.transform.position;
		this.ParentTransform.transform.rotation = ToolController.instance.ToolTargetParent.transform.rotation;
	}

	// Token: 0x04000CB0 RID: 3248
	public bool DebugNoSuck;

	// Token: 0x04000CB1 RID: 3249
	[Space]
	public float SuckingTime;

	// Token: 0x04000CB2 RID: 3250
	private float SuckingTimer;

	// Token: 0x04000CB3 RID: 3251
	private bool Sucking;

	// Token: 0x04000CB4 RID: 3252
	[Space]
	public ToolActiveOffset SuckingOffset;

	// Token: 0x04000CB5 RID: 3253
	public VacuumCleanerBag VacuumCleanerBag;

	// Token: 0x04000CB6 RID: 3254
	public ParticleSystem ParticleSystem;

	// Token: 0x04000CB7 RID: 3255
	public Transform FollowTransform;

	// Token: 0x04000CB8 RID: 3256
	public Transform ParentTransform;

	// Token: 0x04000CB9 RID: 3257
	[Space]
	public AnimNoise SuckNoise;

	// Token: 0x04000CBA RID: 3258
	public float SuckNoiseAmount;

	// Token: 0x04000CBB RID: 3259
	[Space]
	public Sound IntroSound;

	// Token: 0x04000CBC RID: 3260
	public Sound OutroSound;

	// Token: 0x04000CBD RID: 3261
	public Sound LoopSound;

	// Token: 0x04000CBE RID: 3262
	public Sound LoopSuckSound;

	// Token: 0x04000CBF RID: 3263
	private float LoopSuckSoundTimer;

	// Token: 0x04000CC0 RID: 3264
	public Sound LoopStartSound;

	// Token: 0x04000CC1 RID: 3265
	public Sound LoopStopSound;

	// Token: 0x04000CC2 RID: 3266
	private bool OutroAudioPlay = true;
}
