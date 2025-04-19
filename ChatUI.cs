using System;
using TMPro;

// Token: 0x0200024A RID: 586
public class ChatUI : SemiUI
{
	// Token: 0x06001251 RID: 4689 RVA: 0x000A149C File Offset: 0x0009F69C
	protected override void Start()
	{
		base.Start();
		ChatUI.instance = this;
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x000A14AA File Offset: 0x0009F6AA
	protected override void Update()
	{
		base.Update();
	}

	// Token: 0x04001F21 RID: 7969
	public static ChatUI instance;

	// Token: 0x04001F22 RID: 7970
	public TextMeshProUGUI chatText;
}
