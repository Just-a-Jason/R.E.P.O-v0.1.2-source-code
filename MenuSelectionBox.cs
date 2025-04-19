using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001ED RID: 493
public class MenuSelectionBox : MonoBehaviour
{
	// Token: 0x0600106D RID: 4205 RVA: 0x0009446C File Offset: 0x0009266C
	private void Start()
	{
		this.targetPosition = base.transform.localPosition;
		this.targetScale = base.transform.localScale * 100f;
		this.originalPos = base.transform.localPosition;
		this.originalScale = base.transform.localScale;
		this.rawImage = base.GetComponentInChildren<RawImage>();
		this.menuPage = base.GetComponentInParent<MenuPage>();
		this.menuPage.selectionBox = this;
		this.rectTransform = base.GetComponent<RectTransform>();
		this.isInScrollBox = false;
		this.menuScrollBox = base.GetComponentInParent<MenuScrollBox>();
		if (this.menuScrollBox)
		{
			this.isInScrollBox = true;
		}
		else
		{
			MenuSelectionBox.instance = this;
		}
		MenuManager.instance.SelectionBoxAdd(this);
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x00094534 File Offset: 0x00092734
	private void Update()
	{
		if (this.menuPage.currentPageState != MenuPage.PageState.Active && !this.menuPage.addedPageOnTop)
		{
			this.rawImage.color = new Color(0.4f, 0.08f, 0.015f, 0f);
			RectTransform component = this.menuPage.GetComponent<RectTransform>();
			base.transform.localPosition = new Vector3(component.rect.width / 2f, component.rect.height / 2f, base.transform.localPosition.z);
			base.transform.localScale = new Vector3(0f, 0f, 1f);
			this.targetScale = base.transform.localScale * 100f;
			this.targetPosition = this.rectTransform.localPosition;
			this.activeTargetTimer = 0f;
			return;
		}
		if (this.prevPosTimer <= 0f)
		{
			this.prevPos = this.currentPos;
			this.currentPos = base.transform.localPosition - this.pulsatePos;
			this.prevPosTimer = 0.008333334f;
		}
		else
		{
			this.prevPosTimer -= Time.deltaTime;
		}
		this.rectTransform.localPosition = Vector3.Lerp(this.rectTransform.localPosition, this.targetPosition, 20f * Time.deltaTime);
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.targetScale / 100f, 20f * Time.deltaTime);
		if (this.activeTargetTimer > 0f)
		{
			float num = 0.5f;
			base.transform.localScale = base.transform.localScale + new Vector3(num * 0.01f, num * 0.01f, 1f) * Mathf.Sin(Time.time * 20f);
			base.transform.localPosition = base.transform.localPosition + this.pulsatePos;
			this.activeTargetTimer -= Time.deltaTime;
			Color a = new Color(0.08f, 0.2f, 0.4f, 0.75f);
			Color b = new Color(0.2f, 0.5f, 1f, 1f);
			if (Vector3.Distance(base.transform.localPosition, this.targetPosition) <= 5f)
			{
				this.prevPos = this.currentPos;
			}
			this.rawImage.color = Color.Lerp(a, b, Vector3.Distance(this.prevPos, this.currentPos) * 0.5f);
		}
		else
		{
			Color b2 = new Color(0.4f, 0.08f, 0.015f, 0f);
			this.rawImage.color = Color.Lerp(this.rawImage.color, b2, 10f * Time.deltaTime);
		}
		this.ClickColorAnimate();
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x0009484C File Offset: 0x00092A4C
	public void MenuSelectionBoxSetTarget(Vector3 pos, Vector3 scale, MenuPage parentPage, bool _isInScrollBox, MenuScrollBox _menuScrollBox, Vector2 customScale = default(Vector2))
	{
		if (_isInScrollBox != this.isInScrollBox || (_isInScrollBox && _menuScrollBox != this.menuScrollBox))
		{
			MenuSelectionBox menuSelectionBox = MenuManager.instance.SelectionBoxGetCorrect(parentPage, _menuScrollBox);
			if (menuSelectionBox)
			{
				MenuManager.instance.SetActiveSelectionBox(menuSelectionBox);
				parentPage.selectionBox = menuSelectionBox;
				menuSelectionBox.Reinstate();
				menuSelectionBox.MenuSelectionBoxSetTarget(pos, scale, parentPage, _isInScrollBox, _menuScrollBox, customScale);
			}
			return;
		}
		MenuManager.instance.SetActiveSelectionBox(this);
		if (MenuSelectionBox.instance != this)
		{
			this.Reinstate();
		}
		if (this.firstSelection)
		{
			this.firstSelection = false;
			base.transform.localPosition = pos;
			base.transform.localScale = Vector3.zero;
			this.targetPosition = pos;
			this.targetScale = scale;
			return;
		}
		pos = new Vector3(pos.x, pos.y, 0f);
		this.targetPosition = pos;
		this.targetScale = scale + new Vector3(customScale.x, customScale.y, 0f);
		float num = this.targetScale.y * 0.2f;
		this.targetScale += new Vector3(num, num, 0f);
		this.targetPosition += new Vector3(0f, 0f, 0f);
		this.activeTargetTimer = 0.2f;
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x000949AE File Offset: 0x00092BAE
	public void SetClick(Color color)
	{
		this.flashColor = color;
		this.clickTimer = 1f;
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x000949C4 File Offset: 0x00092BC4
	private void ClickColorAnimate()
	{
		if (this.clickTimer <= 0f)
		{
			return;
		}
		Color a = this.flashColor;
		Color b = new Color(0.08f, 0.2f, 0.4f, 0.75f);
		this.rawImage.color = Color.Lerp(a, b, 1f - this.clickTimer);
		this.clickTimer -= Time.deltaTime * 10f;
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x00094A38 File Offset: 0x00092C38
	private void OnEnable()
	{
		base.transform.localPosition = this.originalPos;
		base.transform.localScale = this.originalScale;
		this.targetScale = this.originalScale * 100f;
		this.targetPosition = this.originalPos;
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00094A89 File Offset: 0x00092C89
	private void OnDestroy()
	{
		MenuManager.instance.SelectionBoxRemove(this);
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00094A96 File Offset: 0x00092C96
	public void Reinstate()
	{
		MenuSelectionBox.instance = this;
	}

	// Token: 0x04001B7A RID: 7034
	public static MenuSelectionBox instance;

	// Token: 0x04001B7B RID: 7035
	internal Vector3 targetPosition;

	// Token: 0x04001B7C RID: 7036
	internal Vector3 targetScale;

	// Token: 0x04001B7D RID: 7037
	internal RawImage rawImage;

	// Token: 0x04001B7E RID: 7038
	internal RectTransform rectTransform;

	// Token: 0x04001B7F RID: 7039
	internal Vector3 originalPos;

	// Token: 0x04001B80 RID: 7040
	internal Vector3 originalScale;

	// Token: 0x04001B81 RID: 7041
	private float activeTargetTimer;

	// Token: 0x04001B82 RID: 7042
	private float prevPosTimer;

	// Token: 0x04001B83 RID: 7043
	private Vector3 prevPos;

	// Token: 0x04001B84 RID: 7044
	private Vector3 currentPos;

	// Token: 0x04001B85 RID: 7045
	private Vector3 pulsatePos;

	// Token: 0x04001B86 RID: 7046
	private float clickTimer;

	// Token: 0x04001B87 RID: 7047
	private Color flashColor;

	// Token: 0x04001B88 RID: 7048
	internal MenuPage menuPage;

	// Token: 0x04001B89 RID: 7049
	internal bool firstSelection = true;

	// Token: 0x04001B8A RID: 7050
	internal bool isInScrollBox;

	// Token: 0x04001B8B RID: 7051
	internal MenuScrollBox menuScrollBox;
}
