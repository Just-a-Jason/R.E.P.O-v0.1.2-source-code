using System;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class BuildManager : MonoBehaviour
{
	// Token: 0x0600090C RID: 2316 RVA: 0x000562E4 File Offset: 0x000544E4
	private void Awake()
	{
		if (!BuildManager.instance)
		{
			BuildManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			Debug.Log("VERSION: " + this.version.title);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0400107B RID: 4219
	public static BuildManager instance;

	// Token: 0x0400107C RID: 4220
	public Version version;
}
