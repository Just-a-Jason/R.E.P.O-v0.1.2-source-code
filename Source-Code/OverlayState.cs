using System;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class OverlayState : MonoBehaviour
{
	// Token: 0x060012A1 RID: 4769 RVA: 0x000A2E38 File Offset: 0x000A1038
	private void Update()
	{
		if (this.RewindEffect.PlayRewind)
		{
			this.Play.SetActive(false);
			this.Stop.SetActive(false);
			this.Rewind.SetActive(true);
			return;
		}
		if (GameDirector.instance.currentState < GameDirector.gameState.Outro)
		{
			this.Play.SetActive(true);
			this.Stop.SetActive(false);
			this.Rewind.SetActive(false);
			return;
		}
		this.Play.SetActive(false);
		this.Stop.SetActive(true);
		this.Rewind.SetActive(false);
	}

	// Token: 0x04001F79 RID: 8057
	public GameObject Play;

	// Token: 0x04001F7A RID: 8058
	public GameObject Stop;

	// Token: 0x04001F7B RID: 8059
	public GameObject Rewind;

	// Token: 0x04001F7C RID: 8060
	[Space]
	public RewindEffect RewindEffect;
}
