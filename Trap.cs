using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class Trap : MonoBehaviour
{
	// Token: 0x060011F3 RID: 4595 RVA: 0x0009F246 File Offset: 0x0009D446
	protected virtual void Start()
	{
		this.enemyInvestigateTimer = this.enemyInvestigateTimerMax;
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.isLocal = true;
		}
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x0009F288 File Offset: 0x0009D488
	protected virtual void Update()
	{
		if (this.isLocal)
		{
			if (this.enemyInvestigate)
			{
				if (!this.enemyInvestigatePrev)
				{
					this.enemyInvestigateTimer = this.enemyInvestigateTimerMax;
				}
				this.enemyInvestigateTimer += Time.deltaTime;
				if (this.enemyInvestigateTimer > this.enemyInvestigateTimerMax)
				{
					EnemyDirector.instance.SetInvestigate(base.transform.position, this.enemyInvestigateRange);
					this.enemyInvestigateTimer = 0f;
				}
			}
			this.enemyInvestigatePrev = this.enemyInvestigate;
			this.enemyInvestigate = false;
			if (this.triggerOnTimer)
			{
				if (this.physGrabObject.grabbed)
				{
					if (Application.isEditor && (!GameManager.Multiplayer() || GameManager.instance.localTest) && Input.GetKeyDown(KeyCode.B))
					{
						this.TrapActivateSync();
					}
					if (this.trapActivateTimer > 0f)
					{
						this.trapActivateTimer -= Time.deltaTime;
						if (this.trapActivateTimer <= 0f)
						{
							this.trapActivateTimer = Random.Range(5f, 15f);
							if (SemiFunc.ValuableTrapActivatedDiceRoll((int)this.trapActivateRarityLevel))
							{
								this.TrapActivateSync();
								return;
							}
						}
					}
				}
				else
				{
					this.trapActivateTimer = Random.Range(0f, 15f);
				}
			}
		}
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0009F3C0 File Offset: 0x0009D5C0
	private void TrapActivateSync()
	{
		if (this.trapTriggered)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("TrapActivateSyncRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.TrapActivateSyncRPC();
		}
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0009F3F6 File Offset: 0x0009D5F6
	[PunRPC]
	public void TrapActivateSyncRPC()
	{
		if (this.physGrabObject.grabbedLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		this.trapStart = true;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x0009F418 File Offset: 0x0009D618
	public void TrapStart()
	{
		if (this.trapTriggered)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.ValuableTrapActivatedDiceRoll((int)this.trapActivateRarityLevel))
			{
				this.photonView.RPC("TrapStartRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else if (SemiFunc.ValuableTrapActivatedDiceRoll((int)this.trapActivateRarityLevel))
		{
			this.TrapStartRPC();
		}
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0009F473 File Offset: 0x0009D673
	[PunRPC]
	public void TrapStartRPC()
	{
		if (this.physGrabObject.grabbedLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		this.trapStart = true;
	}

	// Token: 0x04001E41 RID: 7745
	protected PhotonView photonView;

	// Token: 0x04001E42 RID: 7746
	[HideInInspector]
	public bool enemyInvestigate;

	// Token: 0x04001E43 RID: 7747
	private bool enemyInvestigatePrev;

	// Token: 0x04001E44 RID: 7748
	protected float enemyInvestigateRange = 35f;

	// Token: 0x04001E45 RID: 7749
	private float enemyInvestigateTimer = 1f;

	// Token: 0x04001E46 RID: 7750
	private float enemyInvestigateTimerMax = 1f;

	// Token: 0x04001E47 RID: 7751
	[HideInInspector]
	public bool isLocal;

	// Token: 0x04001E48 RID: 7752
	[HideInInspector]
	public bool trapTriggered;

	// Token: 0x04001E49 RID: 7753
	[HideInInspector]
	public bool trapActive;

	// Token: 0x04001E4A RID: 7754
	[HideInInspector]
	public bool trapStart;

	// Token: 0x04001E4B RID: 7755
	private float trapActivateTimer = 10f;

	// Token: 0x04001E4C RID: 7756
	protected PhysGrabObject physGrabObject;

	// Token: 0x04001E4D RID: 7757
	public bool triggerOnTimer;

	// Token: 0x04001E4E RID: 7758
	public Trap.TrapActivateRarityLevel trapActivateRarityLevel;

	// Token: 0x020003AC RID: 940
	public enum TrapActivateRarityLevel
	{
		// Token: 0x04002886 RID: 10374
		no_rarity,
		// Token: 0x04002887 RID: 10375
		level1,
		// Token: 0x04002888 RID: 10376
		level2,
		// Token: 0x04002889 RID: 10377
		level3
	}
}
