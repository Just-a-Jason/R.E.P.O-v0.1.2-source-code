using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class RecordingDirector : MonoBehaviour
{
	// Token: 0x060007CA RID: 1994 RVA: 0x0004BC04 File Offset: 0x00049E04
	private void Start()
	{
		if (RecordingDirector.instance != null && RecordingDirector.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RecordingDirector.instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0004BC40 File Offset: 0x00049E40
	private void Update()
	{
		if (!PlayerAvatar.instance.isDisabled)
		{
			this.playerLight.transform.position = PlayerAvatar.instance.PlayerVisionTarget.VisionTransform.position;
		}
		if (Input.GetKey(KeyCode.Keypad4))
		{
			float num;
			float num2;
			float num3;
			Color.RGBToHSV(this.playerLight.color, out num, out num2, out num3);
			num = (num + Time.deltaTime * 0.2f) % 1f;
			this.playerLight.color = Color.HSVToRGB(num, 1f, 1f);
		}
		if (Input.GetKey(KeyCode.Keypad6))
		{
			float num4;
			float num5;
			float num6;
			Color.RGBToHSV(this.playerLight.color, out num4, out num5, out num6);
			num4 = (num4 - Time.deltaTime * 0.2f) % 1f;
			this.playerLight.color = Color.HSVToRGB(num4, 1f, 1f);
		}
		if (Input.GetKey(KeyCode.Keypad8))
		{
			this.playerLight.range += Time.deltaTime * 30f;
		}
		if (Input.GetKey(KeyCode.Keypad2))
		{
			this.playerLight.range -= Time.deltaTime * 30f;
		}
		if (Input.GetKey(KeyCode.Keypad7))
		{
			this.playerLight.intensity += Time.deltaTime * 2f;
		}
		if (Input.GetKey(KeyCode.Keypad9))
		{
			this.playerLight.intensity -= Time.deltaTime * 2f;
		}
		if (Input.GetKey(KeyCode.Keypad0))
		{
			this.playerLight.intensity = 1f;
			this.playerLight.range = 10f;
			this.playerLight.color = Color.white;
		}
		if (!MenuPageEsc.instance && !ChatManager.instance.chatActive)
		{
			this.hideUI = true;
		}
		else
		{
			this.hideUI = false;
		}
		if (this.hideUI)
		{
			RenderTextureMain.instance.OverlayDisable();
		}
		FlashlightController.Instance.hideFlashlight = true;
		GameplayManager.instance.OverrideCameraAnimation(0f, 0.2f);
		if (SemiFunc.RunIsLobbyMenu() || SemiFunc.MenuLevel())
		{
			GameDirector.instance.CommandRecordingDirectorToggle();
		}
	}

	// Token: 0x04000DF4 RID: 3572
	internal bool hideUI;

	// Token: 0x04000DF5 RID: 3573
	public static RecordingDirector instance;

	// Token: 0x04000DF6 RID: 3574
	public Light playerLight;

	// Token: 0x04000DF7 RID: 3575
	private float lightHue;
}
