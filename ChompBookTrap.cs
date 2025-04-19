using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class ChompBookTrap : Trap
{
	// Token: 0x060013E9 RID: 5097 RVA: 0x000AD80E File Offset: 0x000ABA0E
	protected override void Start()
	{
		base.Start();
		this.initialBookRotation = this.closedBookTop.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x000AD844 File Offset: 0x000ABA44
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.trapActive)
		{
			this.physGrabObject.OverrideIndestructible(0.1f);
			this.enemyInvestigate = true;
		}
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x000AD87C File Offset: 0x000ABA7C
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.trapActive && this.targetTransform)
		{
			this.playerDirection = (this.targetTransform.position - this.physGrabObject.midPoint).normalized;
			Quaternion b = Quaternion.LookRotation(this.targetTransform.position - this.physGrabObject.midPoint);
			this.lookRotation = Quaternion.Slerp(this.lookRotation, b, Time.deltaTime * 5f);
			Vector3 vector = SemiFunc.PhysFollowRotation(base.transform, this.lookRotation, this.rb, 0.3f);
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				vector *= 0.25f;
			}
			this.rb.AddTorque(vector, ForceMode.Impulse);
			if (this.attackedTimer <= 0f)
			{
				Vector3 a = SemiFunc.PhysFollowPosition(base.transform.position, this.targetTransform.position, this.rb.velocity, 1.5f);
				this.rb.AddForce(a * 10f * Time.fixedDeltaTime, ForceMode.Impulse);
			}
			else
			{
				this.attackedTimer -= Time.fixedDeltaTime;
			}
			this.physGrabObject.OverrideZeroGravity(0.1f);
		}
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x000AD9DC File Offset: 0x000ABBDC
	public void Attack()
	{
		if (!this.isLocal)
		{
			return;
		}
		this.targetTransform = SemiFunc.PlayerGetNearestTransformWithinRange(10f, this.physGrabObject.centerPoint, true, LayerMask.GetMask(new string[]
		{
			"Default"
		}));
		if (this.targetTransform)
		{
			this.attackedTimer = 0.5f;
			this.rb.AddForce(this.playerDirection * 2f, ForceMode.Impulse);
		}
		else
		{
			this.rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddForce(normalized * 3f, ForceMode.Impulse);
			this.rb.AddTorque(normalized * 1f, ForceMode.Impulse);
		}
		this.biteCount++;
		if (this.biteCount >= this.biteAmount)
		{
			this.TrapStop();
		}
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x000ADAD5 File Offset: 0x000ABCD5
	public void ChompSound()
	{
		this.chomp.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x000ADB02 File Offset: 0x000ABD02
	public void StopAnimation()
	{
		if (this.trapStopped)
		{
			this.animator.enabled = false;
		}
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x000ADB18 File Offset: 0x000ABD18
	private void TrapStopLogic()
	{
		this.trapActive = false;
		this.trapStopped = true;
		this.DeparentAndDestroy(this.lockParticle);
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x000ADB34 File Offset: 0x000ABD34
	public void TrapStop()
	{
		if (!GameManager.Multiplayer())
		{
			this.TrapStopRPC();
			return;
		}
		this.photonView.RPC("TrapStopRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x000ADB5A File Offset: 0x000ABD5A
	[PunRPC]
	private void TrapStopRPC()
	{
		this.TrapStopLogic();
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x000ADB64 File Offset: 0x000ABD64
	private void DeparentAndDestroy(ParticleSystem particleSystem)
	{
		if (particleSystem && particleSystem.isPlaying)
		{
			particleSystem.gameObject.transform.parent = null;
			particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
			particleSystem.Stop(false);
		}
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x000ADBA8 File Offset: 0x000ABDA8
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing.ToList<PhysGrabber>())
			{
				if (!SemiFunc.IsMultiplayer())
				{
					physGrabber.ReleaseObjectRPC(true, 1f);
				}
				else
				{
					physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
					{
						false,
						1f
					});
				}
			}
			this.trapActive = true;
			this.trapTriggered = true;
			this.biteBookTop.SetActive(true);
			this.biteBookBot.SetActive(true);
			this.closedBookTop.SetActive(false);
			this.closedBookBot.SetActive(false);
			this.chainLock.SetActive(false);
			this.lockBreak.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
			this.lockParticle.Play(false);
			this.animator.enabled = true;
		}
	}

	// Token: 0x040021FF RID: 8703
	private Animator animator;

	// Token: 0x04002200 RID: 8704
	[Space]
	[Header("Book Components")]
	public GameObject closedBookTop;

	// Token: 0x04002201 RID: 8705
	public GameObject closedBookBot;

	// Token: 0x04002202 RID: 8706
	public GameObject chainLock;

	// Token: 0x04002203 RID: 8707
	public GameObject biteBookTop;

	// Token: 0x04002204 RID: 8708
	public GameObject biteBookBot;

	// Token: 0x04002205 RID: 8709
	[Space]
	[Header("Sounds")]
	public Sound chomp;

	// Token: 0x04002206 RID: 8710
	public Sound lockBreak;

	// Token: 0x04002207 RID: 8711
	[Space]
	private Quaternion initialBookRotation;

	// Token: 0x04002208 RID: 8712
	private Rigidbody rb;

	// Token: 0x04002209 RID: 8713
	public ParticleSystem lockParticle;

	// Token: 0x0400220A RID: 8714
	private Transform targetTransform;

	// Token: 0x0400220B RID: 8715
	private Vector3 playerDirection;

	// Token: 0x0400220C RID: 8716
	public int biteAmount;

	// Token: 0x0400220D RID: 8717
	private int biteCount;

	// Token: 0x0400220E RID: 8718
	private Quaternion lookRotation;

	// Token: 0x0400220F RID: 8719
	private float attackedTimer;

	// Token: 0x04002210 RID: 8720
	private bool trapStopped;
}
