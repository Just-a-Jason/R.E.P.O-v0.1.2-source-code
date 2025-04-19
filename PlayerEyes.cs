using System;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class PlayerEyes : MonoBehaviour
{
	// Token: 0x06000ED4 RID: 3796 RVA: 0x00086B54 File Offset: 0x00084D54
	private void Start()
	{
		this.playerAvatarVisuals = base.GetComponent<PlayerAvatarVisuals>();
		this.playerAvatar = this.playerAvatarVisuals.playerAvatar;
		this.playerAvatarRightArm = base.GetComponent<PlayerAvatarRightArm>();
		if (!this.playerAvatarVisuals.isMenuAvatar && (!GameManager.Multiplayer() || (this.playerAvatar && this.playerAvatar.photonView.IsMine)))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x00086BC4 File Offset: 0x00084DC4
	private void MenuLookAt()
	{
		if (!this.playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		this.Override(this.menuAvatarPointer.position, 0.1f, this.menuAvatarPointer.gameObject);
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x00086BF8 File Offset: 0x00084DF8
	private void LookAtTransform(Transform _otherPhysGrabPoint, bool _softOverride)
	{
		this.lookAtActive = false;
		if (this.overrideActive)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.overridePosition;
		}
		else if (this.playerAvatarRightArm.mapToolController.Active && this.playerAvatarRightArm.mapToolController.HideLerp <= 0f)
		{
			this.lookAt.transform.position = this.playerAvatarRightArm.mapToolController.PlayerLookTarget.position;
		}
		else if (this.playerAvatarRightArm.physGrabBeam.isActiveAndEnabled && this.playerAvatar.physGrabber.grabbedObjectTransform && !this.playerAvatar.physGrabber.grabbedObjectTransform.GetComponent<PhysGrabCart>())
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.playerAvatarRightArm.physGrabBeam.PhysGrabPoint.position;
		}
		else if (this.playerAvatar.healthGrab.staticGrabObject.playerGrabbing.Count > 0)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.playerAvatarVisuals.transform.position + this.playerAvatarVisuals.transform.forward * 2f;
		}
		else if (this.playerAvatar.isTumbling && this.playerAvatar.tumble.physGrabObject.playerGrabbing.Count > 0)
		{
			this.lookAtActive = true;
			Vector3 position = this.playerAvatar.tumble.physGrabObject.playerGrabbing[0].playerAvatar.playerAvatarVisuals.headLookAtTransform.position;
			if (this.playerAvatar.isLocal)
			{
				position = this.playerAvatar.localCameraPosition;
			}
			this.lookAt.transform.position = position;
		}
		else if (_otherPhysGrabPoint)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = _otherPhysGrabPoint.position;
		}
		else if (_softOverride)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = this.overrideSoftPosition;
		}
		else if (this.currentTalker)
		{
			Vector3 position2 = this.currentTalker.playerAvatarVisuals.headLookAtTransform.position;
			if (this.currentTalker.isLocal)
			{
				position2 = this.currentTalker.localCameraPosition;
			}
			this.lookAtActive = true;
			this.lookAt.transform.position = position2;
		}
		else if (this.playerAvatar.isTumbling)
		{
			this.lookAtActive = true;
			this.lookAt.transform.position = base.transform.position + this.playerAvatar.localCameraTransform.forward * 2f;
		}
		else
		{
			this.lookAt.transform.position = this.targetIdle.position;
		}
		this.lookAt.transform.rotation = this.targetIdle.rotation;
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x00086F28 File Offset: 0x00085128
	private void ClosestPhysGrabPoint()
	{
		this.otherPhysGrabPoint = null;
		float num = 6f;
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar != this.playerAvatar && playerAvatar.physGrabber.physGrabBeam.gameObject.activeSelf && playerAvatar.physGrabber.grabbedObjectTransform && !playerAvatar.physGrabber.grabbedObjectTransform.GetComponent<PhysGrabCart>())
			{
				float num2 = Vector3.Distance(playerAvatar.physGrabber.physGrabPoint.position, this.eyeLeft.position);
				if (num2 < num)
				{
					num = num2;
					this.otherPhysGrabPoint = playerAvatar.physGrabber.physGrabPoint;
				}
			}
		}
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x00087014 File Offset: 0x00085214
	private void EyesLead()
	{
		if (this.playerAvatarVisuals.turnDifference > 0.5f && this.playerAvatarVisuals.turnDirection != 0f)
		{
			this.eyeSideAmount = this.playerAvatarVisuals.turnDifference * -this.playerAvatarVisuals.turnDirection * 20f;
			this.eyeLeadTimer = 0.1f;
		}
		if (this.playerAvatarVisuals.upDifference > 0.5f && this.playerAvatarVisuals.upDirection != 0f)
		{
			this.eyeUpAmount = this.playerAvatarVisuals.upDifference * -this.playerAvatarVisuals.upDirection * 20f;
			this.eyeLeadTimer = 0.1f;
		}
		if (this.eyeLeadTimer > 0f)
		{
			this.eyeLeadTimer -= this.deltaTime;
			Vector3 localEulerAngles = new Vector3(this.eyeUpAmount, this.eyeSideAmount, 0f);
			Quaternion localRotation = this.targetLead.localRotation;
			this.targetLead.localEulerAngles = localEulerAngles;
			Quaternion localRotation2 = this.targetLead.localRotation;
			this.targetLead.localRotation = localRotation;
			this.targetLead.localRotation = Quaternion.Lerp(localRotation, localRotation2, this.deltaTime * 5f);
			return;
		}
		this.eyeSideAmount = 0f;
		this.eyeUpAmount = 0f;
		this.targetLead.localRotation = Quaternion.Lerp(this.targetLead.localRotation, Quaternion.identity, this.deltaTime * 20f);
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x00087190 File Offset: 0x00085390
	private void Update()
	{
		if (this.playerAvatarVisuals.isMenuAvatar && !this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (!this.playerAvatarVisuals.isMenuAvatar && (!LevelGenerator.Instance.Generated || this.playerAvatar.playerHealth.hurtFreeze))
		{
			return;
		}
		this.deltaTime = this.playerAvatarVisuals.deltaTime;
		this.MenuLookAt();
		this.pupilLeft.localScale = Vector3.one * this.pupilSizeMultiplier * this.pupilLeftSizeMultiplier;
		this.pupilRight.localScale = Vector3.one * this.pupilSizeMultiplier * this.pupilRightSizeMultiplier;
		this.EyesLead();
		this.ClosestPhysGrabPoint();
		if (this.currentTalker && !this.currentTalker.voiceChat.isTalking)
		{
			this.currentTalkerTime = 0f;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (!playerAvatar.isDisabled && playerAvatar.voiceChat && playerAvatar.voiceChat.isTalking && Vector3.Distance(base.transform.position, playerAvatar.transform.position) < 6f)
			{
				this.currentTalkerTimer = Random.Range(2f, 4f);
				if (playerAvatar != this.playerAvatar && playerAvatar.voiceChat.isTalkingStartTime > this.currentTalkerTime)
				{
					this.currentTalker = playerAvatar;
					this.currentTalkerTime = playerAvatar.voiceChat.isTalkingStartTime;
				}
			}
		}
		if (this.currentTalkerTimer > 0f)
		{
			this.currentTalkerTimer -= this.deltaTime;
			if (this.currentTalkerTimer <= 0f)
			{
				this.currentTalker = null;
			}
		}
		bool softOverride = false;
		if (this.overrideSoftActive)
		{
			softOverride = true;
			if (this.overrideSoftObject)
			{
				PlayerAvatar component = this.overrideSoftObject.GetComponent<PlayerAvatar>();
				if (component && component == this.playerAvatar)
				{
					softOverride = false;
				}
				else
				{
					PlayerTumble component2 = this.overrideSoftObject.GetComponent<PlayerTumble>();
					if (component2 && component2.playerAvatar == this.playerAvatar)
					{
						softOverride = false;
					}
				}
			}
		}
		this.LookAtTransform(this.otherPhysGrabPoint, softOverride);
		if (this.overrideSoftTimer > 0f)
		{
			this.overrideSoftTimer -= this.deltaTime;
			if (this.overrideSoftTimer <= 0f)
			{
				this.overrideSoftActive = false;
			}
		}
		if (this.overrideTimer > 0f)
		{
			this.overrideTimer -= this.deltaTime;
			if (this.overrideTimer <= 0f)
			{
				this.overrideActive = false;
			}
		}
		this.EyeLookAt(ref this.eyeRight, ref this.springQuaternionRight, true, 50f, 30f);
		this.EyeLookAt(ref this.eyeLeft, ref this.springQuaternionLeft, true, 50f, 30f);
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x000874B4 File Offset: 0x000856B4
	public void Override(Vector3 _position, float _time, GameObject _obj)
	{
		if (this.overrideActive && _obj != this.overrideObject)
		{
			return;
		}
		this.overrideActive = true;
		this.overrideObject = _obj;
		this.overridePosition = _position;
		this.overrideTimer = _time;
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x000874E9 File Offset: 0x000856E9
	public void OverrideSoft(Vector3 _position, float _time, GameObject _obj)
	{
		if (this.overrideSoftActive && _obj != this.overrideSoftObject)
		{
			return;
		}
		this.overrideSoftActive = true;
		this.overrideSoftObject = _obj;
		this.overrideSoftPosition = _position;
		this.overrideSoftTimer = _time;
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x00087520 File Offset: 0x00085720
	public void EyeLookAt(ref Transform _eyeTransform, ref SpringQuaternion _springQuaternion, bool _useSpring, float _clampX, float _clamY)
	{
		Quaternion localRotation = _eyeTransform.localRotation;
		Vector3 forward = SemiFunc.ClampDirection(this.lookAt.position - _eyeTransform.transform.position, this.lookAt.forward, _clampX);
		_eyeTransform.rotation = Quaternion.LookRotation(forward);
		float num = _eyeTransform.localEulerAngles.y;
		if (num > _clamY && num < 180f)
		{
			num = _clamY;
		}
		else if (num < 360f - _clamY && num > 180f)
		{
			num = 360f - _clamY;
		}
		else if (num < -_clamY)
		{
			num = -_clamY;
		}
		_eyeTransform.localEulerAngles = new Vector3(_eyeTransform.localEulerAngles.x, num, _eyeTransform.localEulerAngles.z);
		Quaternion localRotation2 = _eyeTransform.localRotation;
		_eyeTransform.localRotation = localRotation;
		if (_useSpring)
		{
			_eyeTransform.localRotation = SemiFunc.SpringQuaternionGet(_springQuaternion, localRotation2, this.deltaTime);
			return;
		}
		_eyeTransform.localRotation = localRotation2;
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x00087610 File Offset: 0x00085810
	private void OnDrawGizmos()
	{
		if (!this.debugDraw)
		{
			return;
		}
		float d = 0.075f;
		Gizmos.color = new Color(1f, 0.93f, 0.99f, 0.6f);
		Gizmos.matrix = this.lookAt.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * d);
		Gizmos.color = new Color(0f, 1f, 0.98f, 0.3f);
		Gizmos.DrawCube(Vector3.zero, Vector3.one * d);
	}

	// Token: 0x040018DC RID: 6364
	public bool debugDraw;

	// Token: 0x040018DD RID: 6365
	private PlayerAvatarVisuals playerAvatarVisuals;

	// Token: 0x040018DE RID: 6366
	private PlayerAvatar playerAvatar;

	// Token: 0x040018DF RID: 6367
	private PlayerAvatarRightArm playerAvatarRightArm;

	// Token: 0x040018E0 RID: 6368
	public Transform menuAvatarPointer;

	// Token: 0x040018E1 RID: 6369
	public Transform eyeLeft;

	// Token: 0x040018E2 RID: 6370
	public Transform eyeRight;

	// Token: 0x040018E3 RID: 6371
	public Transform pupilLeft;

	// Token: 0x040018E4 RID: 6372
	public Transform pupilRight;

	// Token: 0x040018E5 RID: 6373
	[Space]
	public Transform targetIdle;

	// Token: 0x040018E6 RID: 6374
	public Transform targetLead;

	// Token: 0x040018E7 RID: 6375
	[Space]
	public Transform lookAt;

	// Token: 0x040018E8 RID: 6376
	public SpringQuaternion springQuaternionLeft;

	// Token: 0x040018E9 RID: 6377
	public SpringQuaternion springQuaternionRight;

	// Token: 0x040018EA RID: 6378
	private Transform otherPhysGrabPoint;

	// Token: 0x040018EB RID: 6379
	private float eyeLeadTimer;

	// Token: 0x040018EC RID: 6380
	private float eyeSideAmount;

	// Token: 0x040018ED RID: 6381
	private float eyeUpAmount;

	// Token: 0x040018EE RID: 6382
	internal bool lookAtActive;

	// Token: 0x040018EF RID: 6383
	private PlayerAvatar currentTalker;

	// Token: 0x040018F0 RID: 6384
	private float currentTalkerTime;

	// Token: 0x040018F1 RID: 6385
	private float currentTalkerTimer;

	// Token: 0x040018F2 RID: 6386
	private bool overrideActive;

	// Token: 0x040018F3 RID: 6387
	private float overrideTimer;

	// Token: 0x040018F4 RID: 6388
	private Vector3 overridePosition;

	// Token: 0x040018F5 RID: 6389
	private GameObject overrideObject;

	// Token: 0x040018F6 RID: 6390
	private bool overrideSoftActive;

	// Token: 0x040018F7 RID: 6391
	private float overrideSoftTimer;

	// Token: 0x040018F8 RID: 6392
	private Vector3 overrideSoftPosition;

	// Token: 0x040018F9 RID: 6393
	private GameObject overrideSoftObject;

	// Token: 0x040018FA RID: 6394
	private float deltaTime;

	// Token: 0x040018FB RID: 6395
	internal float pupilLeftSizeMultiplier = 1f;

	// Token: 0x040018FC RID: 6396
	internal float pupilRightSizeMultiplier = 1f;

	// Token: 0x040018FD RID: 6397
	internal float pupilSizeMultiplier = 1f;
}
