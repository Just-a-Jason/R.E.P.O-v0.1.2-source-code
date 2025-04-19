using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class CursorManager : MonoBehaviour
{
	// Token: 0x06000919 RID: 2329 RVA: 0x00056ACD File Offset: 0x00054CCD
	private void Awake()
	{
		if (!CursorManager.instance)
		{
			CursorManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00056AF8 File Offset: 0x00054CF8
	private void Update()
	{
		if (this.unlockTimer > 0f)
		{
			if (MenuCursor.instance)
			{
				MenuCursor.instance.Show();
			}
			this.unlockTimer -= Time.deltaTime;
			return;
		}
		if (this.unlockTimer != -1234f)
		{
			Cursor.lockState = CursorLockMode.Locked;
			this.unlockTimer = -1234f;
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00056B59 File Offset: 0x00054D59
	public void Unlock(float _time)
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
		this.unlockTimer = _time;
	}

	// Token: 0x04001092 RID: 4242
	public static CursorManager instance;

	// Token: 0x04001093 RID: 4243
	private float unlockTimer;
}
