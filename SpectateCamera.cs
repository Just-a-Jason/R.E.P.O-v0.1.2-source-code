using System;
using System.Linq;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class SpectateCamera : MonoBehaviour
{
	// Token: 0x06000F4B RID: 3915 RVA: 0x0008B9D4 File Offset: 0x00089BD4
	private void Awake()
	{
		SpectateCamera.instance = this;
		this.normalTransformPivot.parent = null;
		this.MainCamera = GameDirector.instance.MainCamera;
		foreach (Camera camera in this.MainCamera.GetComponentsInChildren<Camera>())
		{
			if (camera != this.MainCamera)
			{
				this.TopCamera = camera;
			}
		}
		this.ParentObject = CameraNoise.Instance.transform;
		this.PreviousParent = this.ParentObject.parent;
		this.ParentObject.parent = base.transform;
		this.ParentObject.localPosition = Vector3.zero;
		this.ParentObject.localRotation = Quaternion.identity;
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x0008BA88 File Offset: 0x00089C88
	private void OnDestroy()
	{
		QualitySettings.shadowDistance = 15f;
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x0008BA94 File Offset: 0x00089C94
	private void LateUpdate()
	{
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideAim();
		SemiFunc.UIShowSpectate();
		MissionUI.instance.Hide();
		SpectateCamera.State state = this.currentState;
		if (state != SpectateCamera.State.Death)
		{
			if (state == SpectateCamera.State.Normal)
			{
				this.StateNormal();
			}
		}
		else
		{
			this.StateDeath();
		}
		this.RoomVolumeLogic();
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0008BAE8 File Offset: 0x00089CE8
	private void StateDeath()
	{
		if (this.stateImpulse)
		{
			this.MainCamera.transform.localPosition = new Vector3(0f, 0f, -50f);
			this.MainCamera.transform.localRotation = Quaternion.identity;
			this.MainCamera.nearClipPlane = 0.01f;
			this.previousFarClipPlane = this.MainCamera.farClipPlane;
			this.MainCamera.farClipPlane = 70f;
			this.MainCamera.farClipPlane = 90f;
			this.deathCameraNearClipPlane = 70f;
			this.MainCamera.nearClipPlane = 70f;
			QualitySettings.shadowDistance = 90f;
			RenderSettings.fog = false;
			PostProcessing.Instance.SpectateSet();
			this.DeathNearClipLogic(true);
			LightManager.instance.UpdateInstant();
			CameraGlitch.Instance.PlayShort();
			this.previousFieldOfView = this.MainCamera.fieldOfView;
			this.cameraFieldOfView = 8f;
			this.MainCamera.fieldOfView = 16f;
			this.TopCamera.fieldOfView = this.MainCamera.fieldOfView;
			this.stateImpulse = false;
			this.stateTimer = 4f;
			AudioManager.instance.AudioListener.TargetPositionTransform = this.deathPlayerSpectatePoint;
			GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
			GameDirector.instance.CameraShake.Shake(2f, 1f);
		}
		this.stateTimer -= Time.deltaTime;
		CameraNoise.Instance.Override(0.03f, 0.25f);
		this.deathCurrentY += SemiFunc.InputMouseX() * CameraAim.Instance.AimSpeedMouse;
		Vector3 position = base.transform.position;
		if (this.deathPlayerSpectatePoint)
		{
			position = this.deathPlayerSpectatePoint.position;
		}
		if (this.CheckState(SpectateCamera.State.Death))
		{
			position = this.deathPosition;
		}
		Vector3 vector = position;
		Quaternion rotation = Quaternion.Euler(88f, this.deathCurrentY, 0f);
		this.deathTargetOrbit = rotation;
		float num = Mathf.Lerp(50f, 2.5f, GameplayManager.instance.cameraSmoothing / 100f);
		this.deathSmoothOrbit = Quaternion.Slerp(this.deathSmoothOrbit, this.deathTargetOrbit, num * Time.deltaTime);
		if (this.deathOrbitInstantSet)
		{
			this.deathSmoothOrbit = this.deathTargetOrbit;
			this.deathOrbitInstantSet = false;
		}
		rotation = this.deathSmoothOrbit;
		Vector3 b = rotation * Vector3.back * 2f;
		this.deathFollowPointTarget = vector;
		this.deathFollowPoint = Vector3.SlerpUnclamped(this.deathFollowPoint, this.deathFollowPointTarget, Time.deltaTime * this.deathSpeed);
		base.transform.position = this.deathFollowPoint + b;
		this.deathSmoothLookAtPoint = Vector3.SlerpUnclamped(this.deathSmoothLookAtPoint, position, Time.deltaTime * this.deathSpeed);
		base.transform.LookAt(this.deathSmoothLookAtPoint);
		this.MainCamera.fieldOfView = Mathf.Lerp(this.MainCamera.fieldOfView, this.cameraFieldOfView, Time.deltaTime * 10f);
		this.TopCamera.fieldOfView = this.MainCamera.fieldOfView;
		this.DeathNearClipLogic(false);
		ExtractionPoint extractionPointCurrent = RoundDirector.instance.extractionPointCurrent;
		if (extractionPointCurrent && extractionPointCurrent.currentState != ExtractionPoint.State.Idle && extractionPointCurrent.currentState != ExtractionPoint.State.Active)
		{
			bool flag = false;
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				if (!playerAvatar.isDisabled)
				{
					flag = false;
					break;
				}
				if (playerAvatar.playerDeathHead.inExtractionPoint)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.stateTimer = Mathf.Clamp(this.stateTimer, 0.25f, this.stateTimer);
			}
		}
		if (this.stateTimer <= 0f)
		{
			if (SemiFunc.RunIsTutorial())
			{
				foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetList())
				{
					playerAvatar2.Revive(false);
				}
				return;
			}
			this.UpdateState(SpectateCamera.State.Normal);
		}
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0008BF38 File Offset: 0x0008A138
	private void StateNormal()
	{
		if (this.stateImpulse)
		{
			this.PlayerSwitch(true);
			if (!this.player)
			{
				return;
			}
			RenderSettings.fog = true;
			this.MainCamera.farClipPlane = this.previousFarClipPlane;
			this.MainCamera.fieldOfView = this.previousFieldOfView;
			this.TopCamera.fieldOfView = this.MainCamera.fieldOfView;
			this.MainCamera.transform.localPosition = Vector3.zero;
			this.MainCamera.transform.localRotation = Quaternion.identity;
			AudioManager.instance.AudioListener.TargetPositionTransform = this.MainCamera.transform;
			this.stateImpulse = false;
		}
		CameraNoise.Instance.Override(0.03f, 0.25f);
		float num = SemiFunc.InputMouseX();
		float num2 = SemiFunc.InputMouseY();
		float num3 = SemiFunc.InputScrollY();
		if (CameraAim.Instance.overrideAimStop)
		{
			num = 0f;
			num2 = 0f;
			num3 = 0f;
		}
		this.normalAimHorizontal += num * CameraAim.Instance.AimSpeedMouse * 1.5f;
		if (this.normalAimHorizontal > 360f)
		{
			this.normalAimHorizontal -= 360f;
		}
		if (this.normalAimHorizontal < -360f)
		{
			this.normalAimHorizontal += 360f;
		}
		float num4 = this.normalAimVertical;
		float num5 = -(num2 * CameraAim.Instance.AimSpeedMouse) * 1.5f;
		this.normalAimVertical += num5;
		this.normalAimVertical = Mathf.Clamp(this.normalAimVertical, -70f, 70f);
		if (num3 != 0f)
		{
			this.normalMaxDistance = Mathf.Clamp(this.normalMaxDistance - num3 * 0.0025f, this.normalMinDistance, 6f);
		}
		Vector3 vector = this.normalPreviousPosition;
		if (this.player.spectatePoint)
		{
			vector = this.player.spectatePoint.position;
		}
		else if (this.player.isTumbling)
		{
			vector = this.player.tumble.physGrabObject.centerPoint;
		}
		else if (this.player.isCrouching && !this.player.isCrawling)
		{
			vector += Vector3.up * 0.3f;
		}
		else if (!this.player.isCrawling)
		{
			vector += Vector3.down * 0.15f;
		}
		this.normalPreviousPosition = vector;
		this.normalTransformPivot.position = Vector3.Lerp(this.normalTransformPivot.position, vector, 10f * Time.deltaTime);
		Quaternion b = Quaternion.Euler(this.normalAimVertical, this.normalAimHorizontal, 0f);
		float num6 = Mathf.Lerp(50f, 6.25f, GameplayManager.instance.cameraSmoothing / 100f);
		this.normalTransformPivot.rotation = Quaternion.Lerp(this.normalTransformPivot.rotation, b, num6 * Time.deltaTime);
		this.normalTransformPivot.localRotation = Quaternion.Euler(this.normalTransformPivot.localRotation.eulerAngles.x, this.normalTransformPivot.localRotation.eulerAngles.y, 0f);
		bool flag = false;
		float num7 = this.normalMaxDistance;
		RaycastHit[] array = Physics.SphereCastAll(this.normalTransformPivot.position, 0.1f, -this.normalTransformPivot.forward, this.normalMaxDistance, SemiFunc.LayerMaskGetVisionObstruct());
		if (array.Length != 0)
		{
			foreach (RaycastHit raycastHit in array)
			{
				if (!raycastHit.transform.GetComponent<PlayerHealthGrab>() && !raycastHit.transform.GetComponent<PlayerAvatar>() && !raycastHit.transform.GetComponent<PlayerTumble>())
				{
					num7 = Mathf.Min(num7, raycastHit.distance);
					if (raycastHit.transform.CompareTag("Wall"))
					{
						flag = true;
					}
					if (raycastHit.collider.bounds.size.magnitude > 2f)
					{
						flag = true;
					}
				}
			}
			this.normalDistanceTarget = Mathf.Max(this.normalMinDistance, num7);
		}
		else
		{
			this.normalDistanceTarget = this.normalMaxDistance;
		}
		Vector3 b2 = new Vector3(0f, 0f, -this.normalDistanceTarget);
		this.normalTransformDistance.localPosition = Vector3.Lerp(this.normalTransformDistance.localPosition, b2, Time.deltaTime * 5f);
		float num8 = -this.normalTransformDistance.localPosition.z;
		Vector3 direction = this.normalTransformPivot.position - this.normalTransformDistance.position;
		float num9 = direction.magnitude;
		RaycastHit raycastHit2;
		if (Physics.SphereCast(this.normalTransformDistance.position, 0.15f, direction, out raycastHit2, this.normalMaxDistance, LayerMask.GetMask(new string[]
		{
			"PlayerVisuals"
		}), QueryTriggerInteraction.Collide))
		{
			num9 = raycastHit2.distance;
		}
		num9 = num8 - num9 - 0.1f;
		if (flag)
		{
			float num10 = Mathf.Max(num7, num9);
			this.MainCamera.nearClipPlane = Mathf.Max(num8 - num10, 0.01f);
		}
		else
		{
			this.MainCamera.nearClipPlane = 0.01f;
		}
		RenderSettings.fogStartDistance = this.MainCamera.nearClipPlane;
		base.transform.position = this.normalTransformDistance.position;
		base.transform.rotation = this.normalTransformDistance.rotation;
		if (this.player && base.transform.position.y < this.player.transform.position.y + 0.25f && num5 < 0f)
		{
			this.normalAimVertical = num4;
		}
		if (SemiFunc.InputDown(InputKey.Jump))
		{
			this.PlayerSwitch(true);
		}
		if (SemiFunc.InputDown(InputKey.SpectateNext))
		{
			this.PlayerSwitch(true);
		}
		if (SemiFunc.InputDown(InputKey.SpectatePrevious))
		{
			this.PlayerSwitch(false);
		}
		if (this.player && this.player.voiceChatFetched)
		{
			this.player.voiceChat.SpatialDisable(0.1f);
		}
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x0008C580 File Offset: 0x0008A780
	private void UpdateState(SpectateCamera.State _state)
	{
		if (this.currentState == _state)
		{
			return;
		}
		this.currentState = _state;
		this.stateImpulse = true;
		this.stateTimer = 0f;
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0008C5A5 File Offset: 0x0008A7A5
	public bool CheckState(SpectateCamera.State _state)
	{
		return this.currentState == _state;
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0008C5B0 File Offset: 0x0008A7B0
	private void PlayerSwitch(bool _next = true)
	{
		if (GameDirector.instance.PlayerList.All((PlayerAvatar p) => p.isDisabled))
		{
			return;
		}
		int i = 0;
		int num = this.currentPlayerListIndex;
		int count = GameDirector.instance.PlayerList.Count;
		while (i < count)
		{
			if (_next)
			{
				num = (num + 1) % count;
			}
			else
			{
				num = (num - 1 + count) % count;
			}
			PlayerAvatar playerAvatar = GameDirector.instance.PlayerList[num];
			if (this.player != playerAvatar && !playerAvatar.isDisabled)
			{
				this.currentPlayerListIndex = num;
				this.player = playerAvatar;
				this.normalTransformPivot.position = this.player.spectatePoint.position;
				this.normalAimHorizontal = this.player.transform.eulerAngles.y;
				this.normalAimVertical = 0f;
				this.normalTransformPivot.rotation = Quaternion.Euler(this.normalAimVertical, this.normalAimHorizontal, 0f);
				this.normalTransformPivot.localRotation = Quaternion.Euler(this.normalTransformPivot.localRotation.eulerAngles.x, this.normalTransformPivot.localRotation.eulerAngles.y, 0f);
				this.normalTransformDistance.localPosition = new Vector3(0f, 0f, -2f);
				base.transform.position = this.normalTransformDistance.position;
				base.transform.rotation = this.normalTransformDistance.rotation;
				if (SemiFunc.IsMultiplayer())
				{
					SemiFunc.HUDSpectateSetName(this.player.playerName);
				}
				SemiFunc.LightManagerSetCullTargetTransform(this.player.transform);
				CameraGlitch.Instance.PlayTiny();
				GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
				AudioManager.instance.RestartAudioLoopDistances();
				this.normalMaxDistance = 3f;
				return;
			}
			i++;
		}
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x0008C7B9 File Offset: 0x0008A9B9
	public void UpdatePlayer(PlayerAvatar deadPlayer)
	{
		if (deadPlayer == this.player)
		{
			this.SetDeath(deadPlayer.spectatePoint);
		}
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x0008C7D8 File Offset: 0x0008A9D8
	public void StopSpectate()
	{
		this.ParentObject.parent = this.PreviousParent;
		this.ParentObject.localPosition = Vector3.zero;
		this.ParentObject.localRotation = Quaternion.identity;
		this.MainCamera.nearClipPlane = 0.001f;
		this.MainCamera.farClipPlane = this.previousFarClipPlane;
		this.MainCamera.transform.localPosition = Vector3.zero;
		this.MainCamera.transform.localRotation = Quaternion.identity;
		this.MainCamera.fieldOfView = this.previousFieldOfView;
		RenderSettings.fog = true;
		RenderSettings.fogStartDistance = 0f;
		PostProcessing.Instance.SpectateReset();
		PlayerAvatar.instance.spectating = false;
		SemiFunc.LightManagerSetCullTargetTransform(PlayerAvatar.instance.transform);
		AudioManager.instance.AudioListener.TargetPositionTransform = this.MainCamera.transform;
		Object.Destroy(this.normalTransformPivot.gameObject);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0008C8DC File Offset: 0x0008AADC
	private void DeathNearClipLogic(bool _instant)
	{
		if (!this.CheckState(SpectateCamera.State.Death))
		{
			return;
		}
		Vector3 direction = this.MainCamera.transform.position - this.deathSmoothLookAtPoint;
		RaycastHit[] array = Physics.RaycastAll(this.deathSmoothLookAtPoint, direction, direction.magnitude, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		float num = float.PositiveInfinity;
		Vector3 vector = Vector3.zero;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.transform.CompareTag("Ceiling") && raycastHit.distance < num)
			{
				num = raycastHit.distance;
				vector = raycastHit.point;
			}
		}
		if (vector != Vector3.zero)
		{
			this.deathCameraNearClipPlane = (this.MainCamera.transform.position - vector).magnitude + 0.5f;
		}
		if (_instant)
		{
			this.MainCamera.nearClipPlane = this.deathCameraNearClipPlane;
			return;
		}
		this.MainCamera.nearClipPlane = Mathf.Lerp(this.MainCamera.nearClipPlane, this.deathCameraNearClipPlane, Time.deltaTime * 10f);
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x0008CA04 File Offset: 0x0008AC04
	public void SetDeath(Transform _spectatePoint)
	{
		this.deathPosition = _spectatePoint.position;
		this.deathPlayerSpectatePoint = _spectatePoint;
		base.transform.position = _spectatePoint.position;
		base.transform.rotation = _spectatePoint.rotation;
		this.deathFollowPoint = this.deathPosition;
		this.deathFollowPointTarget = this.deathPosition;
		this.deathSmoothLookAtPoint = this.deathPosition;
		this.deathOrbitInstantSet = true;
		SemiFunc.LightManagerSetCullTargetTransform(this.deathPlayerSpectatePoint);
		this.deathSmoothLookAtPoint = this.deathPlayerSpectatePoint.position;
		base.transform.position = this.deathFollowPointTarget;
		this.deathFollowPoint = this.deathFollowPointTarget;
		this.deathSmoothLookAtPoint = this.deathPlayerSpectatePoint.position;
		this.DeathNearClipLogic(true);
		this.UpdateState(SpectateCamera.State.Death);
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0008CACC File Offset: 0x0008ACCC
	private void RoomVolumeLogic()
	{
		RoomVolumeCheck roomVolumeCheck = PlayerController.instance.playerAvatarScript.RoomVolumeCheck;
		roomVolumeCheck.PauseCheckTimer = 1f;
		if (this.player)
		{
			RoomVolumeCheck roomVolumeCheck2 = this.player.RoomVolumeCheck;
			roomVolumeCheck.CurrentRooms.Clear();
			roomVolumeCheck.CurrentRooms.AddRange(roomVolumeCheck2.CurrentRooms);
		}
	}

	// Token: 0x040019CF RID: 6607
	public static SpectateCamera instance;

	// Token: 0x040019D0 RID: 6608
	private SpectateCamera.State currentState;

	// Token: 0x040019D1 RID: 6609
	private float stateTimer;

	// Token: 0x040019D2 RID: 6610
	private bool stateImpulse = true;

	// Token: 0x040019D3 RID: 6611
	internal PlayerAvatar player;

	// Token: 0x040019D4 RID: 6612
	private float previousFarClipPlane = 0.01f;

	// Token: 0x040019D5 RID: 6613
	private float previousFieldOfView;

	// Token: 0x040019D6 RID: 6614
	private Camera MainCamera;

	// Token: 0x040019D7 RID: 6615
	private Camera TopCamera;

	// Token: 0x040019D8 RID: 6616
	private Transform ParentObject;

	// Token: 0x040019D9 RID: 6617
	private Transform PreviousParent;

	// Token: 0x040019DA RID: 6618
	private float cameraFieldOfView = 10f;

	// Token: 0x040019DB RID: 6619
	private int currentPlayerListIndex;

	// Token: 0x040019DC RID: 6620
	private Transform deathPlayerSpectatePoint;

	// Token: 0x040019DD RID: 6621
	private Vector3 deathCameraOffset;

	// Token: 0x040019DE RID: 6622
	private float deathCameraNearClipPlane;

	// Token: 0x040019DF RID: 6623
	private float deathCurrentY;

	// Token: 0x040019E0 RID: 6624
	private Vector3 deathVelocity;

	// Token: 0x040019E1 RID: 6625
	private float deathSpeed = 6f;

	// Token: 0x040019E2 RID: 6626
	private Vector3 deathFollowPoint;

	// Token: 0x040019E3 RID: 6627
	private Vector3 deathFollowPointTarget;

	// Token: 0x040019E4 RID: 6628
	private Vector3 deathFollowPointVelocity;

	// Token: 0x040019E5 RID: 6629
	private Vector3 deathSmoothLookAtPoint;

	// Token: 0x040019E6 RID: 6630
	private Quaternion deathSmoothOrbit;

	// Token: 0x040019E7 RID: 6631
	private Quaternion deathTargetOrbit;

	// Token: 0x040019E8 RID: 6632
	private bool deathOrbitInstantSet;

	// Token: 0x040019E9 RID: 6633
	private Vector3 deathPosition;

	// Token: 0x040019EA RID: 6634
	public Transform normalTransformPivot;

	// Token: 0x040019EB RID: 6635
	public Transform normalTransformDistance;

	// Token: 0x040019EC RID: 6636
	private Vector3 normalPreviousPosition;

	// Token: 0x040019ED RID: 6637
	private float normalAimHorizontal;

	// Token: 0x040019EE RID: 6638
	private float normalAimVertical;

	// Token: 0x040019EF RID: 6639
	private float normalMinDistance = 1f;

	// Token: 0x040019F0 RID: 6640
	private float normalMaxDistance = 3f;

	// Token: 0x040019F1 RID: 6641
	private float normalDistanceTarget;

	// Token: 0x040019F2 RID: 6642
	private float normalDistanceCheckTimer;

	// Token: 0x0200037A RID: 890
	public enum State
	{
		// Token: 0x040027AC RID: 10156
		Death,
		// Token: 0x040027AD RID: 10157
		Normal
	}
}
