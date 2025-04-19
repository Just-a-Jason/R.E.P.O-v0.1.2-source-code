using System;
using TMPro;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class HaulUI : SemiUI
{
	// Token: 0x06001265 RID: 4709 RVA: 0x000A18D4 File Offset: 0x0009FAD4
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		HaulUI.instance = this;
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x000A18F0 File Offset: 0x0009FAF0
	protected override void Update()
	{
		base.Update();
		string text2;
		if (SemiFunc.RunIsLevel())
		{
			if (!RoundDirector.instance.extractionPointActive)
			{
				base.Hide();
			}
			int num = RoundDirector.instance.currentHaul;
			int extractionHaulGoal = RoundDirector.instance.extractionHaulGoal;
			num = Mathf.Max(0, num);
			this.currentHaulValue = num;
			string text = "<color=#558B2F>$</color>";
			text2 = string.Concat(new string[]
			{
				"<size=30>",
				text,
				SemiFunc.DollarGetString(num),
				"<color=#616161> <size=45>/</size> </color>",
				text,
				"<u>",
				SemiFunc.DollarGetString(extractionHaulGoal)
			});
			if (this.currentHaulValue > this.prevHaulValue)
			{
				base.SemiUISpringShakeY(10f, 10f, 0.3f);
				base.SemiUISpringScale(0.05f, 5f, 0.2f);
				base.SemiUITextFlashColor(Color.green, 0.2f);
				this.prevHaulValue = this.currentHaulValue;
			}
			if (this.currentHaulValue < this.prevHaulValue)
			{
				base.SemiUISpringShakeY(10f, 10f, 0.3f);
				base.SemiUISpringScale(0.05f, 5f, 0.2f);
				base.SemiUITextFlashColor(Color.red, 0.2f);
				this.prevHaulValue = this.currentHaulValue;
			}
		}
		else
		{
			text2 = SemiFunc.DollarGetString(SemiFunc.StatGetRunCurrency());
			base.Hide();
		}
		this.Text.text = text2;
	}

	// Token: 0x04001F37 RID: 7991
	private TextMeshProUGUI Text;

	// Token: 0x04001F38 RID: 7992
	public static HaulUI instance;

	// Token: 0x04001F39 RID: 7993
	private int prevHaulValue;

	// Token: 0x04001F3A RID: 7994
	private int currentHaulValue;
}
