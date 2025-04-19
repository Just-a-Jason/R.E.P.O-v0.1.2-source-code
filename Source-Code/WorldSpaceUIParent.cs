using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class WorldSpaceUIParent : MonoBehaviour
{
	// Token: 0x06001342 RID: 4930 RVA: 0x000A8D21 File Offset: 0x000A6F21
	private void Awake()
	{
		WorldSpaceUIParent.instance = this;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x000A8D38 File Offset: 0x000A6F38
	private void Update()
	{
		float b = 1f;
		if (this.hideTimer > 0f)
		{
			b = 0f;
			this.hideTimer -= Time.deltaTime;
		}
		this.hideAlpha = Mathf.Lerp(this.hideAlpha, b, Time.deltaTime * 20f);
		this.canvasGroup.alpha = this.hideAlpha;
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x000A8DA0 File Offset: 0x000A6FA0
	public void ValueLostCreate(Vector3 _worldPosition, int _value)
	{
		if (PlayerController.instance.isActiveAndEnabled && Vector3.Distance(_worldPosition, PlayerController.instance.transform.position) > 10f)
		{
			return;
		}
		foreach (WorldSpaceUIValueLost worldSpaceUIValueLost in this.valueLostList)
		{
			if (Vector3.Distance(worldSpaceUIValueLost.worldPosition, _worldPosition) < 1f && worldSpaceUIValueLost.timer > 0f)
			{
				worldSpaceUIValueLost.timer = 0f;
				_value += worldSpaceUIValueLost.value;
			}
		}
		WorldSpaceUIValueLost component = Object.Instantiate<GameObject>(this.valueLostPrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<WorldSpaceUIValueLost>();
		component.worldPosition = _worldPosition;
		component.value = _value;
		this.valueLostList.Add(component);
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x000A8E94 File Offset: 0x000A7094
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x000A8EA4 File Offset: 0x000A70A4
	public void TTS(PlayerAvatar _player, string _text, float _time)
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (_player.isDisabled || _player.isLocal)
		{
			return;
		}
		WorldSpaceUITTS component = Object.Instantiate<GameObject>(this.TTSPrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<WorldSpaceUITTS>();
		component.text.text = _text;
		component.playerAvatar = _player;
		component.followTransform = _player.playerAvatarVisuals.TTSTransform;
		component.worldPosition = component.followTransform.position;
		component.followPosition = component.worldPosition;
		component.wordTime = _time;
		component.ttsVoice = _player.voiceChat.ttsVoice;
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x000A8F54 File Offset: 0x000A7154
	public void PlayerName(PlayerAvatar _player)
	{
		if (_player.isLocal || SemiFunc.MenuLevel())
		{
			return;
		}
		WorldSpaceUIPlayerName component = Object.Instantiate<GameObject>(this.playerNamePrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<WorldSpaceUIPlayerName>();
		component.playerAvatar = _player;
		component.text.text = _player.playerName;
		_player.worldSpaceUIPlayerName = component;
	}

	// Token: 0x040020CD RID: 8397
	public static WorldSpaceUIParent instance;

	// Token: 0x040020CE RID: 8398
	private CanvasGroup canvasGroup;

	// Token: 0x040020CF RID: 8399
	[Space]
	public GameObject valueLostPrefab;

	// Token: 0x040020D0 RID: 8400
	public GameObject TTSPrefab;

	// Token: 0x040020D1 RID: 8401
	public GameObject playerNamePrefab;

	// Token: 0x040020D2 RID: 8402
	internal List<WorldSpaceUIValueLost> valueLostList = new List<WorldSpaceUIValueLost>();

	// Token: 0x040020D3 RID: 8403
	private float hideTimer;

	// Token: 0x040020D4 RID: 8404
	internal float hideAlpha = 1f;
}
