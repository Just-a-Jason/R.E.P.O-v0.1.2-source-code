using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class FloaterAttackLogic : MonoBehaviour
{
	// Token: 0x06000263 RID: 611 RVA: 0x00018408 File Offset: 0x00016608
	private void StateMachine()
	{
		if (this.controller.currentState == EnemyFloater.State.ChargeAttack || this.controller.currentState == EnemyFloater.State.DelayAttack || this.controller.currentState == EnemyFloater.State.Attack)
		{
			EnemyFloater.State currentState = this.controller.currentState;
			if (currentState != EnemyFloater.State.ChargeAttack)
			{
				if (currentState == EnemyFloater.State.Stun)
				{
					this.StateSet(FloaterAttackLogic.FloaterAttackState.end);
				}
			}
			else if (this.state != FloaterAttackLogic.FloaterAttackState.levitate)
			{
				this.StateSet(FloaterAttackLogic.FloaterAttackState.start);
			}
		}
		else
		{
			this.StateSet(FloaterAttackLogic.FloaterAttackState.end);
		}
		switch (this.state)
		{
		case FloaterAttackLogic.FloaterAttackState.start:
			this.StateStart();
			return;
		case FloaterAttackLogic.FloaterAttackState.levitate:
			this.StateLevitate();
			return;
		case FloaterAttackLogic.FloaterAttackState.stop:
			this.StateStop();
			return;
		case FloaterAttackLogic.FloaterAttackState.smash:
			this.StateSmash();
			return;
		case FloaterAttackLogic.FloaterAttackState.end:
			this.StateEnd();
			return;
		case FloaterAttackLogic.FloaterAttackState.inactive:
			this.StateInactive();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x000184D0 File Offset: 0x000166D0
	private void Reset()
	{
		this.checkTimer = 0f;
		this.particleCount = 0;
		this.tumblePhysObjectCheckTimer = 0f;
		foreach (FloaterLine floaterLine in this.floaterLines)
		{
			if (floaterLine)
			{
				floaterLine.outro = true;
			}
		}
		this.capturedPlayerAvatars.Clear();
		this.capturedPhysGrabObjects.Clear();
		this.floaterLines.Clear();
		this.sphereEffects.localScale = Vector3.zero;
		this.attackLight.intensity = 0f;
		this.sphereEffects.gameObject.SetActive(false);
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0001859C File Offset: 0x0001679C
	private void StateInactive()
	{
		if (this.stateStart)
		{
			this.Reset();
			this.stateStart = false;
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x000185B4 File Offset: 0x000167B4
	private void StateEnd()
	{
		if (this.stateStart)
		{
			foreach (FloaterLine floaterLine in this.floaterLines)
			{
				if (floaterLine)
				{
					floaterLine.outro = true;
				}
			}
			this.stateStart = false;
		}
		if (this.sphereEffects.gameObject.activeSelf)
		{
			this.sphereEffects.localScale = Vector3.Lerp(this.sphereEffects.localScale, Vector3.zero, Time.deltaTime * 20f);
			this.attackLight.intensity = Mathf.Lerp(this.attackLight.intensity, 0f, Time.deltaTime * 20f);
			if (this.sphereEffects.localScale.x < 0.01f)
			{
				this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
				return;
			}
		}
		else
		{
			this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
		}
	}

	// Token: 0x06000267 RID: 615 RVA: 0x000186AC File Offset: 0x000168AC
	private void StateStart()
	{
		if (this.stateStart)
		{
			this.Reset();
			this.sphereEffects.gameObject.SetActive(true);
			this.stateStart = false;
		}
		this.sphereEffects.localScale = Vector3.Lerp(this.sphereEffects.localScale, Vector3.one * 1.2f, Time.deltaTime * 6f);
		this.attackLight.intensity = 4f * this.sphereEffects.localScale.magnitude;
		if (this.sphereEffects.localScale.x > 1.19f)
		{
			this.attackLight.intensity = 4f;
			this.sphereEffects.localScale = Vector3.one * 1.2f;
			this.StateSet(FloaterAttackLogic.FloaterAttackState.levitate);
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00018780 File Offset: 0x00016980
	private void StateLevitate()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.GetAllWithinRange();
		}
		if (this.checkTimer > 0.35f)
		{
			this.GetAllWithinRange();
			this.checkTimer = 0f;
		}
		this.checkTimer += Time.deltaTime;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
			{
				playerAvatar.tumble.TumbleOverrideTime(2f);
				this.PlayerTumble(playerAvatar);
			}
			foreach (PhysGrabObject physGrabObject in this.capturedPhysGrabObjects)
			{
				if (physGrabObject && physGrabObject.isEnemy)
				{
					Enemy enemy = physGrabObject.GetComponent<EnemyRigidbody>().enemy;
					if (enemy && enemy.HasStateStunned && enemy.Type < EnemyType.Heavy)
					{
						enemy.StateStunned.Set(4f);
					}
				}
				physGrabObject.OverrideZeroGravity(0.1f);
			}
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x000188C4 File Offset: 0x00016AC4
	private void StateStop()
	{
		if (this.stateStart)
		{
			this.checkTimer = 0f;
			this.stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
			{
				if (playerAvatar)
				{
					playerAvatar.tumble.TumbleOverrideTime(2f);
				}
			}
		}
		this.checkTimer += Time.deltaTime;
		if (this.checkTimer > 0.35f)
		{
			this.RemoveAllOutOfRange();
			this.checkTimer = 0f;
		}
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0001897C File Offset: 0x00016B7C
	private void StateSmash()
	{
		if (this.stateStart)
		{
			GameDirector.instance.CameraShake.ShakeDistance(6f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
			foreach (PhysGrabObject physGrabObject in this.capturedPhysGrabObjects)
			{
				if (physGrabObject)
				{
					this.downParticle.transform.position = physGrabObject.midPoint;
					this.downParticle.Emit(1);
				}
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
				{
					if (playerAvatar && playerAvatar.tumble.isTumbling)
					{
						playerAvatar.tumble.TumbleOverrideTime(2f);
						playerAvatar.tumble.ImpactHurtSet(2f, this.damage);
					}
				}
				foreach (PhysGrabObject physGrabObject2 in this.capturedPhysGrabObjects)
				{
					if (physGrabObject2 && physGrabObject2 && physGrabObject2.rb && !physGrabObject2.rb.isKinematic)
					{
						physGrabObject2.rb.AddForce(Vector3.down * 30f, ForceMode.Impulse);
					}
				}
			}
			foreach (FloaterLine floaterLine in this.floaterLines)
			{
				if (floaterLine)
				{
					floaterLine.outro = true;
				}
			}
			this.floaterLines.Clear();
			this.capturedPlayerAvatars.Clear();
			this.capturedPhysGrabObjects.Clear();
			this.stateStart = false;
		}
		this.sphereEffects.localScale = Vector3.Lerp(this.sphereEffects.localScale, Vector3.zero, Time.deltaTime * 2f);
		if (this.sphereEffects.localScale.x > 0.5f)
		{
			this.attackLight.intensity = Mathf.Lerp(this.attackLight.intensity, 20f, Time.deltaTime * 60f);
		}
		else
		{
			this.attackLight.intensity = 20f * this.sphereEffects.localScale.magnitude;
		}
		if (this.sphereEffects.localScale.x < 0.01f)
		{
			this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
		}
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00018C88 File Offset: 0x00016E88
	private void RemoveAllOutOfRange()
	{
		for (int i = this.capturedPlayerAvatars.Count - 1; i >= 0; i--)
		{
			PlayerAvatar playerAvatar = this.capturedPlayerAvatars[i];
			if (!playerAvatar)
			{
				this.capturedPlayerAvatars.RemoveAt(i);
			}
			else if (Vector3.Distance(new Vector3(playerAvatar.transform.position.x, base.transform.position.y, playerAvatar.transform.position.z), base.transform.position) > this.range * 1.2f)
			{
				this.capturedPlayerAvatars.RemoveAt(i);
				foreach (FloaterLine floaterLine in this.floaterLines)
				{
					if (floaterLine && floaterLine.lineTarget == playerAvatar.PlayerVisionTarget.VisionTransform)
					{
						floaterLine.outro = true;
					}
				}
			}
		}
		for (int j = this.capturedPhysGrabObjects.Count - 1; j >= 0; j--)
		{
			PhysGrabObject physGrabObject = this.capturedPhysGrabObjects[j];
			if (!physGrabObject)
			{
				this.capturedPhysGrabObjects.RemoveAt(j);
			}
			else if (Vector3.Distance(new Vector3(physGrabObject.transform.position.x, base.transform.position.y, physGrabObject.transform.position.z), base.transform.position) > this.range * 1.2f)
			{
				this.capturedPhysGrabObjects.RemoveAt(j);
			}
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00018E4C File Offset: 0x0001704C
	private void StateLevitateFixed()
	{
		if (this.state != FloaterAttackLogic.FloaterAttackState.levitate)
		{
			return;
		}
		if (this.tumblePhysObjectCheckTimer > 1f)
		{
			foreach (PlayerAvatar playerAvatar in this.capturedPlayerAvatars)
			{
				if (playerAvatar.tumble.isTumbling)
				{
					PhysGrabObject physGrabObject = playerAvatar.tumble.physGrabObject;
					if (!this.capturedPhysGrabObjects.Contains(physGrabObject))
					{
						this.capturedPhysGrabObjects.Add(physGrabObject);
					}
				}
			}
			this.tumblePhysObjectCheckTimer = 0f;
		}
		else
		{
			this.tumblePhysObjectCheckTimer += Time.fixedDeltaTime;
		}
		foreach (PhysGrabObject physGrabObject2 in this.capturedPhysGrabObjects)
		{
			if (physGrabObject2)
			{
				float d = 10f;
				if (physGrabObject2.GetComponent<PlayerTumble>())
				{
					d = 20f;
				}
				if (physGrabObject2 && physGrabObject2.rb && !physGrabObject2.rb.isKinematic)
				{
					physGrabObject2.rb.AddForce(Vector3.up * Time.fixedDeltaTime * d, ForceMode.Force);
					physGrabObject2.rb.AddTorque(Vector3.up * Time.fixedDeltaTime * 0.2f, ForceMode.Force);
					physGrabObject2.rb.AddTorque(Vector3.left * Time.fixedDeltaTime * 0.1f, ForceMode.Force);
					physGrabObject2.rb.velocity = Vector3.Lerp(physGrabObject2.rb.velocity, new Vector3(0f, physGrabObject2.rb.velocity.y, 0f), Time.fixedDeltaTime * 2f);
				}
			}
		}
		if (this.particleCount < this.capturedPhysGrabObjects.Count)
		{
			if (this.capturedPhysGrabObjects[this.particleCount])
			{
				Vector3 vector = this.capturedPhysGrabObjects[this.particleCount].transform.position;
				Vector3 vector2 = Random.insideUnitSphere * 2f;
				vector2.y = -Mathf.Abs(vector2.y);
				vector += vector2;
				this.upParticle.transform.position = vector;
				this.upParticle.Emit(1);
			}
			this.particleCount++;
			return;
		}
		this.particleCount = 0;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00019118 File Offset: 0x00017318
	private void StateStopFixed()
	{
		if (this.state != FloaterAttackLogic.FloaterAttackState.stop)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PhysGrabObject physGrabObject in this.capturedPhysGrabObjects)
			{
				if (physGrabObject && physGrabObject.rb && !physGrabObject.rb.isKinematic)
				{
					physGrabObject.OverrideZeroGravity(0.1f);
					if (physGrabObject.isEnemy)
					{
						Enemy enemy = physGrabObject.GetComponent<EnemyRigidbody>().enemy;
						if (enemy && enemy.HasStateStunned && enemy.Type < EnemyType.Heavy)
						{
							enemy.StateStunned.Set(4f);
						}
					}
					physGrabObject.rb.velocity = Vector3.Lerp(physGrabObject.rb.velocity, Vector3.zero, Time.deltaTime * 2f);
				}
			}
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0001921C File Offset: 0x0001741C
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.StateLevitateFixed();
			this.StateStopFixed();
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00019231 File Offset: 0x00017431
	private void Update()
	{
		this.StateMachine();
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00019239 File Offset: 0x00017439
	public void StateSet(FloaterAttackLogic.FloaterAttackState _state)
	{
		if (this.state != _state)
		{
			this.state = _state;
			this.stateStart = true;
		}
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00019254 File Offset: 0x00017454
	public void GetAllWithinRange()
	{
		this.RemoveAllOutOfRange();
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetAllPlayerAvatarWithinRange(this.range, base.transform.position, false, default(LayerMask)))
		{
			if (!this.capturedPlayerAvatars.Contains(playerAvatar))
			{
				this.capturedPlayerAvatars.Add(playerAvatar);
				this.PlayerTumble(playerAvatar);
				FloaterLine component = Object.Instantiate<GameObject>(this.linePrefab, base.transform.position, Quaternion.identity, base.transform).GetComponent<FloaterLine>();
				component.lineTarget = playerAvatar.PlayerVisionTarget.VisionTransform;
				component.floaterAttack = this;
				this.floaterLines.Add(component);
			}
		}
		foreach (PhysGrabObject physGrabObject in SemiFunc.PhysGrabObjectGetAllWithinRange(this.range, base.transform.position, false, default(LayerMask), null))
		{
			if (!(physGrabObject == this.enemyFloaterPhysGrabObject) && !this.capturedPhysGrabObjects.Contains(physGrabObject))
			{
				this.capturedPhysGrabObjects.Add(physGrabObject);
			}
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x000193B0 File Offset: 0x000175B0
	private void PlayerTumble(PlayerAvatar _player)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!_player)
		{
			return;
		}
		if (_player.isDisabled)
		{
			return;
		}
		if (!_player.tumble.isTumbling)
		{
			_player.tumble.TumbleRequest(true, false);
		}
		_player.tumble.TumbleOverrideTime(2f);
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00019401 File Offset: 0x00017601
	private void OnEnable()
	{
		this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0001940A File Offset: 0x0001760A
	private void OnDisable()
	{
		this.StateSet(FloaterAttackLogic.FloaterAttackState.inactive);
		this.StateInactive();
	}

	// Token: 0x0400046C RID: 1132
	public GameObject linePrefab;

	// Token: 0x0400046D RID: 1133
	public ParticleSystem upParticle;

	// Token: 0x0400046E RID: 1134
	public ParticleSystem downParticle;

	// Token: 0x0400046F RID: 1135
	public EnemyFloater controller;

	// Token: 0x04000470 RID: 1136
	public PhysGrabObject enemyFloaterPhysGrabObject;

	// Token: 0x04000471 RID: 1137
	internal int damage = 50;

	// Token: 0x04000472 RID: 1138
	internal FloaterAttackLogic.FloaterAttackState state = FloaterAttackLogic.FloaterAttackState.inactive;

	// Token: 0x04000473 RID: 1139
	private bool stateStart = true;

	// Token: 0x04000474 RID: 1140
	public Transform sphereEffects;

	// Token: 0x04000475 RID: 1141
	public Light attackLight;

	// Token: 0x04000476 RID: 1142
	private float range = 4f;

	// Token: 0x04000477 RID: 1143
	private List<PlayerAvatar> capturedPlayerAvatars = new List<PlayerAvatar>();

	// Token: 0x04000478 RID: 1144
	private List<PhysGrabObject> capturedPhysGrabObjects = new List<PhysGrabObject>();

	// Token: 0x04000479 RID: 1145
	private List<FloaterLine> floaterLines = new List<FloaterLine>();

	// Token: 0x0400047A RID: 1146
	private float checkTimer;

	// Token: 0x0400047B RID: 1147
	private int particleCount;

	// Token: 0x0400047C RID: 1148
	private float tumblePhysObjectCheckTimer;

	// Token: 0x020002D1 RID: 721
	public enum FloaterAttackState
	{
		// Token: 0x04002414 RID: 9236
		start,
		// Token: 0x04002415 RID: 9237
		levitate,
		// Token: 0x04002416 RID: 9238
		stop,
		// Token: 0x04002417 RID: 9239
		smash,
		// Token: 0x04002418 RID: 9240
		end,
		// Token: 0x04002419 RID: 9241
		inactive
	}
}
