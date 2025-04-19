using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class FlamethrowerValuable : MonoBehaviour
{
	// Token: 0x06001364 RID: 4964 RVA: 0x000AA2EB File Offset: 0x000A84EB
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.triggerMeshInitialEulerAngles = this.triggerMesh.localEulerAngles;
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x000AA318 File Offset: 0x000A8518
	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.fuelCountdownActive)
			{
				this.fuelTimer -= Time.deltaTime;
				if (this.fuelTimer <= 0f)
				{
					this.fuelTimer = 0f;
					this.ReleaseTrigger();
					this.fuelCountdownActive = false;
					this.SetState(FlamethrowerValuable.States.Empty);
				}
			}
			if (this.triggerStuck)
			{
				this.triggerStuckTimer -= Time.deltaTime;
				if (this.triggerStuckTimer <= 0f)
				{
					this.triggerStuckTimer = 0f;
					this.ReleaseTrigger();
					this.triggerStuck = false;
				}
			}
		}
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x000AA3B4 File Offset: 0x000A85B4
	private void GrabTriggerLogic()
	{
		this.SetTriggerMeshPosition(true);
		if (this.currentState == FlamethrowerValuable.States.Empty)
		{
			this.soundFlameEmpty.Play(this.semiFlames.transform.position, 1f, 1f, 1f, 1f);
			this.flameEndSquirt.Play();
			this.flameEndSparks.Play();
			return;
		}
		EnemyDirector.instance.SetInvestigate(base.transform.position, 5f);
		this.semiFlames.FlamesActive(this.semiFlames.transform.position, this.semiFlames.transform.rotation);
		this.fuelCountdownActive = true;
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x000AA464 File Offset: 0x000A8664
	public void GrabTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.GrabTriggerLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("GrabTriggerRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x000AA496 File Offset: 0x000A8696
	[PunRPC]
	private void GrabTriggerRPC()
	{
		this.GrabTriggerLogic();
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x000AA49E File Offset: 0x000A869E
	private void ReleaseTriggerLogic()
	{
		this.SetTriggerMeshPosition(false);
		if (this.currentState == FlamethrowerValuable.States.Empty)
		{
			this.flameEndSparks.Stop();
			return;
		}
		this.semiFlames.FlamesInactive();
		this.fuelCountdownActive = false;
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x000AA4CE File Offset: 0x000A86CE
	public void ReleaseTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ReleaseTriggerLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ReleaseTriggerRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x0600136B RID: 4971 RVA: 0x000AA500 File Offset: 0x000A8700
	[PunRPC]
	private void ReleaseTriggerRPC()
	{
		this.ReleaseTriggerLogic();
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x000AA508 File Offset: 0x000A8708
	[PunRPC]
	public void SetStateRPC(FlamethrowerValuable.States state)
	{
		this.currentState = state;
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x000AA511 File Offset: 0x000A8711
	private void SetState(FlamethrowerValuable.States state)
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

	// Token: 0x0600136E RID: 4974 RVA: 0x000AA54C File Offset: 0x000A874C
	public void SetTriggerMeshPosition(bool pulled)
	{
		if (!this.triggerStuck)
		{
			if (pulled)
			{
				Vector3 localEulerAngles = new Vector3(this.triggerMeshInitialEulerAngles.x, this.triggerMeshInitialEulerAngles.y, this.triggerMeshInitialEulerAngles.z - 40f);
				this.triggerMesh.localEulerAngles = localEulerAngles;
				return;
			}
			this.triggerMesh.localEulerAngles = this.triggerMeshInitialEulerAngles;
		}
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x000AA5B0 File Offset: 0x000A87B0
	public void TriggerStuck()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.triggerStuckTimer = 0.2f;
			this.triggerStuck = true;
			this.GrabTrigger();
		}
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x000AA5D4 File Offset: 0x000A87D4
	public void Explode()
	{
		this.particleScriptExplosion.Spawn(this.Center.position, 0.2f, 10, 20, 1f, false, false, 1f);
	}

	// Token: 0x0400212B RID: 8491
	public SemiZuperFlames semiFlames;

	// Token: 0x0400212C RID: 8492
	public Transform triggerMesh;

	// Token: 0x0400212D RID: 8493
	public Transform Center;

	// Token: 0x0400212E RID: 8494
	private Vector3 triggerMeshInitialEulerAngles;

	// Token: 0x0400212F RID: 8495
	private bool triggerStuck;

	// Token: 0x04002130 RID: 8496
	private float triggerStuckTimer = 0.2f;

	// Token: 0x04002131 RID: 8497
	public Sound soundFlameEmpty;

	// Token: 0x04002132 RID: 8498
	public float fuelTimer;

	// Token: 0x04002133 RID: 8499
	private bool fuelCountdownActive;

	// Token: 0x04002134 RID: 8500
	private PhotonView photonView;

	// Token: 0x04002135 RID: 8501
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x04002136 RID: 8502
	public ParticleSystem flameEndSquirt;

	// Token: 0x04002137 RID: 8503
	public ParticleSystem flameEndSparks;

	// Token: 0x04002138 RID: 8504
	internal FlamethrowerValuable.States currentState;

	// Token: 0x020003BD RID: 957
	public enum States
	{
		// Token: 0x040028CB RID: 10443
		Full,
		// Token: 0x040028CC RID: 10444
		Empty
	}
}
