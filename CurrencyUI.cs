using System;
using TMPro;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class CurrencyUI : SemiUI
{
	// Token: 0x06001254 RID: 4692 RVA: 0x000A14BA File Offset: 0x0009F6BA
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		CurrencyUI.instance = this;
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x000A14D4 File Offset: 0x0009F6D4
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial())
		{
			base.Hide();
		}
		if (this.showTimer > 0f)
		{
			int value = SemiFunc.StatGetRunCurrency();
			this.currentHaulValue = value;
			if (this.currentHaulValue != this.prevHaulValue)
			{
				Color color = Color.green;
				if (this.currentHaulValue < this.prevHaulValue)
				{
					color = Color.red;
				}
				base.SemiUISpringShakeY(20f, 10f, 0.3f);
				base.SemiUITextFlashColor(color, 0.2f);
				base.SemiUISpringScale(0.4f, 5f, 0.2f);
				this.prevHaulValue = this.currentHaulValue;
			}
			string str = SemiFunc.DollarGetString(value);
			this.Text.text = "$" + str + "K";
		}
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x000A15AC File Offset: 0x0009F7AC
	public void FetchCurrency()
	{
		string str = SemiFunc.DollarGetString(SemiFunc.StatGetRunCurrency());
		this.Text.text = "$" + str + "K";
	}

	// Token: 0x04001F23 RID: 7971
	private TextMeshProUGUI Text;

	// Token: 0x04001F24 RID: 7972
	public static CurrencyUI instance;

	// Token: 0x04001F25 RID: 7973
	private int prevHaulValue;

	// Token: 0x04001F26 RID: 7974
	private int currentHaulValue;
}
