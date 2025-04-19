using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001F3 RID: 499
public class MenuSlider : MonoBehaviour
{
	// Token: 0x06001087 RID: 4231 RVA: 0x00094DD4 File Offset: 0x00092FD4
	public void Start()
	{
		this.inputPercentSetting = base.GetComponent<MenuInputPercentSetting>();
		this.rectTransform = base.GetComponent<RectTransform>();
		if (this.inputPercentSetting)
		{
			this.inputSetting = true;
		}
		this.menuSetting = base.GetComponent<MenuSetting>();
		if (this.menuSetting)
		{
			this.menuSetting.FetchValues();
			int settingValue = this.menuSetting.settingValue;
			if (this.hasCustomOptions)
			{
				int indexFromCustomValue = this.GetIndexFromCustomValue(this.menuSetting.settingValue);
				this.menuSetting.settingValue = indexFromCustomValue;
				settingValue = this.menuSetting.settingValue;
			}
			this.settingsValue = (float)settingValue / 100f;
			this.setting = this.menuSetting.setting;
			this.elementName = this.menuSetting.settingName;
			this.elementNameText.text = this.elementName;
		}
		this.bigSettingText = base.GetComponentInChildren<MenuBigSettingText>();
		if (this.bigSettingText)
		{
			this.hasBigSettingText = true;
		}
		this.prevSettingString = "";
		this.parentPage = base.GetComponentInParent<MenuPage>();
		if (this.elementNameText)
		{
			this.elementNameText.text = this.elementName;
		}
		this.settingSegments = this.endValue - this.startValue;
		this.menuSelectableElement = base.GetComponent<MenuSelectableElement>();
		if (this.hasCustomOptions)
		{
			this.settingSegments = Mathf.Max(this.customOptions.Count - 1, 1);
			this.startValue = 0;
			this.endValue = this.customOptions.Count - 1;
			this.buttonSegmentJump = 1;
			this.settingsValue = this.settingsValue / (float)this.settingSegments * 100f;
		}
		this.barSizeRectTransform = this.barSize.GetComponent<RectTransform>();
		if (this.hasCustomOptions)
		{
			if (Mathf.Max(this.customOptions.Count - 1, 1) != this.settingSegments)
			{
				Debug.LogWarning("Segment text count is not equal to setting segments count");
			}
			else
			{
				int index = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
				string text = this.customOptions[index].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				this.segmentText.text = text;
			}
		}
		else
		{
			this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
			this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
		}
		this.segmentText.enableAutoSizing = false;
		this.segmentMaskText.enableAutoSizing = false;
		if (!this.hasBar && this.segmentText)
		{
			Object.Destroy(this.segmentText.gameObject);
		}
		this.SetStartPositions();
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x000950A8 File Offset: 0x000932A8
	public int GetIndexFromCustomValue(int value)
	{
		int result = 0;
		for (int i = 0; i < this.customOptions.Count; i++)
		{
			if (this.customOptions[i].customValueInt == value)
			{
				return i;
			}
		}
		return result;
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x000950E4 File Offset: 0x000932E4
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.elementNameText = base.GetComponentInChildren<TextMeshProUGUI>();
		this.elementNameText.text = this.elementName;
		base.gameObject.name = "Slider - " + this.elementName;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x00095134 File Offset: 0x00093334
	public void SetStartPositions()
	{
		if (this.startPositionSetup)
		{
			return;
		}
		this.startPositionSetup = true;
		this.barSizeRectTransform.localPosition = new Vector3(this.barSizeRectTransform.localPosition.x + this.sneakyOffsetBecauseIWasLazy, this.barSizeRectTransform.localPosition.y, this.barSizeRectTransform.localPosition.z);
		this.originalPosition = this.rectTransform.position;
		this.originalPositionBarBG = this.sliderBG.GetComponent<RectTransform>().position;
		this.originalPositionBarSize = this.barSizeRectTransform.transform.position;
		this.originalPosition = new Vector3(this.originalPosition.x, this.originalPosition.y - 1.01f, this.originalPosition.z);
		this.barBGRectTransform = this.sliderBG.GetComponent<RectTransform>();
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00095218 File Offset: 0x00093418
	public string CustomOptionGetCurrentString()
	{
		return this.customOptions[this.currentValue].customOptionText;
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00095230 File Offset: 0x00093430
	public void CustomOptionAdd(string optionText, UnityEvent onOption)
	{
		this.customOptions.Add(new MenuSlider.CustomOption
		{
			customOptionText = optionText,
			onOption = onOption
		});
		this.settingSegments = Mathf.Max(this.customOptions.Count - 1, 1);
		this.startValue = 0;
		this.endValue = this.customOptions.Count - 1;
		this.buttonSegmentJump = 1;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x00095298 File Offset: 0x00093498
	private void Update()
	{
		if (this.hasBigSettingText && this.prevSettingString != this.segmentText.text)
		{
			int index = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
			this.bigSettingText.textMeshPro.text = this.customOptions[index].customOptionText;
			this.prevSettingString = this.segmentText.text;
		}
		if (this.prevCurrentValue != this.currentValue || this.valueChangedImpulse)
		{
			this.valueChangedImpulse = false;
			float num = Mathf.Round(this.settingsValue * 100f);
			if (this.customOptions.Count > 0)
			{
				num = (float)Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
			}
			if (this.menuSetting)
			{
				if (!this.hasCustomOptions)
				{
					DataDirector.instance.SettingValueSet(this.setting, (int)num);
				}
				else if (this.hasCustomValues)
				{
					MenuSlider.CustomOption customOption = this.customOptions[this.currentValue];
					DataDirector.instance.SettingValueSet(this.setting, customOption.customValueInt);
					this.customOptions[this.currentValue].onOption.Invoke();
				}
				else
				{
					DataDirector.instance.SettingValueSet(this.setting, (int)num);
				}
			}
			if (this.inputSetting)
			{
				InputManager.instance.inputPercentSettings[this.inputPercentSetting.setting] = (int)num;
			}
			this.onChange.Invoke();
			this.prevCurrentValue = this.currentValue;
		}
		if (this.extraBarActiveTimer > 0f)
		{
			this.extraBarActiveTimer -= Time.deltaTime;
		}
		else if (this.extraBar.gameObject.activeSelf)
		{
			this.extraBar.gameObject.SetActive(false);
		}
		if (this.hasBar)
		{
			this.settingsBar.localScale = Vector3.Lerp(this.settingsBar.localScale, new Vector3(this.settingsValue, this.settingsBar.localScale.y, this.settingsBar.localScale.z), 20f * Time.deltaTime);
			this.maskRectTransform.sizeDelta = new Vector2(this.barSizeRectTransform.sizeDelta.x * this.settingsValue, this.maskRectTransform.sizeDelta.y);
		}
		Vector3 mousePosition = Input.mousePosition;
		float num2 = (float)(Screen.width / MenuManager.instance.screenUIWidth) * 1.05f;
		float num3 = (float)(Screen.height / MenuManager.instance.screenUIHeight) * 1f;
		mousePosition = new Vector3(mousePosition.x / num2, mousePosition.y / num3, 0f);
		if (SemiFunc.UIMouseHover(this.parentPage, this.barSizeRectTransform, this.menuSelectableElement.menuID, 5f, 5f))
		{
			if (!this.hovering)
			{
				MenuManager.instance.MenuEffectHover(SemiFunc.MenuGetPitchFromYPos(this.rectTransform), -1f);
			}
			this.hovering = true;
			int num4 = 10;
			new Vector3(this.barSizeRectTransform.localPosition.x + this.barSizeRectTransform.sizeDelta.x / 2f - this.sneakyOffsetBecauseIWasLazy, this.barSizeRectTransform.localPosition.y + (float)(num4 / 2), this.barSizeRectTransform.localPosition.z);
			new Vector2(this.barSizeRectTransform.sizeDelta.x + (float)num4, this.barSizeRectTransform.sizeDelta.y + (float)num4);
			SemiFunc.MenuSelectionBoxTargetSet(this.parentPage, this.barSizeRectTransform, new Vector2(-3f, 0f), new Vector2(20f, 10f));
			if (this.hasBar)
			{
				this.PointerLogic(mousePosition);
			}
			else if (Input.GetMouseButtonDown(0))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, this.parentPage, -1f, -1f, false);
				this.OnIncrease();
			}
		}
		else
		{
			this.hovering = false;
			if (this.barPointer.gameObject.activeSelf)
			{
				this.barPointer.localPosition = new Vector3(-999f, this.barPointer.localPosition.y, this.barPointer.localPosition.z);
				this.barPointer.gameObject.SetActive(false);
			}
		}
		if (this.segmentMaskText && this.segmentMaskText.text != this.segmentText.text)
		{
			this.segmentMaskText.text = this.segmentText.text;
		}
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x00095744 File Offset: 0x00093944
	public void ExtraBarSet(float value)
	{
		if (!this.extraBar.gameObject.activeSelf)
		{
			this.extraBar.gameObject.SetActive(true);
		}
		value = Mathf.Clamp(value, 0f, 1f);
		this.extraBar.localScale = new Vector3(value, this.extraBar.localScale.y, this.extraBar.localScale.z);
		this.extraBarActiveTimer = 0.2f;
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x000957C4 File Offset: 0x000939C4
	public void SetBar(float value)
	{
		this.settingsValue = Mathf.Clamp(value, 0f, 1f);
		int num = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
		this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
		if (this.hasCustomOptions)
		{
			this.customValue = this.GetCustomValue(num);
			if (num < this.customOptions.Count)
			{
				string text = this.customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				this.segmentText.text = text;
				return;
			}
		}
		else
		{
			this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
		}
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x000958A8 File Offset: 0x00093AA8
	public int GetCustomValue(int index)
	{
		if (!this.hasCustomOptions)
		{
			return this.customValueNull;
		}
		if (this.customOptions.Count == 0)
		{
			return this.customValueNull;
		}
		if (index >= this.customOptions.Count)
		{
			return this.customValueNull;
		}
		if (index < 0)
		{
			return this.customValueNull;
		}
		if (!this.hasCustomValues)
		{
			return this.customValueNull;
		}
		return this.customOptions[index].customValueInt;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x00095918 File Offset: 0x00093B18
	private void PointerLogic(Vector3 mouseScreenPosition)
	{
		if (!this.barPointer)
		{
			return;
		}
		if (!this.barPointer.gameObject.activeSelf)
		{
			this.barPointer.gameObject.SetActive(true);
		}
		ref Vector2 ptr = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(this.barSizeRectTransform);
		int num = (this.endValue - this.startValue) / this.pointerSegmentJump;
		SemiFunc.UIGetRectTransformPositionOnScreen(this.barSizeRectTransform, true);
		float num2 = Mathf.Clamp01(ptr.x / this.barSizeRectTransform.sizeDelta.x);
		num2 = Mathf.Round(num2 * (float)num) / (float)num;
		float num3 = Mathf.Clamp(this.barSizeRectTransform.localPosition.x + num2 * this.barSizeRectTransform.sizeDelta.x, this.barSizeRectTransform.localPosition.x, this.barSizeRectTransform.localPosition.x + this.barSizeRectTransform.sizeDelta.x);
		this.barPointer.localPosition = new Vector3(num3 - 2f, this.barPointer.localPosition.y, this.barPointer.localPosition.z);
		if (Input.GetMouseButton(0))
		{
			this.prevSettingsValue = this.settingsValue;
			this.settingsValue = num2;
			if (this.prevSettingsValue != this.settingsValue)
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, this.parentPage, -1f, -1f, false);
			}
			int num4 = Mathf.RoundToInt(this.settingsValue * (float)num);
			if (this.hasCustomOptions && num4 < this.customOptions.Count)
			{
				this.segmentText.text = this.customOptions[num4].customOptionText;
			}
			else
			{
				this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
			}
			this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
			if (this.hasCustomOptions)
			{
				this.UpdateSegmentTextAndValue();
				this.customValue = this.GetCustomValue(num4);
			}
		}
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x00095B30 File Offset: 0x00093D30
	public void UpdateSegmentTextAndValue()
	{
		int num = Mathf.RoundToInt(this.settingsValue * (float)this.settingSegments);
		this.currentValue = Mathf.RoundToInt(Mathf.Lerp((float)this.startValue, (float)this.endValue, this.settingsValue));
		if (this.hasCustomOptions)
		{
			this.customValue = this.GetCustomValue(num);
			if (num < this.customOptions.Count)
			{
				string text = this.customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				this.segmentText.text = text;
				return;
			}
		}
		else
		{
			this.segmentText.text = this.stringAtStartOfValue + this.currentValue.ToString() + this.stringAtEndOfValue;
		}
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x00095BFC File Offset: 0x00093DFC
	public void OnIncrease()
	{
		this.valueChangedImpulse = true;
		this.prevSettingsValue = this.settingsValue;
		float num = this.settingsValue;
		this.settingsValue += 1f / (float)this.settingSegments * (float)this.buttonSegmentJump;
		if (this.wrapAround)
		{
			this.settingsValue = ((num == 1f) ? 0f : Mathf.Clamp01(this.settingsValue));
		}
		else
		{
			this.settingsValue = Mathf.Clamp(this.settingsValue, 0f, 1f);
		}
		this.UpdateSegmentTextAndValue();
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x00095C90 File Offset: 0x00093E90
	public void OnDecrease()
	{
		this.valueChangedImpulse = true;
		this.prevSettingsValue = this.settingsValue;
		float num = this.settingsValue;
		this.settingsValue -= 1f / (float)this.settingSegments * (float)this.buttonSegmentJump;
		if (this.wrapAround)
		{
			this.settingsValue = ((this.settingsValue + num < 0f) ? 1f : Mathf.Clamp01(this.settingsValue));
		}
		else
		{
			this.settingsValue = Mathf.Clamp(this.settingsValue, 0f, 1f);
		}
		this.UpdateSegmentTextAndValue();
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x00095D2B File Offset: 0x00093F2B
	public void SetBarScaleInstant()
	{
		this.settingsBar.localScale = new Vector3(this.settingsValue, this.settingsBar.localScale.y, this.settingsBar.localScale.z);
	}

	// Token: 0x04001B96 RID: 7062
	public string elementName = "Element Name";

	// Token: 0x04001B97 RID: 7063
	public TextMeshProUGUI elementNameText;

	// Token: 0x04001B98 RID: 7064
	public Transform sliderBG;

	// Token: 0x04001B99 RID: 7065
	public Transform barSize;

	// Token: 0x04001B9A RID: 7066
	public Transform barPointer;

	// Token: 0x04001B9B RID: 7067
	public RectTransform barSizeRectTransform;

	// Token: 0x04001B9C RID: 7068
	public Transform settingsBar;

	// Token: 0x04001B9D RID: 7069
	public Transform extraBar;

	// Token: 0x04001B9E RID: 7070
	private int settingSegments;

	// Token: 0x04001B9F RID: 7071
	public int startValue;

	// Token: 0x04001BA0 RID: 7072
	public int endValue;

	// Token: 0x04001BA1 RID: 7073
	public string stringAtStartOfValue;

	// Token: 0x04001BA2 RID: 7074
	public string stringAtEndOfValue;

	// Token: 0x04001BA3 RID: 7075
	internal int currentValue;

	// Token: 0x04001BA4 RID: 7076
	internal int prevCurrentValue;

	// Token: 0x04001BA5 RID: 7077
	internal bool valueChangedImpulse;

	// Token: 0x04001BA6 RID: 7078
	public int buttonSegmentJump = 1;

	// Token: 0x04001BA7 RID: 7079
	public int pointerSegmentJump = 1;

	// Token: 0x04001BA8 RID: 7080
	internal float settingsValue = 1f;

	// Token: 0x04001BA9 RID: 7081
	internal float prevSettingsValue = 1f;

	// Token: 0x04001BAA RID: 7082
	public TextMeshProUGUI segmentText;

	// Token: 0x04001BAB RID: 7083
	public TextMeshProUGUI segmentMaskText;

	// Token: 0x04001BAC RID: 7084
	public RectTransform maskRectTransform;

	// Token: 0x04001BAD RID: 7085
	public bool wrapAround;

	// Token: 0x04001BAE RID: 7086
	public bool hasBar = true;

	// Token: 0x04001BAF RID: 7087
	public bool hasCustomOptions;

	// Token: 0x04001BB0 RID: 7088
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001BB1 RID: 7089
	private bool hovering;

	// Token: 0x04001BB2 RID: 7090
	private RectTransform rectTransform;

	// Token: 0x04001BB3 RID: 7091
	private float sneakyOffsetBecauseIWasLazy = 3f;

	// Token: 0x04001BB4 RID: 7092
	private MenuPage parentPage;

	// Token: 0x04001BB5 RID: 7093
	internal MenuSetting menuSetting;

	// Token: 0x04001BB6 RID: 7094
	private DataDirector.Setting setting;

	// Token: 0x04001BB7 RID: 7095
	private bool inputSetting;

	// Token: 0x04001BB8 RID: 7096
	private MenuInputPercentSetting inputPercentSetting;

	// Token: 0x04001BB9 RID: 7097
	private Vector3 originalPosition;

	// Token: 0x04001BBA RID: 7098
	private Vector3 originalPositionBarSize;

	// Token: 0x04001BBB RID: 7099
	private Vector3 originalPositionBarBG;

	// Token: 0x04001BBC RID: 7100
	private RectTransform barBGRectTransform;

	// Token: 0x04001BBD RID: 7101
	private string prevSettingString = "";

	// Token: 0x04001BBE RID: 7102
	private bool hasBigSettingText;

	// Token: 0x04001BBF RID: 7103
	internal MenuBigSettingText bigSettingText;

	// Token: 0x04001BC0 RID: 7104
	private int customValue;

	// Token: 0x04001BC1 RID: 7105
	private int customValueNull = -123456;

	// Token: 0x04001BC2 RID: 7106
	public bool hasCustomValues;

	// Token: 0x04001BC3 RID: 7107
	private bool startPositionSetup;

	// Token: 0x04001BC4 RID: 7108
	[Space]
	public UnityEvent onChange;

	// Token: 0x04001BC5 RID: 7109
	public List<MenuSlider.CustomOption> customOptions;

	// Token: 0x04001BC6 RID: 7110
	private float extraBarActiveTimer;

	// Token: 0x02000398 RID: 920
	[Serializable]
	public class CustomOption
	{
		// Token: 0x0400280F RID: 10255
		[Space(25f)]
		[Header("____ Custom Option ____")]
		public string customOptionText;

		// Token: 0x04002810 RID: 10256
		public UnityEvent onOption;

		// Token: 0x04002811 RID: 10257
		public int customValueInt;
	}
}
