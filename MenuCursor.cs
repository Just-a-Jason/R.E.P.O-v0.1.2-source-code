using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class MenuCursor : MonoBehaviour
{
	// Token: 0x060010A4 RID: 4260 RVA: 0x0009646C File Offset: 0x0009466C
	private void Start()
	{
		this.mesh = base.transform.GetChild(0).gameObject;
		base.transform.localScale = Vector3.zero;
		this.mesh.SetActive(false);
		MenuCursor.instance = this;
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x000964A7 File Offset: 0x000946A7
	public void OverridePosition(Vector3 _position)
	{
		base.transform.localPosition = new Vector3(_position.x, _position.y, 0f);
		this.overridePosTimer = 0.1f;
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000964D8 File Offset: 0x000946D8
	private void Update()
	{
		if (this.overridePosTimer <= 0f)
		{
			if (Cursor.lockState == CursorLockMode.None)
			{
				Vector2 vector = SemiFunc.UIMousePosToUIPos();
				base.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			}
		}
		else
		{
			this.overridePosTimer -= Time.deltaTime;
		}
		if (this.showTimer > 0f)
		{
			if (!this.mesh.activeSelf)
			{
				this.mesh.SetActive(true);
			}
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 30f);
			this.showTimer -= Time.deltaTime;
			return;
		}
		if (this.mesh.activeSelf)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.zero, Time.deltaTime * 30f);
			if (base.transform.localScale.magnitude < 0.1f && this.mesh.activeSelf)
			{
				this.mesh.SetActive(false);
			}
		}
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x00096603 File Offset: 0x00094803
	public void Show()
	{
		this.showTimer = 0.01f;
	}

	// Token: 0x04001BDD RID: 7133
	private float showTimer;

	// Token: 0x04001BDE RID: 7134
	private GameObject mesh;

	// Token: 0x04001BDF RID: 7135
	public static MenuCursor instance;

	// Token: 0x04001BE0 RID: 7136
	private float overridePosTimer;
}
