using System;
using TMPro;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class ArenaPedistalScreen : MonoBehaviour
{
	// Token: 0x06000780 RID: 1920 RVA: 0x00047290 File Offset: 0x00045490
	private void Update()
	{
		if (this.glitchTimer > 0f)
		{
			this.glitchTimer -= Time.deltaTime;
			float x = Mathf.Sin(Time.time * 100f) * 0.1f;
			float y = Mathf.Sin(Time.time * 100f) * 0.1f;
			this.glitchMeshRenderer.material.mainTextureOffset = new Vector2(x, y);
			return;
		}
		this.glitchObject.SetActive(false);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00047310 File Offset: 0x00045510
	public void SwitchNumber(int number, bool finalPlayer = false)
	{
		if (!this.glitchMeshRenderer)
		{
			return;
		}
		this.screenText.text = number.ToString();
		float x = Random.Range(0f, 100f);
		float y = Random.Range(0f, 100f);
		this.glitchMeshRenderer.material.mainTextureOffset = new Vector2(x, y);
		this.glitchObject.SetActive(true);
		if (finalPlayer)
		{
			this.screenText.color = Color.green;
			this.numberLight.color = Color.green;
			Color color = new Color(0f, 1f, 0f, 0.65f);
			this.screenScanLines.color = color;
		}
		this.glitchTimer = 0.2f;
	}

	// Token: 0x04000D38 RID: 3384
	public TextMeshPro screenText;

	// Token: 0x04000D39 RID: 3385
	public GameObject glitchObject;

	// Token: 0x04000D3A RID: 3386
	public MeshRenderer glitchMeshRenderer;

	// Token: 0x04000D3B RID: 3387
	private float glitchTimer;

	// Token: 0x04000D3C RID: 3388
	public Light numberLight;

	// Token: 0x04000D3D RID: 3389
	public SpriteRenderer screenScanLines;
}
