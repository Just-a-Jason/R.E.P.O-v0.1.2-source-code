using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000244 RID: 580
public class HurtVignette : MonoBehaviour
{
	// Token: 0x06001231 RID: 4657 RVA: 0x000A0678 File Offset: 0x0009E878
	private void Start()
	{
		HurtVignette.instance = this;
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x000A0680 File Offset: 0x0009E880
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated || SemiFunc.MenuLevel())
		{
			return;
		}
		if (!PlayerController.instance.playerAvatarScript.isDisabled && (float)PlayerController.instance.playerAvatarScript.playerHealth.health < 10f)
		{
			if (this.lerp < 1f)
			{
				this.lerp += 1f * Time.deltaTime;
				this.rectTransform.localScale = Vector3.one * Mathf.Lerp(this.inactiveScale, this.activeScale, this.introCurve.Evaluate(this.lerp));
				this.image.color = Color.Lerp(this.inactiveColor, this.activeColor, this.introCurve.Evaluate(this.lerp));
				return;
			}
		}
		else if (this.lerp > 0f)
		{
			this.lerp -= 1f * Time.deltaTime;
			this.rectTransform.localScale = Vector3.one * Mathf.Lerp(this.inactiveScale, this.activeScale, this.outroCurve.Evaluate(this.lerp));
			this.image.color = Color.Lerp(this.inactiveColor, this.activeColor, this.outroCurve.Evaluate(this.lerp));
		}
	}

	// Token: 0x04001EDF RID: 7903
	public Image image;

	// Token: 0x04001EE0 RID: 7904
	public RectTransform rectTransform;

	// Token: 0x04001EE1 RID: 7905
	[Space]
	public Color activeColor;

	// Token: 0x04001EE2 RID: 7906
	public Color inactiveColor;

	// Token: 0x04001EE3 RID: 7907
	[Space]
	public float activeScale;

	// Token: 0x04001EE4 RID: 7908
	public float inactiveScale;

	// Token: 0x04001EE5 RID: 7909
	[Space]
	public AnimationCurve introCurve;

	// Token: 0x04001EE6 RID: 7910
	public AnimationCurve outroCurve;

	// Token: 0x04001EE7 RID: 7911
	private float lerp;

	// Token: 0x04001EE8 RID: 7912
	public static HurtVignette instance;
}
