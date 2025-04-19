using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class ItemMineStun : MonoBehaviour
{
	// Token: 0x06000B9E RID: 2974 RVA: 0x00067450 File Offset: 0x00065650
	private void Start()
	{
		this.itemMine = base.GetComponent<ItemMine>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.jaw1CurrentRot = this.jaw1Tranform.localRotation.eulerAngles.x;
		this.jaw2CurrentRot = this.jaw2Tranform.localRotation.eulerAngles.x;
		this.jaw2StartRot = this.jaw2Tranform.localRotation;
		this.jaw1StartRot = this.jaw1Tranform.localRotation;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x000674E0 File Offset: 0x000656E0
	private void Reset()
	{
		if (this.triggered)
		{
			this.chomp = false;
			this.bite = false;
			this.jawEval = 0f;
			this.jaw1Tranform.localRotation = this.jaw1StartRot;
			this.jaw2Tranform.localRotation = this.jaw2StartRot;
			this.jaw1CurrentRot = this.jaw1Tranform.localRotation.eulerAngles.x;
			this.jaw2CurrentRot = this.jaw2Tranform.localRotation.eulerAngles.x;
			this.triggered = false;
			this.hurtCollider.SetActive(false);
		}
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00067584 File Offset: 0x00065784
	private void Update()
	{
		if (this.physGrabObject.grabbed && SemiFunc.IsMasterClientOrSingleplayer())
		{
			Quaternion turnX = Quaternion.Euler(0f, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
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
			if (!flag)
			{
				this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
		if (this.itemMine.state == ItemMine.States.Disarmed)
		{
			this.Reset();
			if (this.jawEval > 0f)
			{
				this.jawEval -= Time.deltaTime * 2f;
				if (this.jawEval < 0f)
				{
					this.jawEval = 0f;
				}
				this.jaw1Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.jaw1CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), this.jawAnimationCurve.Evaluate(this.jawEval));
				this.jaw2Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.jaw2CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), this.jawAnimationCurve.Evaluate(this.jawEval));
			}
		}
		if (this.itemMine.state == ItemMine.States.Armed && this.jawEval < 1f)
		{
			this.jawEval += Time.deltaTime * 2f;
			if (this.jawEval > 1f)
			{
				this.jawEval = 1f;
			}
			this.jaw1Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.jaw1CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), this.jawAnimationCurve.Evaluate(this.jawEval));
			this.jaw2Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(this.jaw2CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), this.jawAnimationCurve.Evaluate(this.jawEval));
		}
		if (this.itemMine.state == ItemMine.States.Triggered)
		{
			if (this.jawEval < 1f)
			{
				this.jawEval += Time.deltaTime * 2f;
				if (this.jawEval > 1f)
				{
					this.jawEval = 1f;
				}
				this.jaw1Tranform.localRotation = Quaternion.Euler(-90f * this.jawAnimationCurve.Evaluate(this.jawEval), 0f, 0f);
				this.jaw2Tranform.localRotation = Quaternion.Euler(90f * this.jawAnimationCurve.Evaluate(this.jawEval), 0f, 0f);
				return;
			}
			float num = Mathf.PingPong(Time.time * 5f, 1f);
			float num2 = this.jawAnimationCurve.Evaluate(num);
			if (num > 0.1f)
			{
				if (!this.chomp)
				{
					this.soundChomp.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.chomp = true;
			}
			else
			{
				this.chomp = false;
			}
			if (num > 0.5f)
			{
				if (!this.bite)
				{
					this.ElectricityEffect();
				}
				this.bite = true;
			}
			else
			{
				this.bite = false;
			}
			this.jaw1Tranform.localRotation = Quaternion.Euler(-64f * num2, 0f, 0f);
			this.jaw2Tranform.localRotation = Quaternion.Euler(64f * num2, 0f, 0f);
		}
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0006799C File Offset: 0x00065B9C
	private void ElectricityEffect()
	{
		this.soundElectricity.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		this.particleLightning.Play();
		this.particleFlash.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.bitTransform && Vector3.Distance(base.transform.position, this.bitTransform.position) < 1.5f)
		{
			Vector3 insideUnitSphere = Random.insideUnitSphere;
			this.rb.AddTorque(insideUnitSphere * 1f, ForceMode.Impulse);
			Vector3 insideUnitSphere2 = Random.insideUnitSphere;
			this.rb.AddForce(insideUnitSphere2 * 1f, ForceMode.Impulse);
		}
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00067AB8 File Offset: 0x00065CB8
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemMine.state == ItemMine.States.Armed && Vector3.Angle(base.transform.up, Vector3.up) > 65f)
		{
			Vector3 right = base.transform.right;
			this.rb.AddTorque(right * Time.fixedDeltaTime * 20f, ForceMode.Force);
		}
		if (this.triggered)
		{
			if (!this.itemMine.triggeredTransform)
			{
				Transform transform = SemiFunc.PlayerGetNearestTransformWithinRange(10f, base.transform.position, true, default(LayerMask));
				if (transform)
				{
					this.itemMine.wasTriggeredByPlayer = true;
					this.itemMine.triggeredPlayerAvatar = transform.GetComponentInParent<PlayerAvatar>();
					this.bitTransform = transform;
				}
				else
				{
					Enemy enemy = SemiFunc.EnemyGetNearest(base.transform.position, 10f, true);
					if (enemy)
					{
						this.itemMine.wasTriggeredByEnemy = true;
						this.bitPhysGrabObject = enemy.GetComponentInParent<PhysGrabObject>();
						this.bitTransform = enemy.CenterTransform;
					}
				}
			}
			if (this.itemMine.wasTriggeredByEnemy || this.itemMine.wasTriggeredByRigidBody)
			{
				if (!this.bitPhysGrabObject)
				{
					this.itemMine.DestroyMine();
					return;
				}
				if (this.bitPhysGrabObject && !this.bitPhysGrabObject.gameObject.activeInHierarchy)
				{
					this.itemMine.DestroyMine();
					return;
				}
			}
			if (this.itemMine.wasTriggeredByPlayer)
			{
				if (!this.itemMine.triggeredPlayerAvatar && !this.itemMine.triggeredPlayerTumble)
				{
					this.itemMine.DestroyMine();
					return;
				}
				if (this.itemMine.triggeredPlayerAvatar.isDisabled)
				{
					this.itemMine.DestroyMine();
					return;
				}
			}
			if (this.itemMine.wasTriggeredByPlayer)
			{
				if (this.bitTransform && this.itemMine.triggeredPlayerTumble && !this.itemMine.triggeredPlayerTumble.isActiveAndEnabled)
				{
					this.itemMine.DestroyMine();
					return;
				}
				if (!this.bitTransform)
				{
					if (this.itemMine.triggeredPlayerAvatar)
					{
						this.bitTransform = this.itemMine.triggeredPlayerAvatar.PlayerVisionTarget.VisionTransform;
					}
					if (this.itemMine.triggeredPlayerTumble)
					{
						this.bitTransform = this.itemMine.triggeredPlayerTumble.playerAvatar.PlayerVisionTarget.VisionTransform;
					}
				}
			}
			if (!this.bitTransform)
			{
				this.itemMine.DestroyMine();
				return;
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				Vector3 position = base.transform.position;
				if (this.itemMine.wasTriggeredByPlayer)
				{
					position = this.bitTransform.position;
				}
				if (this.itemMine.wasTriggeredByEnemy)
				{
					position = this.bitTransform.position;
				}
				Vector3 vector = this.bitTransform.position;
				if (this.bitPhysGrabObject)
				{
					vector = this.bitPhysGrabObject.midPoint;
				}
				Vector3 vector2 = position - new Vector3(vector.x, position.y, vector.z);
				this.physGrabObject.OverrideZeroGravity(0.1f);
				Vector3 a = SemiFunc.PhysFollowPosition(base.transform.position, position, this.rb.velocity, 10f);
				this.rb.AddForce(a * Time.fixedDeltaTime, ForceMode.Impulse);
				if (vector2 != Vector3.zero)
				{
					Vector3 a2 = SemiFunc.PhysFollowRotation(base.transform, Quaternion.LookRotation(vector2), this.rb, 20f);
					this.rb.AddTorque(a2 * Time.fixedDeltaTime, ForceMode.Impulse);
				}
			}
		}
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x00067E88 File Offset: 0x00066088
	public void OnTriggered()
	{
		this.ElectricityEffect();
		this.jawEval = 0f;
		this.triggered = true;
		this.bitTransform = this.itemMine.triggeredTransform;
		this.bitPhysGrabObject = this.itemMine.triggeredPhysGrabObject;
		this.hurtCollider.SetActive(true);
	}

	// Token: 0x040012EA RID: 4842
	private ItemMine itemMine;

	// Token: 0x040012EB RID: 4843
	private PhysGrabObject bitPhysGrabObject;

	// Token: 0x040012EC RID: 4844
	private Transform bitTransform;

	// Token: 0x040012ED RID: 4845
	private Vector3 startPosition;

	// Token: 0x040012EE RID: 4846
	private bool triggered;

	// Token: 0x040012EF RID: 4847
	public AnimationCurve jawAnimationCurve;

	// Token: 0x040012F0 RID: 4848
	private Rigidbody rb;

	// Token: 0x040012F1 RID: 4849
	private PhysGrabObject physGrabObject;

	// Token: 0x040012F2 RID: 4850
	public Transform jaw1Tranform;

	// Token: 0x040012F3 RID: 4851
	public Transform jaw2Tranform;

	// Token: 0x040012F4 RID: 4852
	private float jawEval;

	// Token: 0x040012F5 RID: 4853
	private float jaw1CurrentRot;

	// Token: 0x040012F6 RID: 4854
	private float jaw2CurrentRot;

	// Token: 0x040012F7 RID: 4855
	public GameObject hurtCollider;

	// Token: 0x040012F8 RID: 4856
	private bool bite;

	// Token: 0x040012F9 RID: 4857
	public ParticleSystem particleFlash;

	// Token: 0x040012FA RID: 4858
	public ParticleSystem particleLightning;

	// Token: 0x040012FB RID: 4859
	private bool chomp;

	// Token: 0x040012FC RID: 4860
	public Sound soundChomp;

	// Token: 0x040012FD RID: 4861
	public Sound soundElectricity;

	// Token: 0x040012FE RID: 4862
	private Quaternion jaw1StartRot;

	// Token: 0x040012FF RID: 4863
	private Quaternion jaw2StartRot;
}
