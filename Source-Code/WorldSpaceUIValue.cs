using System;
using TMPro;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class WorldSpaceUIValue : WorldSpaceUIChild
{
	// Token: 0x06001350 RID: 4944 RVA: 0x000A9606 File Offset: 0x000A7806
	private void Awake()
	{
		this.positionOffset = new Vector3(0f, -0.05f, 0f);
		WorldSpaceUIValue.instance = this;
		this.scale = base.transform.localScale;
		this.text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x000A9648 File Offset: 0x000A7848
	protected override void Update()
	{
		base.Update();
		this.worldPosition = Vector3.Lerp(this.worldPosition, this.newWorldPosition, 50f * Time.deltaTime);
		if (this.currentPhysGrabObject)
		{
			this.newWorldPosition = this.currentPhysGrabObject.centerPoint + this.offset;
		}
		if (this.showTimer > 0f)
		{
			this.showTimer -= Time.deltaTime;
			this.curveLerp += 10f * Time.deltaTime;
			this.curveLerp = Mathf.Clamp01(this.curveLerp);
			base.transform.localScale = this.scale * this.curveIntro.Evaluate(this.curveLerp);
			return;
		}
		this.curveLerp -= 10f * Time.deltaTime;
		this.curveLerp = Mathf.Clamp01(this.curveLerp);
		base.transform.localScale = this.scale * this.curveOutro.Evaluate(this.curveLerp);
		if (this.curveLerp <= 0f)
		{
			this.currentPhysGrabObject = null;
		}
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x000A977C File Offset: 0x000A797C
	public void Show(PhysGrabObject _grabObject, int _value, bool _cost, Vector3 _offset)
	{
		if (!this.currentPhysGrabObject || this.currentPhysGrabObject == _grabObject)
		{
			this.value = _value;
			if (_cost)
			{
				this.text.text = "-$" + SemiFunc.DollarGetString(this.value) + "K";
				this.text.fontSize = this.textSizeCost;
			}
			else
			{
				this.text.text = "$" + SemiFunc.DollarGetString(this.value);
				this.text.fontSize = this.textSizeValue;
			}
			this.showTimer = 0.1f;
			if (!this.currentPhysGrabObject)
			{
				this.offset = _offset;
				this.currentPhysGrabObject = _grabObject;
				this.newWorldPosition = this.currentPhysGrabObject.centerPoint + this.offset - Vector3.up * 0.1f;
				this.worldPosition = this.newWorldPosition;
				if (_cost)
				{
					this.text.color = this.colorCost;
					return;
				}
				this.text.color = this.colorValue;
			}
		}
	}

	// Token: 0x040020EC RID: 8428
	public static WorldSpaceUIValue instance;

	// Token: 0x040020ED RID: 8429
	private float showTimer;

	// Token: 0x040020EE RID: 8430
	private Vector3 scale;

	// Token: 0x040020EF RID: 8431
	private int value;

	// Token: 0x040020F0 RID: 8432
	private TextMeshProUGUI text;

	// Token: 0x040020F1 RID: 8433
	private Vector3 newWorldPosition;

	// Token: 0x040020F2 RID: 8434
	private Vector3 offset;

	// Token: 0x040020F3 RID: 8435
	private PhysGrabObject currentPhysGrabObject;

	// Token: 0x040020F4 RID: 8436
	public AnimationCurve curveIntro;

	// Token: 0x040020F5 RID: 8437
	public AnimationCurve curveOutro;

	// Token: 0x040020F6 RID: 8438
	private float curveLerp;

	// Token: 0x040020F7 RID: 8439
	[Space]
	public Color colorValue;

	// Token: 0x040020F8 RID: 8440
	public Color colorCost;

	// Token: 0x040020F9 RID: 8441
	[Space]
	public float textSizeValue;

	// Token: 0x040020FA RID: 8442
	public float textSizeCost;
}
