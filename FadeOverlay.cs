using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000240 RID: 576
public class FadeOverlay : MonoBehaviour
{
	// Token: 0x06001225 RID: 4645 RVA: 0x000A04E0 File Offset: 0x0009E6E0
	private void Awake()
	{
		FadeOverlay.Instance = this;
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x000A04E8 File Offset: 0x0009E6E8
	private void Update()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.Start || GameDirector.instance.currentState == GameDirector.gameState.Outro || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			this.Image.color = new Color32(0, 0, 0, byte.MaxValue);
			return;
		}
		this.IntroLerp += Time.deltaTime * this.IntroSpeed;
		float num = this.IntroCurve.Evaluate(this.IntroLerp);
		this.Image.color = new Color32(0, 0, 0, (byte)(255f * num));
	}

	// Token: 0x04001ED3 RID: 7891
	public static FadeOverlay Instance;

	// Token: 0x04001ED4 RID: 7892
	public Image Image;

	// Token: 0x04001ED5 RID: 7893
	[Space]
	public AnimationCurve IntroCurve;

	// Token: 0x04001ED6 RID: 7894
	public float IntroSpeed;

	// Token: 0x04001ED7 RID: 7895
	private float IntroLerp;
}
