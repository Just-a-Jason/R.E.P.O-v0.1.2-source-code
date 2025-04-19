using System;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class AnimScaleFlicker : MonoBehaviour
{
	// Token: 0x0600113A RID: 4410 RVA: 0x00099C34 File Offset: 0x00097E34
	private void Start()
	{
		this.xInit = base.transform.localScale.x;
		this.yInit = base.transform.localScale.y;
		this.zInit = base.transform.localScale.z;
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x00099C84 File Offset: 0x00097E84
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.xOld, this.xNew, this.animCurve.Evaluate(this.xLerp));
		this.xLerp += this.xSpeed * this.speedMult * Time.deltaTime;
		if (this.xLerp >= 1f)
		{
			this.xOld = this.xNew;
			this.xNew = Random.Range(this.xAmountMin, this.xAmountMax);
			this.xSpeed = Random.Range(this.xSpeedMin, this.xSpeedMax);
			this.xLerp = 0f;
		}
		float num2 = Mathf.LerpUnclamped(this.yOld, this.yNew, this.animCurve.Evaluate(this.yLerp));
		this.yLerp += this.ySpeed * this.speedMult * Time.deltaTime;
		if (this.yLerp >= 1f)
		{
			this.yOld = this.yNew;
			this.yNew = Random.Range(this.yAmountMin, this.yAmountMax);
			this.ySpeed = Random.Range(this.ySpeedMin, this.ySpeedMax);
			this.yLerp = 0f;
		}
		float num3 = Mathf.LerpUnclamped(this.zOld, this.zNew, this.animCurve.Evaluate(this.zLerp));
		this.zLerp += this.zSpeed * this.speedMult * Time.deltaTime;
		if (this.zLerp >= 1f)
		{
			this.zOld = this.zNew;
			this.zNew = Random.Range(this.zAmountMin, this.zAmountMax);
			this.zSpeed = Random.Range(this.zSpeedMin, this.zSpeedMax);
			this.zLerp = 0f;
		}
		base.transform.localScale = new Vector3(this.xInit + num * this.amountMult, this.yInit + num2 * this.amountMult, this.zInit + num3 * this.amountMult);
	}

	// Token: 0x04001CD0 RID: 7376
	public AnimationCurve animCurve;

	// Token: 0x04001CD1 RID: 7377
	public float amountMult = 1f;

	// Token: 0x04001CD2 RID: 7378
	public float speedMult = 1f;

	// Token: 0x04001CD3 RID: 7379
	[Header("Scale X")]
	public float xAmountMin;

	// Token: 0x04001CD4 RID: 7380
	public float xAmountMax;

	// Token: 0x04001CD5 RID: 7381
	public float xSpeedMin;

	// Token: 0x04001CD6 RID: 7382
	public float xSpeedMax;

	// Token: 0x04001CD7 RID: 7383
	private float xInit;

	// Token: 0x04001CD8 RID: 7384
	private float xOld;

	// Token: 0x04001CD9 RID: 7385
	private float xNew;

	// Token: 0x04001CDA RID: 7386
	private float xSpeed;

	// Token: 0x04001CDB RID: 7387
	private float xLerp = 1f;

	// Token: 0x04001CDC RID: 7388
	[Header("Scale Y")]
	public float yAmountMin;

	// Token: 0x04001CDD RID: 7389
	public float yAmountMax;

	// Token: 0x04001CDE RID: 7390
	public float ySpeedMin;

	// Token: 0x04001CDF RID: 7391
	public float ySpeedMax;

	// Token: 0x04001CE0 RID: 7392
	private float yInit;

	// Token: 0x04001CE1 RID: 7393
	private float yOld;

	// Token: 0x04001CE2 RID: 7394
	private float yNew;

	// Token: 0x04001CE3 RID: 7395
	private float ySpeed;

	// Token: 0x04001CE4 RID: 7396
	private float yLerp = 1f;

	// Token: 0x04001CE5 RID: 7397
	[Header("Scale Z")]
	public float zAmountMin;

	// Token: 0x04001CE6 RID: 7398
	public float zAmountMax;

	// Token: 0x04001CE7 RID: 7399
	public float zSpeedMin;

	// Token: 0x04001CE8 RID: 7400
	public float zSpeedMax;

	// Token: 0x04001CE9 RID: 7401
	private float zInit;

	// Token: 0x04001CEA RID: 7402
	private float zOld;

	// Token: 0x04001CEB RID: 7403
	private float zNew;

	// Token: 0x04001CEC RID: 7404
	private float zSpeed;

	// Token: 0x04001CED RID: 7405
	private float zLerp = 1f;
}
