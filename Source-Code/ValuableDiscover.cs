using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class ValuableDiscover : MonoBehaviour
{
	// Token: 0x0600132E RID: 4910 RVA: 0x000A7C26 File Offset: 0x000A5E26
	private void Awake()
	{
		ValuableDiscover.instance = this;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x000A7C3C File Offset: 0x000A5E3C
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

	// Token: 0x06001330 RID: 4912 RVA: 0x000A7CA4 File Offset: 0x000A5EA4
	public void New(PhysGrabObject _target, ValuableDiscoverGraphic.State _state)
	{
		ValuableDiscoverGraphic component = Object.Instantiate<GameObject>(this.graphicPrefab, base.transform).GetComponent<ValuableDiscoverGraphic>();
		component.target = _target;
		if (_state == ValuableDiscoverGraphic.State.Reminder)
		{
			component.ReminderSetup();
		}
		if (_state == ValuableDiscoverGraphic.State.Bad)
		{
			component.BadSetup();
		}
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x000A7CE3 File Offset: 0x000A5EE3
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x0400208F RID: 8335
	public static ValuableDiscover instance;

	// Token: 0x04002090 RID: 8336
	public GameObject graphicPrefab;

	// Token: 0x04002091 RID: 8337
	public RectTransform canvasRect;

	// Token: 0x04002092 RID: 8338
	private CanvasGroup canvasGroup;

	// Token: 0x04002093 RID: 8339
	private float hideTimer;

	// Token: 0x04002094 RID: 8340
	internal float hideAlpha = 1f;
}
