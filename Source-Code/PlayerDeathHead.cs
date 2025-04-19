using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class PlayerDeathHead : MonoBehaviour
{
	// Token: 0x06000EC6 RID: 3782 RVA: 0x00085CA4 File Offset: 0x00083EA4
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.smokeParticleRateOverTimeDefault = this.smokeParticles.emission.rateOverTime.constant;
		this.smokeParticleRateOverDistanceDefault = this.smokeParticles.emission.rateOverDistance.constant;
		this.localSeenEffectTimer = this.localSeenEffectTime;
		foreach (MeshRenderer meshRenderer in this.eyeRenderers)
		{
			if (!this.eyeMaterial)
			{
				this.eyeMaterial = meshRenderer.material;
			}
			meshRenderer.material = this.eyeMaterial;
		}
		this.eyeMaterialAmount = Shader.PropertyToID("_ColorOverlayAmount");
		this.eyeMaterialColor = Shader.PropertyToID("_ColorOverlay");
		this.eyeFlashCurve = AssetManager.instance.animationCurveImpact;
		this.smokeParticleTimer = this.smokeParticleTime;
		this.physGrabObject.impactDetector.destroyDisableTeleport = false;
		this.colliders = base.GetComponentsInChildren<Collider>();
		this.SetColliders(false);
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x00085DD1 File Offset: 0x00083FD1
	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (GameManager.Multiplayer())
			{
				this.photonView.RPC("SetupRPC", RpcTarget.OthersBuffered, new object[]
				{
					this.playerAvatar.playerName
				});
			}
			this.SetupDone();
			this.physGrabObject.Teleport(new Vector3(0f, 3000f, 0f), Quaternion.identity);
			if (SemiFunc.RunIsArena())
			{
				this.physGrabObject.impactDetector.destroyDisable = false;
			}
			this.setup = true;
		}
		yield break;
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x00085DE0 File Offset: 0x00083FE0
	private IEnumerator SetupClient()
	{
		while (!this.physGrabObject)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (!this.physGrabObject.impactDetector)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (!this.physGrabObject.impactDetector.particles)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.SetupDone();
		yield break;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x00085DF0 File Offset: 0x00083FF0
	private void SetupDone()
	{
		if (!this.playerAvatar)
		{
			Debug.LogError("PlayerDeathHead: PlayerAvatar not found", base.gameObject);
			return;
		}
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 1) && !this.playerAvatar.isLocal)
		{
			this.tutorialPossible = true;
		}
		base.transform.parent = this.playerAvatar.transform.parent;
		if (SemiFunc.IsMultiplayer() && this.playerAvatar == SessionManager.instance.CrownedPlayerGet())
		{
			this.arenaCrown.SetActive(true);
		}
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x00085E8C File Offset: 0x0008408C
	private void Update()
	{
		if (!this.serverSeen)
		{
			this.mapCustom.Hide();
		}
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && this.setup)
		{
			if (!this.triggered)
			{
				this.physGrabObject.OverrideDeactivate(0.1f);
			}
			else if (this.triggeredTimer > 0f)
			{
				this.physGrabObject.OverrideDeactivate(0.1f);
				this.triggeredTimer -= Time.deltaTime;
				if (this.triggeredTimer <= 0f)
				{
					this.physGrabObject.OverrideDeactivateReset();
					this.physGrabObject.rb.AddForce(this.playerAvatar.localCameraTransform.up * 2f, ForceMode.Impulse);
					this.physGrabObject.rb.AddForce(this.physGrabObject.transform.forward * 0.5f, ForceMode.Impulse);
					this.physGrabObject.rb.AddTorque(this.physGrabObject.transform.right * 0.2f, ForceMode.Impulse);
				}
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.triggered)
			{
				this.inExtractionPoint = this.roomVolumeCheck.inExtractionPoint;
				if (this.inExtractionPoint != this.inExtractionPointPrevious)
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("FlashEyeRPC", RpcTarget.All, new object[]
						{
							this.inExtractionPoint
						});
					}
					else
					{
						this.FlashEyeRPC(this.inExtractionPoint);
					}
					this.inExtractionPointPrevious = this.inExtractionPoint;
				}
			}
			else
			{
				this.inExtractionPoint = false;
				this.inExtractionPointPrevious = false;
			}
		}
		if (this.smokeParticles.isPlaying)
		{
			this.smokeParticleTimer -= Time.deltaTime;
			if (this.smokeParticleTimer <= 0f)
			{
				this.smokeParticleRateOverTimeCurrent -= 1f * Time.deltaTime;
				this.smokeParticleRateOverTimeCurrent = Mathf.Max(this.smokeParticleRateOverTimeCurrent, 0f);
				this.smokeParticleRateOverDistanceCurrent -= 10f * Time.deltaTime;
				this.smokeParticleRateOverDistanceCurrent = Mathf.Max(this.smokeParticleRateOverDistanceCurrent, 0f);
				ParticleSystem.EmissionModule emission = this.smokeParticles.emission;
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(this.smokeParticleRateOverTimeCurrent);
				emission.rateOverDistance = new ParticleSystem.MinMaxCurve(this.smokeParticleRateOverDistanceCurrent);
				if (this.smokeParticleRateOverTimeCurrent <= 0f && this.smokeParticleRateOverDistanceCurrent <= 0f)
				{
					this.smokeParticles.Stop();
				}
			}
		}
		if (this.eyeFlash)
		{
			this.eyeFlashLerp += 2f * Time.deltaTime;
			this.eyeFlashLerp = Mathf.Clamp01(this.eyeFlashLerp);
			this.eyeMaterial.SetFloat(this.eyeMaterialAmount, this.eyeFlashCurve.Evaluate(this.eyeFlashLerp));
			this.eyeFlashLight.intensity = this.eyeFlashCurve.Evaluate(this.eyeFlashLerp) * this.eyeFlashLightIntensity;
			if (this.eyeFlashLerp > 1f)
			{
				this.eyeFlash = false;
				this.eyeMaterial.SetFloat(this.eyeMaterialAmount, 0f);
				this.eyeFlashLight.gameObject.SetActive(false);
			}
		}
		if (this.triggered && !this.localSeen && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			if (this.seenCooldownTimer > 0f)
			{
				this.seenCooldownTimer -= Time.deltaTime;
			}
			else
			{
				Vector3 localCameraPosition = PlayerController.instance.playerAvatarScript.localCameraPosition;
				float num = Vector3.Distance(base.transform.position, localCameraPosition);
				if (num <= 10f && SemiFunc.OnScreen(base.transform.position, -0.15f, -0.15f))
				{
					Vector3 normalized = (localCameraPosition - base.transform.position).normalized;
					RaycastHit raycastHit;
					if (!Physics.Raycast(this.physGrabObject.centerPoint, normalized, out raycastHit, num, LayerMask.GetMask(new string[]
					{
						"Default"
					})))
					{
						this.localSeen = true;
						TutorialDirector.instance.playerSawHead = true;
						if (!this.serverSeen && SemiFunc.RunIsLevel())
						{
							if (SemiFunc.IsMultiplayer())
							{
								this.photonView.RPC("SeenSetRPC", RpcTarget.All, new object[]
								{
									true
								});
							}
							else
							{
								this.SeenSetRPC(true);
							}
							if (PlayerController.instance.deathSeenTimer <= 0f)
							{
								this.localSeenEffect = true;
								PlayerController.instance.deathSeenTimer = 30f;
								GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
								GameDirector.instance.CameraShake.Shake(2f, 1f);
								AudioScare.instance.PlayCustom(this.seenSound, 0.3f, 60f);
								ValuableDiscover.instance.New(this.physGrabObject, ValuableDiscoverGraphic.State.Bad);
							}
						}
					}
				}
			}
		}
		if (this.localSeenEffect)
		{
			this.localSeenEffectTimer -= Time.deltaTime;
			CameraZoom.Instance.OverrideZoomSet(75f, 0.1f, 0.25f, 0.25f, base.gameObject, 150);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.4f, 1f, 1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.SaturationOverride(-50f, 1f, 0.5f, 0.1f, base.gameObject);
			PostProcessing.Instance.ContrastOverride(5f, 1f, 0.5f, 0.1f, base.gameObject);
			GameDirector.instance.CameraImpact.Shake(10f * Time.deltaTime, 0.1f);
			GameDirector.instance.CameraShake.Shake(10f * Time.deltaTime, 1f);
			if (this.localSeenEffectTimer <= 0f)
			{
				this.localSeenEffect = false;
			}
		}
		if (this.triggered && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.roomVolumeCheck.CurrentRooms.Count <= 0)
			{
				this.outsideLevelTimer += Time.deltaTime;
				if (this.outsideLevelTimer >= 5f)
				{
					if (RoundDirector.instance.extractionPointActive)
					{
						this.physGrabObject.Teleport(RoundDirector.instance.extractionPointCurrent.safetySpawn.position, RoundDirector.instance.extractionPointCurrent.safetySpawn.rotation);
					}
					else
					{
						this.physGrabObject.Teleport(TruckSafetySpawnPoint.instance.transform.position, TruckSafetySpawnPoint.instance.transform.rotation);
					}
				}
			}
			else
			{
				this.outsideLevelTimer = 0f;
			}
		}
		if (this.tutorialPossible)
		{
			if (this.triggered && this.localSeen)
			{
				this.tutorialTimer -= Time.deltaTime;
				if (this.tutorialTimer <= 0f)
				{
					if (!RoundDirector.instance.allExtractionPointsCompleted && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 1))
					{
						TutorialDirector.instance.ActivateTip("Reviving", 0.5f, false);
					}
					this.tutorialPossible = false;
				}
			}
			else
			{
				this.tutorialTimer = 5f;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (RoundDirector.instance.allExtractionPointsCompleted && this.triggered && !this.playerAvatar.finalHeal)
			{
				this.inTruck = this.roomVolumeCheck.inTruck;
				if (this.inTruck != this.inTruckPrevious)
				{
					if (GameManager.Multiplayer())
					{
						this.photonView.RPC("FlashEyeRPC", RpcTarget.All, new object[]
						{
							this.inTruck
						});
					}
					else
					{
						this.FlashEyeRPC(this.inTruck);
					}
					this.inTruckPrevious = this.inTruck;
				}
			}
			else
			{
				this.inTruck = false;
				this.inTruckPrevious = false;
			}
			if (this.inTruck)
			{
				this.inTruckReviveTimer -= Time.deltaTime;
				if (this.inTruckReviveTimer <= 0f)
				{
					this.playerAvatar.Revive(true);
					return;
				}
			}
			else
			{
				this.inTruckReviveTimer = 2f;
			}
		}
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x000866BC File Offset: 0x000848BC
	private void UpdateColor()
	{
		if (!this.headRenderer)
		{
			return;
		}
		this.headRenderer.material = this.playerAvatar.playerHealth.bodyMaterial;
		this.headRenderer.material.SetFloat(Shader.PropertyToID("_ColorOverlayAmount"), 0f);
		Color color = this.playerAvatar.playerAvatarVisuals.color;
		this.physGrabObject.impactDetector.particles.gradient = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(color, 0f),
				new GradientColorKey(color, 1f)
			}
		};
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0008676E File Offset: 0x0008496E
	public void Revive()
	{
		if (this.triggered && this.inExtractionPoint)
		{
			this.playerAvatar.Revive(false);
		}
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0008678C File Offset: 0x0008498C
	public void Trigger()
	{
		this.seenCooldownTimer = this.seenCooldownTime;
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (this.playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = false;
			}
			else
			{
				this.playerAvatar.playerAvatarCollision.Collider.enabled = false;
			}
			Collider[] array = this.playerAvatar.tumble.colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			this.physGrabObject.Teleport(this.playerAvatar.playerAvatarCollision.deathHeadPosition, this.playerAvatar.localCameraTransform.rotation);
			this.triggeredTimer = 0.1f;
		}
		this.UpdateColor();
		this.triggered = true;
		this.SetColliders(true);
		if (this.smokeParticles)
		{
			this.smokeParticles.Play();
		}
		this.smokeParticleRateOverTimeCurrent = this.smokeParticleRateOverTimeDefault;
		this.smokeParticleRateOverDistanceCurrent = this.smokeParticleRateOverDistanceDefault;
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x00086890 File Offset: 0x00084A90
	public void Reset()
	{
		this.triggered = false;
		this.smokeParticleTimer = this.smokeParticleTime;
		this.localSeenEffectTimer = this.localSeenEffectTime;
		this.localSeen = false;
		this.localSeenEffect = false;
		this.SetColliders(false);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.Teleport(new Vector3(0f, 3000f, 0f), Quaternion.identity);
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("SeenSetRPC", RpcTarget.All, new object[]
				{
					false
				});
				return;
			}
			this.SeenSetRPC(false);
		}
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0008692C File Offset: 0x00084B2C
	private void SetColliders(bool _enabled)
	{
		foreach (Collider collider in this.colliders)
		{
			if (collider)
			{
				collider.enabled = _enabled;
			}
		}
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x00086964 File Offset: 0x00084B64
	[PunRPC]
	public void SetupRPC(string _playerName)
	{
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (playerAvatar.playerName == _playerName)
			{
				this.playerAvatar = playerAvatar;
				this.playerAvatar.playerDeathHead = this;
				break;
			}
		}
		base.StartCoroutine(this.SetupClient());
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x000869E4 File Offset: 0x00084BE4
	[PunRPC]
	public void FlashEyeRPC(bool _positive)
	{
		this.inExtractionPoint = _positive;
		if (_positive)
		{
			this.eyeMaterial.SetColor(this.eyeMaterialColor, this.eyeFlashPositiveColor);
			this.eyeFlashPositiveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.eyeFlashLight.color = this.eyeFlashPositiveColor;
		}
		else
		{
			this.eyeMaterial.SetColor(this.eyeMaterialColor, this.eyeFlashNegativeColor);
			this.eyeFlashNegativeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.eyeFlashLight.color = this.eyeFlashNegativeColor;
		}
		this.eyeFlash = true;
		this.eyeFlashLerp = 0f;
		this.eyeFlashLight.gameObject.SetActive(true);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 2f, 8f, base.transform.position, 0.25f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 2f, 8f, base.transform.position, 0.5f);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x00086B22 File Offset: 0x00084D22
	[PunRPC]
	public void SeenSetRPC(bool _toggle)
	{
		this.serverSeen = _toggle;
	}

	// Token: 0x040018AC RID: 6316
	public PlayerAvatar playerAvatar;

	// Token: 0x040018AD RID: 6317
	public MeshRenderer headRenderer;

	// Token: 0x040018AE RID: 6318
	public ParticleSystem smokeParticles;

	// Token: 0x040018AF RID: 6319
	public MapCustom mapCustom;

	// Token: 0x040018B0 RID: 6320
	public GameObject arenaCrown;

	// Token: 0x040018B1 RID: 6321
	private float smokeParticleTime = 3f;

	// Token: 0x040018B2 RID: 6322
	private float smokeParticleTimer;

	// Token: 0x040018B3 RID: 6323
	private float smokeParticleRateOverTimeDefault;

	// Token: 0x040018B4 RID: 6324
	private float smokeParticleRateOverTimeCurrent;

	// Token: 0x040018B5 RID: 6325
	private float smokeParticleRateOverDistanceDefault;

	// Token: 0x040018B6 RID: 6326
	private float smokeParticleRateOverDistanceCurrent;

	// Token: 0x040018B7 RID: 6327
	internal PhysGrabObject physGrabObject;

	// Token: 0x040018B8 RID: 6328
	private PhotonView photonView;

	// Token: 0x040018B9 RID: 6329
	private RoomVolumeCheck roomVolumeCheck;

	// Token: 0x040018BA RID: 6330
	private bool setup;

	// Token: 0x040018BB RID: 6331
	private bool triggered;

	// Token: 0x040018BC RID: 6332
	private float triggeredTimer;

	// Token: 0x040018BD RID: 6333
	internal bool inExtractionPoint;

	// Token: 0x040018BE RID: 6334
	private bool inExtractionPointPrevious;

	// Token: 0x040018BF RID: 6335
	internal bool inTruck;

	// Token: 0x040018C0 RID: 6336
	private bool inTruckPrevious;

	// Token: 0x040018C1 RID: 6337
	[Space]
	public MeshRenderer[] eyeRenderers;

	// Token: 0x040018C2 RID: 6338
	public Light eyeFlashLight;

	// Token: 0x040018C3 RID: 6339
	public Color eyeFlashPositiveColor;

	// Token: 0x040018C4 RID: 6340
	public Color eyeFlashNegativeColor;

	// Token: 0x040018C5 RID: 6341
	public float eyeFlashStrength;

	// Token: 0x040018C6 RID: 6342
	public float eyeFlashLightIntensity;

	// Token: 0x040018C7 RID: 6343
	public Sound eyeFlashPositiveSound;

	// Token: 0x040018C8 RID: 6344
	public Sound eyeFlashNegativeSound;

	// Token: 0x040018C9 RID: 6345
	private Material eyeMaterial;

	// Token: 0x040018CA RID: 6346
	private int eyeMaterialAmount;

	// Token: 0x040018CB RID: 6347
	private int eyeMaterialColor;

	// Token: 0x040018CC RID: 6348
	private AnimationCurve eyeFlashCurve;

	// Token: 0x040018CD RID: 6349
	private float eyeFlashLerp;

	// Token: 0x040018CE RID: 6350
	private bool eyeFlash;

	// Token: 0x040018CF RID: 6351
	public AudioClip seenSound;

	// Token: 0x040018D0 RID: 6352
	private bool serverSeen;

	// Token: 0x040018D1 RID: 6353
	private float seenCooldownTime = 2f;

	// Token: 0x040018D2 RID: 6354
	private float seenCooldownTimer;

	// Token: 0x040018D3 RID: 6355
	private bool localSeen;

	// Token: 0x040018D4 RID: 6356
	private bool localSeenEffect;

	// Token: 0x040018D5 RID: 6357
	private float localSeenEffectTime = 2f;

	// Token: 0x040018D6 RID: 6358
	private float localSeenEffectTimer;

	// Token: 0x040018D7 RID: 6359
	private float outsideLevelTimer;

	// Token: 0x040018D8 RID: 6360
	private bool tutorialPossible;

	// Token: 0x040018D9 RID: 6361
	private float tutorialTimer;

	// Token: 0x040018DA RID: 6362
	private float inTruckReviveTimer;

	// Token: 0x040018DB RID: 6363
	private Collider[] colliders;
}
