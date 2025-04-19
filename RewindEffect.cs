using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000265 RID: 613
public class RewindEffect : MonoBehaviour
{
	// Token: 0x0600131B RID: 4891 RVA: 0x000A73BE File Offset: 0x000A55BE
	private void Awake()
	{
		RewindEffect.Instance = this;
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x000A73C6 File Offset: 0x000A55C6
	private void Start()
	{
		this.RewindEffectUI.SetActive(false);
		this.RewindLines.SetActive(false);
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x000A73E0 File Offset: 0x000A55E0
	private void Update()
	{
		if (this.PlayRewind)
		{
			GameDirector.instance.SetDisableInput(0.5f);
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
		}
		this.RewindLoopSound.PlayLoop(this.PlayRewind, 0.9f, 0.9f, 1f);
		if (!this.PlayRewind && !this.RewindEnd && Vector3.Distance(this.PlayerTransfrom.position, this.lastScreenshotPosition) > this.movementThreshold)
		{
			if (!this.FirstStep)
			{
				this.CaptureScreenshot();
				this.lastScreenshotPosition = this.PlayerTransfrom.position;
				return;
			}
			this.ClearScreenshots();
			this.FirstStep = false;
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000A7498 File Offset: 0x000A5698
	private void CaptureScreenshot()
	{
		if (this.screenshots.Count >= this.maxScreenshots)
		{
			Object.Destroy(this.screenshots[0]);
			this.screenshots.RemoveAt(0);
			this.TimeSnapshots.RemoveAt(0);
		}
		Texture texture = this.RenderTextureMain.texture;
		RenderTexture renderTexture = new RenderTexture(128, 72, 24);
		renderTexture.Create();
		Graphics.Blit(texture, renderTexture);
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(128, 72, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, 128f, 72f), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		this.screenshots.Add(texture2D);
		this.TimeSnapshots.Add(this.Timecode.GetSnapshot());
		Object.Destroy(renderTexture);
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x000A7574 File Offset: 0x000A5774
	public void PlayRewindEffect()
	{
		if (this.screenshots.Count >= 10)
		{
			this.RewindStartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.PlayRewind = true;
			this.RewindLines.SetActive(true);
			this.RewindEffectUI.SetActive(true);
			base.StartCoroutine(this.RewindCoroutine());
			return;
		}
		this.RewindEnding();
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x000A75EE File Offset: 0x000A57EE
	private IEnumerator RewindCoroutine()
	{
		Image rewindImage = this.RewindEffectUI.GetComponent<Image>();
		rewindImage.enabled = true;
		float displayTimePerScreenshot = this.rewindDuration / (float)this.screenshots.Count;
		int num;
		for (int i = this.screenshots.Count - 1; i >= 0; i = num - 1)
		{
			this.Timecode.SetTime(this.TimeSnapshots[i]);
			Texture2D screenshot = this.screenshots[i];
			Sprite sprite = Sprite.Create(screenshot, new Rect(0f, 0f, (float)screenshot.width, (float)screenshot.height), new Vector2(0.5f, 0.5f));
			rewindImage.sprite = sprite;
			yield return new WaitForSeconds(displayTimePerScreenshot);
			Object.Destroy(sprite);
			Object.Destroy(screenshot);
			screenshot = null;
			sprite = null;
			num = i;
		}
		this.RewindEnding();
		yield break;
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x000A7600 File Offset: 0x000A5800
	public void ClearScreenshots()
	{
		foreach (Texture2D obj in this.screenshots)
		{
			Object.Destroy(obj);
		}
		this.screenshots.Clear();
		this.lastScreenshotPosition = this.PlayerTransfrom.position;
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x000A766C File Offset: 0x000A586C
	public void RewindEnding()
	{
		this.Timecode.SetToStartSnapshot();
		this.RewindEndSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.PlayRewind = false;
		this.ClearScreenshots();
		this.RewindLines.SetActive(false);
		this.lastScreenshotPosition = this.PlayerTransfrom.position;
		this.FirstStep = true;
	}

	// Token: 0x04002060 RID: 8288
	public static RewindEffect Instance;

	// Token: 0x04002061 RID: 8289
	public Timecode Timecode;

	// Token: 0x04002062 RID: 8290
	[HideInInspector]
	public List<Timecode.TimeSnapshot> TimeSnapshots;

	// Token: 0x04002063 RID: 8291
	[Space]
	public int maxScreenshots = 50;

	// Token: 0x04002064 RID: 8292
	public float movementThreshold = 3f;

	// Token: 0x04002065 RID: 8293
	public Transform PlayerTransfrom;

	// Token: 0x04002066 RID: 8294
	private List<Texture2D> screenshots = new List<Texture2D>();

	// Token: 0x04002067 RID: 8295
	public GameObject RewindEffectUI;

	// Token: 0x04002068 RID: 8296
	private Vector3 lastScreenshotPosition;

	// Token: 0x04002069 RID: 8297
	[HideInInspector]
	public bool PlayRewind;

	// Token: 0x0400206A RID: 8298
	public float rewindDuration = 1.5f;

	// Token: 0x0400206B RID: 8299
	private bool RewindEnd;

	// Token: 0x0400206C RID: 8300
	private bool FirstStep = true;

	// Token: 0x0400206D RID: 8301
	public GameObject RewindLines;

	// Token: 0x0400206E RID: 8302
	public RawImage RenderTextureMain;

	// Token: 0x0400206F RID: 8303
	public Sound RewindStartSound;

	// Token: 0x04002070 RID: 8304
	public Sound RewindEndSound;

	// Token: 0x04002071 RID: 8305
	public Sound RewindLoopSound;
}
