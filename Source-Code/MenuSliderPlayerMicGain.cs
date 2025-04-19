using System;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class MenuSliderPlayerMicGain : MonoBehaviour
{
	// Token: 0x0600107F RID: 4223 RVA: 0x00094B55 File Offset: 0x00092D55
	private void Start()
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00094B64 File Offset: 0x00092D64
	private void Update()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (!this.playerAvatar.voiceChatFetched)
		{
			return;
		}
		if (this.currentValue != (float)this.menuSlider.currentValue && this.playerAvatar.voiceChatFetched)
		{
			if (!this.fetched)
			{
				this.playerAvatar.voiceChat.voiceGain = GameManager.instance.PlayerMicrophoneSettingGet(this.playerAvatar.steamID);
				this.menuSlider.settingsValue = this.playerAvatar.voiceChat.voiceGain;
				this.menuSlider.currentValue = (int)(this.playerAvatar.voiceChat.voiceGain * 200f);
				this.menuSlider.SetBar(this.menuSlider.settingsValue);
				this.menuSlider.SetBarScaleInstant();
				this.fetched = true;
			}
			this.currentValue = (float)this.menuSlider.currentValue;
			this.playerAvatar.voiceChat.voiceGain = this.currentValue / 200f;
			GameManager.instance.PlayerMicrophoneSettingSet(this.playerAvatar.steamID, this.playerAvatar.voiceChat.voiceGain);
		}
		this.menuSlider.ExtraBarSet(this.playerAvatar.voiceChat.clipLoudnessNoTTS * 5f);
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x00094CB7 File Offset: 0x00092EB7
	public void SliderNameSet(string name)
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
		this.menuSlider.elementName = name;
		this.menuSlider.elementNameText.text = name;
	}

	// Token: 0x04001B91 RID: 7057
	internal MenuSlider menuSlider;

	// Token: 0x04001B92 RID: 7058
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001B93 RID: 7059
	private float currentValue;

	// Token: 0x04001B94 RID: 7060
	private bool fetched;
}
