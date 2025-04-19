using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000266 RID: 614
public class RewindTexture : MonoBehaviour
{
	// Token: 0x06001324 RID: 4900 RVA: 0x000A7718 File Offset: 0x000A5918
	private void Start()
	{
		this.rawImage = base.GetComponent<RawImage>();
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x000A7728 File Offset: 0x000A5928
	private void Update()
	{
		float num = Mathf.Repeat(Time.time * this.scrollSpeed, 1f);
		Rect uvRect = this.rawImage.uvRect;
		uvRect.x = num;
		uvRect.y = num;
		this.rawImage.uvRect = uvRect;
	}

	// Token: 0x04002072 RID: 8306
	public float scrollSpeed = 0.5f;

	// Token: 0x04002073 RID: 8307
	private RawImage rawImage;
}
