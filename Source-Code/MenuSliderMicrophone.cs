using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200020A RID: 522
public class MenuSliderMicrophone : MonoBehaviour
{
	// Token: 0x06001121 RID: 4385 RVA: 0x00098F32 File Offset: 0x00097132
	private void Awake()
	{
		this.menuSlider = base.GetComponent<MenuSlider>();
		this.micData = new float[128];
		this.SetOptions();
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x00098F58 File Offset: 0x00097158
	private void Update()
	{
		if (this.currentDeviceCount != SessionManager.instance.micDeviceList.Count)
		{
			this.SetOptions();
			return;
		}
		if (this.menuSlider.bigSettingText.textMeshPro.text != "device name" && SessionManager.instance.micDeviceCurrent != this.menuSlider.bigSettingText.textMeshPro.text)
		{
			SessionManager.instance.micDeviceCurrent = this.menuSlider.bigSettingText.textMeshPro.text;
			DataDirector.instance.micDevice = SessionManager.instance.micDeviceCurrent;
			DataDirector.instance.SaveSettings();
		}
		if (!PlayerVoiceChat.instance)
		{
			if (SessionManager.instance.micDeviceCurrent != this.currentDeviceName)
			{
				Microphone.End(this.currentDeviceName);
				this.currentDeviceName = SessionManager.instance.micDeviceCurrent;
				this.microphoneClipEnabled = false;
			}
			if (this.currentDeviceName != "NONE")
			{
				if (!this.microphoneClipEnabled)
				{
					bool flag = false;
					string[] devices = Microphone.devices;
					for (int i = 0; i < devices.Length; i++)
					{
						if (devices[i] == this.currentDeviceName)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						this.microphoneClipEnabled = true;
						this.microphoneClip = Microphone.Start(this.currentDeviceName, true, 1, 44100);
					}
				}
				if (this.microphoneClipEnabled)
				{
					int num = Microphone.GetPosition(this.currentDeviceName) - 128 + 1;
					if (num < 0)
					{
						return;
					}
					this.microphoneClip.GetData(this.micData, num);
					float num2 = 0f;
					for (int j = 0; j < this.micData.Length; j++)
					{
						num2 += this.micData[j] * this.micData[j];
					}
					this.micLevel = Mathf.Sqrt(num2 / (float)this.micData.Length) * this.micGain;
					this.micLevel = Mathf.Clamp01(this.micLevel);
				}
			}
			if (!this.microphoneClipEnabled)
			{
				this.micLevel = 0f;
			}
		}
		else
		{
			this.micLevel = PlayerVoiceChat.instance.clipLoudnessNoTTS * 5f;
		}
		this.micLevelBar.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Lerp(this.micLevelBar.GetComponent<RectTransform>().localScale.x, this.micLevel, Time.deltaTime * 10f), 0.2f, 1f);
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x000991CC File Offset: 0x000973CC
	private void SetOptions()
	{
		this.menuSlider.customOptions.Clear();
		foreach (string optionText in SessionManager.instance.micDeviceList)
		{
			this.menuSlider.CustomOptionAdd(optionText, this.micEvent);
		}
		this.menuSlider.CustomOptionAdd("NONE", this.micEvent);
		foreach (MenuSlider.CustomOption customOption in this.menuSlider.customOptions)
		{
			customOption.customValueInt = this.menuSlider.customOptions.IndexOf(customOption);
		}
		this.currentDeviceCount = SessionManager.instance.micDeviceList.Count;
	}

	// Token: 0x04001C62 RID: 7266
	private MenuSlider menuSlider;

	// Token: 0x04001C63 RID: 7267
	public UnityEvent micEvent;

	// Token: 0x04001C64 RID: 7268
	public Image micLevelBar;

	// Token: 0x04001C65 RID: 7269
	private AudioClip microphoneClip;

	// Token: 0x04001C66 RID: 7270
	private bool microphoneClipEnabled;

	// Token: 0x04001C67 RID: 7271
	private string currentDeviceName;

	// Token: 0x04001C68 RID: 7272
	private int currentDeviceCount;

	// Token: 0x04001C69 RID: 7273
	private const int sampleDataLength = 128;

	// Token: 0x04001C6A RID: 7274
	private float[] micData;

	// Token: 0x04001C6B RID: 7275
	private float micLevel;

	// Token: 0x04001C6C RID: 7276
	private float micGain = 10f;
}
