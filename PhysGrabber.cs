using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class PhysGrabber : MonoBehaviour, IPunObservable
{
	// Token: 0x06000D7D RID: 3453 RVA: 0x00078C5C File Offset: 0x00076E5C
	private void Start()
	{
		base.StartCoroutine(this.LateStart());
		this.physRotation = Quaternion.identity;
		this.physRotationBase = Quaternion.identity;
		this.mask = SemiFunc.LayerMaskGetVisionObstruct() - LayerMask.GetMask(new string[]
		{
			"Player"
		});
		this.playerAvatar = base.GetComponent<PlayerAvatar>();
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0 || this.photonView.IsMine)
		{
			this.isLocal = true;
			PhysGrabber.instance = this;
		}
		foreach (object obj in this.physGrabPoint)
		{
			Transform transform = (Transform)obj;
			if (transform.name == "Visual1")
			{
				this.physGrabPointVisual1 = transform.gameObject;
				foreach (object obj2 in transform)
				{
					Transform transform2 = (Transform)obj2;
					if (transform2.name == "Visual2")
					{
						this.physGrabPointVisual2 = transform2.gameObject;
					}
				}
			}
			if (transform.name == "Rotate")
			{
				this.physGrabPointVisualRotate = transform;
				transform.GetComponent<PhysGrabPointRotate>().physGrabber = this;
			}
			if (transform.name == "Grid")
			{
				this.physGrabPointVisualGrid = transform;
				foreach (object obj3 in transform)
				{
					Transform transform3 = (Transform)obj3;
					this.physGrabPointVisualGridObject = transform3.gameObject;
					this.physGrabPointVisualGridObject.SetActive(false);
				}
			}
		}
		this.physGrabPoint.SetParent(null, true);
		this.PhysGrabPointDeactivate();
		this.physGrabPointPuller.gameObject.SetActive(false);
		this.physGrabBeam.transform.SetParent(null, false);
		this.physGrabBeam.transform.position = Vector3.zero;
		this.physGrabBeam.transform.rotation = Quaternion.identity;
		this.physGrabBeam.SetActive(false);
		this.physGrabBeamAlphaOriginal = this.physGrabBeam.GetComponent<LineRenderer>().material.color.a;
		this.SoundSetup(this.startSound);
		this.SoundSetup(this.loopSound);
		this.SoundSetup(this.stopSound);
		if (!this.isLocal)
		{
			return;
		}
		this.playerCamera = Camera.main;
		PlayerController.instance.physGrabPoint = this.physGrabPoint;
		this.physGrabPointPlane.SetParent(null, false);
		this.physGrabPointPlane.position = Vector3.zero;
		this.physGrabPointPlane.rotation = Quaternion.identity;
		this.physGrabPointPlane.SetParent(CameraAim.Instance.transform, false);
		this.physGrabPointPlane.localPosition = Vector3.zero;
		this.physGrabPointPlane.localRotation = Quaternion.identity;
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00078FB0 File Offset: 0x000771B0
	private void OnDestroy()
	{
		Object.Destroy(this.physGrabBeam);
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00078FBD File Offset: 0x000771BD
	public void OverrideGrabDistance(float dist)
	{
		this.prevPullerDistance = this.pullerDistance;
		this.pullerDistance = dist;
		this.overrideGrabDistance = dist;
		this.overrideGrabDistanceTimer = 0.1f;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00078FE4 File Offset: 0x000771E4
	private void OverrideGrabDistanceTick()
	{
		if (this.overrideGrabDistanceTimer > 0f)
		{
			this.overrideGrabDistanceTimer -= Time.deltaTime;
			return;
		}
		if (this.overrideGrabDistanceTimer != -123f)
		{
			this.overrideGrabDistance = 0f;
			this.overrideGrabDistanceTimer = -123f;
		}
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00079034 File Offset: 0x00077234
	private IEnumerator LateStart()
	{
		while (!this.playerAvatar)
		{
			yield return new WaitForSeconds(0.2f);
		}
		string _steamID = SemiFunc.PlayerGetSteamID(this.playerAvatar);
		yield return new WaitForSeconds(0.2f);
		while (!StatsManager.instance.playerUpgradeStrength.ContainsKey(_steamID))
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (!SemiFunc.MenuLevel())
		{
			this.grabStrength += (float)StatsManager.instance.playerUpgradeStrength[_steamID] * 0.2f;
			this.throwStrength += (float)StatsManager.instance.playerUpgradeThrow[_steamID] * 0.3f;
			this.grabRange += (float)StatsManager.instance.playerUpgradeRange[_steamID] * 1f;
		}
		yield break;
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00079044 File Offset: 0x00077244
	public void SoundSetup(Sound _sound)
	{
		if (!SemiFunc.IsMultiplayer() || this.photonView.IsMine)
		{
			_sound.SpatialBlend = 0f;
			return;
		}
		_sound.Volume *= 0.5f;
		_sound.VolumeRandom *= 0.5f;
		_sound.SpatialBlend = 1f;
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000790A0 File Offset: 0x000772A0
	public void OverrideDisableRotationControls()
	{
		this.overrideDisableRotationControls = true;
		this.overrideDisableRotationControlsTimer = 0.1f;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x000790B4 File Offset: 0x000772B4
	private void OverrideDisableRotationControlsTick()
	{
		if (this.overrideDisableRotationControlsTimer > 0f)
		{
			this.overrideDisableRotationControlsTimer -= Time.fixedDeltaTime;
			if (this.overrideDisableRotationControlsTimer <= 0f)
			{
				this.overrideDisableRotationControls = false;
			}
		}
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x000790E9 File Offset: 0x000772E9
	public void OverrideGrab(PhysGrabObject target)
	{
		this.overrideGrab = true;
		this.overrideGrabTarget = target;
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x000790F9 File Offset: 0x000772F9
	public void OverrideGrabPoint(Transform grabPoint)
	{
		this.overrideGrabPointTransform = grabPoint;
		this.overrideGrabPointTimer = 0.1f;
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x0007910D File Offset: 0x0007730D
	public void OverrideGrabRelease()
	{
		this.overrideGrabRelease = true;
		this.overrideGrab = false;
		this.overrideGrabTarget = null;
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x00079124 File Offset: 0x00077324
	public void GrabberHeal()
	{
		if (!this.healing)
		{
			this.photonView.RPC("HealStart", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00079144 File Offset: 0x00077344
	private void ColorStateSetColor(Color mainColor, Color emissionColor)
	{
		Material material = this.physGrabBeam.GetComponent<LineRenderer>().material;
		Material material2 = this.physGrabPointVisual1.GetComponent<MeshRenderer>().material;
		Material material3 = this.physGrabPointVisual2.GetComponent<MeshRenderer>().material;
		Material material4 = this.physGrabPointVisualRotate.GetComponent<MeshRenderer>().material;
		Light grabberLight = this.playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberLight;
		Material material5 = this.playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberOrbSpheres[0].GetComponent<MeshRenderer>().material;
		Material material6 = this.playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberOrbSpheres[1].GetComponent<MeshRenderer>().material;
		if (material)
		{
			material.color = mainColor;
		}
		if (material)
		{
			material.SetColor("_EmissionColor", emissionColor);
		}
		if (material2)
		{
			material2.color = mainColor;
		}
		if (material2)
		{
			material2.SetColor("_EmissionColor", emissionColor);
		}
		if (material3)
		{
			material3.color = mainColor;
		}
		if (material3)
		{
			material3.SetColor("_EmissionColor", emissionColor);
		}
		if (material4)
		{
			material4.color = mainColor;
		}
		if (material4)
		{
			material4.SetColor("_EmissionColor", emissionColor);
		}
		if (grabberLight)
		{
			grabberLight.color = mainColor;
		}
		if (material5)
		{
			material5.color = mainColor;
		}
		if (material5)
		{
			material5.SetColor("_EmissionColor", emissionColor);
		}
		if (material6)
		{
			material6.color = mainColor;
		}
		if (material6)
		{
			material6.SetColor("_EmissionColor", emissionColor);
		}
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x000792DD File Offset: 0x000774DD
	public void OverrideColorToGreen(float time = 0.1f)
	{
		this.colorState = 1;
		this.colorStateOverrideTimer = time;
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x000792ED File Offset: 0x000774ED
	public void OverrideColorToPurple(float time = 0.1f)
	{
		this.colorState = 2;
		this.colorStateOverrideTimer = time;
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00079300 File Offset: 0x00077500
	private void ColorStates()
	{
		if (this.prevColorState == this.colorState)
		{
			return;
		}
		this.prevColorState = this.colorState;
		Color mainColor = new Color(1f, 0.1856f, 0f, 0.15f);
		Color emissionColor = new Color(1f, 0.1856f, 0f, 1f);
		if (this.colorState == 0)
		{
			if (!VideoGreenScreen.instance)
			{
				mainColor = new Color(1f, 0.1856f, 0f, 0.15f);
			}
			else
			{
				mainColor = new Color(1f, 0.1856f, 0f, 1f);
			}
			emissionColor = new Color(1f, 0.1856f, 0f, 1f);
			this.ColorStateSetColor(mainColor, emissionColor);
			return;
		}
		if (this.colorState == 1)
		{
			if (!VideoGreenScreen.instance)
			{
				mainColor = new Color(0f, 1f, 0f, 0.15f);
			}
			else
			{
				mainColor = new Color(0f, 1f, 0f, 1f);
			}
			emissionColor = new Color(0f, 1f, 0f, 1f);
			this.ColorStateSetColor(mainColor, emissionColor);
			return;
		}
		if (this.colorState == 2)
		{
			if (!VideoGreenScreen.instance)
			{
				mainColor = new Color(1f, 0f, 1f, 0.15f);
			}
			else
			{
				mainColor = new Color(1f, 0f, 1f, 1f);
			}
			emissionColor = new Color(1f, 0f, 1f, 1f);
			this.ColorStateSetColor(mainColor, emissionColor);
		}
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x000794AF File Offset: 0x000776AF
	private void ColorStateTick()
	{
		if (this.colorStateOverrideTimer > 0f)
		{
			this.colorStateOverrideTimer -= Time.fixedDeltaTime;
			return;
		}
		this.colorState = 0;
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x000794D8 File Offset: 0x000776D8
	[PunRPC]
	private void HealStart()
	{
		this.physGrabBeam.GetComponent<LineRenderer>().material = this.physGrabBeamMaterialBatteryCharge;
		this.physGrabPointVisual1.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterialBatteryCharge;
		this.physGrabPointVisual2.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterialBatteryCharge;
		this.physGrabBeam.GetComponent<PhysGrabBeam>().scrollSpeed = new Vector2(-5f, 0f);
		this.physGrabBeam.GetComponent<PhysGrabBeam>().lineMaterial = this.physGrabBeam.GetComponent<LineRenderer>().material;
		this.healing = true;
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00079570 File Offset: 0x00077770
	private void ResetBeam()
	{
		if (this.healing)
		{
			this.physGrabBeam.GetComponent<LineRenderer>().material = this.physGrabBeamMaterial;
			this.physGrabPointVisual1.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterial;
			this.physGrabPointVisual2.GetComponent<MeshRenderer>().material = this.physGrabBeamMaterial;
			this.physGrabBeam.GetComponent<PhysGrabBeam>().scrollSpeed = this.physGrabBeam.GetComponent<PhysGrabBeam>().originalScrollSpeed;
			this.physGrabBeam.GetComponent<PhysGrabBeam>().lineMaterial = this.physGrabBeam.GetComponent<LineRenderer>().material;
			this.healing = false;
		}
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x00079611 File Offset: 0x00077811
	public void ChangeBeamAlpha(float alpha)
	{
		if (this.physGramBeamAlphaTimer == -123f)
		{
			this.physGrabBeamAlpha = this.physGrabBeamAlphaOriginal;
		}
		this.physGrabBeamAlphaChangeTo = alpha;
		this.physGramBeamAlphaTimer = 0.1f;
		this.physGrabBeamAlphaChangeProgress = 0f;
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x0007964C File Offset: 0x0007784C
	private void TickerBeamAlphaChange()
	{
		if (this.physGramBeamAlphaTimer > 0f)
		{
			this.physGrabBeamAlpha = Mathf.Lerp(this.physGrabBeamAlpha, this.physGrabBeamAlphaChangeTo, this.physGrabBeamAlphaChangeProgress);
			if (this.physGrabBeamAlphaChangeProgress < 1f)
			{
				this.physGrabBeamAlphaChangeProgress += 4f * Time.deltaTime;
				Material material = this.physGrabBeam.GetComponent<LineRenderer>().material;
				material.SetColor("_Color", new Color(material.color.r, material.color.g, material.color.b, this.physGrabBeamAlpha));
				Material material2 = this.physGrabPointVisual1.GetComponent<MeshRenderer>().material;
				Material material3 = this.physGrabPointVisual2.GetComponent<MeshRenderer>().material;
				material2.SetColor("_Color", new Color(material2.color.r, material2.color.g, material2.color.b, this.physGrabBeamAlpha));
				material3.SetColor("_Color", new Color(material3.color.r, material3.color.g, material3.color.b, this.physGrabBeamAlpha));
			}
		}
		else if (this.physGramBeamAlphaTimer != -123f)
		{
			this.physGrabBeamAlphaChangeProgress = 0f;
			Material material4 = this.physGrabBeam.GetComponent<LineRenderer>().material;
			material4.SetColor("_Color", new Color(material4.color.r, material4.color.g, material4.color.b, this.physGrabBeamAlphaOriginal));
			Material material5 = this.physGrabPointVisual1.GetComponent<MeshRenderer>().material;
			Material material6 = this.physGrabPointVisual2.GetComponent<MeshRenderer>().material;
			material5.SetColor("_Color", new Color(material5.color.r, material5.color.g, material5.color.b, this.physGrabBeamAlphaOriginal));
			material6.SetColor("_Color", new Color(material6.color.r, material6.color.g, material6.color.b, this.physGrabBeamAlphaOriginal));
			this.physGramBeamAlphaTimer = -123f;
		}
		if (this.physGramBeamAlphaTimer > 0f)
		{
			this.physGramBeamAlphaTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x000798B4 File Offset: 0x00077AB4
	public Quaternion GetRotationInput()
	{
		Quaternion rhs = Quaternion.AngleAxis(this.mouseTurningVelocity.y, Vector3.right);
		Quaternion lhs = Quaternion.AngleAxis(-this.mouseTurningVelocity.x, Vector3.up);
		Quaternion rhs2 = Quaternion.AngleAxis(this.mouseTurningVelocity.z, Vector3.forward);
		return lhs * rhs * rhs2;
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00079910 File Offset: 0x00077B10
	private void ObjectTurning()
	{
		if (!this.grabbedPhysGrabObject)
		{
			return;
		}
		if (!this.grabbed)
		{
			this.mouseTurningVelocity = Vector3.zero;
			this.physGrabPointVisualGrid.gameObject.SetActive(false);
			this.isRotating = false;
			return;
		}
		if (this.physGrabPointVisualGrid && this.grabbedPhysGrabObject)
		{
			this.physGrabPointVisualGrid.position = this.grabbedPhysGrabObject.midPoint;
		}
		if (this.mouseTurningVelocity.magnitude > 0.01f)
		{
			this.mouseTurningVelocity = Vector3.Lerp(this.mouseTurningVelocity, Vector3.zero, 1f * Time.deltaTime);
		}
		else
		{
			this.mouseTurningVelocity = Vector3.zero;
		}
		this.cameraRelativeGrabbedForward = this.cameraRelativeGrabbedForward.normalized;
		this.cameraRelativeGrabbedUp = this.cameraRelativeGrabbedUp.normalized;
		bool flag = false;
		if (this.isLocal && SemiFunc.InputHold(InputKey.Rotate))
		{
			flag = true;
		}
		if (flag)
		{
			float axis = Input.GetAxis("Mouse X");
			float axis2 = Input.GetAxis("Mouse Y");
			Vector3 b = new Vector3(axis, axis2, 0f) * 8f * Time.deltaTime;
			this.mouseTurningVelocity += b;
			if (this.isLocal)
			{
				this.isRotatingTimer = 0.1f;
			}
		}
		if (this.isRotating)
		{
			this.physGrabPointVisualGrid.gameObject.SetActive(true);
			Transform localCameraTransform = this.playerAvatar.localCameraTransform;
			if (this.physRotatingTimer <= 0f)
			{
				this.physRotatingTimer = 0.25f;
				this.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.forward);
				this.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.up);
				this.physGrabPointVisualGrid.rotation = this.grabbedObjectTransform.rotation;
			}
			this.physRotatingTimer = 0.25f;
			float mass = this.grabbedPhysGrabObject.rb.mass;
			float num = 1f / mass;
			num = Mathf.Clamp(num, 0f, 0.5f);
			if (num != 0f)
			{
				this.grabbedPhysGrabObject.OverrideAngularDrag(40f * num, 0.1f);
			}
			Quaternion rhs = Quaternion.AngleAxis(this.mouseTurningVelocity.y, localCameraTransform.right);
			Quaternion lhs = Quaternion.AngleAxis(-this.mouseTurningVelocity.x, localCameraTransform.up);
			Quaternion rhs2 = Quaternion.AngleAxis(this.mouseTurningVelocity.z, localCameraTransform.forward);
			Quaternion quaternion = lhs * rhs * rhs2;
			float fixedDeltaTime = Time.fixedDeltaTime;
			float num2 = 10000f * Time.fixedDeltaTime;
			float num3 = Quaternion.Angle(Quaternion.identity, quaternion);
			if (num3 > num2)
			{
				quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, num2 / num3);
			}
			quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, fixedDeltaTime * 20f);
			this.physGrabPointVisualGrid.rotation = quaternion * this.physGrabPointVisualGrid.rotation;
			this.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.forward);
			this.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(this.grabbedObjectTransform.up);
			foreach (PhysGrabber physGrabber in this.grabbedPhysGrabObject.playerGrabbing)
			{
				Transform localCameraTransform2 = physGrabber.playerAvatar.localCameraTransform;
				physGrabber.cameraRelativeGrabbedForward = localCameraTransform2.InverseTransformDirection(this.physGrabPointVisualGrid.forward);
				physGrabber.cameraRelativeGrabbedUp = localCameraTransform2.InverseTransformDirection(this.physGrabPointVisualGrid.up);
			}
			this.physGrabPointVisualGrid.transform.rotation = Quaternion.Slerp(this.physGrabPointVisualGrid.transform.rotation, this.grabbedObjectTransform.rotation, Time.deltaTime * 10f);
			return;
		}
		this.physGrabPointVisualGrid.gameObject.SetActive(false);
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00079CFC File Offset: 0x00077EFC
	private void OverrideGrabPointTimer()
	{
		if (this.overrideGrabPointTimer > 0f)
		{
			this.overrideGrabPointTimer -= Time.fixedDeltaTime;
			return;
		}
		this.overrideGrabPointTransform = null;
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00079D28 File Offset: 0x00077F28
	private void FixedUpdate()
	{
		this.OverrideGrabPointTimer();
		this.OverrideDisableRotationControlsTick();
		if (this.isLocal)
		{
			if (this.grabbedPhysGrabObject)
			{
				bool isMelee = this.grabbedPhysGrabObject.isMelee;
			}
			if (!this.overrideDisableRotationControls)
			{
				if (this.isRotatingTimer > 0f)
				{
					SemiFunc.CameraOverrideStopAim();
					if (!this.isRotating && this.grabbedObjectTransform)
					{
						Transform localCameraTransform = this.playerAvatar.localCameraTransform;
						this.mouseTurningVelocity = Vector3.zero;
					}
					this.isRotating = true;
				}
				else
				{
					this.isRotating = false;
				}
			}
		}
		if (this.stopRotationTimer > 0f)
		{
			this.stopRotationTimer -= Time.fixedDeltaTime;
		}
		this.ColorStateTick();
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00079DE0 File Offset: 0x00077FE0
	private void PushingPullingChecker()
	{
		if (this.overrideGrabDistanceTimer > 0f)
		{
			this.pullerDistance = this.overrideGrabDistance;
			this.prevPullerDistance = this.pullerDistance;
		}
		if (!this.grabbed)
		{
			this.isPushing = false;
			this.isPulling = false;
			this.isPushingTimer = 0f;
			this.isPullingTimer = 0f;
			this.prevPullerDistance = this.pullerDistance;
			return;
		}
		if (this.initialPressTimer > 0f)
		{
			this.prevPullerDistance = this.pullerDistance;
			this.isPushingTimer = 0f;
		}
		if (SemiFunc.InputScrollY() > 0f)
		{
			this.isPushingTimer = 0.1f;
		}
		if (SemiFunc.InputScrollY() < 0f)
		{
			this.isPullingTimer = 0.1f;
		}
		if (this.isPushingTimer > 0f)
		{
			this.isPushing = true;
			this.isPushingTimer -= Time.deltaTime;
		}
		else
		{
			this.isPushing = false;
		}
		if (this.isPullingTimer > 0f)
		{
			this.isPulling = true;
			this.isPullingTimer -= Time.deltaTime;
		}
		else
		{
			this.isPulling = false;
		}
		this.prevPullerDistance = this.pullerDistance;
		if (this.overrideGrabDistanceTimer > 0f)
		{
			this.pullerDistance = this.overrideGrabDistance;
			this.prevPullerDistance = this.pullerDistance;
		}
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00079F2C File Offset: 0x0007812C
	public void OverridePullDistanceIncrement(float distSpeed)
	{
		this.physGrabPointPlane.position += this.playerCamera.transform.forward * distSpeed;
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00079F5C File Offset: 0x0007815C
	private void Update()
	{
		if (this.isRotatingTimer > 0f)
		{
			this.isRotatingTimer -= Time.deltaTime;
		}
		this.PushingPullingChecker();
		this.ColorStates();
		this.ObjectTurning();
		if (this.grabbedObjectTransform && this.grabbedObjectTransform.name == this.playerAvatar.healthGrab.name)
		{
			this.OverrideColorToGreen(0.1f);
		}
		this.OverrideGrabDistanceTick();
		this.TickerBeamAlphaChange();
		if (this.initialPressTimer > 0f)
		{
			this.initialPressTimer -= Time.deltaTime;
		}
		if (this.physRotatingTimer > 0f)
		{
			this.physRotatingTimer -= Time.deltaTime;
		}
		if (this.grabbed && this.grabbedObjectTransform)
		{
			if (!this.overrideGrabPointTransform)
			{
				this.physGrabPoint.position = this.grabbedObjectTransform.TransformPoint(this.localGrabPosition);
			}
			else
			{
				this.physGrabPoint.position = this.overrideGrabPointTransform.position;
			}
		}
		if (this.isLocal)
		{
			bool flag = this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.isMelee;
			if (!SemiFunc.InputHold(InputKey.Rotate))
			{
				if (InputManager.instance.KeyPullAndPush() > 0f && Vector3.Distance(this.physGrabPointPuller.position, this.playerCamera.transform.position) < this.grabRange && !flag)
				{
					this.physGrabPointPlane.position += this.playerCamera.transform.forward * 0.2f;
				}
				if (InputManager.instance.KeyPullAndPush() < 0f && Vector3.Distance(this.physGrabPointPuller.position, this.playerCamera.transform.position) > this.minDistanceFromPlayer && !flag)
				{
					this.physGrabPointPlane.position -= this.playerCamera.transform.forward * 0.2f;
				}
			}
			if (this.overrideGrabDistanceTimer < 0f)
			{
				this.pullerDistance = Vector3.Distance(this.physGrabPointPuller.position, this.playerCamera.transform.position);
			}
			if (this.overrideGrabDistance > 0f)
			{
				Transform visionTransform = this.playerAvatar.PlayerVisionTarget.VisionTransform;
				this.physGrabPointPlane.position = visionTransform.position + visionTransform.forward * this.overrideGrabDistance;
			}
			else
			{
				if (this.pullerDistance < this.minDistanceFromPlayer)
				{
					this.physGrabPointPuller.position = this.playerCamera.transform.position + this.playerCamera.transform.forward * this.minDistanceFromPlayer;
				}
				if (this.pullerDistance > this.maxDistanceFromPlayer)
				{
					this.physGrabPointPuller.position = this.playerCamera.transform.position + this.playerCamera.transform.forward * this.maxDistanceFromPlayer;
				}
			}
		}
		else if (this.overrideGrabDistanceTimer <= 0f)
		{
			this.pullerDistance = Vector3.Distance(this.physGrabPointPuller.position, this.playerAvatar.localCameraPosition);
		}
		this.grabberAudioTransform.position = this.physGrabBeamComponent.PhysGrabPointOrigin.position;
		this.loopSound.PlayLoop(this.physGrabBeam.gameObject.activeSelf, 10f, 10f, 1f);
		if (!this.isLocal)
		{
			return;
		}
		this.ShowValue();
		bool flag2 = SemiFunc.InputHold(InputKey.Grab) || this.toggleGrab;
		if (this.debugStickyGrabber && !SemiFunc.InputHold(InputKey.Rotate))
		{
			flag2 = true;
		}
		if (InputManager.instance.InputToggleGet(InputKey.Grab))
		{
			if (SemiFunc.InputDown(InputKey.Grab))
			{
				this.toggleGrab = !this.toggleGrab;
				if (this.toggleGrab)
				{
					this.toggleGrabTimer = 0.1f;
				}
			}
		}
		else
		{
			this.toggleGrab = false;
		}
		if (this.toggleGrabTimer > 0f)
		{
			this.toggleGrabTimer -= Time.deltaTime;
		}
		else if (!this.grabbed && this.toggleGrab)
		{
			this.toggleGrab = false;
		}
		if (this.overrideGrab && (SemiFunc.InputHold(InputKey.Grab) || this.toggleGrab))
		{
			this.overrideGrab = false;
			this.overrideGrabTarget = null;
		}
		if (this.overrideGrab)
		{
			flag2 = true;
		}
		if (this.overrideGrabRelease)
		{
			flag2 = false;
			this.overrideGrabRelease = false;
		}
		if (PlayerController.instance.InputDisableTimer > 0f)
		{
			flag2 = false;
		}
		bool flag3 = false;
		if (flag2 && !this.grabbed)
		{
			if (this.grabDisableTimer <= 0f)
			{
				flag3 = true;
			}
		}
		else if (!flag2 && this.grabbed)
		{
			this.ReleaseObject(0.1f);
		}
		if (LevelGenerator.Instance.Generated && PlayerController.instance.InputDisableTimer <= 0f)
		{
			if (this.grabCheckTimer <= 0f || flag3)
			{
				this.grabCheckTimer = 0.02f;
				this.RayCheck(flag3);
			}
			else
			{
				this.grabCheckTimer -= Time.deltaTime;
			}
		}
		this.PhysGrabLogic();
		if (this.grabDisableTimer > 0f)
		{
			this.grabDisableTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0007A4B8 File Offset: 0x000786B8
	private void PhysGrabLogic()
	{
		this.grabReleaseDistance = Mathf.Max(this.grabRange * 2f, 10f);
		if (this.grabbed)
		{
			if (this.physRotatingTimer > 0f)
			{
				Aim.instance.SetState(Aim.State.Rotate);
			}
			else
			{
				Aim.instance.SetState(Aim.State.Grab);
			}
			if (Vector3.Distance(this.physGrabPoint.position, this.playerCamera.transform.position) > this.grabReleaseDistance)
			{
				this.ReleaseObject(0.1f);
				return;
			}
			if (this.grabbedPhysGrabObject)
			{
				if (!this.grabbedPhysGrabObject.enabled || this.grabbedPhysGrabObject.dead || !this.grabbedPhysGrabObjectCollider || !this.grabbedPhysGrabObjectCollider.enabled)
				{
					this.ReleaseObject(0.1f);
					return;
				}
			}
			else
			{
				if (!this.grabbedStaticGrabObject)
				{
					this.ReleaseObject(0.1f);
					return;
				}
				if (!this.grabbedStaticGrabObject.isActiveAndEnabled || this.grabbedStaticGrabObject.dead)
				{
					this.ReleaseObject(0.1f);
					return;
				}
			}
			this.physGrabPointPullerPosition = this.physGrabPointPuller.position;
			this.PhysGrabStarted();
			this.PhysGrabBeamActivate();
		}
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0007A5F0 File Offset: 0x000787F0
	private void PhysGrabBeamActivate()
	{
		if (GameManager.instance.gameMode == 0)
		{
			if (!this.physGrabBeamActive)
			{
				this.physGrabForcesDisabled = false;
				this.physGrabBeam.SetActive(true);
				this.physGrabBeamComponent.physGrabPointPullerSmoothPosition = this.physGrabPoint.position;
				this.physGrabBeamActive = true;
				this.PhysGrabStartEffects();
				return;
			}
		}
		else if (!this.physGrabBeamActive)
		{
			this.photonView.RPC("PhysGrabBeamActivateRPC", RpcTarget.All, Array.Empty<object>());
			this.physGrabBeamActive = true;
		}
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0007A670 File Offset: 0x00078870
	public void ShowValue()
	{
		if (this.grabbed && this.grabbedPhysGrabObject)
		{
			ValuableObject component = this.grabbedPhysGrabObject.GetComponent<ValuableObject>();
			if (component)
			{
				WorldSpaceUIValue.instance.Show(this.grabbedPhysGrabObject, (int)component.dollarValueCurrent, false, Vector3.zero);
				return;
			}
			if (SemiFunc.RunIsShop())
			{
				ItemAttributes component2 = this.grabbedPhysGrabObject.GetComponent<ItemAttributes>();
				if (component2)
				{
					WorldSpaceUIValue.instance.Show(this.grabbedPhysGrabObject, component2.value, true, component2.costOffset);
				}
			}
		}
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x0007A6FC File Offset: 0x000788FC
	private void PhysGrabStartEffects()
	{
		this.startSound.Play(this.loopSound.Source.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
		}
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0007A76C File Offset: 0x0007896C
	private void PhysGrabEndEffects()
	{
		this.stopSound.Play(this.loopSound.Source.transform.position, 1f, 1f, 1f, 1f);
		if (!GameManager.Multiplayer() || this.photonView.IsMine)
		{
			GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0007A7DC File Offset: 0x000789DC
	[PunRPC]
	private void PhysGrabBeamActivateRPC()
	{
		this.PhysGrabStartEffects();
		this.initialPressTimer = 0.1f;
		this.physGrabForcesDisabled = false;
		this.physGrabBeam.SetActive(true);
		this.physGrabBeamComponent.physGrabPointPullerSmoothPosition = this.physGrabPoint.position;
		this.physGrabBeamActive = true;
		this.physGrabPointVisualRotate.GetComponent<PhysGrabPointRotate>().animationEval = 0f;
		this.PhysGrabPointActivate();
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0007A848 File Offset: 0x00078A48
	private void PhysGrabPointDeactivate()
	{
		this.physGrabPointVisualGrid.parent = this.physGrabPoint;
		this.physGrabPointVisualRotate.localScale = Vector3.zero;
		this.physGrabPointVisualRotate.GetComponent<PhysGrabPointRotate>().animationEval = 0f;
		this.GridObjectsRemove();
		this.physGrabPoint.gameObject.SetActive(false);
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0007A8A4 File Offset: 0x00078AA4
	private void PhysGrabPointActivate()
	{
		if (!this.grabbedObjectTransform)
		{
			return;
		}
		this.physGrabPointVisualRotate.localScale = Vector3.zero;
		PhysGrabPointRotate component = this.physGrabPointVisualRotate.GetComponent<PhysGrabPointRotate>();
		if (component)
		{
			component.animationEval = 0f;
			component.rotationActiveTimer = 0f;
		}
		this.physGrabPointVisualGrid.localPosition = Vector3.zero;
		this.physGrabPointVisualGrid.parent = null;
		this.physGrabPointVisualGrid.localScale = Vector3.one;
		this.grabbedPhysGrabObject = this.grabbedObjectTransform.GetComponent<PhysGrabObject>();
		if (this.grabbedPhysGrabObject)
		{
			this.physGrabPointVisualGrid.localRotation = this.grabbedPhysGrabObject.rb.rotation;
		}
		if (this.grabbedPhysGrabObject)
		{
			this.GridObjectsInstantiate();
		}
		this.physGrabPointVisualGrid.gameObject.SetActive(false);
		this.physGrabPoint.gameObject.SetActive(true);
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0007A993 File Offset: 0x00078B93
	[PunRPC]
	private void PhysGrabBeamDeactivateRPC()
	{
		this.physGrabForcesDisabled = false;
		this.ResetBeam();
		this.physGrabBeam.SetActive(false);
		this.PhysGrabPointDeactivate();
		this.physGrabBeamActive = false;
		this.PhysGrabEndEffects();
		this.physRotation = Quaternion.identity;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x0007A9CC File Offset: 0x00078BCC
	private void PhysGrabBeamDeactivate()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.PhysGrabBeamDeactivateRPC();
			return;
		}
		this.photonView.RPC("PhysGrabBeamDeactivateRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0007A9F4 File Offset: 0x00078BF4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.physGrabPointPullerPosition);
			stream.SendNext(this.physGrabPointPlane.position);
			stream.SendNext(this.mouseTurningVelocity);
			stream.SendNext(this.isRotating);
			stream.SendNext(this.colorState);
			return;
		}
		this.physGrabPointPullerPosition = (Vector3)stream.ReceiveNext();
		this.physGrabPointPuller.position = this.physGrabPointPullerPosition;
		this.physGrabPointPlane.position = (Vector3)stream.ReceiveNext();
		this.mouseTurningVelocity = (Vector3)stream.ReceiveNext();
		this.isRotating = (bool)stream.ReceiveNext();
		this.colorState = (int)stream.ReceiveNext();
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0007AAD0 File Offset: 0x00078CD0
	private void PhysGrabStarted()
	{
		if (this.grabbedPhysGrabObject)
		{
			this.grabbedPhysGrabObject.GrabStarted(this);
			return;
		}
		if (this.grabbedStaticGrabObject)
		{
			this.grabbedStaticGrabObject.GrabStarted(this);
			return;
		}
		this.ReleaseObject(0.1f);
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x0007AB1C File Offset: 0x00078D1C
	private void PhysGrabEnded()
	{
		if (this.grabbedPhysGrabObject)
		{
			this.grabbedPhysGrabObject.GrabEnded(this);
			return;
		}
		if (this.grabbedStaticGrabObject)
		{
			this.grabbedStaticGrabObject.GrabEnded(this);
		}
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x0007AB54 File Offset: 0x00078D54
	private void RayCheck(bool _grab)
	{
		if (this.playerAvatar.isDisabled || this.playerAvatar.isTumbling || this.playerAvatar.deadSet)
		{
			return;
		}
		float maxDistance = 10f;
		if (_grab)
		{
			this.grabDisableTimer = 0.1f;
		}
		Vector3 direction = this.playerCamera.transform.forward;
		if (this.overrideGrab && this.overrideGrabTarget)
		{
			direction = (this.overrideGrabTarget.transform.position - this.playerCamera.transform.position).normalized;
		}
		if (!_grab)
		{
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.playerCamera.transform.position, 1f, direction, maxDistance, this.mask, QueryTriggerInteraction.Collide))
			{
				ValuableObject component = raycastHit.transform.GetComponent<ValuableObject>();
				if (component)
				{
					if (!component.discovered)
					{
						Vector3 direction2 = this.playerCamera.transform.position - raycastHit.point;
						RaycastHit[] array2 = Physics.SphereCastAll(raycastHit.point, 0.01f, direction2, direction2.magnitude, this.mask, QueryTriggerInteraction.Collide);
						bool flag = true;
						foreach (RaycastHit raycastHit2 in array2)
						{
							if (!raycastHit2.transform.CompareTag("Player") && raycastHit2.transform != raycastHit.transform)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							component.Discover(ValuableDiscoverGraphic.State.Discover);
						}
					}
					else if (component.discoveredReminder)
					{
						Vector3 direction3 = this.playerCamera.transform.position - raycastHit.point;
						RaycastHit[] array4 = Physics.RaycastAll(raycastHit.point, direction3, direction3.magnitude, this.mask, QueryTriggerInteraction.Collide);
						bool flag2 = true;
						foreach (RaycastHit raycastHit3 in array4)
						{
							if (raycastHit3.collider.transform.CompareTag("Wall"))
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							component.discoveredReminder = false;
							component.Discover(ValuableDiscoverGraphic.State.Reminder);
						}
					}
				}
			}
		}
		RaycastHit raycastHit4;
		if (Physics.Raycast(this.playerCamera.transform.position, direction, out raycastHit4, maxDistance, this.mask, QueryTriggerInteraction.Ignore))
		{
			bool flag3 = this.overrideGrab && !this.overrideGrabTarget;
			bool flag4 = this.overrideGrab && this.overrideGrabTarget && raycastHit4.transform.GetComponentInParent<PhysGrabObject>() == this.overrideGrabTarget;
			if (!this.overrideGrab)
			{
				flag4 = true;
			}
			if (raycastHit4.collider.CompareTag("Phys Grab Object") && flag4)
			{
				if (raycastHit4.distance > this.grabRange)
				{
					return;
				}
				if (_grab)
				{
					this.grabbedPhysGrabObject = raycastHit4.transform.GetComponent<PhysGrabObject>();
					if (this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.grabDisableTimer > 0f)
					{
						return;
					}
					if (this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.rb.IsSleeping())
					{
						this.grabbedPhysGrabObject.OverrideIndestructible(0.5f);
						this.grabbedPhysGrabObject.OverrideBreakEffects(0.5f);
					}
					this.grabbedObjectTransform = raycastHit4.transform;
					if (this.grabbedPhysGrabObject)
					{
						PhysGrabObjectCollider component2 = raycastHit4.collider.GetComponent<PhysGrabObjectCollider>();
						this.grabbedPhysGrabObjectCollider = raycastHit4.collider;
						this.grabbedPhysGrabObjectColliderID = component2.colliderID;
						this.grabbedStaticGrabObject = null;
					}
					else
					{
						this.grabbedPhysGrabObject = null;
						this.grabbedPhysGrabObjectCollider = null;
						this.grabbedPhysGrabObjectColliderID = 0;
						this.grabbedStaticGrabObject = this.grabbedObjectTransform.GetComponent<StaticGrabObject>();
						if (!this.grabbedStaticGrabObject)
						{
							foreach (StaticGrabObject staticGrabObject in this.grabbedObjectTransform.GetComponentsInParent<StaticGrabObject>())
							{
								if (staticGrabObject.colliderTransform == raycastHit4.collider.transform)
								{
									this.grabbedStaticGrabObject = staticGrabObject;
								}
							}
						}
						if (!this.grabbedStaticGrabObject || !this.grabbedStaticGrabObject.enabled)
						{
							return;
						}
					}
					this.PhysGrabPointActivate();
					this.physGrabPointPuller.gameObject.SetActive(true);
					this.grabbedObject = raycastHit4.rigidbody;
					Vector3 vector = raycastHit4.point;
					if (this.grabbedPhysGrabObject && this.grabbedPhysGrabObject.roomVolumeCheck.currentSize.magnitude < 0.5f)
					{
						vector = raycastHit4.collider.bounds.center;
					}
					float d = Vector3.Distance(this.playerCamera.transform.position, vector);
					Vector3 position = this.playerCamera.transform.position + this.playerCamera.transform.forward * d;
					this.physGrabPointPlane.position = position;
					this.physGrabPointPuller.position = position;
					if (this.physRotatingTimer <= 0f)
					{
						this.cameraRelativeGrabbedForward = Camera.main.transform.InverseTransformDirection(this.grabbedObjectTransform.forward);
						this.cameraRelativeGrabbedUp = Camera.main.transform.InverseTransformDirection(this.grabbedObjectTransform.up);
						this.cameraRelativeGrabbedRight = Camera.main.transform.InverseTransformDirection(this.grabbedObjectTransform.right);
					}
					if (GameManager.instance.gameMode == 0)
					{
						this.physGrabPoint.position = vector;
						if (!this.grabbedPhysGrabObject || !this.grabbedPhysGrabObject.forceGrabPoint)
						{
							this.localGrabPosition = this.grabbedObjectTransform.InverseTransformPoint(vector);
						}
						else
						{
							vector = this.grabbedPhysGrabObject.forceGrabPoint.position;
							d = 1f;
							position = this.playerCamera.transform.position + this.playerCamera.transform.forward * d - this.playerCamera.transform.up * 0.3f;
							this.physGrabPoint.position = vector;
							this.physGrabPointPlane.position = position;
							this.physGrabPointPuller.position = position;
							this.localGrabPosition = this.grabbedObjectTransform.InverseTransformPoint(vector);
						}
					}
					else if (this.grabbedPhysGrabObject)
					{
						if (this.grabbedPhysGrabObject.forceGrabPoint)
						{
							vector = this.grabbedPhysGrabObject.forceGrabPoint.position;
							Quaternion rotation = Quaternion.Euler(45f, 0f, 0f);
							this.cameraRelativeGrabbedForward = rotation * Vector3.forward;
							this.cameraRelativeGrabbedUp = rotation * Vector3.up;
							this.cameraRelativeGrabbedRight = rotation * Vector3.right;
							d = 1f;
							position = this.playerCamera.transform.position + this.playerCamera.transform.forward * d - this.playerCamera.transform.up * 0.3f;
							if (!this.overrideGrabPointTransform)
							{
								this.physGrabPoint.position = vector;
							}
							else
							{
								this.physGrabPoint.position = this.overrideGrabPointTransform.position;
							}
							this.physGrabPointPlane.position = position;
							this.physGrabPointPuller.position = position;
						}
						this.grabbedPhysGrabObject.GrabLink(this.photonView.ViewID, this.grabbedPhysGrabObjectColliderID, vector, this.cameraRelativeGrabbedForward, this.cameraRelativeGrabbedUp);
					}
					else if (this.grabbedStaticGrabObject)
					{
						this.grabbedStaticGrabObject.GrabLink(this.photonView.ViewID, vector);
					}
					if (this.isLocal)
					{
						PlayerController.instance.physGrabObject = this.grabbedObjectTransform.gameObject;
						PlayerController.instance.physGrabActive = true;
					}
					this.initialPressTimer = 0.1f;
					this.prevGrabbed = this.grabbed;
					this.grabbed = true;
				}
				if (!this.grabbed)
				{
					bool flag5 = false;
					PhysGrabObject exists = raycastHit4.transform.GetComponent<PhysGrabObject>();
					if (!exists)
					{
						exists = raycastHit4.transform.GetComponentInParent<PhysGrabObject>();
					}
					if (exists)
					{
						this.currentlyLookingAtPhysGrabObject = exists;
						flag5 = true;
					}
					StaticGrabObject staticGrabObject2 = raycastHit4.transform.GetComponent<StaticGrabObject>();
					if (!staticGrabObject2)
					{
						staticGrabObject2 = raycastHit4.transform.GetComponentInParent<StaticGrabObject>();
					}
					if (staticGrabObject2 && staticGrabObject2.enabled)
					{
						this.currentlyLookingAtStaticGrabObject = staticGrabObject2;
						flag5 = true;
					}
					ItemAttributes component3 = raycastHit4.transform.GetComponent<ItemAttributes>();
					if (component3)
					{
						this.currentlyLookingAtItemAttributes = component3;
						component3.ShowInfo();
					}
					if (flag5)
					{
						Aim.instance.SetState(Aim.State.Grabbable);
					}
				}
			}
		}
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x0007B468 File Offset: 0x00079668
	public void ReleaseObject(float _disableTimer = 0.1f)
	{
		if (!this.grabbed)
		{
			return;
		}
		this.overrideGrab = false;
		this.overrideGrabTarget = null;
		if (!this.physGrabPoint)
		{
			return;
		}
		this.PhysGrabEnded();
		this.physGrabPoint.SetParent(null, true);
		this.grabbedObject = null;
		this.grabbedObjectTransform = null;
		this.prevGrabbed = this.grabbed;
		this.grabbed = false;
		if (this.isLocal)
		{
			PlayerController.instance.physGrabObject = null;
			PlayerController.instance.physGrabActive = false;
		}
		if (this.physGrabPoint)
		{
			this.PhysGrabPointDeactivate();
		}
		if (this.physGrabPointPuller)
		{
			this.physGrabPointPuller.gameObject.SetActive(false);
		}
		this.PhysGrabBeamDeactivate();
		this.grabDisableTimer = 0.1f;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x0007B52E File Offset: 0x0007972E
	[PunRPC]
	public void ReleaseObjectRPC(bool physGrabEnded, float _disableTimer = 0.1f)
	{
		if (this.isLocal)
		{
			if (!physGrabEnded)
			{
				this.grabbedStaticGrabObject = null;
			}
			this.ReleaseObject(0.1f);
			this.grabDisableTimer = _disableTimer;
		}
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x0007B554 File Offset: 0x00079754
	private void GridObjectsInstantiate()
	{
		PhysGrabObject physGrabObject = this.grabbedPhysGrabObject;
		if (physGrabObject.GetComponent<PhysGrabObjectImpactDetector>().isCart)
		{
			return;
		}
		Quaternion rotation = this.grabbedPhysGrabObject.rb.rotation;
		this.grabbedPhysGrabObject.rb.rotation = Quaternion.identity;
		foreach (Collider collider in physGrabObject.GetComponentsInChildren<Collider>())
		{
			if (!collider.isTrigger && collider.gameObject.activeSelf && !(collider is MeshCollider))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.physGrabPointVisualGridObject);
				gameObject.SetActive(true);
				this.SetGridObjectScale(gameObject.transform, collider);
				Quaternion rotation2 = this.grabbedObjectTransform.rotation;
				this.physGrabPointVisualGrid.rotation = Quaternion.identity;
				this.grabbedObjectTransform.rotation = Quaternion.identity;
				this.physGrabPointVisualGrid.localRotation = Quaternion.identity;
				Vector3 position = this.grabbedPhysGrabObject.transform.position;
				Quaternion localRotation = this.grabbedPhysGrabObject.transform.localRotation;
				this.physGrabPointVisualGrid.position = this.grabbedPhysGrabObject.transform.TransformPoint(this.grabbedPhysGrabObject.midPointOffset);
				this.grabbedPhysGrabObject.transform.position = Vector3.zero;
				gameObject.transform.position = collider.bounds.center;
				gameObject.transform.rotation = collider.transform.rotation;
				gameObject.transform.SetParent(this.physGrabPointVisualGrid);
				this.physGrabPointVisualGridObjects.Add(gameObject);
				this.grabbedObjectTransform.rotation = rotation2;
				this.grabbedPhysGrabObject.transform.position = position;
			}
		}
		this.grabbedPhysGrabObject.rb.rotation = rotation;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x0007B728 File Offset: 0x00079928
	private void SetGridObjectScale(Transform _itemEquipCubeTransform, Collider _collider)
	{
		Quaternion rotation = _collider.transform.rotation;
		_collider.transform.rotation = Quaternion.identity;
		BoxCollider boxCollider = _collider as BoxCollider;
		if (boxCollider != null)
		{
			_itemEquipCubeTransform.localScale = Vector3.Scale(boxCollider.size, _collider.transform.lossyScale);
		}
		else
		{
			SphereCollider sphereCollider = _collider as SphereCollider;
			if (sphereCollider != null)
			{
				float num = sphereCollider.radius * Mathf.Max(new float[]
				{
					_collider.transform.lossyScale.x,
					_collider.transform.lossyScale.y,
					_collider.transform.lossyScale.z
				}) * 2f;
				_itemEquipCubeTransform.localScale = new Vector3(num, num, num);
			}
			else
			{
				CapsuleCollider capsuleCollider = _collider as CapsuleCollider;
				if (capsuleCollider != null)
				{
					float num2 = capsuleCollider.radius * Mathf.Max(_collider.transform.lossyScale.x, _collider.transform.lossyScale.z) * 2f;
					float y = capsuleCollider.height * _collider.transform.lossyScale.y;
					_itemEquipCubeTransform.localScale = new Vector3(num2, y, num2);
				}
				else
				{
					_itemEquipCubeTransform.localScale = _collider.bounds.size;
				}
			}
		}
		_collider.transform.rotation = rotation;
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x0007B87C File Offset: 0x00079A7C
	private void GridObjectsRemove()
	{
		foreach (GameObject obj in this.physGrabPointVisualGridObjects)
		{
			Object.Destroy(obj);
		}
		this.physGrabPointVisualGridObjects.Clear();
	}

	// Token: 0x04001616 RID: 5654
	private Camera playerCamera;

	// Token: 0x04001617 RID: 5655
	[HideInInspector]
	public float grabRange = 4f;

	// Token: 0x04001618 RID: 5656
	[HideInInspector]
	public float grabReleaseDistance = 8f;

	// Token: 0x04001619 RID: 5657
	public static PhysGrabber instance;

	// Token: 0x0400161A RID: 5658
	[Space]
	[HideInInspector]
	public float minDistanceFromPlayer = 1f;

	// Token: 0x0400161B RID: 5659
	[HideInInspector]
	public float maxDistanceFromPlayer = 2.5f;

	// Token: 0x0400161C RID: 5660
	[Space]
	public PhysGrabBeam physGrabBeamComponent;

	// Token: 0x0400161D RID: 5661
	public GameObject physGrabBeam;

	// Token: 0x0400161E RID: 5662
	public Transform physGrabPoint;

	// Token: 0x0400161F RID: 5663
	public Transform physGrabPointPuller;

	// Token: 0x04001620 RID: 5664
	public Transform physGrabPointPlane;

	// Token: 0x04001621 RID: 5665
	private GameObject physGrabPointVisual1;

	// Token: 0x04001622 RID: 5666
	private GameObject physGrabPointVisual2;

	// Token: 0x04001623 RID: 5667
	internal Vector3 grabbedcObjectPrevCamRelForward;

	// Token: 0x04001624 RID: 5668
	internal Vector3 grabbedObjectPrevCamRelUp;

	// Token: 0x04001625 RID: 5669
	internal PhysGrabObject grabbedPhysGrabObject;

	// Token: 0x04001626 RID: 5670
	internal int grabbedPhysGrabObjectColliderID;

	// Token: 0x04001627 RID: 5671
	internal Collider grabbedPhysGrabObjectCollider;

	// Token: 0x04001628 RID: 5672
	internal StaticGrabObject grabbedStaticGrabObject;

	// Token: 0x04001629 RID: 5673
	internal Rigidbody grabbedObject;

	// Token: 0x0400162A RID: 5674
	[HideInInspector]
	public Transform grabbedObjectTransform;

	// Token: 0x0400162B RID: 5675
	[HideInInspector]
	public float physGrabPointPullerDampen = 80f;

	// Token: 0x0400162C RID: 5676
	[HideInInspector]
	public float springConstant = 0.9f;

	// Token: 0x0400162D RID: 5677
	[HideInInspector]
	public float dampingConstant = 0.5f;

	// Token: 0x0400162E RID: 5678
	[HideInInspector]
	public float forceConstant = 4f;

	// Token: 0x0400162F RID: 5679
	[HideInInspector]
	public float forceMax = 4f;

	// Token: 0x04001630 RID: 5680
	private bool physGrabBeamActive;

	// Token: 0x04001631 RID: 5681
	[HideInInspector]
	public PhotonView photonView;

	// Token: 0x04001632 RID: 5682
	[HideInInspector]
	public bool isLocal;

	// Token: 0x04001633 RID: 5683
	[HideInInspector]
	public bool grabbed;

	// Token: 0x04001634 RID: 5684
	internal float grabDisableTimer;

	// Token: 0x04001635 RID: 5685
	[HideInInspector]
	public Vector3 physGrabPointPosition;

	// Token: 0x04001636 RID: 5686
	[HideInInspector]
	public Vector3 physGrabPointPullerPosition;

	// Token: 0x04001637 RID: 5687
	[HideInInspector]
	public PlayerAvatar playerAvatar;

	// Token: 0x04001638 RID: 5688
	[HideInInspector]
	public Vector3 localGrabPosition;

	// Token: 0x04001639 RID: 5689
	[HideInInspector]
	public Vector3 cameraRelativeGrabbedForward;

	// Token: 0x0400163A RID: 5690
	[HideInInspector]
	public Vector3 cameraRelativeGrabbedUp;

	// Token: 0x0400163B RID: 5691
	[HideInInspector]
	public Vector3 cameraRelativeGrabbedRight;

	// Token: 0x0400163C RID: 5692
	private Transform physGrabPointVisualRotate;

	// Token: 0x0400163D RID: 5693
	[HideInInspector]
	public Transform physGrabPointVisualGrid;

	// Token: 0x0400163E RID: 5694
	[HideInInspector]
	public GameObject physGrabPointVisualGridObject;

	// Token: 0x0400163F RID: 5695
	private List<GameObject> physGrabPointVisualGridObjects = new List<GameObject>();

	// Token: 0x04001640 RID: 5696
	private int prevColorState = -1;

	// Token: 0x04001641 RID: 5697
	[HideInInspector]
	public int colorState;

	// Token: 0x04001642 RID: 5698
	private float colorStateOverrideTimer;

	// Token: 0x04001643 RID: 5699
	[Space]
	public LayerMask maskLayers;

	// Token: 0x04001644 RID: 5700
	internal bool healing;

	// Token: 0x04001645 RID: 5701
	internal ItemAttributes currentlyLookingAtItemAttributes;

	// Token: 0x04001646 RID: 5702
	internal PhysGrabObject currentlyLookingAtPhysGrabObject;

	// Token: 0x04001647 RID: 5703
	internal StaticGrabObject currentlyLookingAtStaticGrabObject;

	// Token: 0x04001648 RID: 5704
	[Space]
	public Material physGrabBeamMaterial;

	// Token: 0x04001649 RID: 5705
	public Material physGrabBeamMaterialBatteryCharge;

	// Token: 0x0400164A RID: 5706
	[HideInInspector]
	public bool physGrabForcesDisabled;

	// Token: 0x0400164B RID: 5707
	[HideInInspector]
	public float initialPressTimer;

	// Token: 0x0400164C RID: 5708
	private bool overrideGrab;

	// Token: 0x0400164D RID: 5709
	private bool overrideGrabRelease;

	// Token: 0x0400164E RID: 5710
	private PhysGrabObject overrideGrabTarget;

	// Token: 0x0400164F RID: 5711
	private float physGrabBeamAlpha = 1f;

	// Token: 0x04001650 RID: 5712
	private float physGrabBeamAlphaChangeTo = 1f;

	// Token: 0x04001651 RID: 5713
	private float physGramBeamAlphaTimer;

	// Token: 0x04001652 RID: 5714
	private float physGrabBeamAlphaChangeProgress;

	// Token: 0x04001653 RID: 5715
	private float physGrabBeamAlphaOriginal;

	// Token: 0x04001654 RID: 5716
	private float overrideGrabDistance;

	// Token: 0x04001655 RID: 5717
	private float overrideGrabDistanceTimer;

	// Token: 0x04001656 RID: 5718
	private float overrideDisableRotationControlsTimer;

	// Token: 0x04001657 RID: 5719
	private bool overrideDisableRotationControls;

	// Token: 0x04001658 RID: 5720
	private LayerMask mask;

	// Token: 0x04001659 RID: 5721
	private float grabCheckTimer;

	// Token: 0x0400165A RID: 5722
	internal float pullerDistance;

	// Token: 0x0400165B RID: 5723
	[Space]
	public Transform grabberAudioTransform;

	// Token: 0x0400165C RID: 5724
	public Sound startSound;

	// Token: 0x0400165D RID: 5725
	public Sound loopSound;

	// Token: 0x0400165E RID: 5726
	public Sound stopSound;

	// Token: 0x0400165F RID: 5727
	private float physRotatingTimer;

	// Token: 0x04001660 RID: 5728
	internal Quaternion physRotation;

	// Token: 0x04001661 RID: 5729
	private Quaternion physRotationBase;

	// Token: 0x04001662 RID: 5730
	[HideInInspector]
	public Vector3 mouseTurningVelocity;

	// Token: 0x04001663 RID: 5731
	[HideInInspector]
	public float grabStrength = 1f;

	// Token: 0x04001664 RID: 5732
	[HideInInspector]
	public float throwStrength;

	// Token: 0x04001665 RID: 5733
	internal bool debugStickyGrabber;

	// Token: 0x04001666 RID: 5734
	[HideInInspector]
	public float stopRotationTimer;

	// Token: 0x04001667 RID: 5735
	[HideInInspector]
	public Quaternion nextPhysRotation;

	// Token: 0x04001668 RID: 5736
	[HideInInspector]
	public bool isRotating;

	// Token: 0x04001669 RID: 5737
	private float isRotatingTimer;

	// Token: 0x0400166A RID: 5738
	internal bool isPushing;

	// Token: 0x0400166B RID: 5739
	internal bool isPulling;

	// Token: 0x0400166C RID: 5740
	private float isPushingTimer;

	// Token: 0x0400166D RID: 5741
	private float isPullingTimer;

	// Token: 0x0400166E RID: 5742
	private float prevPullerDistance;

	// Token: 0x0400166F RID: 5743
	private bool prevGrabbed;

	// Token: 0x04001670 RID: 5744
	private bool toggleGrab;

	// Token: 0x04001671 RID: 5745
	private float toggleGrabTimer;

	// Token: 0x04001672 RID: 5746
	private float overrideGrabPointTimer;

	// Token: 0x04001673 RID: 5747
	private Transform overrideGrabPointTransform;

	// Token: 0x02000367 RID: 871
	private enum ColorState
	{
		// Token: 0x0400276B RID: 10091
		Orange,
		// Token: 0x0400276C RID: 10092
		Green,
		// Token: 0x0400276D RID: 10093
		Purple
	}
}
