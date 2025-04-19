using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class SledgehammerController : MonoBehaviour
{
	// Token: 0x0600070D RID: 1805 RVA: 0x00042CF0 File Offset: 0x00040EF0
	private void Start()
	{
		this.MainCamera = Camera.main;
		this.Mask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		this.Hit.gameObject.SetActive(false);
		this.Transition.gameObject.SetActive(false);
		this.SoundMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00042D8C File Offset: 0x00040F8C
	private void OnTriggerStay(Collider other)
	{
		if (ToolController.instance.ToolHide.Active && this.Swing.CanHit)
		{
			RoachTrigger component = other.GetComponent<RoachTrigger>();
			if (component)
			{
				float magnitude = (component.transform.position - this.MainCamera.transform.position).magnitude;
				Vector3 normalized = (component.transform.position - this.MainCamera.transform.position).normalized;
				RaycastHit raycastHit;
				if (!Physics.Raycast(this.MainCamera.transform.position, (component.transform.position - this.MainCamera.transform.position).normalized, out raycastHit, (component.transform.position - this.MainCamera.transform.position).magnitude, this.Mask))
				{
					this.Roach = component;
					this.Swing.HitOutro();
					this.Swing.CanHit = false;
					this.Transition.gameObject.SetActive(true);
					this.Transition.IntroSet();
					this.Hit.gameObject.SetActive(true);
					this.Hit.Spawn(this.Roach);
				}
			}
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00042EF4 File Offset: 0x000410F4
	public void HitDone()
	{
		this.Transition.gameObject.SetActive(true);
		this.Transition.OutroSet();
		GameDirector.instance.CameraImpact.Shake(3f, 0f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		this.SoundHitOutro.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.Hit.gameObject.SetActive(false);
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00042F8C File Offset: 0x0004118C
	public void IntroDone()
	{
		GameDirector.instance.CameraImpact.Shake(5f, 0f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		this.Hit.gameObject.SetActive(true);
		this.Hit.Hit();
		this.Roach.RoachOrbit.Squash();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00042FF8 File Offset: 0x000411F8
	public void OutroDone()
	{
		this.Swing.gameObject.SetActive(true);
		this.Swing.MeshTransform.gameObject.SetActive(true);
		PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, -5f, 0.25f);
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00043050 File Offset: 0x00041250
	private void Update()
	{
		if (this.Hit.gameObject.activeSelf)
		{
			float magnitude = (this.Hit.transform.position - PlayerController.instance.transform.position).magnitude;
			PlayerController.instance.MoveForce(this.Hit.transform.position - PlayerController.instance.transform.position, magnitude * 30f, 0.01f);
			PlayerController.instance.InputDisable(0.1f);
		}
		if (ToolController.instance.Interact && !this.Swing.Swinging)
		{
			this.InteractImpulse = true;
		}
		if (this.InteractImpulse && ToolController.instance.ToolHide.Active && ToolController.instance.ToolHide.ActiveLerp >= 1f)
		{
			this.InteractImpulse = false;
			this.Swing.Swing();
		}
		if (this.Swing.Swinging || this.Hit.gameObject.activeSelf)
		{
			ToolController.instance.ForceActiveTimer = 0.1f;
		}
		this.ControllerTransform.transform.position = ToolController.instance.ToolTargetParent.transform.position;
		this.ControllerTransform.transform.rotation = ToolController.instance.ToolTargetParent.transform.rotation;
		this.FollowTransform.position = ToolController.instance.ToolFollow.transform.position;
		this.FollowTransform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		this.FollowTransform.localScale = ToolController.instance.ToolHide.transform.localScale;
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			this.SoundMoveLong.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
	}

	// Token: 0x04000C20 RID: 3104
	public Transform ControllerTransform;

	// Token: 0x04000C21 RID: 3105
	public Transform FollowTransform;

	// Token: 0x04000C22 RID: 3106
	[Space]
	public SledgehammerSwing Swing;

	// Token: 0x04000C23 RID: 3107
	public SledgehammerHit Hit;

	// Token: 0x04000C24 RID: 3108
	public SledgehammerTransition Transition;

	// Token: 0x04000C25 RID: 3109
	private RoachTrigger Roach;

	// Token: 0x04000C26 RID: 3110
	[Space]
	[Header("Sounds")]
	public Sound SoundMoveLong;

	// Token: 0x04000C27 RID: 3111
	public Sound SoundMoveShort;

	// Token: 0x04000C28 RID: 3112
	public Sound SoundSwing;

	// Token: 0x04000C29 RID: 3113
	public Sound SoundHit;

	// Token: 0x04000C2A RID: 3114
	public Sound SoundHitOutro;

	// Token: 0x04000C2B RID: 3115
	private bool OutroAudioPlay = true;

	// Token: 0x04000C2C RID: 3116
	private LayerMask Mask;

	// Token: 0x04000C2D RID: 3117
	private Camera MainCamera;

	// Token: 0x04000C2E RID: 3118
	private bool InteractImpulse;
}
