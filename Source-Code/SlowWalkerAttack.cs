using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class SlowWalkerAttack : MonoBehaviour
{
	// Token: 0x0600049E RID: 1182 RVA: 0x0002DBCE File Offset: 0x0002BDCE
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.vacuumParticles.AddRange(this.attackVacuumBuildup.GetComponentsInChildren<ParticleSystem>());
		this.impactParticles.AddRange(this.attackImpact.GetComponentsInChildren<ParticleSystem>());
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0002DC08 File Offset: 0x0002BE08
	private void SuckInListUpdate()
	{
		if (!this.clubHitPoint)
		{
			return;
		}
		base.transform.position = this.clubHitPoint.position;
		RaycastHit[] array = Physics.RaycastAll(this.slowWalkerCenter, (this.clubHitPoint.position - this.slowWalkerCenter).normalized, 4f, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		bool flag = false;
		Vector3 point = this.slowWalkerCenter;
		float num = float.MaxValue;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.collider.gameObject.CompareTag("Wall"))
			{
				float num2 = Vector3.Distance(this.slowWalkerCenter, raycastHit.point);
				if (num2 < num)
				{
					num = num2;
					point = raycastHit.point;
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.foundPosition = point;
			this.foundPosition = Vector3.MoveTowards(this.foundPosition, this.slowWalkerCenter, 0.2f);
			this.didFindPosition = true;
		}
		point = this.slowWalkerCenter;
		num = float.MaxValue;
		RaycastHit[] array3 = Physics.RaycastAll(base.transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		flag = false;
		foreach (RaycastHit raycastHit2 in array3)
		{
			if (raycastHit2.collider.gameObject.CompareTag("Wall"))
			{
				float num3 = Vector3.Distance(base.transform.position, raycastHit2.point);
				if (num3 < num)
				{
					num = num3;
					point = raycastHit2.point;
					flag = true;
				}
			}
		}
		if (flag)
		{
			this.foundPosition = point;
			this.didFindPosition = true;
		}
		if (this.didFindPosition)
		{
			base.transform.position = this.foundPosition;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.physGrabObjects.Clear();
		foreach (PhysGrabObject physGrabObject in SemiFunc.PhysGrabObjectGetAllWithinRange(this.vacuumSphere.localScale.x * 0.5f, this.vacuumSphere.position + Vector3.up * 0.5f, false, default(LayerMask), null))
		{
			RaycastHit[] array4 = Physics.RaycastAll(physGrabObject.midPoint, base.transform.position + Vector3.up * 0.5f - physGrabObject.midPoint, this.vacuumSphere.localScale.x, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
			bool flag2 = false;
			foreach (RaycastHit raycastHit3 in array4)
			{
				if (raycastHit3.collider.gameObject.CompareTag("Wall"))
				{
					flag2 = true;
				}
			}
			if (!flag2 && !physGrabObject.isPlayer && physGrabObject != this.enemyPhysGrabObject)
			{
				this.physGrabObjects.Add(physGrabObject);
			}
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0002DF38 File Offset: 0x0002C138
	private void SuckInListPlayerUpdate()
	{
		Vector3 vector = this.vacuumSphere.position + Vector3.up * 2f;
		Vector3 a = vector;
		this.playersBeingVacuumed.Clear();
		List<PlayerAvatar> collection = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(this.vacuumSphere.localScale.x, vector, false, default(LayerMask));
		this.playersBeingVacuumed.AddRange(collection);
		this.playerTumbles.Clear();
		foreach (PlayerAvatar playerAvatar in this.playersBeingVacuumed)
		{
			vector = this.vacuumSphere.position + Vector3.up * 2f;
			Vector3 position = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			Vector3 normalized = (position - vector).normalized;
			float maxDistance = Vector3.Distance(vector, position);
			RaycastHit[] array = Physics.RaycastAll(vector, normalized, maxDistance, LayerMask.GetMask(new string[]
			{
				"Default"
			}));
			bool flag = false;
			foreach (RaycastHit raycastHit in array)
			{
				if (raycastHit.collider.gameObject.CompareTag("Wall"))
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			if (flag)
			{
				foreach (RaycastHit raycastHit2 in Physics.RaycastAll(vector, Vector3.up, this.vacuumSphere.localScale.x * 0.25f, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					if (raycastHit2.collider.gameObject.CompareTag("Wall"))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					vector = a + Vector3.up * this.vacuumSphere.localScale.x * 0.25f;
					normalized = (position - vector).normalized;
					maxDistance = Vector3.Distance(vector, position);
					RaycastHit[] array3 = Physics.RaycastAll(vector, normalized, maxDistance, LayerMask.GetMask(new string[]
					{
						"Default"
					}));
					flag = false;
					foreach (RaycastHit raycastHit3 in array3)
					{
						if (raycastHit3.collider.gameObject.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag && !flag2)
			{
				foreach (RaycastHit raycastHit4 in Physics.RaycastAll(vector, Vector3.up, this.vacuumSphere.localScale.x * 0.5f, LayerMask.GetMask(new string[]
				{
					"Default"
				})))
				{
					if (raycastHit4.collider.gameObject.CompareTag("Wall"))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					vector = a + Vector3.up * this.vacuumSphere.localScale.x * 0.5f;
					normalized = (position - vector).normalized;
					maxDistance = Vector3.Distance(vector, position);
					RaycastHit[] array4 = Physics.RaycastAll(vector, normalized, maxDistance, LayerMask.GetMask(new string[]
					{
						"Default"
					}));
					flag = false;
					foreach (RaycastHit raycastHit5 in array4)
					{
						if (raycastHit5.collider.gameObject.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				if (playerAvatar.isTumbling)
				{
					this.playerTumbles.Add(playerAvatar.tumble);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						playerAvatar.tumble.TumbleOverrideTime(2f);
						playerAvatar.tumble.OverrideEnemyHurt(0.5f);
					}
				}
				if (!playerAvatar.isDisabled && !playerAvatar.isTumbling)
				{
					this.playerTumbles.Add(playerAvatar.tumble);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						playerAvatar.tumble.TumbleRequest(true, false);
						playerAvatar.tumble.TumbleOverrideTime(2f);
						playerAvatar.tumble.OverrideEnemyHurt(0.5f);
					}
				}
			}
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0002E3A4 File Offset: 0x0002C5A4
	private void StateIdle()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart)
		{
			this.didFindPosition = false;
			this.stateStart = false;
			this.attackImpactHurtColliders.SetActive(false);
			this.attackVacuumHurtCollider.SetActive(false);
			this.hurtColliderFirstHit.SetActive(false);
		}
		if (SemiFunc.FPSImpulse1())
		{
			if (this.hurtColliderFirstHit.activeSelf)
			{
				this.hurtColliderFirstHit.SetActive(false);
			}
			if (this.attackVacuumHurtCollider.activeSelf)
			{
				this.attackVacuumHurtCollider.SetActive(false);
			}
			if (this.attackImpactHurtColliders.activeSelf)
			{
				this.attackImpactHurtColliders.SetActive(false);
			}
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0002E446 File Offset: 0x0002C646
	private void StateCheckInitial()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart)
		{
			this.didFindPosition = false;
			this.SuckInListUpdate();
			this.stateStart = false;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.StateSet(SlowWalkerAttack.State.Implosion);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0002E47C File Offset: 0x0002C67C
	private void StateImplosion()
	{
		if (this.stateStart)
		{
			this.stateTimer = 1.5f;
			this.hurtColliderTimer = 0.2f;
			this.attackVacuumHurtCollider.SetActive(true);
			this.hurtColliderFirstHit.SetActive(true);
			this.stateStart = false;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 15f, base.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 6f, 15f, base.transform.position, 0.1f);
			this.ParticlesPlayVacuum();
			this.soundVacuumImpact.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundVacuumImpactGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundVacuumBuildup.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Vector3 normalized = (base.transform.position - this.slowWalkerCenter).normalized;
			base.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			float y = base.transform.rotation.eulerAngles.y;
			base.transform.rotation = Quaternion.Euler(0f, y, 0f);
			this.SuckInListPlayerUpdate();
		}
		if (this.stateFixed)
		{
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			foreach (PlayerTumble playerTumble in this.playerTumbles)
			{
				if (playerTumble.isTumbling)
				{
					Vector3 normalized2 = (this.vacuumSphere.position - playerTumble.physGrabObject.transform.position).normalized;
					Rigidbody rb = playerTumble.physGrabObject.rb;
					rb.AddForce(normalized2 * 2500f * Time.fixedDeltaTime, ForceMode.Force);
					Vector3 a = SemiFunc.PhysFollowDirection(rb.transform, normalized2, rb, 10f) * 2f;
					rb.AddTorque(a / rb.mass, ForceMode.Force);
				}
			}
			foreach (PhysGrabObject physGrabObject in this.physGrabObjects)
			{
				if (physGrabObject)
				{
					Vector3 normalized3 = (this.vacuumSphere.position - physGrabObject.transform.position).normalized;
					Rigidbody rb2 = physGrabObject.rb;
					rb2.AddForce(normalized3 * 2500f * Time.fixedDeltaTime, ForceMode.Force);
					Vector3 a2 = SemiFunc.PhysFollowDirection(rb2.transform, normalized3, rb2, 10f) * 2f;
					rb2.AddTorque(a2 / rb2.mass, ForceMode.Force);
				}
			}
		}
		if (!this.stateFixed)
		{
			if (this.didFindPosition)
			{
				base.transform.position = this.foundPosition;
			}
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			this.enemy.Health.ObjectHurtDisable(0.5f);
			if (this.stateTimer <= 0f)
			{
				this.StateSet(SlowWalkerAttack.State.Attack);
				this.attackVacuumHurtCollider.SetActive(false);
			}
			if (this.stateTimer < 1f && this.hurtColliderFirstHit.activeSelf)
			{
				this.hurtColliderFirstHit.SetActive(false);
			}
			if (SemiFunc.FPSImpulse5())
			{
				this.SuckInListPlayerUpdate();
			}
			if (this.hurtColliderTimer > 0f)
			{
				this.hurtColliderTimer -= Time.deltaTime;
				return;
			}
			this.attackVacuumHurtCollider.SetActive(false);
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0002E8A4 File Offset: 0x0002CAA4
	private void StateDelay()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0002E8BE File Offset: 0x0002CABE
	private void StateCheckAttack()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0002E8D8 File Offset: 0x0002CAD8
	private void StateAttack()
	{
		if (this.stateFixed)
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
			this.stateTimer = 3.5f;
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 6f, 15f, base.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 6f, 15f, base.transform.position, 0.1f);
			this.ParticlesPlayImpact();
			this.soundImpact.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.soundImpactGlobal.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Vector3 normalized = (base.transform.position - this.slowWalkerCenter).normalized;
			base.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			this.hurtColliderTimer = 0.2f;
			this.attackImpactHurtColliders.SetActive(true);
		}
		if (this.stateTimer <= 0f)
		{
			this.StateSet(SlowWalkerAttack.State.Idle);
			this.attackImpactHurtColliders.SetActive(false);
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			return;
		}
		this.attackImpactHurtColliders.SetActive(false);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0002EA5C File Offset: 0x0002CC5C
	private void StateMachine(bool _stateFixed)
	{
		if (_stateFixed)
		{
			this.stateFixed = true;
		}
		switch (this.currentState)
		{
		case SlowWalkerAttack.State.Idle:
			this.StateIdle();
			break;
		case SlowWalkerAttack.State.CheckInitial:
			this.StateCheckInitial();
			break;
		case SlowWalkerAttack.State.Implosion:
			this.StateImplosion();
			break;
		case SlowWalkerAttack.State.Attack:
			this.StateAttack();
			break;
		}
		if (_stateFixed && this.stateFixed)
		{
			this.stateFixed = false;
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0002EACA File Offset: 0x0002CCCA
	public void AttackStart()
	{
		this.currentState = SlowWalkerAttack.State.CheckInitial;
		this.stateStart = true;
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0002EADC File Offset: 0x0002CCDC
	private void Update()
	{
		if (this.enemyPhysGrabObject)
		{
			this.slowWalkerCenter = this.enemyPhysGrabObject.midPoint;
		}
		if (SemiFunc.FPSImpulse1() && this.enemy && this.enemy.EnemyParent && !this.enemy.EnemyParent.Spawned && this.currentState != SlowWalkerAttack.State.Idle)
		{
			this.StateSet(SlowWalkerAttack.State.Idle);
		}
		this.StateMachine(false);
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0002EB74 File Offset: 0x0002CD74
	private void FixedUpdate()
	{
		this.StateMachine(true);
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0002EB7D File Offset: 0x0002CD7D
	public void StateSet(SlowWalkerAttack.State state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.StateSetRPC(state);
			return;
		}
		this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0002EBB6 File Offset: 0x0002CDB6
	[PunRPC]
	public void StateSetRPC(SlowWalkerAttack.State state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0002EBC8 File Offset: 0x0002CDC8
	private void ParticlesPlayVacuum()
	{
		foreach (ParticleSystem particleSystem in this.vacuumParticles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0002EC18 File Offset: 0x0002CE18
	private void ParticlesPlayImpact()
	{
		foreach (ParticleSystem particleSystem in this.impactParticles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0002EC68 File Offset: 0x0002CE68
	private void OnDisable()
	{
		this.attackVacuumHurtCollider.SetActive(false);
		this.hurtColliderFirstHit.SetActive(false);
		this.attackImpactHurtColliders.SetActive(false);
		this.attackImpact.SetActive(false);
		this.attackVacuumBuildup.SetActive(false);
	}

	// Token: 0x04000789 RID: 1929
	public Transform vacuumSphere;

	// Token: 0x0400078A RID: 1930
	[Space(10f)]
	public GameObject attackVacuumBuildup;

	// Token: 0x0400078B RID: 1931
	public GameObject attackVacuumHurtCollider;

	// Token: 0x0400078C RID: 1932
	public GameObject attackImpact;

	// Token: 0x0400078D RID: 1933
	public GameObject attackImpactHurtColliders;

	// Token: 0x0400078E RID: 1934
	private PhotonView photonView;

	// Token: 0x0400078F RID: 1935
	[Space(10f)]
	private List<PlayerAvatar> playersBeingVacuumed = new List<PlayerAvatar>();

	// Token: 0x04000790 RID: 1936
	private List<PlayerTumble> playerTumbles = new List<PlayerTumble>();

	// Token: 0x04000791 RID: 1937
	private List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	// Token: 0x04000792 RID: 1938
	private List<ParticleSystem> vacuumParticles = new List<ParticleSystem>();

	// Token: 0x04000793 RID: 1939
	private List<ParticleSystem> impactParticles = new List<ParticleSystem>();

	// Token: 0x04000794 RID: 1940
	[Space(10f)]
	public Sound soundVacuumImpact;

	// Token: 0x04000795 RID: 1941
	public Sound soundVacuumImpactGlobal;

	// Token: 0x04000796 RID: 1942
	public Sound soundVacuumBuildup;

	// Token: 0x04000797 RID: 1943
	public Sound soundImpact;

	// Token: 0x04000798 RID: 1944
	public Sound soundImpactGlobal;

	// Token: 0x04000799 RID: 1945
	public PhysGrabObject enemyPhysGrabObject;

	// Token: 0x0400079A RID: 1946
	public Enemy enemy;

	// Token: 0x0400079B RID: 1947
	internal SlowWalkerAttack.State currentState;

	// Token: 0x0400079C RID: 1948
	private bool stateStart;

	// Token: 0x0400079D RID: 1949
	private bool stateFixed;

	// Token: 0x0400079E RID: 1950
	private float stateTimer;

	// Token: 0x0400079F RID: 1951
	private float hurtColliderTimer;

	// Token: 0x040007A0 RID: 1952
	public Transform clubHitPoint;

	// Token: 0x040007A1 RID: 1953
	public GameObject hurtColliderFirstHit;

	// Token: 0x040007A2 RID: 1954
	private Vector3 foundPosition;

	// Token: 0x040007A3 RID: 1955
	private bool didFindPosition;

	// Token: 0x040007A4 RID: 1956
	private Vector3 slowWalkerCenter;

	// Token: 0x020002E1 RID: 737
	public enum State
	{
		// Token: 0x040024B7 RID: 9399
		Idle,
		// Token: 0x040024B8 RID: 9400
		CheckInitial,
		// Token: 0x040024B9 RID: 9401
		Implosion,
		// Token: 0x040024BA RID: 9402
		Delay,
		// Token: 0x040024BB RID: 9403
		CheckAttack,
		// Token: 0x040024BC RID: 9404
		Attack
	}
}
