using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class EnemyBangFuse : MonoBehaviour
{
	// Token: 0x0600014E RID: 334 RVA: 0x0000C9ED File Offset: 0x0000ABED
	private void Awake()
	{
		this.tipObject.SetActive(false);
		this.delayTimer = Random.Range(0.25f, 0.6f);
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000CA10 File Offset: 0x0000AC10
	private void Update()
	{
		if (!this.setup)
		{
			return;
		}
		if (this.controller.fuseActive)
		{
			if (!this.active)
			{
				if (this.delayTimer <= 0f)
				{
					this.tipObject.SetActive(true);
					this.particleFire.Play();
					this.SparkPlay();
					this.active = true;
				}
				else
				{
					this.delayTimer -= Time.deltaTime;
				}
			}
		}
		else if (this.active)
		{
			this.tipObject.SetActive(false);
			this.particleFire.Stop();
			this.active = false;
			this.delayTimer = Random.Range(0.25f, 0.6f);
		}
		float t = this.stiffCurve.Evaluate(this.controller.fuseLerp);
		float t2 = this.shrinkCurve.Evaluate(this.controller.fuseLerp);
		Vector3 vector = this.botTransformTarget.position - (this.botTransformTarget.position + Vector3.up);
		vector = SemiFunc.ClampDirection(vector, base.transform.forward, Mathf.Lerp(30f, 0f, t));
		this.botTransformTarget.rotation = Quaternion.RotateTowards(this.botTransformTarget.rotation, Quaternion.LookRotation(vector), 800f * Time.deltaTime);
		Vector3 vector2 = this.topTransformTarget.position - (this.topTransformTarget.position + Vector3.up);
		vector2 = SemiFunc.ClampDirection(vector2, this.botTransformTarget.forward, Mathf.Lerp(90f, 0f, t));
		this.topTransformTarget.rotation = Quaternion.RotateTowards(this.topTransformTarget.rotation, Quaternion.LookRotation(vector2), 800f * Time.deltaTime);
		this.botTransformSource.rotation = SemiFunc.SpringQuaternionGet(this.botSpring, this.botTransformTarget.rotation, -1f);
		this.topTransformSource.rotation = SemiFunc.SpringQuaternionGet(this.topSpring, this.topTransformTarget.rotation, -1f);
		this.botSpring.damping = Mathf.Lerp(0.3f, 0.8f, t);
		this.botSpring.speed = Mathf.Lerp(10f, 15f, t);
		this.botSpring.maxAngle = Mathf.Lerp(90f, 5f, t);
		this.topSpring.damping = Mathf.Lerp(0.3f, 0.8f, t);
		this.topSpring.speed = Mathf.Lerp(10f, 15f, t);
		this.topSpring.maxAngle = Mathf.Lerp(90f, 5f, t);
		this.botTransformSource.localPosition = Vector3.Lerp(Vector3.zero, -Vector3.forward * 0.1f, t2);
		if (!this.active)
		{
			this.glowTargetOld = Vector3.one * 5f;
			this.glowTargetNew = Vector3.one;
			this.glowSpeed = 5f;
			this.glowLerp = 0f;
			return;
		}
		this.controller.anim.FuseLoop();
		this.particleParent.position = this.tipObject.transform.position;
		this.glowLerp += this.glowSpeed * Time.deltaTime;
		this.glowLerp = Mathf.Clamp01(this.glowLerp);
		if (this.glowLerp >= 1f)
		{
			this.glowTargetOld = this.glowTransform.localScale;
			this.glowTargetNew = Vector3.one * Random.Range(0.75f, 1.25f);
			this.glowSpeed = Random.Range(5f, 30f);
			this.glowLerp = 0f;
		}
		this.glowTransform.localScale = Vector3.Lerp(this.glowTargetOld, this.glowTargetNew, this.glowCurve.Evaluate(this.glowLerp));
		if (this.controller.fuseLerp < this.controller.explosionTellFuseThreshold)
		{
			this.sparkTimer = 0f;
			return;
		}
		if (this.sparkTimer <= 0f)
		{
			this.sparkTimer = 0.2f;
			this.SparkPlay();
			return;
		}
		this.sparkTimer -= Time.deltaTime;
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000CE64 File Offset: 0x0000B064
	public void SparkPlay()
	{
		this.particleSpark.Play();
		this.controller.anim.soundFuseIgnite.Play(this.tipObject.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0000CEB6 File Offset: 0x0000B0B6
	private void OnDisable()
	{
		this.particleFire.Stop();
	}

	// Token: 0x040002B9 RID: 697
	internal EnemyBang controller;

	// Token: 0x040002BA RID: 698
	internal bool setup;

	// Token: 0x040002BB RID: 699
	private bool active;

	// Token: 0x040002BC RID: 700
	private float delayTimer;

	// Token: 0x040002BD RID: 701
	public GameObject tipObject;

	// Token: 0x040002BE RID: 702
	public Transform glowTransform;

	// Token: 0x040002BF RID: 703
	private float glowLerp = 1f;

	// Token: 0x040002C0 RID: 704
	private float glowSpeed;

	// Token: 0x040002C1 RID: 705
	private Vector3 glowTargetOld;

	// Token: 0x040002C2 RID: 706
	private Vector3 glowTargetNew;

	// Token: 0x040002C3 RID: 707
	[Space]
	public Transform particleParent;

	// Token: 0x040002C4 RID: 708
	public ParticleSystem particleFire;

	// Token: 0x040002C5 RID: 709
	public ParticleSystem particleSpark;

	// Token: 0x040002C6 RID: 710
	private float sparkTimer;

	// Token: 0x040002C7 RID: 711
	[Space]
	public AnimationCurve glowCurve;

	// Token: 0x040002C8 RID: 712
	public AnimationCurve stiffCurve;

	// Token: 0x040002C9 RID: 713
	public AnimationCurve shrinkCurve;

	// Token: 0x040002CA RID: 714
	[Space]
	public SpringQuaternion botSpring;

	// Token: 0x040002CB RID: 715
	public Transform botTransformSource;

	// Token: 0x040002CC RID: 716
	public Transform botTransformTarget;

	// Token: 0x040002CD RID: 717
	[Space]
	public SpringQuaternion topSpring;

	// Token: 0x040002CE RID: 718
	public Transform topTransformSource;

	// Token: 0x040002CF RID: 719
	public Transform topTransformTarget;
}
