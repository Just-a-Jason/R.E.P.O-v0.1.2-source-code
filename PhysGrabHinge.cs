using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class PhysGrabHinge : MonoBehaviour
{
	// Token: 0x06000D09 RID: 3337 RVA: 0x0007208C File Offset: 0x0007028C
	private void Awake()
	{
		this.photon = base.GetComponent<PhotonView>();
		Sound.CopySound(this.hingeAudio.moveLoop, this.moveLoop);
		this.moveLoop.Source = this.audioSource;
		this.joint = base.GetComponent<HingeJoint>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.impactDetector.particleDisable = true;
		this.joint.anchor = this.hingePoint.localPosition;
		this.hingePointRb = this.hingePoint.GetComponent<Rigidbody>();
		if (this.hingePointRb)
		{
			this.hingePointHasRb = true;
			this.hingePointPosition = this.hingePoint.position;
		}
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsNotMasterClient())
		{
			Object.Destroy(this.joint);
			this.joint = null;
			this.hingePointHasRb = false;
		}
		this.restPosition = base.transform.position;
		this.restRotation = base.transform.rotation;
		base.StartCoroutine(this.RigidBodyGet());
		base.gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectHinge");
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectHinge");
		}
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x0007220C File Offset: 0x0007040C
	private IEnumerator RigidBodyGet()
	{
		while (!this.physGrabObject.spawned)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.hingePoint.transform.parent = base.transform.parent;
		this.WallTagSet();
		yield break;
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x0007221C File Offset: 0x0007041C
	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.closeDisableTimer = 0.1f;
			return;
		}
		if (this.closing && other.gameObject.CompareTag("Phys Grab Object"))
		{
			this.closing = false;
		}
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00072268 File Offset: 0x00070468
	private void OnJointBreak(float breakForce)
	{
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.physGrabObject.rb.AddForce(-this.physGrabObject.rb.velocity * 2f, ForceMode.Impulse);
			this.physGrabObject.rb.AddTorque(-this.physGrabObject.rb.angularVelocity * 10f, ForceMode.Impulse);
			this.HingeBreakImpulse();
			this.broken = true;
		}
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x000722F8 File Offset: 0x000704F8
	private void FixedUpdate()
	{
		if (this.broken)
		{
			this.brokenTimer += Time.fixedDeltaTime;
		}
		if (this.dead || this.broken || !this.physGrabObject.spawned)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			if (GameManager.Multiplayer())
			{
				this.physGrabObject.photonTransformView.KinematicClientForce(0.1f);
			}
			if (this.hingePointHasRb)
			{
				if (this.joint.angle >= this.hingeOffsetPositiveThreshold)
				{
					Vector3 b = this.hingePointPosition + this.hingePoint.TransformDirection(this.hingeOffsetPositive);
					Vector3 vector = Vector3.Lerp(this.hingePointRb.transform.position, b, this.hingeOffsetSpeed * Time.fixedDeltaTime);
					if (this.hingePointRb.position != vector)
					{
						this.hingePointRb.MovePosition(vector);
					}
				}
				else if (this.joint.angle <= this.hingeOffsetNegativeThreshold)
				{
					Vector3 b2 = this.hingePointPosition + this.hingePoint.TransformDirection(this.hingeOffsetNegative);
					Vector3 vector2 = Vector3.Lerp(this.hingePointRb.transform.position, b2, this.hingeOffsetSpeed * Time.fixedDeltaTime);
					if (this.hingePointRb.position != vector2)
					{
						this.hingePointRb.MovePosition(vector2);
					}
				}
				else
				{
					Vector3 vector3 = Vector3.Lerp(this.hingePointRb.transform.position, this.hingePointPosition, this.hingeOffsetSpeed * Time.fixedDeltaTime);
					if (this.closed)
					{
						vector3 = this.hingePointPosition;
					}
					if (this.hingePointRb.position != vector3)
					{
						this.hingePointRb.MovePosition(vector3);
					}
				}
			}
			if (!this.closed && this.closeDisableTimer <= 0f && this.joint)
			{
				if (!this.closing)
				{
					float num = Vector3.Dot(this.physGrabObject.rb.angularVelocity.normalized, (-this.joint.axis * this.joint.angle).normalized);
					if (this.physGrabObject.rb.angularVelocity.magnitude < this.closeMaxSpeed && Mathf.Abs(this.joint.angle) < this.closeThreshold && (num > 0f || this.physGrabObject.rb.angularVelocity.magnitude < 0.1f))
					{
						this.closeHeavy = false;
						this.closeSpeed = Mathf.Max(this.physGrabObject.rb.angularVelocity.magnitude, 0.2f);
						if (this.closeSpeed > this.closeHeavySpeed)
						{
							this.closeHeavy = true;
						}
						this.closing = true;
					}
				}
				else if (this.physGrabObject.playerGrabbing.Count > 0)
				{
					this.closing = false;
				}
				else
				{
					Vector3 vector4 = this.restRotation.eulerAngles - this.physGrabObject.rb.rotation.eulerAngles;
					vector4 = Vector3.ClampMagnitude(vector4, this.closeSpeed);
					this.physGrabObject.rb.AddRelativeTorque(vector4, ForceMode.Acceleration);
					if (Mathf.Abs(this.joint.angle) < 2f)
					{
						this.closedForceTimer = 0.25f;
						this.closing = false;
						this.CloseImpulse(this.closeHeavy);
					}
				}
			}
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				this.closeDisableTimer = 0.1f;
			}
			else if (this.closeDisableTimer > 0f)
			{
				this.closeDisableTimer -= 1f * Time.fixedDeltaTime;
			}
			if (this.closed)
			{
				if (this.closedForceTimer > 0f)
				{
					this.closedForceTimer -= 1f * Time.fixedDeltaTime;
				}
				else if (this.physGrabObject.rb.angularVelocity.magnitude > this.openForceNeeded)
				{
					this.OpenImpulse();
					this.closeDisableTimer = 2f;
					this.closing = false;
				}
				if (this.closed && !this.physGrabObject.rb.isKinematic && (this.physGrabObject.rb.position != this.restPosition || this.physGrabObject.rb.rotation != this.restRotation))
				{
					this.physGrabObject.rb.MovePosition(this.restPosition);
					this.physGrabObject.rb.MoveRotation(this.restRotation);
					this.physGrabObject.rb.angularVelocity = Vector3.zero;
					this.physGrabObject.rb.velocity = Vector3.zero;
				}
			}
			if (this.physGrabObject.playerGrabbing.Count <= 0 && !this.closing && !this.closed)
			{
				Vector3 angularVelocity = this.physGrabObject.rb.angularVelocity;
				if (angularVelocity.magnitude <= 0.1f && this.bounceVelocity.magnitude > 0.5f && this.bounceCooldown <= 0f)
				{
					this.bounceCooldown = 1f;
					this.physGrabObject.rb.AddTorque(this.bounceAmount * -this.bounceVelocity.normalized, ForceMode.Impulse);
					if (this.bounceEffect == PhysGrabHinge.BounceEffect.Heavy)
					{
						this.physGrabObject.heavyImpactImpulse = true;
					}
					else if (this.bounceEffect == PhysGrabHinge.BounceEffect.Medium)
					{
						this.physGrabObject.mediumImpactImpulse = true;
					}
					else
					{
						this.physGrabObject.lightImpactImpulse = true;
					}
					this.moveLoopEndDisableTimer = 1f;
				}
				this.bounceVelocity = angularVelocity;
			}
			else
			{
				this.bounceVelocity = Vector3.zero;
			}
			if (this.bounceCooldown > 0f)
			{
				this.bounceCooldown -= 1f * Time.fixedDeltaTime;
			}
			if (!this.closing)
			{
				this.physGrabObject.OverrideDrag(this.drag, 0.1f);
				this.physGrabObject.OverrideAngularDrag(this.drag, 0.1f);
			}
		}
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0007295C File Offset: 0x00070B5C
	private void Update()
	{
		if (this.dead)
		{
			this.deadTimer -= 1f * Time.deltaTime;
			if (this.deadTimer <= 0f)
			{
				this.impactDetector.DestroyObject(true);
			}
			return;
		}
		if (this.broken)
		{
			this.moveLoop.PlayLoop(false, 1f, 1f, 1f);
			return;
		}
		if (this.hingeAudio.moveLoopEnabled)
		{
			if (this.physGrabObject.rbVelocity.magnitude > this.hingeAudio.moveLoopThreshold)
			{
				if (!this.moveLoopActive)
				{
					this.fadeOutFast = false;
					this.moveLoopActive = true;
				}
				this.moveLoop.PlayLoop(true, this.hingeAudio.moveLoopFadeInSpeed, this.hingeAudio.moveLoopFadeOutSpeed, 1f);
				this.moveLoop.LoopPitch = Mathf.Max(this.moveLoop.Pitch + this.physGrabObject.rbVelocity.magnitude * this.hingeAudio.moveLoopVelocityMult, 0.1f);
			}
			else
			{
				if (this.moveLoopActive)
				{
					if (this.moveLoopEndDisableTimer <= 0f)
					{
						this.hingeAudio.moveLoopEnd.Play(this.moveLoop.Source.transform.position, 1f, 1f, 1f, 1f);
						this.moveLoopEndDisableTimer = 3f;
					}
					this.moveLoopActive = false;
				}
				if (this.fadeOutFast)
				{
					this.moveLoop.PlayLoop(false, this.hingeAudio.moveLoopFadeInSpeed, 20f, 1f);
				}
				else
				{
					this.moveLoop.PlayLoop(false, this.hingeAudio.moveLoopFadeInSpeed, this.hingeAudio.moveLoopFadeOutSpeed, 1f);
				}
				this.moveLoopEndDisableTimer = 0.5f;
			}
			if (this.moveLoopEndDisableTimer > 0f)
			{
				this.moveLoopEndDisableTimer -= 1f * Time.deltaTime;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.investigateDelay > 0f)
		{
			this.investigateDelay -= 1f * Time.deltaTime;
			if (this.investigateDelay <= 0f && this.physGrabObject.enemyInteractTimer <= 0f)
			{
				EnemyDirector.instance.SetInvestigate(this.physGrabObject.midPoint, this.investigateRadius);
			}
		}
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00072BBC File Offset: 0x00070DBC
	private void WallTagSet()
	{
		string text = "Untagged";
		if (this.closed && !this.broken && !this.dead)
		{
			text = "Wall";
		}
		if (text == "Wall" && this.wallTagHinges.Length != 0)
		{
			foreach (PhysGrabHinge physGrabHinge in this.wallTagHinges)
			{
				if (!physGrabHinge || !physGrabHinge.closed)
				{
					return;
				}
			}
		}
		if (this.wallTagObjects.Length != 0)
		{
			foreach (GameObject gameObject in this.wallTagObjects)
			{
				if (gameObject)
				{
					gameObject.tag = text;
				}
			}
		}
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00072C64 File Offset: 0x00070E64
	private void EnemyInvestigate(float radius)
	{
		this.investigateDelay = 0.1f;
		this.investigateRadius = radius;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00072C78 File Offset: 0x00070E78
	private void CloseImpulse(bool heavy)
	{
		this.EnemyInvestigate(1f);
		if (GameManager.instance.gameMode == 0)
		{
			this.CloseImpulseRPC(heavy);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("CloseImpulseRPC", RpcTarget.All, new object[]
			{
				heavy
			});
		}
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00072CCC File Offset: 0x00070ECC
	[PunRPC]
	private void CloseImpulseRPC(bool heavy)
	{
		this.fadeOutFast = true;
		GameDirector.instance.CameraImpact.ShakeDistance(this.closeShake * 0.5f, 3f, 10f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(this.closeShake, 3f, 10f, base.transform.position, 0.1f);
		if (heavy)
		{
			this.hingeAudio.CloseHeavy.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			this.hingeAudio.Close.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		this.moveLoopEndDisableTimer = 1f;
		this.closed = true;
		this.WallTagSet();
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00072DCB File Offset: 0x00070FCB
	private void OpenImpulse()
	{
		this.EnemyInvestigate(0.5f);
		if (GameManager.instance.gameMode == 0)
		{
			this.OpenImpulseRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("OpenImpulseRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00072E08 File Offset: 0x00071008
	[PunRPC]
	private void OpenImpulseRPC()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(this.openShake * 0.5f, 3f, 10f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(this.openShake, 3f, 10f, base.transform.position, 0.1f);
		if (this.physGrabObject.rbAngularVelocity.magnitude > this.openHeavyThreshold)
		{
			this.hingeAudio.OpenHeavy.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		else
		{
			this.hingeAudio.Open.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		}
		this.closed = false;
		this.WallTagSet();
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00072F0A File Offset: 0x0007110A
	private void HingeBreakImpulse()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.HingeBreakRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("HingeBreakRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00072F3C File Offset: 0x0007113C
	[PunRPC]
	private void HingeBreakRPC()
	{
		GameDirector.instance.CameraImpact.ShakeDistance(this.hingeBreakShake * 0.5f, 3f, 10f, base.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(this.hingeBreakShake, 3f, 10f, base.transform.position, 0.1f);
		this.hingeAudio.HingeBreak.Play(this.audioSource.transform.position, 1f, 1f, 1f, 1f);
		this.physGrabObject.heavyBreakImpulse = true;
		this.impactDetector.isHinge = false;
		this.impactDetector.isBrokenHinge = true;
		this.impactDetector.particleDisable = false;
		this.broken = true;
		this.WallTagSet();
		int layer = LayerMask.NameToLayer("PhysGrabObject");
		base.gameObject.layer = layer;
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.layer = layer;
		}
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00073088 File Offset: 0x00071288
	public void DestroyHinge()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.DestroyHingeRPC();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photon.RPC("DestroyHingeRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000730BA File Offset: 0x000712BA
	[PunRPC]
	private void DestroyHingeRPC()
	{
		this.dead = true;
		this.WallTagSet();
	}

	// Token: 0x040014F6 RID: 5366
	private PhotonView photon;

	// Token: 0x040014F7 RID: 5367
	public HingeAudio hingeAudio;

	// Token: 0x040014F8 RID: 5368
	public AudioSource audioSource;

	// Token: 0x040014F9 RID: 5369
	[Space]
	public Transform hingePoint;

	// Token: 0x040014FA RID: 5370
	private Rigidbody hingePointRb;

	// Token: 0x040014FB RID: 5371
	private bool hingePointHasRb;

	// Token: 0x040014FC RID: 5372
	public float hingeOffsetPositiveThreshold = 15f;

	// Token: 0x040014FD RID: 5373
	public float hingeOffsetNegativeThreshold = -15f;

	// Token: 0x040014FE RID: 5374
	public float hingeOffsetSpeed = 5f;

	// Token: 0x040014FF RID: 5375
	public Vector3 hingeOffsetPositive;

	// Token: 0x04001500 RID: 5376
	public Vector3 hingeOffsetNegative;

	// Token: 0x04001501 RID: 5377
	private Vector3 hingePointPosition;

	// Token: 0x04001502 RID: 5378
	[Space]
	public float hingeBreakShake = 3f;

	// Token: 0x04001503 RID: 5379
	[Space]
	public float closeThreshold = 10f;

	// Token: 0x04001504 RID: 5380
	public float closeMaxSpeed = 1f;

	// Token: 0x04001505 RID: 5381
	public float closeHeavySpeed = 5f;

	// Token: 0x04001506 RID: 5382
	public float closeShake = 3f;

	// Token: 0x04001507 RID: 5383
	private bool closeHeavy;

	// Token: 0x04001508 RID: 5384
	private float closeSpeed;

	// Token: 0x04001509 RID: 5385
	internal bool closed = true;

	// Token: 0x0400150A RID: 5386
	private float closedForceTimer;

	// Token: 0x0400150B RID: 5387
	private bool closing;

	// Token: 0x0400150C RID: 5388
	private float closeDisableTimer;

	// Token: 0x0400150D RID: 5389
	[Space]
	private float openForceNeeded = 0.04f;

	// Token: 0x0400150E RID: 5390
	public float openHeavyThreshold = 3f;

	// Token: 0x0400150F RID: 5391
	public float openShake = 3f;

	// Token: 0x04001510 RID: 5392
	internal HingeJoint joint;

	// Token: 0x04001511 RID: 5393
	private PhysGrabObject physGrabObject;

	// Token: 0x04001512 RID: 5394
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04001513 RID: 5395
	private bool moveLoopActive;

	// Token: 0x04001514 RID: 5396
	private float moveLoopEndDisableTimer;

	// Token: 0x04001515 RID: 5397
	[HideInInspector]
	public Sound moveLoop;

	// Token: 0x04001516 RID: 5398
	private Vector3 restPosition;

	// Token: 0x04001517 RID: 5399
	private Quaternion restRotation;

	// Token: 0x04001518 RID: 5400
	internal bool dead;

	// Token: 0x04001519 RID: 5401
	private float deadTimer = 0.1f;

	// Token: 0x0400151A RID: 5402
	internal bool broken;

	// Token: 0x0400151B RID: 5403
	internal float brokenTimer;

	// Token: 0x0400151C RID: 5404
	[Space]
	public float drag;

	// Token: 0x0400151D RID: 5405
	[Space]
	public float bounceAmount = 0.2f;

	// Token: 0x0400151E RID: 5406
	public PhysGrabHinge.BounceEffect bounceEffect = PhysGrabHinge.BounceEffect.Medium;

	// Token: 0x0400151F RID: 5407
	private Vector3 bounceVelocity;

	// Token: 0x04001520 RID: 5408
	private float bounceCooldown;

	// Token: 0x04001521 RID: 5409
	[Space]
	public PhysGrabHinge[] wallTagHinges;

	// Token: 0x04001522 RID: 5410
	public GameObject[] wallTagObjects;

	// Token: 0x04001523 RID: 5411
	private float investigateDelay;

	// Token: 0x04001524 RID: 5412
	private float investigateRadius;

	// Token: 0x04001525 RID: 5413
	private bool fadeOutFast;

	// Token: 0x02000363 RID: 867
	public enum BounceEffect
	{
		// Token: 0x0400275C RID: 10076
		Light,
		// Token: 0x0400275D RID: 10077
		Medium,
		// Token: 0x0400275E RID: 10078
		Heavy
	}
}
