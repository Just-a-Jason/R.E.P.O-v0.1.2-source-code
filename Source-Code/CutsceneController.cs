using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class CutsceneController : MonoBehaviour
{
	// Token: 0x0600091D RID: 2333 RVA: 0x00056B78 File Offset: 0x00054D78
	private void Start()
	{
		this.Animator = base.GetComponent<Animator>();
		this.StartTimer = Random.Range(this.StartTimeMin, this.StartTimeMax);
		this.MainCamera = GameDirector.instance.MainCamera;
		this.PreviousParent = GameDirector.instance.MainCameraParent;
		this.PreviousFOV = this.MainCamera.fieldOfView;
		this.MainCamera.transform.parent = this.Parent;
		this.MainCamera.fieldOfView = this.Camera.fieldOfView;
		this.MainCamera.transform.localPosition = Vector3.zero;
		this.MainCamera.transform.localRotation = Quaternion.identity;
		GameDirector.instance.volumeCutsceneOnly.TransitionTo(0.1f);
		this.Camera.enabled = false;
		this.Active = true;
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00056C58 File Offset: 0x00054E58
	private void Update()
	{
		if (!this.Started)
		{
			HUD.instance.Hide();
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
			this.StartTimer -= Time.deltaTime;
			if (this.StartTimer <= 0f || this.DebugLoop)
			{
				if (this.DebugLoop || !GameDirectorStatic.CatchCutscenePlayed || Random.Range(0, 3) == 0)
				{
					this.Animator.SetBool("Play", true);
					this.Started = true;
				}
				else
				{
					this.End();
				}
			}
		}
		if (this.Active)
		{
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
			GameDirector.instance.SetDisableInput(0.5f);
			this.MainCamera.fieldOfView = this.Camera.fieldOfView;
			if (this.EndActive)
			{
				this.EndTimer -= Time.deltaTime;
				if (this.DebugLoop || this.EndTimer <= 0f)
				{
					this.End();
					return;
				}
			}
			else
			{
				VideoOverlay.Instance.Override(0.1f, 0.2f, 5f);
			}
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00056D8C File Offset: 0x00054F8C
	private void ResetCamera()
	{
		this.MainCamera.transform.parent = GameDirector.instance.MainCameraParent;
		this.MainCamera.fieldOfView = this.PreviousFOV;
		this.MainCamera.transform.localPosition = Vector3.zero;
		this.MainCamera.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00056DEE File Offset: 0x00054FEE
	public void EndSet()
	{
		if (!this.DebugLoop)
		{
			this.Animator.SetBool("Play", false);
		}
		this.EndActive = true;
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00056E10 File Offset: 0x00055010
	private void End()
	{
		if (!this.DebugLoop)
		{
			GameDirectorStatic.CatchCutscenePlayed = true;
			this.Active = false;
			this.ResetCamera();
			if (!this.CatchCutscene)
			{
				GameDirector.instance.volumeOn.TransitionTo(0.1f);
			}
			this.ParentEnable.SetActive(false);
		}
	}

	// Token: 0x04001094 RID: 4244
	public Camera Camera;

	// Token: 0x04001095 RID: 4245
	private Camera MainCamera;

	// Token: 0x04001096 RID: 4246
	public Transform Parent;

	// Token: 0x04001097 RID: 4247
	private Transform PreviousParent;

	// Token: 0x04001098 RID: 4248
	private float PreviousFOV;

	// Token: 0x04001099 RID: 4249
	private bool Active;

	// Token: 0x0400109A RID: 4250
	public GameObject ParentEnable;

	// Token: 0x0400109B RID: 4251
	[Space]
	public bool CatchCutscene;

	// Token: 0x0400109C RID: 4252
	[Space]
	public float StartTimeMin;

	// Token: 0x0400109D RID: 4253
	public float StartTimeMax;

	// Token: 0x0400109E RID: 4254
	private float StartTimer;

	// Token: 0x0400109F RID: 4255
	private bool Started;

	// Token: 0x040010A0 RID: 4256
	[Space]
	private bool EndActive;

	// Token: 0x040010A1 RID: 4257
	public float EndTimer;

	// Token: 0x040010A2 RID: 4258
	[Space]
	public bool DebugLoop;

	// Token: 0x040010A3 RID: 4259
	private Animator Animator;
}
