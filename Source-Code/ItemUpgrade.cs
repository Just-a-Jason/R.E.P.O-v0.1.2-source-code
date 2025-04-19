using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000175 RID: 373
public class ItemUpgrade : MonoBehaviour
{
	// Token: 0x06000C7F RID: 3199 RVA: 0x0006DC18 File Offset: 0x0006BE18
	private void Start()
	{
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.customTargetingCondition = base.GetComponent<ITargetingCondition>();
		this.particleEffects = base.transform.Find("Particle Effects");
		this.cameraMain = Camera.main;
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.rb = base.GetComponent<Rigidbody>();
		this.photonView = base.GetComponent<PhotonView>();
		this.lineBetweenTwoPoints = base.GetComponent<LineBetweenTwoPoints>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.beamColor = this.colorPreset.GetColorDark();
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		int num = 0;
		if (num < componentsInChildren.Length)
		{
			Collider collider = componentsInChildren[num];
			this.physicMaterialOriginal = collider.material;
		}
		if (SemiFunc.RunIsShop())
		{
			this.itemToggle.enabled = false;
		}
		this.physGrabObject.clientNonKinematic = true;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0006DCFC File Offset: 0x0006BEFC
	private void Update()
	{
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			bool flag = false;
			using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isRotating)
					{
						flag = true;
					}
				}
			}
			float dist = 0.5f;
			if (this.physGrabObject.grabbed)
			{
				if (this.physGrabObject.grabbedLocal && !this.pushedOrPulled)
				{
					PhysGrabber.instance.OverrideGrabDistance(dist);
				}
				if (PhysGrabber.instance.isPulling || PhysGrabber.instance.isPushing)
				{
					this.pushedOrPulled = true;
				}
			}
			else
			{
				this.pushedOrPulled = false;
			}
			if (!flag && !this.pushedOrPulled)
			{
				Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
				Quaternion turnY = Quaternion.Euler(45f, 180f, 0f);
				Quaternion identity = Quaternion.identity;
				this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
		else
		{
			this.pushedOrPulled = false;
		}
		this.TargetingLogic();
		this.PlayerUpgradeLogic();
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0006DE2C File Offset: 0x0006C02C
	private void PlayerUpgrade()
	{
		if (this.upgradeDone)
		{
			return;
		}
		this.upgradeEvent.Invoke();
		this.particleEffects.parent = null;
		this.particleEffects.gameObject.SetActive(true);
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(this.itemToggle.playerTogglePhotonID);
		if (playerAvatar.isLocal)
		{
			StatsUI.instance.Fetch();
			StatsUI.instance.ShowStats();
			CameraGlitch.Instance.PlayUpgrade();
		}
		else
		{
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, base.transform.position, 0.2f);
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			playerAvatar.playerHealth.MaterialEffectOverride(PlayerHealth.Effect.Upgrade);
		}
		StatsManager.instance.itemsPurchased[this.itemAttributes.item.itemAssetName] = Mathf.Max(StatsManager.instance.itemsPurchased[this.itemAttributes.item.itemAssetName] - 1, 0);
		this.impactDetector.DestroyObject(false);
		this.upgradeDone = true;
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0006DF44 File Offset: 0x0006C144
	private void PlayerUpgradeLogic()
	{
		if (!this.isPlayerUpgrade)
		{
			return;
		}
		if (this.itemToggle.toggleState)
		{
			this.PlayerUpgrade();
		}
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0006DF64 File Offset: 0x0006C164
	private void TargetingLogic()
	{
		if (this.isPlayerUpgrade)
		{
			return;
		}
		if (this.magnetActive && !this.physGrabObject.grabbed)
		{
			this.DeactivateMagnet();
		}
		if (this.itemActivated && this.magnetActive)
		{
			if (this.magnetTarget && !this.magnetTarget.gameObject.activeSelf)
			{
				this.magnetActive = false;
				this.magnetTarget = null;
			}
			if (this.magnetTarget)
			{
				if (this.rayHitPosition != Vector3.zero && !this.targetIsPlayer)
				{
					this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
					this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.TransformPoint(this.animatedRayHitPosition));
					this.connectionPoint = this.magnetTarget.TransformPoint(this.animatedRayHitPosition);
				}
				else
				{
					this.animatedRayHitPosition = Vector3.Lerp(this.animatedRayHitPosition, this.rayHitPosition, Time.deltaTime * 10f);
					if (!this.targetIsPlayer)
					{
						this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTargetPhysGrabObject.midPoint);
						this.connectionPoint = this.magnetTargetPhysGrabObject.midPoint;
					}
					if (this.targetIsPlayer)
					{
						Vector3 b = new Vector3(0f, -0.5f, 0f);
						if (!this.targetIsLocalPlayer)
						{
							this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, this.magnetTarget.position + b);
							this.connectionPoint = this.magnetTarget.position + b;
						}
						else
						{
							Vector3 point = this.cameraMain.transform.position + b;
							this.lineBetweenTwoPoints.DrawLine(this.lineStartPoint.position, point);
							this.connectionPoint = point;
						}
					}
				}
			}
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.itemToggle.toggleState != this.itemActivated)
		{
			this.ButtonToggle();
		}
		if (this.physGrabObject.playerGrabbing.Count == 1)
		{
			this.lastPlayerToTouch = this.physGrabObject.playerGrabbing[0].transform;
		}
		if (!this.itemActivated)
		{
			return;
		}
		this.springConstant = 40f;
		this.dampingCoefficient = 10f;
		if (this.physGrabObject.grabbed)
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 0.5f)
			{
				if (this.SphereCheck())
				{
					this.ActivateMagnet();
				}
				this.checkTimer = 0f;
				return;
			}
		}
		else if (!this.attachPointFound)
		{
			if (!this.targetIsPlayer)
			{
				if (this.rayTimer <= 0f)
				{
					this.FindBeamAttachPosition();
					this.rayTimer = 0.5f;
					return;
				}
				this.rayTimer -= Time.deltaTime;
				return;
			}
		}
		else if (this.rb.velocity.magnitude > 0.2f)
		{
			this.newAttachPointTimer += Time.deltaTime;
			if (this.newAttachPointTimer > 0.5f)
			{
				this.attachPointFound = false;
				this.newAttachPointTimer = 0f;
				this.rayTimer = 0f;
			}
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0006E2C4 File Offset: 0x0006C4C4
	private void MagnetLogic()
	{
		if (this.isPlayerUpgrade)
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.itemActivated)
		{
			return;
		}
		if (this.magnetActive)
		{
			if (!this.magnetTarget)
			{
				this.DeactivateMagnet();
				return;
			}
			if (this.attachPointFound)
			{
				Vector3 a = this.magnetTarget.TransformPoint(this.attachPoint) - base.transform.position;
				Vector3 a2 = this.springConstant * a;
				Vector3 velocity = this.rb.velocity;
				Vector3 b = -this.dampingCoefficient * velocity;
				Vector3.ClampMagnitude(a2 + b, 20f);
			}
			else
			{
				float magnitude = (this.magnetTarget.position - base.transform.position).magnitude;
			}
			if (this.targetIsPlayer)
			{
				if (Vector3.Distance(base.transform.position, this.magnetTarget.position) > 1.8f)
				{
					this.DeactivateMagnet();
				}
			}
			else if (Vector3.Distance(base.transform.position, this.magnetTarget.position) > 1f)
			{
				this.DeactivateMagnet();
			}
			if (this.magnetTargetPhysGrabObject)
			{
				this.magnetTargetPhysGrabObject.OverrideZeroGravity(0.1f);
				this.magnetTargetPhysGrabObject.OverrideMass(0.1f, 0.1f);
				this.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky(), 0.1f);
				this.magnetTargetRigidbody.AddForce((base.transform.position - this.magnetTarget.position).normalized * 1f, ForceMode.Force);
			}
		}
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0006E479 File Offset: 0x0006C679
	private void FixedUpdate()
	{
		this.MagnetLogic();
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0006E481 File Offset: 0x0006C681
	private void DeactivateMagnet()
	{
		this.attachPointFound = false;
		this.MagnetActiveToggle(false);
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0006E491 File Offset: 0x0006C691
	private void ActivateMagnet()
	{
		this.MagnetActiveToggle(true);
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0006E49A File Offset: 0x0006C69A
	private void ButtonToggleLogic(bool activated)
	{
		if (!activated && this.magnetActive)
		{
			this.DeactivateMagnet();
		}
		this.itemActivated = activated;
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0006E4B4 File Offset: 0x0006C6B4
	public void ButtonToggle()
	{
		this.itemActivated = !this.itemActivated;
		if (GameManager.instance.gameMode == 0)
		{
			this.ButtonToggleLogic(this.itemActivated);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ButtonToggleRPC", RpcTarget.All, new object[]
			{
				this.itemActivated
			});
		}
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0006E515 File Offset: 0x0006C715
	[PunRPC]
	private void ButtonToggleRPC(bool activated)
	{
		this.ButtonToggleLogic(activated);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0006E520 File Offset: 0x0006C720
	private Transform GetHighestParentWithRigidbody(Transform child)
	{
		if (base.GetComponent<Rigidbody>() != null && child.GetComponent<PhotonView>() != null)
		{
			return child;
		}
		Transform transform = child;
		while (transform.parent != null)
		{
			if (transform.parent.GetComponent<Rigidbody>() != null && transform.parent.GetComponent<PhotonView>() != null)
			{
				return transform.parent;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0006E592 File Offset: 0x0006C792
	private void MagnetActiveToggleLogic(bool activated)
	{
		this.magnetActive = activated;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0006E59B File Offset: 0x0006C79B
	public void MagnetActiveToggle(bool toggleBool)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.MagnetActiveToggleLogic(toggleBool);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("MagnetActiveToggleRPC", RpcTarget.All, new object[]
			{
				toggleBool
			});
		}
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0006E5D8 File Offset: 0x0006C7D8
	[PunRPC]
	private void MagnetActiveToggleRPC(bool activated)
	{
		this.MagnetActiveToggleLogic(activated);
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0006E5E4 File Offset: 0x0006C7E4
	private void NewRayHitPointLogic(Vector3 newRayHitPosition, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		this.targetIsPlayer = false;
		this.targetIsLocalPlayer = false;
		if (newMagnetTarget)
		{
			this.magnetTargetPhysGrabObject = newMagnetTarget.GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				this.magnetTarget = newMagnetTarget.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
			}
			else
			{
				this.magnetTarget = newMagnetTarget;
			}
			this.animatedRayHitPosition = this.rayHitPosition;
			this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
			this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
			return;
		}
		this.magnetTargetPhysGrabObject = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>();
		if (colliderID != -1)
		{
			this.magnetTarget = PhotonView.Find(photonViewId).gameObject.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
		}
		else
		{
			this.targetIsPlayer = true;
			this.magnetTarget = PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>().PlayerVisionTarget.VisionTransform;
			if (PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>().isLocal)
			{
				this.targetIsLocalPlayer = true;
			}
			this.playerAvatarTarget = PhotonView.Find(photonViewId).GetComponent<PlayerAvatar>();
		}
		this.animatedRayHitPosition = this.rayHitPosition;
		this.rayHitPosition = this.magnetTarget.InverseTransformPoint(newRayHitPosition);
		this.magnetTargetRigidbody = this.GetHighestParentWithRigidbody(this.magnetTarget).GetComponent<Rigidbody>();
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0006E728 File Offset: 0x0006C928
	private void NewRayHitPoint(Vector3 newAttachPoint, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, newMagnetTarget);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("NewRayHitPointRPC", RpcTarget.All, new object[]
			{
				newAttachPoint,
				photonViewId,
				colliderID
			});
		}
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0006E786 File Offset: 0x0006C986
	[PunRPC]
	private void NewRayHitPointRPC(Vector3 newAttachPoint, int photonViewId, int colliderID)
	{
		this.NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, null);
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0006E794 File Offset: 0x0006C994
	private bool SphereCheck()
	{
		bool result = false;
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.75f);
		float num = 10000f;
		foreach (Collider collider in array)
		{
			Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(collider.transform);
			PhysGrabObjectCollider component = collider.GetComponent<PhysGrabObjectCollider>();
			bool flag = false;
			if (highestParentWithRigidbody != null)
			{
				PhysGrabObjectImpactDetector component2 = highestParentWithRigidbody.GetComponent<PhysGrabObjectImpactDetector>();
				if (component2 != null && component2.isValuable)
				{
					flag = true;
				}
			}
			bool flag2 = true;
			if (this.customTargetingCondition != null && highestParentWithRigidbody != null)
			{
				flag2 = this.customTargetingCondition.CustomTargetingCondition(highestParentWithRigidbody.gameObject);
			}
			bool flag3 = false;
			if (!flag3)
			{
				flag3 = !flag;
			}
			if (component != null && highestParentWithRigidbody != base.transform && highestParentWithRigidbody != null && flag3 && flag2)
			{
				float num2 = Vector3.Distance(base.transform.position, collider.transform.position);
				if (num2 < num)
				{
					bool flag4 = false;
					RaycastHit raycastHit;
					if (Physics.Raycast(base.transform.position, collider.transform.position - base.transform.position, out raycastHit, 1f, SemiFunc.LayerMaskGetVisionObstruct()) && raycastHit.collider.transform != collider.transform && raycastHit.collider.transform != base.transform)
					{
						flag4 = true;
					}
					if (!flag4)
					{
						num = num2;
						this.magnetTarget = collider.transform;
						this.magnetTargetPhysGrabObject = highestParentWithRigidbody.GetComponent<PhysGrabObject>();
						this.magnetTargetRigidbody = highestParentWithRigidbody.GetComponent<Rigidbody>();
						Vector3 position = collider.transform.position;
						this.NewRayHitPoint(position, highestParentWithRigidbody.GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
						this.attachPoint = this.rayHitPosition;
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0006E988 File Offset: 0x0006CB88
	private void FindBeamAttachPosition()
	{
		for (int i = 0; i < 6; i++)
		{
			float num = 0.5f;
			Vector3 b = new Vector3(Random.Range(-num, num), Random.Range(-num, num), Random.Range(-num, num));
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, this.magnetTarget.position - base.transform.position + b, out raycastHit, 1f, SemiFunc.LayerMaskGetPhysGrabObject()))
			{
				Transform highestParentWithRigidbody = this.GetHighestParentWithRigidbody(raycastHit.collider.transform);
				PhysGrabObjectCollider component = raycastHit.collider.transform.GetComponent<PhysGrabObjectCollider>();
				if (component != null && highestParentWithRigidbody == this.magnetTargetPhysGrabObject.transform)
				{
					Vector3 normalized = (base.transform.position - raycastHit.point).normalized;
					this.NewRayHitPoint(raycastHit.point, highestParentWithRigidbody.GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
					Vector3 position = raycastHit.point + normalized * 0.5f;
					this.attachPoint = this.magnetTarget.InverseTransformPoint(position);
					this.attachPointFound = true;
				}
			}
		}
	}

	// Token: 0x040013C3 RID: 5059
	public UnityEvent upgradeEvent;

	// Token: 0x040013C4 RID: 5060
	public bool isPlayerUpgrade;

	// Token: 0x040013C5 RID: 5061
	public ColorPresets colorPreset;

	// Token: 0x040013C6 RID: 5062
	internal Color beamColor;

	// Token: 0x040013C7 RID: 5063
	private float checkTimer;

	// Token: 0x040013C8 RID: 5064
	private Transform magnetTarget;

	// Token: 0x040013C9 RID: 5065
	internal PhysGrabObject magnetTargetPhysGrabObject;

	// Token: 0x040013CA RID: 5066
	internal Rigidbody magnetTargetRigidbody;

	// Token: 0x040013CB RID: 5067
	internal bool magnetActive;

	// Token: 0x040013CC RID: 5068
	private Rigidbody rb;

	// Token: 0x040013CD RID: 5069
	private bool attachPointFound;

	// Token: 0x040013CE RID: 5070
	private Vector3 attachPoint;

	// Token: 0x040013CF RID: 5071
	private float springConstant = 50f;

	// Token: 0x040013D0 RID: 5072
	private float dampingCoefficient = 5f;

	// Token: 0x040013D1 RID: 5073
	private float newAttachPointTimer;

	// Token: 0x040013D2 RID: 5074
	internal bool itemActivated;

	// Token: 0x040013D3 RID: 5075
	private PhotonView photonView;

	// Token: 0x040013D4 RID: 5076
	private Vector3 rayHitPosition;

	// Token: 0x040013D5 RID: 5077
	private Vector3 animatedRayHitPosition;

	// Token: 0x040013D6 RID: 5078
	private LineBetweenTwoPoints lineBetweenTwoPoints;

	// Token: 0x040013D7 RID: 5079
	public Transform lineStartPoint;

	// Token: 0x040013D8 RID: 5080
	private float rayTimer;

	// Token: 0x040013D9 RID: 5081
	private Transform prevMagnetTarget;

	// Token: 0x040013DA RID: 5082
	private Transform droneTransform;

	// Token: 0x040013DB RID: 5083
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040013DC RID: 5084
	private ItemAttributes itemAttributes;

	// Token: 0x040013DD RID: 5085
	private Transform particleEffects;

	// Token: 0x040013DE RID: 5086
	private Transform onSwitchTransform;

	// Token: 0x040013DF RID: 5087
	internal Vector3 connectionPoint;

	// Token: 0x040013E0 RID: 5088
	internal Transform lastPlayerToTouch;

	// Token: 0x040013E1 RID: 5089
	private PhysGrabObject physGrabObject;

	// Token: 0x040013E2 RID: 5090
	private PhysicMaterial physicMaterialOriginal;

	// Token: 0x040013E3 RID: 5091
	private ItemToggle itemToggle;

	// Token: 0x040013E4 RID: 5092
	internal PlayerAvatar playerAvatarTarget;

	// Token: 0x040013E5 RID: 5093
	private bool targetIsPlayer;

	// Token: 0x040013E6 RID: 5094
	internal bool targetIsLocalPlayer;

	// Token: 0x040013E7 RID: 5095
	private Camera cameraMain;

	// Token: 0x040013E8 RID: 5096
	private bool upgradeDone;

	// Token: 0x040013E9 RID: 5097
	private bool pushedOrPulled;

	// Token: 0x040013EA RID: 5098
	private ITargetingCondition customTargetingCondition;
}
