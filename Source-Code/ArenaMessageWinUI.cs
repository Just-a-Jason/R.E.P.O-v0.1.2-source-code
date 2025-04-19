using System;
using TMPro;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class ArenaMessageWinUI : SemiUI
{
	// Token: 0x06001244 RID: 4676 RVA: 0x000A0DAF File Offset: 0x0009EFAF
	protected override void Start()
	{
		base.Start();
		this.Text = base.GetComponent<TextMeshProUGUI>();
		ArenaMessageWinUI.instance = this;
		this.Text.text = "";
		this.originalGradient = this.Text.colorGradient;
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x000A0DEC File Offset: 0x0009EFEC
	public void ArenaText(string message, bool _kingCrowned = false)
	{
		if (message != this.Text.text)
		{
			this.messageTimer = 0f;
			base.SemiUIResetAllShakeEffects();
		}
		this.messageTimer = 0.1f;
		if (_kingCrowned)
		{
			if (!this.kingObject.activeSelf)
			{
				this.kingObject.SetActive(true);
				this.loserObject.transform.localPosition = new Vector3(0f, 3000f, 0f);
				this.loserObject.SetActive(false);
			}
		}
		else if (!this.loserObject.activeSelf)
		{
			this.loserObject.SetActive(true);
			this.kingObject.transform.localPosition = new Vector3(0f, 3000f, 0f);
			this.kingObject.SetActive(false);
		}
		if (message != this.messagePrev)
		{
			this.Text.text = message;
			base.SemiUISpringShakeY(5f, 5f, 0.3f);
			base.SemiUISpringScale(0.1f, 2.5f, 0.2f);
			this.messagePrev = message;
		}
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x000A0F0D File Offset: 0x0009F10D
	protected override void Update()
	{
		base.Update();
		if (this.messageTimer > 0f)
		{
			this.messageTimer -= Time.deltaTime;
			return;
		}
		this.messagePrev = "prev";
		base.Hide();
	}

	// Token: 0x04001F01 RID: 7937
	private TextMeshProUGUI Text;

	// Token: 0x04001F02 RID: 7938
	public static ArenaMessageWinUI instance;

	// Token: 0x04001F03 RID: 7939
	private string messagePrev = "prev";

	// Token: 0x04001F04 RID: 7940
	private float messageTimer;

	// Token: 0x04001F05 RID: 7941
	private GameObject bigMessageEmojiObject;

	// Token: 0x04001F06 RID: 7942
	private TextMeshProUGUI emojiText;

	// Token: 0x04001F07 RID: 7943
	private VertexGradient originalGradient;

	// Token: 0x04001F08 RID: 7944
	public GameObject kingObject;

	// Token: 0x04001F09 RID: 7945
	public GameObject loserObject;
}
