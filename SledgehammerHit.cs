using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class SledgehammerHit : MonoBehaviour
{
	// Token: 0x06000714 RID: 1812 RVA: 0x0004328C File Offset: 0x0004148C
	public void Spawn(RoachTrigger roach)
	{
		this.Intro.Active = true;
		this.Intro.ActiveLerp = 1f;
		this.Roach = roach;
		base.transform.position = this.Roach.transform.position;
		base.transform.LookAt(this.LookAtTransform);
		this.DelayTimer = this.DelayTime;
		this.MeshTransform.gameObject.SetActive(false);
		this.Spawning = true;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0004330C File Offset: 0x0004150C
	public void Hit()
	{
		this.MeshTransform.gameObject.SetActive(true);
		this.Controller.SoundHit.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.Spawning = false;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00043364 File Offset: 0x00041564
	public void Update()
	{
		if (this.Spawning)
		{
			base.transform.position = this.Roach.RoachOrbit.transform.position;
			base.transform.LookAt(this.LookAtTransform);
			return;
		}
		if (!this.DebugDelayDisable)
		{
			this.DelayTimer -= Time.deltaTime;
			if (this.DelayTimer <= 0f)
			{
				this.Controller.HitDone();
			}
		}
	}

	// Token: 0x04000C2F RID: 3119
	public SledgehammerController Controller;

	// Token: 0x04000C30 RID: 3120
	public Transform LookAtTransform;

	// Token: 0x04000C31 RID: 3121
	public ToolActiveOffset Intro;

	// Token: 0x04000C32 RID: 3122
	public Transform MeshTransform;

	// Token: 0x04000C33 RID: 3123
	[Space]
	public Transform Outro;

	// Token: 0x04000C34 RID: 3124
	public AnimationCurve OutroCurve;

	// Token: 0x04000C35 RID: 3125
	private Vector3 OutroPositionStart;

	// Token: 0x04000C36 RID: 3126
	private Quaternion OutroRotationStart;

	// Token: 0x04000C37 RID: 3127
	[Space]
	public bool DebugDelayDisable;

	// Token: 0x04000C38 RID: 3128
	public float DelayTime;

	// Token: 0x04000C39 RID: 3129
	private float DelayTimer;

	// Token: 0x04000C3A RID: 3130
	private bool Spawning;

	// Token: 0x04000C3B RID: 3131
	private RoachTrigger Roach;
}
