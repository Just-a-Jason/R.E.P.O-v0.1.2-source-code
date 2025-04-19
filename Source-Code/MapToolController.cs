using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class MapToolController : MonoBehaviour
{
	// Token: 0x06000CF6 RID: 3318 RVA: 0x00071540 File Offset: 0x0006F740
	private void Start()
	{
		this.VisualTransform.gameObject.SetActive(false);
		this.photonView = base.GetComponent<PhotonView>();
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			this.DisplayMesh.material = this.DisplayMaterial;
			base.transform.parent.parent = this.FollowTransform;
			base.transform.parent.localPosition = Vector3.zero;
			base.transform.parent.localRotation = Quaternion.identity;
			return;
		}
		this.DisplayMesh.material = this.DisplayMaterialClient;
		Transform[] componentsInChildren = this.VisualTransform.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Triggers");
		}
		this.SoundStart.SpatialBlend = 1f;
		this.SoundStop.SpatialBlend = 1f;
		this.SoundLoop.SpatialBlend = 1f;
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00071644 File Offset: 0x0006F844
	private void Update()
	{
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			if (!this.PlayerAvatar.isDisabled && !this.PlayerAvatar.isTumbling && !CameraAim.Instance.AimTargetActive && !SemiFunc.MenuLevel())
			{
				if (InputManager.instance.InputToggleGet(InputKey.Map))
				{
					if (SemiFunc.InputDown(InputKey.Map))
					{
						this.mapToggled = !this.mapToggled;
					}
				}
				else
				{
					this.mapToggled = false;
				}
				if ((SemiFunc.InputHold(InputKey.Map) || this.mapToggled || Map.Instance.debugActive) && !PlayerController.instance.sprinting)
				{
					if (this.HideLerp >= 1f)
					{
						this.Active = true;
					}
				}
				else
				{
					this.mapToggled = false;
					if (this.HideLerp <= 0f)
					{
						this.Active = false;
					}
				}
			}
			else
			{
				this.Active = false;
			}
			if (this.Active)
			{
				StatsUI.instance.Show();
				ItemInfoUI.instance.Hide();
				ItemInfoExtraUI.instance.Hide();
				if (MissionUI.instance.Text.text != "")
				{
					MissionUI.instance.Show();
				}
			}
		}
		if (this.Active != this.ActivePrev)
		{
			this.ActivePrev = this.Active;
			if (GameManager.Multiplayer() && this.photonView.IsMine)
			{
				this.photonView.RPC("SetActiveRPC", RpcTarget.Others, new object[]
				{
					this.Active
				});
			}
			if (this.Active)
			{
				if (!GameManager.Multiplayer() || this.photonView.IsMine)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.1f);
					Map.Instance.ActiveSet(true);
				}
				this.VisualTransform.gameObject.SetActive(true);
				this.SoundStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				if (!GameManager.Multiplayer() || this.photonView.IsMine)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.1f);
					Map.Instance.ActiveSet(false);
				}
				this.SoundStop.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		float x = 90f;
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			x = 0f;
		}
		float num = 1f;
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			num = 2f;
		}
		if (this.Active)
		{
			if (this.HideLerp > 0f)
			{
				this.HideLerp -= Time.deltaTime * this.IntroSpeed * num;
			}
			this.HideScale = Mathf.LerpUnclamped(1f, 0f, this.IntroCurve.Evaluate(this.HideLerp));
			this.HideTransform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(x, 0f, 0f), this.IntroCurve.Evaluate(this.HideLerp));
		}
		else
		{
			if (this.HideLerp < 1f)
			{
				this.HideLerp += Time.deltaTime * this.OutroSpeed * num;
				if (this.HideLerp > 1f)
				{
					this.VisualTransform.gameObject.SetActive(false);
				}
			}
			this.HideScale = Mathf.LerpUnclamped(1f, 0f, this.OutroCurve.Evaluate(this.HideLerp));
		}
		if ((!GameManager.Multiplayer() || this.photonView.IsMine) && this.Active)
		{
			PlayerController.instance.MoveMult(this.MoveMultiplier, 0.1f);
			CameraTopFade.Instance.Set(this.FadeAmount, 0.1f);
			CameraBob.Instance.SetMultiplier(this.BobMultiplier, 0.1f);
			CameraZoom.Instance.OverrideZoomSet(50f, 0.05f, 2f, 2f, base.gameObject, 100);
			CameraNoise.Instance.Override(0.025f, 0.25f);
			Aim.instance.SetState(Aim.State.Hidden);
		}
		Vector3 a = Vector3.one;
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			base.transform.parent.position = this.FollowTransformClient.position;
			base.transform.parent.rotation = this.FollowTransformClient.rotation;
			a = this.FollowTransformClient.localScale;
			this.mainSpringTransform.rotation = SemiFunc.SpringQuaternionGet(this.mainSpring, this.mainSpringTransformTarget.rotation, -1f);
		}
		base.transform.parent.localScale = Vector3.Lerp(base.transform.parent.localScale, a * this.HideScale, Time.deltaTime * 20f);
		this.displaySpringTransform.rotation = SemiFunc.SpringQuaternionGet(this.displaySpring, this.displaySpringTransformTarget.rotation, -1f);
		if (this.Active)
		{
			this.DisplayJointAngleDiff = (this.DisplayJointAnglePreviousX - this.displaySpringTransform.localRotation.x) * 50f;
			this.DisplayJointAngleDiff = Mathf.Clamp(this.DisplayJointAngleDiff, -0.1f, 0.1f);
			this.DisplayJointAnglePreviousX = this.displaySpringTransform.localRotation.x;
			this.SoundLoop.LoopPitch = Mathf.Lerp(this.SoundLoop.LoopPitch, 1f - this.DisplayJointAngleDiff, Time.deltaTime * 10f);
		}
		this.SoundLoop.PlayLoop(this.Active, 5f, 5f, 1f);
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00071C2A File Offset: 0x0006FE2A
	[PunRPC]
	public void SetActiveRPC(bool active)
	{
		this.Active = active;
	}

	// Token: 0x040014B6 RID: 5302
	public static MapToolController instance;

	// Token: 0x040014B7 RID: 5303
	internal bool Active;

	// Token: 0x040014B8 RID: 5304
	private bool ActivePrev;

	// Token: 0x040014B9 RID: 5305
	internal PhotonView photonView;

	// Token: 0x040014BA RID: 5306
	public PlayerAvatar PlayerAvatar;

	// Token: 0x040014BB RID: 5307
	[Space]
	public Transform FollowTransform;

	// Token: 0x040014BC RID: 5308
	public Transform FollowTransformClient;

	// Token: 0x040014BD RID: 5309
	[Space]
	public Transform ControllerTransform;

	// Token: 0x040014BE RID: 5310
	public Transform VisualTransform;

	// Token: 0x040014BF RID: 5311
	public Transform PlayerLookTarget;

	// Token: 0x040014C0 RID: 5312
	public Transform HideTransform;

	// Token: 0x040014C1 RID: 5313
	[Space]
	private float DisplayJointAngleDiff;

	// Token: 0x040014C2 RID: 5314
	private float DisplayJointAnglePreviousX;

	// Token: 0x040014C3 RID: 5315
	[Space]
	public float MoveMultiplier = 0.5f;

	// Token: 0x040014C4 RID: 5316
	public float FadeAmount = 0.5f;

	// Token: 0x040014C5 RID: 5317
	public float BobMultiplier = 0.1f;

	// Token: 0x040014C6 RID: 5318
	[Space]
	public Sound SoundStart;

	// Token: 0x040014C7 RID: 5319
	public Sound SoundStop;

	// Token: 0x040014C8 RID: 5320
	public Sound SoundLoop;

	// Token: 0x040014C9 RID: 5321
	[Space]
	public MeshRenderer DisplayMesh;

	// Token: 0x040014CA RID: 5322
	public Material DisplayMaterial;

	// Token: 0x040014CB RID: 5323
	public Material DisplayMaterialClient;

	// Token: 0x040014CC RID: 5324
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x040014CD RID: 5325
	public float IntroSpeed;

	// Token: 0x040014CE RID: 5326
	public AnimationCurve OutroCurve;

	// Token: 0x040014CF RID: 5327
	public float OutroSpeed;

	// Token: 0x040014D0 RID: 5328
	internal float HideLerp;

	// Token: 0x040014D1 RID: 5329
	private float HideScale;

	// Token: 0x040014D2 RID: 5330
	[Space]
	public Transform displaySpringTransform;

	// Token: 0x040014D3 RID: 5331
	public Transform displaySpringTransformTarget;

	// Token: 0x040014D4 RID: 5332
	public SpringQuaternion displaySpring;

	// Token: 0x040014D5 RID: 5333
	[Space]
	public Transform mainSpringTransform;

	// Token: 0x040014D6 RID: 5334
	public Transform mainSpringTransformTarget;

	// Token: 0x040014D7 RID: 5335
	public SpringQuaternion mainSpring;

	// Token: 0x040014D8 RID: 5336
	private bool mapToggled;
}
