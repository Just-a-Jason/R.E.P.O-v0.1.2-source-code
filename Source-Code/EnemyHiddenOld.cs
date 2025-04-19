using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class EnemyHiddenOld : MonoBehaviour
{
	// Token: 0x0600034D RID: 845 RVA: 0x00020DB1 File Offset: 0x0001EFB1
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.startPosition = base.transform.position;
		this.footStepPosition = this.startPosition;
		this.previousPosition = this.startPosition;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x00020DE8 File Offset: 0x0001EFE8
	private void Update()
	{
		this.StateRoam();
		this.StatePlayerNotice();
		this.StateGetPlayer();
		this.StateGoToTarget();
		this.StatePickUpTarget();
		this.StateFindFarawayPoint();
		this.StateKidnapTarget();
		this.StateTauntTarget();
		this.StateDropTarget();
		this.StateDespawn();
		this.FootstepLogic();
		this.SprintTick();
		this.BreathTick();
		this.Breathing();
		this.stateEnd = false;
		if (this.stateTimer > 0f)
		{
			if (this.initialStateTime == 0f)
			{
				this.initialStateTime = this.stateTimer;
			}
			this.stateTimer -= Time.deltaTime;
			this.stateTimer = Mathf.Max(0f, this.stateTimer);
		}
		else if (!this.stateEnd && this.stateTimer != -123f)
		{
			this.stateEnd = true;
			this.stateTimer = -123f;
			this.initialStateTime = 0f;
		}
		if (this.stateSetTo != -1)
		{
			this.currentState = this.stateSetTo;
			this.stateStart = true;
			this.settingState = false;
			this.stateEnd = false;
			this.stateSetTo = -1;
		}
		float num = 0.5f;
		base.transform.position = this.startPosition + new Vector3(Mathf.Sin(Time.time * num) * 1f, 0f, Mathf.Cos(Time.time * num) * 1f);
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00020F50 File Offset: 0x0001F150
	private void FootstepLogic()
	{
		Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
		base.transform.LookAt(base.transform.position + normalized);
		Debug.DrawRay(base.transform.position, normalized, Color.green, 0.1f);
		this.previousPosition = base.transform.position;
		float num = 1f;
		if (this.isSprinting)
		{
			num = 1.8f;
		}
		if (Vector3.Distance(this.footStepPosition, this.grounded.position) > 0.5f * num)
		{
			Vector3 a = Vector3.Cross(Vector3.up, base.transform.forward);
			Vector3 a2 = -a;
			Vector3 vector = Vector3.down + (this.rightFoot ? (a * 0.2f) : (a2 * 0.2f));
			vector += base.transform.forward * 0.3f * num;
			this.rightFoot = !this.rightFoot;
			Debug.DrawRay(base.transform.position, vector, Color.red, 0.1f);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, vector * 2f, out raycastHit, 3f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.footStepPosition = this.grounded.position;
				this.footstepParticlesTransform.position = raycastHit.point;
				this.footstepParticleSmoke.Play();
				this.footstepParticlesTransform.transform.LookAt(base.transform.position + normalized);
				this.footstepParticleFoot.Play();
				if (this.isSprinting)
				{
					Materials.Instance.Impulse(raycastHit.point, Vector3.down, Materials.SoundType.Heavy, true, this.material, Materials.HostType.Enemy);
					this.soundFootstepSprint.Play(raycastHit.point, 1f, 1f, 1f, 1f);
				}
				else
				{
					Materials.Instance.Impulse(raycastHit.point, Vector3.down, Materials.SoundType.Medium, true, this.material, Materials.HostType.Enemy);
					this.soundFootstepWalk.Play(raycastHit.point, 1f, 1f, 1f, 1f);
				}
				Quaternion.LookRotation(base.transform.forward);
				Debug.DrawRay(base.transform.position, normalized, Color.blue, 2f);
				ParticleSystem.MainModule main = this.footstepParticleFoot.main;
				main.startRotation3D = true;
				Vector2 to = new Vector2(base.transform.forward.x, base.transform.forward.z);
				float num2 = Vector2.SignedAngle(Vector2.up, to) + 90f;
				float constant = (this.rightFoot ? -90f : 90f) * 0.017453292f;
				float num3 = this.rightFoot ? -90f : 90f;
				num3 += num2;
				num3 *= 0.017453292f;
				main.startRotationX = new ParticleSystem.MinMaxCurve(constant);
				main.startRotationY = new ParticleSystem.MinMaxCurve(num3);
				main.startRotationZ = new ParticleSystem.MinMaxCurve(0f);
			}
		}
	}

	// Token: 0x06000350 RID: 848 RVA: 0x000212AC File Offset: 0x0001F4AC
	private void StateSet(EnemyHiddenOld.State newState)
	{
		if (this.settingState)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient() && this.stateSetTo == -1)
			{
				this.settingState = true;
				this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
				{
					(int)newState
				});
				return;
			}
		}
		else if (this.stateSetTo == -1)
		{
			this.settingState = true;
			this.StateSetRPC((int)newState);
		}
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00021318 File Offset: 0x0001F518
	[PunRPC]
	public void StateSetRPC(int state)
	{
		this.stateSetTo = state;
		this.stateTimer = 0f;
		this.stateEnd = true;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00021333 File Offset: 0x0001F533
	private bool StateIs(EnemyHiddenOld.State state)
	{
		return this.currentState == (int)state;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0002133E File Offset: 0x0001F53E
	private void Sprinting()
	{
		this.sprintingTime = 0.2f;
		this.isSprinting = true;
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00021352 File Offset: 0x0001F552
	private void SprintTick()
	{
		if (this.sprintingTime > 0f)
		{
			this.sprintingTime -= Time.deltaTime;
			return;
		}
		this.isSprinting = false;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002137B File Offset: 0x0001F57B
	private void Breathing()
	{
		this.breathTimer = 0.2f;
		this.isBreathing = true;
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00021390 File Offset: 0x0001F590
	private void BreathTick()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.breathTimer > 0f)
		{
			this.breathTimer -= Time.deltaTime;
		}
		else
		{
			this.isBreathing = false;
		}
		if (this.isBreathing)
		{
			this.breathCycleTimer += Time.deltaTime;
			float num = 3f;
			if (this.breatheIn)
			{
				num = 4.5f;
			}
			if (this.breathCycleTimer > num)
			{
				this.breathCycleTimer = 0f;
				if (this.breatheIn)
				{
					this.BreatheIn();
				}
				else
				{
					this.BreatheOut();
				}
				this.breatheIn = !this.breatheIn;
			}
		}
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00021434 File Offset: 0x0001F634
	private void BreatheIn()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("BreatheInRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.BreatheInRPC();
		}
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00021461 File Offset: 0x0001F661
	[PunRPC]
	public void BreatheInRPC()
	{
		this.soundBreatheIn.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0002148E File Offset: 0x0001F68E
	private void BreatheOut()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("BreatheOutRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.BreatheOutRPC();
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x000214BB File Offset: 0x0001F6BB
	[PunRPC]
	public void BreatheOutRPC()
	{
		this.soundBreatheOut.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.breathParticles.Play();
	}

	// Token: 0x0600035B RID: 859 RVA: 0x000214F3 File Offset: 0x0001F6F3
	private void StateRoam()
	{
		if (!this.StateIs(EnemyHiddenOld.State.Roam))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00021515 File Offset: 0x0001F715
	private void StatePlayerNotice()
	{
		if (!this.StateIs(EnemyHiddenOld.State.PlayerNotice))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00021537 File Offset: 0x0001F737
	private void StateGetPlayer()
	{
		if (!this.StateIs(EnemyHiddenOld.State.GetPlayer))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00021559 File Offset: 0x0001F759
	private void StateGoToTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.GoToTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002157B File Offset: 0x0001F77B
	private void StatePickUpTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.PickUpTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0002159D File Offset: 0x0001F79D
	private void StateFindFarawayPoint()
	{
		if (!this.StateIs(EnemyHiddenOld.State.FindFarawayPoint))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000361 RID: 865 RVA: 0x000215BF File Offset: 0x0001F7BF
	private void StateKidnapTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.KidnapTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x000215E1 File Offset: 0x0001F7E1
	private void StateTauntTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.TauntTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00021603 File Offset: 0x0001F803
	private void StateDropTarget()
	{
		if (!this.StateIs(EnemyHiddenOld.State.DropTarget))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00021625 File Offset: 0x0001F825
	private void StateDespawn()
	{
		if (!this.StateIs(EnemyHiddenOld.State.Despawn))
		{
			return;
		}
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		bool flag = this.stateEnd;
	}

	// Token: 0x040005DC RID: 1500
	private Vector3 startPosition;

	// Token: 0x040005DD RID: 1501
	private Vector3 footStepPosition;

	// Token: 0x040005DE RID: 1502
	public Materials.MaterialTrigger material;

	// Token: 0x040005DF RID: 1503
	public Transform grounded;

	// Token: 0x040005E0 RID: 1504
	public Transform footstepParticlesTransform;

	// Token: 0x040005E1 RID: 1505
	public ParticleSystem footstepParticleSmoke;

	// Token: 0x040005E2 RID: 1506
	private bool rightFoot = true;

	// Token: 0x040005E3 RID: 1507
	private Vector3 previousPosition;

	// Token: 0x040005E4 RID: 1508
	public ParticleSystem footstepParticleFoot;

	// Token: 0x040005E5 RID: 1509
	private bool isSprinting;

	// Token: 0x040005E6 RID: 1510
	private int currentState;

	// Token: 0x040005E7 RID: 1511
	private bool settingState;

	// Token: 0x040005E8 RID: 1512
	private int stateSetTo = -1;

	// Token: 0x040005E9 RID: 1513
	private PhotonView photonView;

	// Token: 0x040005EA RID: 1514
	private float stateTimer;

	// Token: 0x040005EB RID: 1515
	private bool stateEnd;

	// Token: 0x040005EC RID: 1516
	private bool stateStart;

	// Token: 0x040005ED RID: 1517
	private float initialStateTime;

	// Token: 0x040005EE RID: 1518
	private float sprintingTime;

	// Token: 0x040005EF RID: 1519
	public ParticleSystem breathParticles;

	// Token: 0x040005F0 RID: 1520
	private float breathTimer;

	// Token: 0x040005F1 RID: 1521
	private bool isBreathing;

	// Token: 0x040005F2 RID: 1522
	private bool breatheIn = true;

	// Token: 0x040005F3 RID: 1523
	private float breathCycleTimer;

	// Token: 0x040005F4 RID: 1524
	public Sound soundBreatheIn;

	// Token: 0x040005F5 RID: 1525
	public Sound soundBreatheOut;

	// Token: 0x040005F6 RID: 1526
	public Sound soundFootstepWalk;

	// Token: 0x040005F7 RID: 1527
	public Sound soundFootstepSprint;

	// Token: 0x020002D8 RID: 728
	private enum State
	{
		// Token: 0x04002450 RID: 9296
		Roam,
		// Token: 0x04002451 RID: 9297
		PlayerNotice,
		// Token: 0x04002452 RID: 9298
		GetPlayer,
		// Token: 0x04002453 RID: 9299
		GoToTarget,
		// Token: 0x04002454 RID: 9300
		PickUpTarget,
		// Token: 0x04002455 RID: 9301
		FindFarawayPoint,
		// Token: 0x04002456 RID: 9302
		KidnapTarget,
		// Token: 0x04002457 RID: 9303
		TauntTarget,
		// Token: 0x04002458 RID: 9304
		DropTarget,
		// Token: 0x04002459 RID: 9305
		Despawn
	}
}
