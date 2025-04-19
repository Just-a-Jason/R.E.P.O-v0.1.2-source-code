using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001DE RID: 478
public class MenuButton : MonoBehaviour
{
	// Token: 0x06000FEF RID: 4079 RVA: 0x00091354 File Offset: 0x0008F554
	private void Awake()
	{
		if (!this.customColors)
		{
			this.colorNormal = Color.gray;
			this.colorHover = Color.white;
			this.colorClick = AssetManager.instance.colorYellow;
		}
		this.menuButtonPopUp = base.GetComponent<MenuButtonPopUp>();
		this.menuSelectableElement = base.GetComponent<MenuSelectableElement>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.button = base.GetComponent<Button>();
		this.buttonText = base.GetComponentInChildren<TextMeshProUGUI>();
		this.buttonTextSelectedOriginalPos = this.buttonText.transform.localPosition;
		if (this.buttonTextString != "BUTTON")
		{
			this.buttonText.text = this.buttonTextString;
		}
		this.originalText = this.buttonText.text;
		Vector2 sizeDelta = this.rectTransform.sizeDelta;
		this.buttonPitch = SemiFunc.MenuGetPitchFromYPos(this.rectTransform);
		float fontSize = this.buttonText.fontSize;
		this.buttonText.fontSize = fontSize;
		this.buttonText.enableAutoSizing = false;
		TextAlignmentOptions alignment = this.buttonText.alignment;
		this.buttonText.alignment = TextAlignmentOptions.MidlineLeft;
		this.buttonPadding = 0f;
		Vector2 sizeDelta2 = this.rectTransform.sizeDelta;
		this.rectTransform.sizeDelta = new Vector2(this.buttonText.GetPreferredValues(this.originalText, 0f, 0f).x + this.buttonPadding, this.buttonText.GetPreferredValues(this.originalText, 0f, 0f).y + this.buttonPadding / 2f);
		this.buttonText.alignment = alignment;
		if (alignment == TextAlignmentOptions.Midline)
		{
			this.buttonText.enableAutoSizing = true;
		}
		if (this.middleAlignFix)
		{
			this.rectTransform.position += new Vector3((sizeDelta2.x - this.rectTransform.sizeDelta.x) / 2f, 0f, 0f);
			this.buttonText.enableAutoSizing = false;
		}
		if (this.customHoverArea)
		{
			this.rectTransform.sizeDelta = sizeDelta;
		}
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00091584 File Offset: 0x0008F784
	private void Update()
	{
		this.button.image.color = new Color(0f, 0f, 0f, 0f);
		this.HoverLogic();
		switch (this.buttonState)
		{
		case 0:
			this.<Update>g__ButtonHover|33_1();
			this.buttonStateStart = false;
			break;
		case 1:
			this.<Update>g__ButtonClicked|33_2();
			this.buttonStateStart = false;
			break;
		case 2:
			this.<Update>g__ButtonNormal|33_0();
			this.buttonStateStart = false;
			break;
		}
		if (this.hoverTimer > 0f)
		{
			this.hoverTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x00091628 File Offset: 0x0008F828
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (this.buttonTextString != "BUTTON")
		{
			this.buttonText = base.GetComponentInChildren<TextMeshProUGUI>();
			if (this.buttonText.text != this.buttonTextString)
			{
				this.buttonText.text = this.buttonTextString;
			}
			if (base.gameObject.name != "Menu Button - " + this.buttonTextString)
			{
				base.gameObject.name = "Menu Button - " + this.buttonTextString;
			}
		}
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x000916C4 File Offset: 0x0008F8C4
	private void HoverLogic()
	{
		int num = 0;
		if (!this.customHoverArea)
		{
			num = 10;
		}
		if (SemiFunc.UIMouseHover(this.parentPage, this.rectTransform, this.menuSelectableElement.menuID, (float)num, 0f))
		{
			if (!this.hovering)
			{
				this.OnHoverStart();
				this.hovering = true;
			}
			this.hoverTimer = 0.01f;
		}
		if (this.hovering || (this.clicked && this.hovering))
		{
			if (Input.GetMouseButtonDown(0))
			{
				this.OnSelect();
				this.holdTimer = 0f;
				this.clickTimer = 0.2f;
			}
			if (this.hasHold)
			{
				if (Input.GetMouseButton(0))
				{
					this.holdTimer += Time.deltaTime;
				}
				else
				{
					this.holdTimer = 0f;
					this.clickFrequencyTicker = 0f;
					this.clickFrequency = 0.2f;
				}
			}
		}
		if (this.clickTimer > 0f)
		{
			this.clickTimer -= Time.deltaTime;
			this.clicked = true;
		}
		else
		{
			if (this.clicked)
			{
				this.OnSelectEnd();
			}
			this.clicked = false;
		}
		if (this.hoverTimer <= 0f)
		{
			if (this.hovering)
			{
				this.OnHoverEnd();
			}
			this.hovering = false;
		}
		if (this.hoverTimer > 0f)
		{
			bool flag = this.hovering;
			this.OnHovering();
			this.hovering = true;
		}
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x00091824 File Offset: 0x0008FA24
	private void ButtonStateSet(int state)
	{
		this.buttonState = state;
		this.buttonStateStart = true;
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x00091834 File Offset: 0x0008FA34
	private void OnHoverStart()
	{
		this.ButtonStateSet(0);
		this.buttonStateStart = true;
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x00091844 File Offset: 0x0008FA44
	private void OnHovering()
	{
		this.buttonText.transform.localPosition = new Vector3(this.buttonTextSelectedOriginalPos.x, this.buttonTextSelectedOriginalPos.y + this.buttonTextSelectedScootPos, this.buttonTextSelectedOriginalPos.z);
		Vector2 sizeDelta = this.rectTransform.sizeDelta;
		new Vector3(sizeDelta.x / 2f, sizeDelta.y / 2f, 0f) + (base.transform.localPosition - new Vector3(this.buttonPadding / 2f, 0f, 0f));
		SemiFunc.MenuSelectionBoxTargetSet(this.parentPage, this.rectTransform, default(Vector2), default(Vector2));
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x00091910 File Offset: 0x0008FB10
	private void OnHoverEnd()
	{
		this.ButtonStateSet(2);
		this.buttonStateStart = true;
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x00091920 File Offset: 0x0008FB20
	private void OnSelect()
	{
		if (this.disabled)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, -1f, -1f, false);
			return;
		}
		if (this.doButtonEffect)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, this.parentPage, -1f, -1f, false);
		}
		this.ButtonStateSet(1);
		if (!this.menuButtonPopUp)
		{
			this.button.onClick.Invoke();
			return;
		}
		MenuManager.instance.PagePopUpTwoOptions(this.menuButtonPopUp, this.menuButtonPopUp.headerText, this.menuButtonPopUp.headerColor, this.menuButtonPopUp.bodyText, this.menuButtonPopUp.option1Text, this.menuButtonPopUp.option2Text);
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x000919DD File Offset: 0x0008FBDD
	private void OnSelectEnd()
	{
		if (!this.hovering)
		{
			this.ButtonStateSet(2);
			return;
		}
		this.ButtonStateSet(0);
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x000919F6 File Offset: 0x0008FBF6
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.OnHoverStart();
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x000919FE File Offset: 0x0008FBFE
	public void OnPointerExit(PointerEventData eventData)
	{
		this.OnHoverEnd();
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x00091A06 File Offset: 0x0008FC06
	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnSelect();
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x00091A10 File Offset: 0x0008FC10
	private void HoldTimer()
	{
		if (!this.holdLogic)
		{
			return;
		}
		if (this.holdTimer > 0.5f)
		{
			if (this.clickFrequencyTicker <= 0f)
			{
				this.OnSelect();
				this.clickFrequencyTicker = this.clickFrequency;
				this.clickFrequency -= this.clickFrequency * 0.2f;
				this.clickFrequency = Mathf.Clamp(this.clickFrequency, 0.025f, 0.2f);
				return;
			}
			this.clickFrequencyTicker -= Time.deltaTime;
		}
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x00091AF0 File Offset: 0x0008FCF0
	[CompilerGenerated]
	private void <Update>g__ButtonNormal|33_0()
	{
		bool flag = this.buttonStateStart;
		this.holdTimer = 0f;
		this.buttonText.transform.localPosition = this.buttonTextSelectedOriginalPos;
		this.buttonText.color = this.colorNormal;
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x00091B2B File Offset: 0x0008FD2B
	[CompilerGenerated]
	private void <Update>g__ButtonHover|33_1()
	{
		if (this.buttonStateStart)
		{
			MenuManager.instance.MenuEffectHover(this.buttonPitch, -1f);
		}
		this.HoldTimer();
		this.buttonText.color = this.colorHover;
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00091B61 File Offset: 0x0008FD61
	[CompilerGenerated]
	private void <Update>g__ButtonClicked|33_2()
	{
		bool flag = this.buttonStateStart;
		this.HoldTimer();
		this.buttonText.color = this.colorClick;
	}

	// Token: 0x04001AC2 RID: 6850
	public string buttonTextString = "BUTTON";

	// Token: 0x04001AC3 RID: 6851
	internal TextMeshProUGUI buttonText;

	// Token: 0x04001AC4 RID: 6852
	public bool customHoverArea;

	// Token: 0x04001AC5 RID: 6853
	public bool doButtonEffect = true;

	// Token: 0x04001AC6 RID: 6854
	public bool holdLogic = true;

	// Token: 0x04001AC7 RID: 6855
	private Button button;

	// Token: 0x04001AC8 RID: 6856
	internal bool hovering;

	// Token: 0x04001AC9 RID: 6857
	private float hoverTimer;

	// Token: 0x04001ACA RID: 6858
	private float clickTimer;

	// Token: 0x04001ACB RID: 6859
	internal bool clicked;

	// Token: 0x04001ACC RID: 6860
	private float buttonPitch = 1f;

	// Token: 0x04001ACD RID: 6861
	private string originalText;

	// Token: 0x04001ACE RID: 6862
	private RectTransform rectTransform;

	// Token: 0x04001ACF RID: 6863
	public bool hasHold;

	// Token: 0x04001AD0 RID: 6864
	private float holdTimer;

	// Token: 0x04001AD1 RID: 6865
	private float clickFrequency = 0.2f;

	// Token: 0x04001AD2 RID: 6866
	private float clickFrequencyTicker;

	// Token: 0x04001AD3 RID: 6867
	private MenuSelectableElement menuSelectableElement;

	// Token: 0x04001AD4 RID: 6868
	private float buttonPadding;

	// Token: 0x04001AD5 RID: 6869
	private MenuPage parentPage;

	// Token: 0x04001AD6 RID: 6870
	private MenuButtonPopUp menuButtonPopUp;

	// Token: 0x04001AD7 RID: 6871
	public bool middleAlignFix;

	// Token: 0x04001AD8 RID: 6872
	public bool customColors;

	// Token: 0x04001AD9 RID: 6873
	[Header("Custom Colors")]
	public Color colorNormal;

	// Token: 0x04001ADA RID: 6874
	public Color colorHover;

	// Token: 0x04001ADB RID: 6875
	public Color colorClick;

	// Token: 0x04001ADC RID: 6876
	private int buttonState = 2;

	// Token: 0x04001ADD RID: 6877
	private bool buttonStateStart;

	// Token: 0x04001ADE RID: 6878
	private float buttonTextSelectedScootPos = 1f;

	// Token: 0x04001ADF RID: 6879
	private Vector3 buttonTextSelectedOriginalPos;

	// Token: 0x04001AE0 RID: 6880
	internal bool disabled;

	// Token: 0x0200038A RID: 906
	private enum ButtonState
	{
		// Token: 0x040027E8 RID: 10216
		Hover,
		// Token: 0x040027E9 RID: 10217
		Clicked,
		// Token: 0x040027EA RID: 10218
		Normal
	}
}
