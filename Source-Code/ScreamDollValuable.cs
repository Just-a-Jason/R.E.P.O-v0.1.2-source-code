using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class ScreamDollValuable : MonoBehaviour
{
	// Token: 0x060013C0 RID: 5056 RVA: 0x000AC9E4 File Offset: 0x000AABE4
	private void StateActive()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loopPlaying = true;
		this.animator.SetBool("active", true);
		this.animator.enabled = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (Random.Range(0, 100) < 7)
			{
				this.rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse);
				this.rb.AddTorque(Random.insideUnitSphere * 7f, ForceMode.Impulse);
			}
			Quaternion turnX = Quaternion.Euler(0f, 180f, 0f);
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
			if (!this.physGrabObject.grabbed)
			{
				this.SetState(ScreamDollValuable.States.Idle);
			}
		}
		if (this.physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverridePullDistanceIncrement(-1f * Time.fixedDeltaTime);
		}
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x000ACB38 File Offset: 0x000AAD38
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.loopPlaying = false;
		this.animator.SetBool("active", false);
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.physGrabObject.grabbed)
		{
			this.SetState(ScreamDollValuable.States.Active);
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x000ACB87 File Offset: 0x000AAD87
	[PunRPC]
	public void SetStateRPC(ScreamDollValuable.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x000ACB97 File Offset: 0x000AAD97
	private void SetState(ScreamDollValuable.States state)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.SetStateRPC(state);
			return;
		}
		this.photonView.RPC("SetStateRPC", RpcTarget.All, new object[]
		{
			state
		});
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x000ACBD0 File Offset: 0x000AADD0
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x000ACC02 File Offset: 0x000AAE02
	private void Update()
	{
		this.soundScreamLoop.PlayLoop(this.loopPlaying, 5f, 5f, 1f);
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x000ACC24 File Offset: 0x000AAE24
	private void FixedUpdate()
	{
		ScreamDollValuable.States states = this.currentState;
		if (states != ScreamDollValuable.States.Idle)
		{
			if (states == ScreamDollValuable.States.Active)
			{
				this.StateActive();
				return;
			}
		}
		else
		{
			this.StateIdle();
		}
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x000ACC4C File Offset: 0x000AAE4C
	public void EnemyInvestigate()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			EnemyDirector.instance.SetInvestigate(base.transform.position, 20f);
		}
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x000ACC76 File Offset: 0x000AAE76
	public void StopAnimator()
	{
		this.animator.enabled = false;
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x000ACC84 File Offset: 0x000AAE84
	public void OnHurtColliderHitEnemy()
	{
		this.physGrabObject.heavyBreakImpulse = true;
	}

	// Token: 0x040021CE RID: 8654
	private Animator animator;

	// Token: 0x040021CF RID: 8655
	private PhysGrabObject physGrabObject;

	// Token: 0x040021D0 RID: 8656
	private Rigidbody rb;

	// Token: 0x040021D1 RID: 8657
	public Sound soundScreamLoop;

	// Token: 0x040021D2 RID: 8658
	private PhotonView photonView;

	// Token: 0x040021D3 RID: 8659
	internal ScreamDollValuable.States currentState;

	// Token: 0x040021D4 RID: 8660
	private bool stateStart;

	// Token: 0x040021D5 RID: 8661
	private bool loopPlaying;

	// Token: 0x020003C1 RID: 961
	public enum States
	{
		// Token: 0x040028D9 RID: 10457
		Idle,
		// Token: 0x040028DA RID: 10458
		Active
	}
}
