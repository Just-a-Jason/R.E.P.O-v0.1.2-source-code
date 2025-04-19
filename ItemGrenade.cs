using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000149 RID: 329
public class ItemGrenade : MonoBehaviour
{
	// Token: 0x06000B01 RID: 2817 RVA: 0x00061F6C File Offset: 0x0006016C
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObjectImpactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.splinterTransform = base.transform.Find("Splinter");
		GameObject gameObject = base.transform.Find("Mesh").gameObject;
		this.grenadeEmissionMaterial = gameObject.GetComponent<Renderer>().material;
		this.grenadeStartPosition = base.transform.position;
		this.grenadeStartRotation = base.transform.rotation;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.throwLineTrail = this.throwLine.GetComponent<TrailRenderer>();
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00062040 File Offset: 0x00060240
	private void FixedUpdate()
	{
		if (this.itemEquippable.isEquipped || this.itemEquippable.wasEquippedTimer > 0f)
		{
			this.prevPosition = this.rb.position;
			return;
		}
		Vector3 vector = (this.rb.position - this.prevPosition) / Time.fixedDeltaTime;
		Vector3 normalized = (this.rb.position - this.prevPosition).normalized;
		this.prevPosition = this.rb.position;
		if (!this.physGrabObject.grabbed && vector.magnitude > 2f)
		{
			this.throwLineTimer = 0.2f;
		}
		if (this.throwLineTimer > 0f)
		{
			this.throwLineTrail.emitting = true;
			this.throwLineTimer -= Time.fixedDeltaTime;
			return;
		}
		this.throwLineTrail.emitting = false;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00062130 File Offset: 0x00060330
	private void Update()
	{
		this.soundTick.PlayLoop(this.isActive, 2f, 2f, 1f);
		if (this.itemEquippable.isEquipped)
		{
			if (this.isActive)
			{
				this.isActive = false;
				this.grenadeTimer = 0f;
				this.splinterAnimationProgress = 0f;
				this.itemToggle.ToggleItem(false, -1);
				this.splinterTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
				this.grenadeEmissionMaterial.SetColor("_EmissionColor", Color.black);
			}
			return;
		}
		if (this.isActive)
		{
			if (this.splinterAnimationProgress < 1f)
			{
				this.splinterAnimationProgress += 5f * Time.deltaTime;
				float num = this.splinterAnimationCurve.Evaluate(this.splinterAnimationProgress);
				this.splinterTransform.localEulerAngles = new Vector3(num * 90f, 0f, 0f);
			}
			float value = Mathf.PingPong(Time.time * 8f, 1f);
			Color value2 = this.blinkColor * Mathf.LinearToGammaSpace(value);
			this.grenadeEmissionMaterial.SetColor("_EmissionColor", value2);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemToggle.toggleState && !this.isActive)
		{
			this.isActive = true;
			this.TickStart();
		}
		if (this.isActive)
		{
			this.grenadeTimer += Time.deltaTime;
			if (this.grenadeTimer >= this.tickTime)
			{
				this.grenadeTimer = 0f;
				this.TickEnd();
			}
		}
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x000622D0 File Offset: 0x000604D0
	private void GrenadeReset()
	{
		this.isActive = false;
		this.grenadeTimer = 0f;
		this.throwLine.SetActive(false);
		this.splinterAnimationProgress = 0f;
		this.itemToggle.ToggleItem(false, -1);
		this.splinterTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.grenadeEmissionMaterial.SetColor("_EmissionColor", Color.black);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00062369 File Offset: 0x00060569
	private void TickStart()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("TickStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.TickStartRPC();
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0006238F File Offset: 0x0006058F
	private void TickEnd()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("TickEndRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.TickEndRPC();
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x000623B5 File Offset: 0x000605B5
	[PunRPC]
	private void TickStartRPC()
	{
		this.soundSplinter.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.isActive = true;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x000623EC File Offset: 0x000605EC
	[PunRPC]
	private void TickEndRPC()
	{
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		this.onDetonate.Invoke();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.RunIsShop() || this.isSpawnedGrenade)
			{
				if (!this.isSpawnedGrenade)
				{
					StatsManager.instance.ItemRemove(this.itemAttributes.instanceName);
				}
				this.physGrabObjectImpactDetector.DestroyObject(true);
			}
			else
			{
				this.physGrabObject.Teleport(this.grenadeStartPosition, this.grenadeStartRotation);
			}
		}
		if (SemiFunc.RunIsShop() && !this.isSpawnedGrenade)
		{
			this.GrenadeReset();
		}
	}

	// Token: 0x040011CE RID: 4558
	public Color blinkColor;

	// Token: 0x040011CF RID: 4559
	public UnityEvent onDetonate;

	// Token: 0x040011D0 RID: 4560
	private ItemToggle itemToggle;

	// Token: 0x040011D1 RID: 4561
	private ItemAttributes itemAttributes;

	// Token: 0x040011D2 RID: 4562
	internal bool isActive;

	// Token: 0x040011D3 RID: 4563
	private float grenadeTimer;

	// Token: 0x040011D4 RID: 4564
	public float tickTime = 3f;

	// Token: 0x040011D5 RID: 4565
	private PhotonView photonView;

	// Token: 0x040011D6 RID: 4566
	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	// Token: 0x040011D7 RID: 4567
	public Sound soundSplinter;

	// Token: 0x040011D8 RID: 4568
	public Sound soundTick;

	// Token: 0x040011D9 RID: 4569
	private float splinterAnimationProgress;

	// Token: 0x040011DA RID: 4570
	public AnimationCurve splinterAnimationCurve;

	// Token: 0x040011DB RID: 4571
	private Transform splinterTransform;

	// Token: 0x040011DC RID: 4572
	private Material grenadeEmissionMaterial;

	// Token: 0x040011DD RID: 4573
	private ItemEquippable itemEquippable;

	// Token: 0x040011DE RID: 4574
	private Vector3 grenadeStartPosition;

	// Token: 0x040011DF RID: 4575
	private Quaternion grenadeStartRotation;

	// Token: 0x040011E0 RID: 4576
	private PhysGrabObject physGrabObject;

	// Token: 0x040011E1 RID: 4577
	private Vector3 prevPosition;

	// Token: 0x040011E2 RID: 4578
	[FormerlySerializedAs("isThiefGrenade")]
	[HideInInspector]
	public bool isSpawnedGrenade;

	// Token: 0x040011E3 RID: 4579
	public GameObject throwLine;

	// Token: 0x040011E4 RID: 4580
	private Rigidbody rb;

	// Token: 0x040011E5 RID: 4581
	private float throwLineTimer;

	// Token: 0x040011E6 RID: 4582
	private TrailRenderer throwLineTrail;
}
