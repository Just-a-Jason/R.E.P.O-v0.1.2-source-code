using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000281 RID: 641
public class ToyMonkeyTrap : Trap
{
	// Token: 0x060013D2 RID: 5074 RVA: 0x000ACDF8 File Offset: 0x000AAFF8
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x000ACE0C File Offset: 0x000AB00C
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.ToyMonkeyTrapActivated();
		}
		this.mechanicalLoop.PlayLoop(this.trapPlaying, 0.8f, 0.8f, 1f);
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			this.trapPlaying = true;
			if (this.armRotationLerp < 1f)
			{
				this.armRotationLerp += Time.deltaTime * 3f;
			}
			if (this.armRotationLerp >= 1f)
			{
				this.armRotationLerp = 0f;
				Vector3 a = Vector3.Slerp(Vector3.up, base.transform.right, 0.25f);
				this.cymbal.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				Vector3 normalized = Random.insideUnitSphere.normalized;
				this.rb.AddForce(a * 1.3f, ForceMode.Impulse);
				this.rb.AddTorque(base.transform.up * Random.Range(-0.25f, 0.25f), ForceMode.Impulse);
			}
			float num = Mathf.Lerp(-15f, 40f, this.armAnimationCurve.Evaluate(this.armRotationLerp));
			this.leftArm.localEulerAngles = new Vector3(0f, num, 0f);
			this.rightArm.localEulerAngles = new Vector3(0f, -num, 0f);
			if (this.headRotationXLerp < 1f)
			{
				this.headRotationXLerp += Time.deltaTime * this.headRotationSpeed;
			}
			if (this.headRotationXLerp >= 1f)
			{
				this.headRotationXLerp = 0f;
			}
			float x = Mathf.Lerp(-15f, 15f, this.headAnimationCurve.Evaluate(this.headRotationXLerp));
			if (this.headRotationZLerp < 1f)
			{
				this.headRotationZLerp += Time.deltaTime * this.headRotationSpeed;
			}
			if (this.headRotationZLerp >= 1f)
			{
				this.headRotationZLerp = 0f;
			}
			float z = Mathf.Lerp(-15f, 15f, this.headAnimationCurve.Evaluate(this.headRotationZLerp));
			this.head.localEulerAngles = new Vector3(x, 0f, z);
		}
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x000AD06C File Offset: 0x000AB26C
	private void FixedUpdate()
	{
		if (this.trapActive && this.isLocal)
		{
			Vector3 normalized = Random.insideUnitSphere.normalized;
			if (this.physGrabObject.playerGrabbing.Count == 0)
			{
				if (this.spinLerp < 1f)
				{
					this.spinLerp += Time.deltaTime;
				}
				float d = Mathf.Lerp(0f, 0.5f, this.spinLerp);
				this.rb.AddTorque(base.transform.right * d + normalized * 0.05f, ForceMode.Force);
				return;
			}
			this.spinLerp = 0f;
			this.rb.AddTorque(normalized * 0.5f, ForceMode.Force);
		}
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x000AD134 File Offset: 0x000AB334
	public void ToyMonkeyTrapStop()
	{
		this.trapActive = false;
		this.trapPlaying = false;
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x000AD144 File Offset: 0x000AB344
	public void ToyMonkeyTrapActivated()
	{
		if (!this.trapTriggered)
		{
			this.toyMonkeyTimer.Invoke();
			this.trapTriggered = true;
			this.trapActive = true;
		}
	}

	// Token: 0x040021DB RID: 8667
	public UnityEvent toyMonkeyTimer;

	// Token: 0x040021DC RID: 8668
	[Space]
	[Header("Components")]
	public Transform head;

	// Token: 0x040021DD RID: 8669
	public Transform leftArm;

	// Token: 0x040021DE RID: 8670
	public Transform rightArm;

	// Token: 0x040021DF RID: 8671
	[Space]
	[Header("Sounds")]
	public Sound cymbal;

	// Token: 0x040021E0 RID: 8672
	public Sound mechanicalLoop;

	// Token: 0x040021E1 RID: 8673
	[Space]
	[Header("Animation")]
	public AnimationCurve armAnimationCurve;

	// Token: 0x040021E2 RID: 8674
	public AnimationCurve headAnimationCurve;

	// Token: 0x040021E3 RID: 8675
	private float armRotationLerp = 0.5f;

	// Token: 0x040021E4 RID: 8676
	private float headRotationXLerp;

	// Token: 0x040021E5 RID: 8677
	private float headRotationZLerp = 0.25f;

	// Token: 0x040021E6 RID: 8678
	private float headRotationSpeed = 4f;

	// Token: 0x040021E7 RID: 8679
	private float spinLerp;

	// Token: 0x040021E8 RID: 8680
	[Space]
	private Rigidbody rb;

	// Token: 0x040021E9 RID: 8681
	private bool trapPlaying;
}
