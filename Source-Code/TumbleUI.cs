using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000267 RID: 615
public class TumbleUI : MonoBehaviour
{
	// Token: 0x06001327 RID: 4903 RVA: 0x000A7787 File Offset: 0x000A5987
	private void Awake()
	{
		TumbleUI.instance = this;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x000A779C File Offset: 0x000A599C
	private void Start()
	{
		this.images1 = new Image[this.parts1.Length];
		int num = 0;
		foreach (GameObject gameObject in this.parts1)
		{
			this.images1[num] = gameObject.GetComponent<Image>();
			num++;
		}
		this.images2 = new Image[this.parts2.Length];
		num = 0;
		foreach (GameObject gameObject2 in this.parts2)
		{
			this.images2[num] = gameObject2.GetComponent<Image>();
			num++;
		}
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x000A782C File Offset: 0x000A5A2C
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PlayerController.instance.playerAvatarScript.isTumbling && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			this.active = true;
		}
		else
		{
			this.active = false;
		}
		if (this.active != this.activePrevious)
		{
			this.activePrevious = this.active;
			this.animationLerp = 0f;
			this.updateTimer = 0f;
			this.animating = true;
		}
		this.canExit = true;
		if (this.active && (PlayerController.instance.playerAvatarScript.tumble.tumbleOverride || PlayerController.instance.tumbleInputDisableTimer > 0f))
		{
			this.canExit = false;
		}
		if (this.canExit != this.canExitPrevious)
		{
			this.canExitPrevious = this.canExit;
			if (this.canExit)
			{
				this.canExitSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.animating = true;
				this.animationLerp = 0.5f;
				this.updateTimer = 0f;
				Image[] array = this.images1;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canExitColor1;
				}
				array = this.images2;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canExitColor2;
				}
			}
			else
			{
				Image[] array = this.images1;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canNotExitColor2;
				}
				array = this.images2;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].color = this.canNotExitColor1;
				}
			}
		}
		if (this.animating)
		{
			if (this.updateTimer <= 0f)
			{
				if (this.active)
				{
					if (this.animationLerp == 0f)
					{
						GameObject[] array2 = this.parts1;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(true);
						}
						array2 = this.parts2;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(true);
						}
					}
					this.animationLerp += Time.deltaTime * this.introSpeed;
					this.updateTimer = this.updateTime;
				}
				else
				{
					this.animationLerp += Time.deltaTime * this.outroSpeed;
					this.updateTimer = this.updateTime;
					if (this.animationLerp >= 1f)
					{
						GameObject[] array2 = this.parts1;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(false);
						}
						array2 = this.parts2;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(false);
						}
					}
				}
			}
			else
			{
				this.updateTimer -= Time.deltaTime;
			}
		}
		if (this.animating)
		{
			if (this.active)
			{
				base.transform.localScale = Vector3.LerpUnclamped(Vector3.one * 1.25f, Vector3.one, this.introCurve.Evaluate(this.animationLerp));
			}
			else
			{
				base.transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 1.25f, this.outroCurve.Evaluate(this.animationLerp));
			}
			if (this.animationLerp >= 1f)
			{
				this.animating = false;
			}
		}
		float b = 1f;
		if (this.hideTimer > 0f)
		{
			b = 0f;
			this.hideTimer -= Time.deltaTime;
		}
		this.hideAlpha = Mathf.Lerp(this.hideAlpha, b, Time.deltaTime * 20f);
		this.canvasGroup.alpha = this.hideAlpha;
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x000A7BEE File Offset: 0x000A5DEE
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x04002074 RID: 8308
	public static TumbleUI instance;

	// Token: 0x04002075 RID: 8309
	private CanvasGroup canvasGroup;

	// Token: 0x04002076 RID: 8310
	private bool active;

	// Token: 0x04002077 RID: 8311
	private bool activePrevious = true;

	// Token: 0x04002078 RID: 8312
	private bool canExit;

	// Token: 0x04002079 RID: 8313
	private bool canExitPrevious;

	// Token: 0x0400207A RID: 8314
	private bool animating;

	// Token: 0x0400207B RID: 8315
	private float animationLerp;

	// Token: 0x0400207C RID: 8316
	public Color canNotExitColor1;

	// Token: 0x0400207D RID: 8317
	public Color canNotExitColor2;

	// Token: 0x0400207E RID: 8318
	public Color canExitColor1;

	// Token: 0x0400207F RID: 8319
	public Color canExitColor2;

	// Token: 0x04002080 RID: 8320
	[Space]
	public AnimationCurve introCurve;

	// Token: 0x04002081 RID: 8321
	public float introSpeed;

	// Token: 0x04002082 RID: 8322
	[Space]
	public AnimationCurve outroCurve;

	// Token: 0x04002083 RID: 8323
	public float outroSpeed;

	// Token: 0x04002084 RID: 8324
	[Space]
	public float updateTime;

	// Token: 0x04002085 RID: 8325
	private float updateTimer;

	// Token: 0x04002086 RID: 8326
	[Space]
	public GameObject[] parts1;

	// Token: 0x04002087 RID: 8327
	public GameObject[] parts2;

	// Token: 0x04002088 RID: 8328
	private Image[] images1;

	// Token: 0x04002089 RID: 8329
	private Image[] images2;

	// Token: 0x0400208A RID: 8330
	[Space]
	public Sound canExitSound;

	// Token: 0x0400208B RID: 8331
	private float hideTimer;

	// Token: 0x0400208C RID: 8332
	private float hideAlpha;
}
