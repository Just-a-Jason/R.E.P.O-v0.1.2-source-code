using System;
using TMPro;

// Token: 0x0200025D RID: 605
public class SpectateNameUI : SemiUI
{
	// Token: 0x060012BB RID: 4795 RVA: 0x000A4065 File Offset: 0x000A2265
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		SpectateNameUI.instance = this;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x000A407F File Offset: 0x000A227F
	protected override void Update()
	{
		base.Update();
		base.Hide();
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x000A408D File Offset: 0x000A228D
	public void SetName(string name)
	{
		this.spectateName = name;
		this.Text.text = this.spectateName;
	}

	// Token: 0x04001FB9 RID: 8121
	internal TextMeshProUGUI Text;

	// Token: 0x04001FBA RID: 8122
	public static SpectateNameUI instance;

	// Token: 0x04001FBB RID: 8123
	private string spectateName;
}
