using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200027D RID: 637
public class FrogTrap : Trap
{
	// Token: 0x060013B7 RID: 5047 RVA: 0x000AC440 File Offset: 0x000AA640
	protected override void Start()
	{
		base.Start();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.initialFrogRotation = this.Frog.transform.localRotation;
		this.rb = base.GetComponent<Rigidbody>();
		this.physgrabobject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x000AC490 File Offset: 0x000AA690
	protected override void Update()
	{
		base.Update();
		this.CrankLoop.PlayLoop(this.LoopPlaying, 0.8f, 0.8f, 1f);
		if (this.physGrabObject.grabbed)
		{
			if (!this.grabbedPrev)
			{
				this.Jump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
				this.grabbedPrev = true;
				if (this.physGrabObject.grabbedLocal)
				{
					PhysGrabber.instance.OverrideGrabDistance(0.8f);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						Quaternion turnX = Quaternion.Euler(45f, 180f, 0f);
						Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
						Quaternion identity = Quaternion.identity;
						this.physGrabObject.TurnXYZ(turnX, turnY, identity);
					}
				}
			}
			this.everPickedUp = true;
			this.LoopPlaying = false;
			if (this.trapActive)
			{
				this.TrapStop();
			}
		}
		else
		{
			this.grabbedPrev = false;
			if (this.everPickedUp)
			{
				this.trapStart = true;
			}
		}
		if (this.trapStart && !this.impactDetector.inCart)
		{
			this.TrapActivate();
		}
		if (this.trapActive && !this.physGrabObject.grabbed)
		{
			this.enemyInvestigate = true;
			this.LoopPlaying = true;
			if (this.FrogJumpActive)
			{
				this.FrogJumpLerp += this.FrogJumpSpeed * Time.deltaTime;
				if (this.FrogJumpLerp >= 1f)
				{
					this.FrogJumpLerp = 0f;
					this.FrogJumpActive = false;
				}
			}
			this.FrogFeet.transform.localEulerAngles = new Vector3(0f, 0f, this.FrogJumpCurve.Evaluate(this.FrogJumpLerp) * this.FrogJumpIntensity);
			this.FrogCrank.transform.Rotate(0f, 0f, 80f * Time.deltaTime);
			float num = 1f;
			float num2 = 40f;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float z = num * Mathf.Sin(Time.time * num2 + 1.5707964f);
			this.Frog.transform.localRotation = this.initialFrogRotation * Quaternion.Euler(num3, 0f, z);
			this.Frog.transform.localPosition = new Vector3(this.Frog.transform.localPosition.x, this.Frog.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, this.Frog.transform.localPosition.z);
			if (this.frogJumpTimer > 0f)
			{
				this.frogJumpTimer -= Time.deltaTime;
				return;
			}
			if (SemiFunc.IsMultiplayer())
			{
				if (SemiFunc.IsMasterClient())
				{
					this.photonView.RPC("FrogJumpRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
			}
			else
			{
				this.FrogJump();
			}
		}
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x000AC78C File Offset: 0x000AA98C
	public void FrogJump()
	{
		if (this.impactDetector.inCart)
		{
			this.TrapStop();
			return;
		}
		this.frogJumpTimer = Random.Range(0.5f, 0.8f);
		this.enemyInvestigate = true;
		this.Jump.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
		this.FrogJumpActive = true;
		this.FrogJumpLerp = 0f;
		this.enemyInvestigateRange = 15f;
		if (this.isLocal)
		{
			if (Vector3.Dot(this.Frog.transform.up, Vector3.up) > 0.5f)
			{
				this.rb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
				this.rb.AddForce(base.transform.forward * 1.5f, ForceMode.Impulse);
				Vector3 a = Random.insideUnitSphere.normalized * Random.Range(0.05f, 0.1f);
				a.z = 0f;
				a.x = 0f;
				this.rb.AddTorque(a * 0.25f, ForceMode.Impulse);
				return;
			}
			this.rb.AddForce(Vector3.up * 1f, ForceMode.Impulse);
			Vector3 normalized = Random.insideUnitSphere.normalized;
			this.rb.AddTorque(normalized * 0.03f, ForceMode.Impulse);
		}
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x000AC90E File Offset: 0x000AAB0E
	[PunRPC]
	public void FrogJumpRPC()
	{
		this.FrogJump();
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x000AC918 File Offset: 0x000AAB18
	public void TrapStop()
	{
		this.trapActive = false;
		this.trapStart = false;
		this.LoopPlaying = false;
		this.trapTriggered = false;
		this.CrankEnd.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x000AC96C File Offset: 0x000AAB6C
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.CrankStart.Play(this.physgrabobject.centerPoint, 1f, 1f, 1f, 1f);
			this.trapActive = true;
			this.trapTriggered = true;
			this.frogJumpTimer = 0f;
		}
	}

	// Token: 0x040021B9 RID: 8633
	private PhysGrabObject physgrabobject;

	// Token: 0x040021BA RID: 8634
	[Space]
	[Header("Frog Components")]
	public GameObject Frog;

	// Token: 0x040021BB RID: 8635
	public GameObject FrogFeet;

	// Token: 0x040021BC RID: 8636
	public GameObject FrogCrank;

	// Token: 0x040021BD RID: 8637
	[Space]
	[Header("Sounds")]
	public Sound CrankStart;

	// Token: 0x040021BE RID: 8638
	public Sound CrankEnd;

	// Token: 0x040021BF RID: 8639
	public Sound CrankLoop;

	// Token: 0x040021C0 RID: 8640
	public Sound Jump;

	// Token: 0x040021C1 RID: 8641
	[Space]
	public AnimationCurve FrogJumpCurve;

	// Token: 0x040021C2 RID: 8642
	private float FrogJumpLerp;

	// Token: 0x040021C3 RID: 8643
	public float FrogJumpSpeed;

	// Token: 0x040021C4 RID: 8644
	private bool FrogJumpActive;

	// Token: 0x040021C5 RID: 8645
	public float FrogJumpIntensity;

	// Token: 0x040021C6 RID: 8646
	private Quaternion initialFrogRotation;

	// Token: 0x040021C7 RID: 8647
	private Rigidbody rb;

	// Token: 0x040021C8 RID: 8648
	private bool LoopPlaying;

	// Token: 0x040021C9 RID: 8649
	private bool everPickedUp;

	// Token: 0x040021CA RID: 8650
	private float frogJumpTimer;

	// Token: 0x040021CB RID: 8651
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040021CC RID: 8652
	private bool grabbedPrev;
}
