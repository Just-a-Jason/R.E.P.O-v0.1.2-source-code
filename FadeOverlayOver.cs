using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000241 RID: 577
public class FadeOverlayOver : MonoBehaviour
{
	// Token: 0x06001228 RID: 4648 RVA: 0x000A05A6 File Offset: 0x0009E7A6
	private void Awake()
	{
		FadeOverlayOver.Instance = this;
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x000A05B0 File Offset: 0x0009E7B0
	private void Update()
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			this.Image.color = new Color32(0, 0, 0, byte.MaxValue);
			return;
		}
		this.Image.color = new Color32(0, 0, 0, 0);
	}

	// Token: 0x04001ED8 RID: 7896
	public static FadeOverlayOver Instance;

	// Token: 0x04001ED9 RID: 7897
	public Image Image;
}
