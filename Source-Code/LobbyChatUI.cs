using System;
using TMPro;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class LobbyChatUI : SemiUI
{
	// Token: 0x0600129A RID: 4762 RVA: 0x000A2A1C File Offset: 0x000A0C1C
	protected override void Start()
	{
		base.Start();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.menuPlayerListed = base.GetComponentInParent<MenuPlayerListed>();
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x000A2A3C File Offset: 0x000A0C3C
	protected override void Update()
	{
		base.Update();
		if (this.isGameplay && (SemiFunc.RunIsLobbyMenu() || (PlayerAvatar.instance && PlayerAvatar.instance.isDisabled)))
		{
			this.uiText.text = "";
			return;
		}
		if (this.spectateName && this.prevPlayerName != this.spectateName.text)
		{
			this.offsetFetched = false;
		}
		if (this.isSpectate)
		{
			base.SemiUIScoot(new Vector2(-200f + this.chatOffsetXPos, 0f));
		}
		if (this.isSpectate && this.spectateName && !this.offsetFetched)
		{
			float num = this.spectateName.preferredWidth;
			if (num > 155f)
			{
				num = 155f;
			}
			this.rectTransform.localPosition = this.spectateName.rectTransform.localPosition + new Vector3(num, 25f, 0f);
			this.chatOffsetXPos = this.rectTransform.localPosition.x;
			this.offsetFetched = true;
			this.prevPlayerName = this.spectateName.text;
		}
		if (!this.ttsVoice)
		{
			if (!this.isGameplay)
			{
				if (this.menuPlayerListed.playerAvatar.voiceChat && this.menuPlayerListed.playerAvatar.voiceChat.TTSinstantiated)
				{
					this.ttsVoice = this.menuPlayerListed.playerAvatar.voiceChat.ttsVoice;
					return;
				}
			}
			else if (PlayerAvatar.instance && PlayerAvatar.instance.voiceChat && PlayerAvatar.instance.voiceChat.ttsVoice)
			{
				this.ttsVoice = PlayerAvatar.instance.voiceChat.ttsVoice;
			}
			return;
		}
		if (this.prevWordTime != this.ttsVoice.currentWordTime)
		{
			base.SemiUITextFlashColor(Color.yellow, 0.2f);
			base.SemiUISpringShakeY(4f, 5f, 0.2f);
			this.prevWordTime = this.ttsVoice.currentWordTime;
			this.uiText.text = this.ttsVoice.voiceText;
		}
		if (this.ttsVoice.isSpeaking)
		{
			this.uiText.text = this.ttsVoice.voiceText;
			return;
		}
		this.uiText.text = "";
	}

	// Token: 0x04001F67 RID: 8039
	private TTSVoice ttsVoice;

	// Token: 0x04001F68 RID: 8040
	private MenuPlayerListed menuPlayerListed;

	// Token: 0x04001F69 RID: 8041
	private float prevWordTime;

	// Token: 0x04001F6A RID: 8042
	public bool isSpectate;

	// Token: 0x04001F6B RID: 8043
	public bool isGameplay;

	// Token: 0x04001F6C RID: 8044
	public TextMeshProUGUI spectateName;

	// Token: 0x04001F6D RID: 8045
	private RectTransform rectTransform;

	// Token: 0x04001F6E RID: 8046
	private float chatOffsetXPos;

	// Token: 0x04001F6F RID: 8047
	private bool offsetFetched;

	// Token: 0x04001F70 RID: 8048
	private string prevPlayerName = "";
}
