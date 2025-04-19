using System;
using TMPro;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class TuckScreenLocked : MonoBehaviour
{
	// Token: 0x0600089C RID: 2204 RVA: 0x00052838 File Offset: 0x00050A38
	private void Update()
	{
		new Color(1f, 0.2f, 0f);
		this.isLocked = true;
		if (!this.isLocked)
		{
			this.text.text = this.lockedText;
			return;
		}
		this.cycleTimer += Time.deltaTime;
		if (this.cycleTimer >= this.cycleInterval)
		{
			this.cycleTimer = 0f;
			this.textIndex++;
			if (this.textIndex >= this.textPhases.Length)
			{
				this.textIndex = 0;
			}
		}
		if (this.textIndex == -1)
		{
			this.text.text = this.lockedText + this.textPhases[0];
			return;
		}
		this.text.text = this.lockedText + this.textPhases[this.textIndex];
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0005291C File Offset: 0x00050B1C
	public void LockChatToggle(bool _lock, string _lockedText = "", Color _lightColor = default(Color), Color _darkColor = default(Color))
	{
		this.isLocked = _lock;
		this.text.color = _lightColor;
		string text = ColorUtility.ToHtmlStringRGB(_darkColor);
		string text2 = ColorUtility.ToHtmlStringRGB(_lightColor);
		this.textPhases = new string[]
		{
			string.Concat(new string[]
			{
				"<color=#",
				text,
				">.</color><color=#",
				text,
				">.</color><color=#",
				text,
				">.</color>"
			}),
			string.Concat(new string[]
			{
				"<color=#",
				text2,
				">.</color><color=#",
				text,
				">.</color><color=#",
				text,
				">.</color>"
			}),
			string.Concat(new string[]
			{
				"<color=#",
				text2,
				">.</color><color=#",
				text2,
				">.</color><color=#",
				text,
				">.</color>"
			}),
			string.Concat(new string[]
			{
				"<color=#",
				text2,
				">.</color><color=#",
				text2,
				">.</color><color=#",
				text2,
				">.</color>"
			})
		};
		if (this.isLocked)
		{
			this.lockedText = _lockedText;
			this.scanLines.material.color = _lightColor;
			this.enableScreenLock.SetActive(true);
			this.cycleTimer = 0f;
			this.textIndex = -1;
			this.text.text = this.lockedText;
			return;
		}
		this.lockedText = "";
		this.enableScreenLock.SetActive(false);
		this.text.text = "";
	}

	// Token: 0x04000F97 RID: 3991
	public TextMeshProUGUI text;

	// Token: 0x04000F98 RID: 3992
	public MeshRenderer scanLines;

	// Token: 0x04000F99 RID: 3993
	public GameObject enableScreenLock;

	// Token: 0x04000F9A RID: 3994
	internal bool isLocked;

	// Token: 0x04000F9B RID: 3995
	private string lockedText = "";

	// Token: 0x04000F9C RID: 3996
	[SerializeField]
	private float cycleInterval = 0.5f;

	// Token: 0x04000F9D RID: 3997
	private float cycleTimer;

	// Token: 0x04000F9E RID: 3998
	private int textIndex = -1;

	// Token: 0x04000F9F RID: 3999
	private string[] textPhases = new string[]
	{
		"<color=#2B0050>.</color><color=#2B0050>.</color><color=#2B0050>.</color>",
		"<color=#FF0000>.</color><color=#2B0050>.</color><color=#2B0050>.</color>",
		"<color=#FF0000>.</color><color=#FF0000>.</color><color=#2B0050>.</color>",
		"<color=#FF0000>.</color><color=#FF0000>.</color><color=#FF0000>.</color>"
	};
}
