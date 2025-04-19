using System;
using UnityEngine;

// Token: 0x02000237 RID: 567
public class TVBackground : MonoBehaviour
{
	// Token: 0x06001208 RID: 4616 RVA: 0x0009F98B File Offset: 0x0009DB8B
	private void Start()
	{
		this.material = base.GetComponent<Renderer>().material;
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x0009F9A0 File Offset: 0x0009DBA0
	private void Update()
	{
		this.offset.x = Time.time * this.scrollSpeed.x;
		this.offset.y = Time.time * this.scrollSpeed.y;
		this.material.SetTextureOffset("_MainTex", this.offset);
	}

	// Token: 0x04001E86 RID: 7814
	public Vector2 scrollSpeed = new Vector2(0.5f, 0.5f);

	// Token: 0x04001E87 RID: 7815
	private Vector2 offset;

	// Token: 0x04001E88 RID: 7816
	private Material material;
}
