using System;
using TMPro;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class ShopCostUI : SemiUI
{
	// Token: 0x060012B8 RID: 4792 RVA: 0x000A3F00 File Offset: 0x000A2100
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ShopCostUI.instance = this;
		this.originalColor = this.Text.color;
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x000A3F2C File Offset: 0x000A212C
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			int num = SemiFunc.ShopGetTotalCost();
			string str = SemiFunc.DollarGetString(num);
			if (num > 0)
			{
				this.Text.text = "-$" + str + "K";
				this.Text.color = this.originalColor;
			}
			else
			{
				base.Hide();
			}
			this.currentValue = num;
			if (this.currentValue != this.prevValue)
			{
				Color color = Color.white;
				if (this.currentValue > this.prevValue)
				{
					color = Color.red;
				}
				base.SemiUISpringShakeY(20f, 10f, 0.3f);
				base.SemiUITextFlashColor(color, 0.2f);
				base.SemiUISpringScale(0.4f, 5f, 0.2f);
				this.prevValue = this.currentValue;
			}
		}
		if (!SemiFunc.RunIsShop())
		{
			base.Hide();
		}
		if (this.showTimer > 0f && SemiFunc.RunIsLevel())
		{
			this.Text.text = "+$" + this.animatedValue.ToString() + "K";
			this.Text.color = Color.green;
		}
	}

	// Token: 0x04001FB3 RID: 8115
	private TextMeshProUGUI Text;

	// Token: 0x04001FB4 RID: 8116
	public static ShopCostUI instance;

	// Token: 0x04001FB5 RID: 8117
	public int animatedValue;

	// Token: 0x04001FB6 RID: 8118
	private Color originalColor;

	// Token: 0x04001FB7 RID: 8119
	private int currentValue;

	// Token: 0x04001FB8 RID: 8120
	private int prevValue;
}
