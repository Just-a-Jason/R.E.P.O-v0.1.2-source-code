using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200009C RID: 156
[RequireComponent(typeof(EnemyGrounded))]
public class EnemyJump : MonoBehaviour
{
	// Token: 0x060005F2 RID: 1522 RVA: 0x0003A1C7 File Offset: 0x000383C7
	private void Awake()
	{
		this.enemy.Jump = this;
		this.enemy.HasJump = true;
		if (this.gapJump && !this.gapCheckerActive)
		{
			base.StartCoroutine(this.GapChecker());
			this.gapCheckerActive = true;
		}
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0003A205 File Offset: 0x00038405
	private void Start()
	{
		if (!this.enemy.HasRigidbody)
		{
			Debug.LogError("EnemyJump: No Rigidbody found on " + this.enemy.name);
			this.stuckJump = false;
		}
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x0003A235 File Offset: 0x00038435
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.gapCheckerActive = false;
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0003A244 File Offset: 0x00038444
	private void OnEnable()
	{
		if (this.gapJump && !this.gapCheckerActive)
		{
			base.StartCoroutine(this.GapChecker());
			this.gapCheckerActive = true;
		}
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0003A26A File Offset: 0x0003846A
	public void StuckReset()
	{
		this.stuckJumpImpulse = false;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x0003A273 File Offset: 0x00038473
	public void SurfaceJumpTrigger(Vector3 _direction)
	{
		if (!this.jumping)
		{
			this.surfaceJumpImpulse = true;
			this.surfaceJumpDirection = _direction;
		}
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0003A28B File Offset: 0x0003848B
	public void SurfaceJumpDisable(float _time)
	{
		this.surfaceJumpImpulse = false;
		this.surfaceJumpDisableTimer = _time;
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x0003A29B File Offset: 0x0003849B
	public void StuckTrigger(Vector3 _direction)
	{
		if (!this.jumping)
		{
			this.stuckJumpImpulse = true;
			this.stuckJumpImpulseDirection = _direction;
		}
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0003A2B3 File Offset: 0x000384B3
	public void StuckDisable(float _time)
	{
		this.stuckJumpDisableTimer = _time;
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0003A2BC File Offset: 0x000384BC
	private IEnumerator GapChecker()
	{
		this.gapCheckerActive = true;
		for (;;)
		{
			if (this.enemy.Grounded.grounded && this.enemy.NavMeshAgent.HasPath())
			{
				int num = 8;
				float d = 0.5f;
				float maxDistance = 2f;
				Vector3 forward = this.enemy.Rigidbody.transform.forward;
				forward.y = 0f;
				Vector3 vector = this.enemy.Rigidbody.physGrabObject.centerPoint + forward * d;
				bool flag = false;
				for (int i = 0; i < num; i++)
				{
					if (Physics.Raycast(vector, Vector3.down * 0.25f, maxDistance, SemiFunc.LayerMaskGetVisionObstruct()))
					{
						if (flag)
						{
							this.gapJumpImpulse = true;
						}
					}
					else if (i < 2)
					{
						flag = true;
					}
					vector += forward * d;
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
		yield break;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0003A2CC File Offset: 0x000384CC
	private void FixedUpdate()
	{
		if (!this.jumping)
		{
			this.timeSinceJumped += Time.fixedDeltaTime;
		}
		else
		{
			this.timeSinceJumped = 0f;
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		bool flag = false;
		if (this.enemy.Rigidbody.grabbed || this.enemy.IsStunned() || this.enemy.Rigidbody.teleportedTimer > 0f)
		{
			this.stuckJumpImpulse = false;
			this.gapJumpImpulse = false;
			return;
		}
		float d = this.gapJumpForceUp;
		float d2 = this.gapJumpForceForward;
		if (this.gapJumpOverrideTimer > 0f)
		{
			d = this.gapJumpOverrideUp;
			d2 = this.gapJumpOverrideForward;
			this.gapJumpOverrideTimer -= Time.fixedDeltaTime;
		}
		if (this.gapJumpImpulse && !this.jumping && this.jumpCooldown <= 0f)
		{
			if (this.gapJumpDelayTimer > 0f)
			{
				this.JumpingDelaySet(true);
				this.enemy.NavMeshAgent.Stop(0.1f);
				this.enemy.Rigidbody.OverrideFollowPosition(0.1f, 0f, -1f);
				this.enemy.Rigidbody.OverrideColliderMaterialStunned(0.1f);
				this.gapJumpDelayTimer -= Time.fixedDeltaTime;
			}
			else
			{
				this.enemy.Rigidbody.DisableFollowPosition(0.5f, 10f);
				Vector3 vector = this.enemy.Rigidbody.transform.forward * d2;
				vector.y = 0f;
				vector += Vector3.up * d;
				this.enemy.Rigidbody.JumpImpulse();
				this.enemy.Rigidbody.rb.AddForce(vector, ForceMode.Impulse);
				this.enemy.NavMeshAgent.OverrideAgent(10f, 999f, 0.5f);
				this.gapJumpImpulse = false;
				this.stuckJumpImpulse = false;
				flag = true;
			}
		}
		else
		{
			this.gapJumpDelayTimer = this.gapJumpDelay;
		}
		if (this.enemy.TeleportedTimer > 0f)
		{
			this.StuckDisable(0.5f);
		}
		if (this.stuckJumpDisableTimer > 0f)
		{
			this.stuckJumpDisableTimer -= Time.fixedDeltaTime;
			this.stuckJumpImpulse = false;
		}
		else if (this.stuckJump)
		{
			if (this.cartJumpTimer > 0f && this.enemy.Rigidbody.touchingCartTimer > 0f)
			{
				if (this.cartJumpCooldown > 0f)
				{
					this.cartJumpCooldown -= Time.fixedDeltaTime;
				}
				else
				{
					this.stuckJumpImpulse = true;
					this.cartJumpCooldown = 2f;
				}
			}
			if (this.enemy.StuckCount >= this.stuckJumpCount)
			{
				this.stuckJumpImpulse = true;
				this.enemy.StuckCount = 0;
			}
			if (!flag && this.stuckJumpImpulse && this.enemy.Grounded.grounded && !this.jumping && this.jumpCooldown <= 0f)
			{
				if (this.stuckJumpImpulseDirection == Vector3.zero)
				{
					this.stuckJumpImpulseDirection = this.enemy.transform.position - this.enemy.Rigidbody.transform.position;
				}
				Vector3 vector2 = this.stuckJumpImpulseDirection.normalized * this.stuckJumpForceSide;
				vector2.y = 0f;
				vector2 += Vector3.up * this.stuckJumpForceUp;
				this.stuckJumpImpulseDirection = Vector3.zero;
				this.enemy.Rigidbody.JumpImpulse();
				this.enemy.Rigidbody.rb.AddForce(vector2, ForceMode.Impulse);
				this.stuckJumpImpulse = false;
				flag = true;
			}
		}
		if (this.cartJumpTimer > 0f)
		{
			this.cartJumpTimer -= Time.fixedDeltaTime;
		}
		if (this.surfaceJump)
		{
			if (this.surfaceJumpDisableTimer > 0f)
			{
				this.surfaceJumpDisableTimer -= Time.fixedDeltaTime;
			}
			else if (!flag && this.surfaceJumpImpulse && this.enemy.Grounded.grounded && !this.jumping && this.jumpCooldown <= 0f)
			{
				this.enemy.Rigidbody.DisableFollowPosition(0.2f, 20f);
				this.enemy.NavMeshAgent.Stop(0.3f);
				Vector3 a = this.surfaceJumpDirection * this.surfaceJumpForceSide;
				a.y = 0f;
				this.enemy.Rigidbody.JumpImpulse();
				this.enemy.Rigidbody.rb.AddForce(a + Vector3.up * this.surfaceJumpForceUp, ForceMode.Impulse);
				this.surfaceJumpImpulse = false;
				flag = true;
			}
		}
		if (!this.jumping)
		{
			if (flag)
			{
				this.JumpingDelaySet(false);
				this.JumpingSet(true);
				this.LandDelaySet(false);
				this.enemy.Grounded.GroundedDisable(0.1f);
			}
		}
		else if (this.enemy.Grounded.grounded)
		{
			if (this.warpAgentOnLand && !this.enemy.NavMeshAgent.IsDisabled())
			{
				this.enemy.NavMeshAgent.Warp(this.enemy.Rigidbody.transform.position);
			}
			this.JumpingDelaySet(false);
			this.JumpingSet(false);
			if (this.gapLandDelay > 0f)
			{
				this.LandDelaySet(true);
				this.gapLandDelayTimer = this.gapLandDelay;
			}
			this.jumpCooldown = 0.25f;
		}
		if (this.jumpCooldown > 0f)
		{
			this.jumpCooldown -= Time.fixedDeltaTime;
			this.jumpCooldown = Mathf.Max(this.jumpCooldown, 0f);
			this.enemy.StuckCount = 0;
			this.surfaceJumpImpulse = false;
			this.stuckJumpImpulse = false;
			this.gapJumpImpulse = false;
		}
		if (this.gapLandDelayTimer > 0f)
		{
			this.enemy.NavMeshAgent.Stop(0.1f);
			this.enemy.Rigidbody.OverrideFollowPosition(0.1f, 0f, -1f);
			this.enemy.Rigidbody.OverrideColliderMaterialStunned(0.1f);
			this.gapLandDelayTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0003A958 File Offset: 0x00038B58
	public void JumpingSet(bool _jumping)
	{
		if (_jumping == this.jumping)
		{
			return;
		}
		if (_jumping)
		{
			this.enemy.Grounded.grounded = false;
		}
		this.jumping = _jumping;
		if (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient)
		{
			this.enemy.Rigidbody.photonView.RPC("JumpingSetRPC", RpcTarget.Others, new object[]
			{
				this.jumping
			});
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0003A9C8 File Offset: 0x00038BC8
	public void JumpingDelaySet(bool _jumpingDelay)
	{
		if (this.jumpingDelay == _jumpingDelay)
		{
			return;
		}
		this.jumpingDelay = _jumpingDelay;
		if (SemiFunc.IsMasterClient())
		{
			this.enemy.Rigidbody.photonView.RPC("JumpingDelaySetRPC", RpcTarget.Others, new object[]
			{
				this.jumpingDelay
			});
		}
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x0003AA1C File Offset: 0x00038C1C
	public void LandDelaySet(bool _landDelay)
	{
		if (this.landDelay == _landDelay)
		{
			return;
		}
		this.landDelay = _landDelay;
		if (SemiFunc.IsMasterClient())
		{
			this.enemy.Rigidbody.photonView.RPC("LandDelaySetRPC", RpcTarget.Others, new object[]
			{
				this.landDelay
			});
		}
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x0003AA70 File Offset: 0x00038C70
	public void CartJump(float _time)
	{
		this.cartJumpTimer = _time;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0003AA79 File Offset: 0x00038C79
	public void GapJumpOverride(float _time, float _up, float _forward)
	{
		this.gapJumpOverrideTimer = _time;
		this.gapJumpOverrideUp = _up;
		this.gapJumpOverrideForward = _forward;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x0003AA90 File Offset: 0x00038C90
	[PunRPC]
	private void JumpingSetRPC(bool _jumping)
	{
		this.jumping = _jumping;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x0003AA99 File Offset: 0x00038C99
	[PunRPC]
	private void JumpingDelaySetRPC(bool _jumpingDelay)
	{
		this.jumpingDelay = _jumpingDelay;
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x0003AAA2 File Offset: 0x00038CA2
	[PunRPC]
	private void LandDelaySetRPC(bool _landDelay)
	{
		this.landDelay = _landDelay;
	}

	// Token: 0x040009DE RID: 2526
	public Enemy enemy;

	// Token: 0x040009DF RID: 2527
	internal bool jumping;

	// Token: 0x040009E0 RID: 2528
	internal bool jumpingDelay;

	// Token: 0x040009E1 RID: 2529
	internal bool landDelay;

	// Token: 0x040009E2 RID: 2530
	internal float jumpCooldown;

	// Token: 0x040009E3 RID: 2531
	internal float timeSinceJumped;

	// Token: 0x040009E4 RID: 2532
	[Space]
	public bool warpAgentOnLand;

	// Token: 0x040009E5 RID: 2533
	[Space]
	public bool surfaceJump = true;

	// Token: 0x040009E6 RID: 2534
	public float surfaceJumpForceUp = 5f;

	// Token: 0x040009E7 RID: 2535
	public float surfaceJumpForceSide = 2f;

	// Token: 0x040009E8 RID: 2536
	private bool surfaceJumpImpulse;

	// Token: 0x040009E9 RID: 2537
	private Vector3 surfaceJumpDirection;

	// Token: 0x040009EA RID: 2538
	private float surfaceJumpDisableTimer;

	// Token: 0x040009EB RID: 2539
	[Space]
	public bool stuckJump;

	// Token: 0x040009EC RID: 2540
	private float stuckJumpDisableTimer;

	// Token: 0x040009ED RID: 2541
	private float cartJumpTimer;

	// Token: 0x040009EE RID: 2542
	private float cartJumpCooldown;

	// Token: 0x040009EF RID: 2543
	public int stuckJumpCount = 5;

	// Token: 0x040009F0 RID: 2544
	public float stuckJumpForceUp = 5f;

	// Token: 0x040009F1 RID: 2545
	public float stuckJumpForceSide = 2f;

	// Token: 0x040009F2 RID: 2546
	private bool stuckJumpImpulse;

	// Token: 0x040009F3 RID: 2547
	private Vector3 stuckJumpImpulseDirection;

	// Token: 0x040009F4 RID: 2548
	[Space]
	public bool gapJump;

	// Token: 0x040009F5 RID: 2549
	public float gapJumpForceUp = 5f;

	// Token: 0x040009F6 RID: 2550
	public float gapJumpForceForward = 5f;

	// Token: 0x040009F7 RID: 2551
	internal bool gapJumpImpulse;

	// Token: 0x040009F8 RID: 2552
	private float gapJumpOverrideTimer;

	// Token: 0x040009F9 RID: 2553
	private float gapJumpOverrideUp;

	// Token: 0x040009FA RID: 2554
	private float gapJumpOverrideForward;

	// Token: 0x040009FB RID: 2555
	public float gapJumpDelay;

	// Token: 0x040009FC RID: 2556
	private float gapJumpDelayTimer;

	// Token: 0x040009FD RID: 2557
	public float gapLandDelay;

	// Token: 0x040009FE RID: 2558
	private float gapLandDelayTimer;

	// Token: 0x040009FF RID: 2559
	private bool gapCheckerActive;
}
