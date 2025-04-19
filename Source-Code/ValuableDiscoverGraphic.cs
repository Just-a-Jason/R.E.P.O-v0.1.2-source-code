using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026A RID: 618
public class ValuableDiscoverGraphic : MonoBehaviour
{
	// Token: 0x06001333 RID: 4915 RVA: 0x000A7D04 File Offset: 0x000A5F04
	private void Start()
	{
		this.canvasRect = ValuableDiscover.instance.canvasRect;
		this.mainCamera = Camera.main;
		this.waitTimer = this.waitTime;
		if (this.state == ValuableDiscoverGraphic.State.Reminder)
		{
			this.waitTimer = this.waitTime * 0.5f;
		}
		if (this.state == ValuableDiscoverGraphic.State.Bad)
		{
			this.waitTimer = this.waitTime * 3f;
		}
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x000A7D70 File Offset: 0x000A5F70
	private void Update()
	{
		if (this.target)
		{
			bool flag = true;
			Bounds bigBounds = new Bounds(this.target.centerPoint, Vector3.zero);
			foreach (MeshRenderer meshRenderer in this.target.GetComponentsInChildren<MeshRenderer>())
			{
				bigBounds.Encapsulate(meshRenderer.bounds);
			}
			if (SemiFunc.OnScreen(bigBounds.center, 0.5f, 0.5f))
			{
				Rect rect = this.RendererBoundsInScreenSpace(bigBounds);
				if (rect.width > 2f || rect.height > 2f)
				{
					this.topLeftTargetNew = rect.center;
					this.topRightTargetNew = rect.center;
					this.botLeftTargetNew = rect.center;
					this.botRightTargetNew = rect.center;
					this.middleTargetNew = rect.center;
					this.middleTargetSizeNew = new Vector2(0f, 0f);
				}
				else
				{
					this.topLeftTargetNew = this.GetScreenPosition(new Vector3(rect.xMin, rect.yMax, 0f));
					this.topRightTargetNew = this.GetScreenPosition(new Vector3(rect.xMax, rect.yMax, 0f));
					this.botLeftTargetNew = this.GetScreenPosition(new Vector3(rect.xMin, rect.yMin, 0f));
					this.botRightTargetNew = this.GetScreenPosition(new Vector3(rect.xMax, rect.yMin, 0f));
					this.middleTargetNew = this.GetScreenPosition(rect.center);
					this.middleTargetSizeNew = new Vector2(rect.width * 1.9f + 0.025f, rect.height + 0.025f);
				}
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				if (this.first)
				{
					if (this.state == ValuableDiscoverGraphic.State.Reminder)
					{
						this.sound.Play(this.target.centerPoint, 0.3f, 1f, 1f, 1f);
					}
					else
					{
						this.sound.Play(this.target.centerPoint, 1f, 1f, 1f, 1f);
					}
					this.middle.gameObject.SetActive(true);
					this.topLeft.gameObject.SetActive(true);
					this.topRight.gameObject.SetActive(true);
					this.botLeft.gameObject.SetActive(true);
					this.botRight.gameObject.SetActive(true);
					this.first = false;
				}
				if (this.hidden)
				{
					this.middleTarget = this.middleTargetNew;
					this.middleTargetSize = this.middleTargetSizeNew;
					this.topLeftTarget = this.topLeftTargetNew;
					this.topRightTarget = this.topRightTargetNew;
					this.botLeftTarget = this.botLeftTargetNew;
					this.botRightTarget = this.botRightTargetNew;
					this.hidden = false;
				}
				this.middleTarget = Vector2.Lerp(this.middleTarget, this.middleTargetNew, 50f * Time.deltaTime);
				this.middleTargetSize = Vector2.Lerp(this.middleTargetSize, this.middleTargetSizeNew, 50f * Time.deltaTime);
				this.topLeftTarget = Vector2.Lerp(this.topLeftTarget, this.topLeftTargetNew, 50f * Time.deltaTime);
				this.topRightTarget = Vector2.Lerp(this.topRightTarget, this.topRightTargetNew, 50f * Time.deltaTime);
				this.botLeftTarget = Vector2.Lerp(this.botLeftTarget, this.botLeftTargetNew, 50f * Time.deltaTime);
				this.botRightTarget = Vector2.Lerp(this.botRightTarget, this.botRightTargetNew, 50f * Time.deltaTime);
			}
			else
			{
				this.hidden = true;
				this.topLeftTarget = this.middleTarget;
				this.topRightTarget = this.middleTarget;
				this.botLeftTarget = this.middleTarget;
				this.botRightTarget = this.middleTarget;
				this.middleTargetSize = Vector2.zero;
			}
		}
		else
		{
			this.waitTimer = 0f;
		}
		this.middle.anchoredPosition = this.middleTarget;
		if (this.waitTimer > 0f)
		{
			this.animLerp = Mathf.Clamp01(this.animLerp + this.introSpeed * Time.deltaTime);
			this.middle.sizeDelta = Vector2.LerpUnclamped(Vector2.zero, this.middleTargetSize, this.introCurve.Evaluate(this.animLerp));
			this.topLeft.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.topLeftTarget, this.introCurve.Evaluate(this.animLerp));
			this.topRight.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.topRightTarget, this.introCurve.Evaluate(this.animLerp));
			this.botLeft.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.botLeftTarget, this.introCurve.Evaluate(this.animLerp));
			this.botRight.anchoredPosition = Vector2.LerpUnclamped(this.middleTarget, this.botRightTarget, this.introCurve.Evaluate(this.animLerp));
			if (this.animLerp >= 1f)
			{
				this.waitTimer -= Time.deltaTime;
				if (this.waitTimer <= 0f)
				{
					this.animLerp = 0f;
					return;
				}
			}
		}
		else
		{
			this.animLerp = Mathf.Clamp01(this.animLerp + this.outroSpeed * Time.deltaTime);
			this.middle.sizeDelta = Vector2.LerpUnclamped(this.middleTargetSize, Vector2.zero, this.outroCurve.Evaluate(this.animLerp));
			this.topLeft.anchoredPosition = Vector2.LerpUnclamped(this.topLeftTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			this.topRight.anchoredPosition = Vector2.LerpUnclamped(this.topRightTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			this.botLeft.anchoredPosition = Vector2.LerpUnclamped(this.botLeftTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			this.botRight.anchoredPosition = Vector2.LerpUnclamped(this.botRightTarget, this.middleTarget, this.outroCurve.Evaluate(this.animLerp));
			if (this.animLerp >= 1f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x000A8414 File Offset: 0x000A6614
	public void ReminderSetup()
	{
		this.state = ValuableDiscoverGraphic.State.Reminder;
		this.middle.GetComponent<Image>().color = this.ColorReminderMiddle;
		this.topLeft.GetComponent<Image>().color = this.ColorReminderCorner;
		this.topRight.GetComponent<Image>().color = this.ColorReminderCorner;
		this.botLeft.GetComponent<Image>().color = this.ColorReminderCorner;
		this.botRight.GetComponent<Image>().color = this.ColorReminderCorner;
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x000A8498 File Offset: 0x000A6698
	public void BadSetup()
	{
		this.state = ValuableDiscoverGraphic.State.Bad;
		this.middle.GetComponent<Image>().color = this.ColorBadMiddle;
		this.topLeft.GetComponent<Image>().color = this.ColorBadCorner;
		this.topRight.GetComponent<Image>().color = this.ColorBadCorner;
		this.botLeft.GetComponent<Image>().color = this.ColorBadCorner;
		this.botRight.GetComponent<Image>().color = this.ColorBadCorner;
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x000A851C File Offset: 0x000A671C
	private Vector3 GetScreenPosition(Vector3 _position)
	{
		return new Vector3(_position.x * this.canvasRect.sizeDelta.x - this.canvasRect.sizeDelta.x * 0.5f, _position.y * this.canvasRect.sizeDelta.y - this.canvasRect.sizeDelta.y * 0.5f, _position.z) / SemiFunc.UIMulti();
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x000A859C File Offset: 0x000A679C
	private Rect RendererBoundsInScreenSpace(Bounds bigBounds)
	{
		if (this.screenSpaceCorners == null)
		{
			this.screenSpaceCorners = new Vector3[8];
		}
		this.screenSpaceCorners[0] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[1] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		this.screenSpaceCorners[2] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[3] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		this.screenSpaceCorners[4] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[5] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		this.screenSpaceCorners[6] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
		this.screenSpaceCorners[7] = this.mainCamera.WorldToViewportPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
		float x = this.screenSpaceCorners[0].x;
		float y = this.screenSpaceCorners[0].y;
		float x2 = this.screenSpaceCorners[0].x;
		float y2 = this.screenSpaceCorners[0].y;
		for (int i = 1; i < 8; i++)
		{
			if (this.screenSpaceCorners[i].x < x)
			{
				x = this.screenSpaceCorners[i].x;
			}
			if (this.screenSpaceCorners[i].y < y)
			{
				y = this.screenSpaceCorners[i].y;
			}
			if (this.screenSpaceCorners[i].x > x2)
			{
				x2 = this.screenSpaceCorners[i].x;
			}
			if (this.screenSpaceCorners[i].y > y2)
			{
				y2 = this.screenSpaceCorners[i].y;
			}
		}
		return Rect.MinMaxRect(x, y, x2, y2);
	}

	// Token: 0x04002095 RID: 8341
	public PhysGrabObject target;

	// Token: 0x04002096 RID: 8342
	private ValuableDiscoverGraphic.State state;

	// Token: 0x04002097 RID: 8343
	private Camera mainCamera;

	// Token: 0x04002098 RID: 8344
	private RectTransform canvasRect;

	// Token: 0x04002099 RID: 8345
	private Vector3[] screenSpaceCorners;

	// Token: 0x0400209A RID: 8346
	private bool hidden = true;

	// Token: 0x0400209B RID: 8347
	private bool first = true;

	// Token: 0x0400209C RID: 8348
	[Space]
	public Color colorDiscoverCorner;

	// Token: 0x0400209D RID: 8349
	public Color colorDiscoverMiddle;

	// Token: 0x0400209E RID: 8350
	[Space]
	public Color ColorReminderCorner;

	// Token: 0x0400209F RID: 8351
	public Color ColorReminderMiddle;

	// Token: 0x040020A0 RID: 8352
	[Space]
	public Color ColorBadCorner;

	// Token: 0x040020A1 RID: 8353
	public Color ColorBadMiddle;

	// Token: 0x040020A2 RID: 8354
	[Space]
	public Sound sound;

	// Token: 0x040020A3 RID: 8355
	[Space]
	public AnimationCurve introCurve;

	// Token: 0x040020A4 RID: 8356
	public float introSpeed;

	// Token: 0x040020A5 RID: 8357
	public AnimationCurve outroCurve;

	// Token: 0x040020A6 RID: 8358
	public float outroSpeed;

	// Token: 0x040020A7 RID: 8359
	public float waitTime;

	// Token: 0x040020A8 RID: 8360
	private float waitTimer;

	// Token: 0x040020A9 RID: 8361
	private float animLerp;

	// Token: 0x040020AA RID: 8362
	[Space]
	public RectTransform middle;

	// Token: 0x040020AB RID: 8363
	private Vector2 middleTarget;

	// Token: 0x040020AC RID: 8364
	private Vector2 middleTargetNew;

	// Token: 0x040020AD RID: 8365
	private Vector2 middleTargetSize;

	// Token: 0x040020AE RID: 8366
	private Vector2 middleTargetSizeNew;

	// Token: 0x040020AF RID: 8367
	public RectTransform topLeft;

	// Token: 0x040020B0 RID: 8368
	private Vector2 topLeftTarget;

	// Token: 0x040020B1 RID: 8369
	private Vector2 topLeftTargetNew;

	// Token: 0x040020B2 RID: 8370
	public RectTransform topRight;

	// Token: 0x040020B3 RID: 8371
	private Vector2 topRightTarget;

	// Token: 0x040020B4 RID: 8372
	private Vector2 topRightTargetNew;

	// Token: 0x040020B5 RID: 8373
	public RectTransform botLeft;

	// Token: 0x040020B6 RID: 8374
	private Vector2 botLeftTarget;

	// Token: 0x040020B7 RID: 8375
	private Vector2 botLeftTargetNew;

	// Token: 0x040020B8 RID: 8376
	public RectTransform botRight;

	// Token: 0x040020B9 RID: 8377
	private Vector2 botRightTarget;

	// Token: 0x040020BA RID: 8378
	private Vector2 botRightTargetNew;

	// Token: 0x020003BC RID: 956
	public enum State
	{
		// Token: 0x040028C7 RID: 10439
		Discover,
		// Token: 0x040028C8 RID: 10440
		Reminder,
		// Token: 0x040028C9 RID: 10441
		Bad
	}
}
