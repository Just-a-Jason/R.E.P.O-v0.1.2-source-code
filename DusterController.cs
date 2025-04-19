using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class DusterController : MonoBehaviour
{
	// Token: 0x060006FD RID: 1789 RVA: 0x00041D84 File Offset: 0x0003FF84
	private void Start()
	{
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		this.MoveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00041DD8 File Offset: 0x0003FFD8
	private void Update()
	{
		if (ToolController.instance.Interact && ToolController.instance.ToolHide.Active && ToolController.instance.ToolHide.ActiveLerp > 0.75f)
		{
			Interaction activeInteraction = ToolController.instance.ActiveInteraction;
			if (activeInteraction)
			{
				DirtyPainting component = activeInteraction.GetComponent<DirtyPainting>();
				if (component)
				{
					CanvasHandler canvasHandler = component.CanvasHandler;
					if (canvasHandler && canvasHandler.currentState != CanvasHandler.State.Clean)
					{
						this.DustingTimer = 0.5f;
						if (this.DusterDusting.ActiveAmount >= 0.1f)
						{
							canvasHandler.cleanInput = true;
							if (!canvasHandler.CleanDone && canvasHandler.fadeMultiplier <= 0.5f)
							{
								canvasHandler.CleanDone = true;
							}
						}
					}
				}
			}
		}
		if (this.DustingTimer > 0f)
		{
			this.ToolActiveOffset.Active = true;
			this.ToolBackAway.Active = true;
			this.Dusting = true;
			this.DustingTimer -= Time.deltaTime;
			if (this.DustingTimer <= 0f)
			{
				this.Dusting = false;
				this.ToolActiveOffset.Active = false;
				this.ToolBackAway.Active = false;
			}
		}
		if (this.Dusting && this.ToolActiveOffset.Active && this.ToolActiveOffset.ActiveLerp >= 0.3f)
		{
			this.DusterDusting.Active = true;
		}
		else
		{
			this.DusterDusting.Active = false;
		}
		this.FollowTransform.position = ToolController.instance.ToolFollow.transform.position;
		this.FollowTransform.rotation = ToolController.instance.ToolFollow.transform.rotation;
		this.FollowTransform.localScale = ToolController.instance.ToolHide.transform.localScale;
		this.ParentTransform.transform.position = ToolController.instance.ToolTargetParent.transform.position;
		this.ParentTransform.transform.rotation = ToolController.instance.ToolTargetParent.transform.rotation;
		if (this.OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			this.MoveSound.Play(this.FollowTransform.position, 1f, 1f, 1f, 1f);
			this.OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
	}

	// Token: 0x04000BD7 RID: 3031
	public Transform FollowTransform;

	// Token: 0x04000BD8 RID: 3032
	public Transform ParentTransform;

	// Token: 0x04000BD9 RID: 3033
	[Space]
	public ToolActiveOffset ToolActiveOffset;

	// Token: 0x04000BDA RID: 3034
	public DusterDusting DusterDusting;

	// Token: 0x04000BDB RID: 3035
	public ToolBackAway ToolBackAway;

	// Token: 0x04000BDC RID: 3036
	private bool Dusting;

	// Token: 0x04000BDD RID: 3037
	private float DustingTimer;

	// Token: 0x04000BDE RID: 3038
	[Space]
	public Sound MoveSound;

	// Token: 0x04000BDF RID: 3039
	private bool OutroAudioPlay = true;
}
