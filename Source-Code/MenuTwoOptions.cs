using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001F5 RID: 501
public class MenuTwoOptions : MonoBehaviour
{
	// Token: 0x0600109B RID: 4251 RVA: 0x000960D8 File Offset: 0x000942D8
	private void Start()
	{
		if (this.option1TextMesh)
		{
			this.option1TextMesh.text = this.option1Text;
		}
		if (this.option1TextMesh)
		{
			this.option2TextMesh.text = this.option2Text;
		}
		this.StartFetch();
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00096128 File Offset: 0x00094328
	private void StartFetch()
	{
		if (this.customEvents && this.customFetch)
		{
			this.fetchSetting.Invoke();
		}
		else
		{
			bool flag = DataDirector.instance.SettingValueFetch(this.setting) == 1;
			this.startSettingFetch = flag;
		}
		if (this.startSettingFetch)
		{
			this.OnOption1();
		}
		else
		{
			this.OnOption2();
		}
		this.fetchComplete = true;
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x0009618C File Offset: 0x0009438C
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		if (this.option1TextMesh)
		{
			this.option1TextMesh.text = this.option1Text;
		}
		if (this.option1TextMesh)
		{
			this.option2TextMesh.text = this.option2Text;
		}
		TextMeshProUGUI componentInChildren = base.GetComponentInChildren<TextMeshProUGUI>();
		if (componentInChildren)
		{
			componentInChildren.text = this.settingName;
		}
		base.gameObject.name = "Bool Setting - " + this.settingName;
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x00096213 File Offset: 0x00094413
	private void OnEnable()
	{
		this.StartFetch();
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x0009621C File Offset: 0x0009441C
	private void Update()
	{
		if (!this.optionsBox)
		{
			return;
		}
		this.optionsBox.localPosition = Vector3.Lerp(this.optionsBox.localPosition, this.targetPosition, 20f * Time.deltaTime);
		this.optionsBox.localScale = Vector3.Lerp(this.optionsBox.localScale, this.targetScale / 10f, 20f * Time.deltaTime);
		this.optionsBoxBehind.localPosition = Vector3.Lerp(this.optionsBoxBehind.localPosition, this.targetPosition, 20f * Time.deltaTime);
		this.optionsBoxBehind.localScale = Vector3.Lerp(this.optionsBoxBehind.localScale, new Vector3(this.targetScale.x + 4f, this.targetScale.y + 2f, 1f) / 10f, 20f * Time.deltaTime);
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00096321 File Offset: 0x00094521
	public void SetTarget(Vector3 pos, Vector3 scale)
	{
		this.targetPosition = pos;
		this.targetScale = scale;
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x00096334 File Offset: 0x00094534
	public void OnOption1()
	{
		this.SetTarget(new Vector3(37.8f, 12.3f, 0f), new Vector3(73f, 22f, 1f));
		if (this.fetchComplete)
		{
			if (this.customEvents)
			{
				if (this.settingSet)
				{
					DataDirector.instance.SettingValueSet(this.setting, 1);
				}
				this.onOption1.Invoke();
				return;
			}
			DataDirector.instance.SettingValueSet(this.setting, 1);
		}
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x000963B8 File Offset: 0x000945B8
	public void OnOption2()
	{
		this.SetTarget(new Vector3(112.644f, 12.3f, 0f), new Vector3(74f, 22f, 1f));
		if (this.fetchComplete)
		{
			if (this.customEvents)
			{
				if (this.settingSet)
				{
					DataDirector.instance.SettingValueSet(this.setting, 0);
				}
				this.onOption2.Invoke();
				return;
			}
			DataDirector.instance.SettingValueSet(this.setting, 0);
		}
	}

	// Token: 0x04001BCB RID: 7115
	public string option1Text = "ON";

	// Token: 0x04001BCC RID: 7116
	public string option2Text = "OFF";

	// Token: 0x04001BCD RID: 7117
	public RectTransform optionsBox;

	// Token: 0x04001BCE RID: 7118
	public RectTransform optionsBoxBehind;

	// Token: 0x04001BCF RID: 7119
	public Vector3 targetPosition;

	// Token: 0x04001BD0 RID: 7120
	public Vector3 targetScale;

	// Token: 0x04001BD1 RID: 7121
	public DataDirector.Setting setting;

	// Token: 0x04001BD2 RID: 7122
	public bool customEvents = true;

	// Token: 0x04001BD3 RID: 7123
	public bool settingSet;

	// Token: 0x04001BD4 RID: 7124
	public bool customFetch = true;

	// Token: 0x04001BD5 RID: 7125
	public UnityEvent onOption1;

	// Token: 0x04001BD6 RID: 7126
	public UnityEvent onOption2;

	// Token: 0x04001BD7 RID: 7127
	public UnityEvent fetchSetting;

	// Token: 0x04001BD8 RID: 7128
	public TextMeshProUGUI option1TextMesh;

	// Token: 0x04001BD9 RID: 7129
	public TextMeshProUGUI option2TextMesh;

	// Token: 0x04001BDA RID: 7130
	public bool startSettingFetch = true;

	// Token: 0x04001BDB RID: 7131
	private bool fetchComplete;

	// Token: 0x04001BDC RID: 7132
	public string settingName;
}
