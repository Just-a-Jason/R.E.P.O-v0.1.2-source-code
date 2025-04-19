using System;
using LeastSquares.Overtone;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class TTSDirector : MonoBehaviour
{
	// Token: 0x06000A81 RID: 2689 RVA: 0x0005CD4E File Offset: 0x0005AF4E
	private void Start()
	{
		TTSDirector.instance = this;
		this.engine = base.GetComponent<TTSEngine>();
	}

	// Token: 0x0400110C RID: 4364
	public static TTSDirector instance;

	// Token: 0x0400110D RID: 4365
	internal TTSEngine engine;
}
