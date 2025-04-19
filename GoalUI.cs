using System;
using TMPro;
using UnityEngine;

// Token: 0x0200024F RID: 591
public class GoalUI : SemiUI
{
	// Token: 0x06001262 RID: 4706 RVA: 0x000A1829 File Offset: 0x0009FA29
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		GoalUI.instance = this;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x000A1844 File Offset: 0x0009FA44
	protected override void Update()
	{
		base.Update();
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial())
		{
			int extractionPoints = RoundDirector.instance.extractionPoints;
			int extractionPointsCompleted = RoundDirector.instance.extractionPointsCompleted;
			this.Text.text = extractionPointsCompleted.ToString() + "<color=#7D250B> <size=45>/</size> </color><b>" + extractionPoints.ToString();
		}
		else
		{
			base.Hide();
		}
		if (HaulUI.instance.hideTimer > 0f)
		{
			base.SemiUIScoot(new Vector2(0f, 45f));
		}
	}

	// Token: 0x04001F35 RID: 7989
	private TextMeshProUGUI Text;

	// Token: 0x04001F36 RID: 7990
	public static GoalUI instance;
}
