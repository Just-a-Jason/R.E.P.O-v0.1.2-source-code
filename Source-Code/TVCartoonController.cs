using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class TVCartoonController : MonoBehaviour
{
	// Token: 0x0600120B RID: 4619 RVA: 0x0009FA18 File Offset: 0x0009DC18
	private void Start()
	{
		this.TVStaticOutro = false;
		this.TVActivated = false;
		this.TVBackground.enabled = false;
		this.TVStatic.enabled = false;
		this.TVStaticIntro = true;
		this.TVActiveTime = Random.Range(this.TVActiveTimeMin, this.TVActiveTimeMax);
		this.TVLight.enabled = false;
		this.CatObject.SetActive(false);
		this.MouseObject.SetActive(false);
		this.CatMaterial = this.CatObject.GetComponent<Renderer>().material;
		this.MouseMaterial = this.MouseObject.GetComponent<Renderer>().material;
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			this.isLocal = true;
		}
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0009FAD8 File Offset: 0x0009DCD8
	private IEnumerator AnimationCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.updateInterval);
			float num = this.updateInterval;
			if (this.TrapDone)
			{
				break;
			}
			float y = 0.5f;
			if (this.state == this.stateCatTalk)
			{
				y = 0f;
			}
			if (this.CatMaterial.mainTextureOffset.x < 1f)
			{
				this.CatMaterial.mainTextureOffset = new Vector2(this.CatMaterial.mainTextureOffset.x + 0.33f, y);
			}
			else
			{
				this.CatMaterial.mainTextureOffset = new Vector2(0f, y);
			}
			float y2 = 0.5f;
			if (this.state == this.stateMouseTalk)
			{
				y2 = 0f;
			}
			if (this.CatMaterial.mainTextureOffset.x < 1f)
			{
				this.MouseMaterial.mainTextureOffset = new Vector2(this.MouseMaterial.mainTextureOffset.x + 0.33f, y2);
			}
			else
			{
				this.MouseMaterial.mainTextureOffset = new Vector2(0f, y2);
			}
			if (this.state == this.stateRunning)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.runTime)
				{
					this.Timer = 0f;
					this.state = this.stateCatTalk;
					this.catTalkSound1.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
			}
			if (this.state == this.stateCatTalk)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.catTalkTime)
				{
					this.Timer = 0f;
					this.state = this.stateCatTalkPause;
					this.catTalkCounter++;
				}
			}
			if (this.state == this.stateCatTalkPause)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.catTalkPauseTime)
				{
					this.Timer = 0f;
					if (this.catTalkCounter < this.catTalkCount)
					{
						this.state = this.stateCatTalk;
						this.catTalkSound2.Play(base.transform.position, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.catTalkCounter = 0;
						this.state = this.stateMouseTalk;
						this.mouseTalkSound1.Play(base.transform.position, 1f, 1f, 1f, 1f);
					}
				}
			}
			if (this.state == this.stateMouseTalk)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.mouseTalkTime)
				{
					this.Timer = 0f;
					this.state = this.stateMouseTalkPause;
					this.mouseTalkCounter++;
				}
			}
			if (this.state == this.stateMouseTalkPause)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.mouseTalkPauseTime)
				{
					this.Timer = 0f;
					if (this.mouseTalkCounter < this.mouseTalkCount)
					{
						this.state = this.stateMouseTalk;
						this.mouseTalkSound2.Play(base.transform.position, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.mouseTalkCounter = 0;
						this.state = this.stateRunning;
					}
				}
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0009FAE8 File Offset: 0x0009DCE8
	private void Update()
	{
		this.LoopSound.PlayLoop(this.TVActivated, 0.9f, 0.9f, 1f);
		if (this.TVActivated)
		{
			if (this.isLocal)
			{
				this.TVActiveTime -= Time.deltaTime;
			}
			if (this.TVActiveTime <= this.TVStaticTime)
			{
				bool tvstaticOutro = this.TVStaticOutro;
			}
			if (this.TVStart)
			{
				this.TVBackground.enabled = true;
				this.TVLight.enabled = true;
				this.TVStart = false;
				this.StartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.CatObject.SetActive(true);
				this.MouseObject.SetActive(true);
				base.StartCoroutine(this.AnimationCoroutine());
			}
			if (this.TVStaticIntro || this.TVStaticOutro)
			{
				float num = this.TVStaticCurve.Evaluate(this.TVStaticTimer / this.TVStaticTime);
				this.TVStaticTimer += 1f * Time.deltaTime * this.speedMulti;
				if (num > 0.5f)
				{
					this.TVStatic.enabled = true;
				}
				else
				{
					this.TVStatic.enabled = false;
				}
				if (this.TVStaticTimer > this.TVStaticTime)
				{
					this.TVStaticIntro = false;
					this.TVStaticTimer = 0f;
					this.TVStatic.enabled = false;
				}
			}
			float tvactiveTime = this.TVActiveTime;
		}
	}

	// Token: 0x04001E89 RID: 7817
	public GameObject TVScreen;

	// Token: 0x04001E8A RID: 7818
	public float runTime = 10f;

	// Token: 0x04001E8B RID: 7819
	public float TVActiveTimeMin = 20f;

	// Token: 0x04001E8C RID: 7820
	public float TVActiveTimeMax = 35f;

	// Token: 0x04001E8D RID: 7821
	private float TVActiveTime;

	// Token: 0x04001E8E RID: 7822
	private float Timer;

	// Token: 0x04001E8F RID: 7823
	private float speedMulti = 1f;

	// Token: 0x04001E90 RID: 7824
	public MeshRenderer TVBackground;

	// Token: 0x04001E91 RID: 7825
	public GameObject CatObject;

	// Token: 0x04001E92 RID: 7826
	public GameObject MouseObject;

	// Token: 0x04001E93 RID: 7827
	public MeshRenderer TVStatic;

	// Token: 0x04001E94 RID: 7828
	public Light TVLight;

	// Token: 0x04001E95 RID: 7829
	public AnimationCurve TVStaticCurve;

	// Token: 0x04001E96 RID: 7830
	public float TVStaticTime = 0.5f;

	// Token: 0x04001E97 RID: 7831
	public float TVStaticTimer;

	// Token: 0x04001E98 RID: 7832
	public TrapTV trapTV;

	// Token: 0x04001E99 RID: 7833
	[Space]
	[Header("___________________ Cartoon Cat Talk ___________________")]
	public float catTalkTime = 10f;

	// Token: 0x04001E9A RID: 7834
	public float catTalkPauseTime = 2f;

	// Token: 0x04001E9B RID: 7835
	public int catTalkCount = 2;

	// Token: 0x04001E9C RID: 7836
	private int catTalkCounter;

	// Token: 0x04001E9D RID: 7837
	public float catTalkStartScale = 0.2f;

	// Token: 0x04001E9E RID: 7838
	public float catTalkEndScale = 0.277f;

	// Token: 0x04001E9F RID: 7839
	public Sound catTalkSound1;

	// Token: 0x04001EA0 RID: 7840
	public Sound catTalkSound2;

	// Token: 0x04001EA1 RID: 7841
	[Space]
	[Header("___________________ Cartoon Mouse Talk ___________________")]
	public float mouseTalkTime = 10f;

	// Token: 0x04001EA2 RID: 7842
	public float mouseTalkPauseTime = 2f;

	// Token: 0x04001EA3 RID: 7843
	public int mouseTalkCount = 2;

	// Token: 0x04001EA4 RID: 7844
	private int mouseTalkCounter;

	// Token: 0x04001EA5 RID: 7845
	public float mouseTalkStartScale = 0.12f;

	// Token: 0x04001EA6 RID: 7846
	public float mouseTalkEndScale = 0.155f;

	// Token: 0x04001EA7 RID: 7847
	public Sound mouseTalkSound1;

	// Token: 0x04001EA8 RID: 7848
	public Sound mouseTalkSound2;

	// Token: 0x04001EA9 RID: 7849
	private int state;

	// Token: 0x04001EAA RID: 7850
	private int stateRunning;

	// Token: 0x04001EAB RID: 7851
	private int stateCatTalk = 1;

	// Token: 0x04001EAC RID: 7852
	private int stateCatTalkPause = 2;

	// Token: 0x04001EAD RID: 7853
	private int stateMouseTalk = 3;

	// Token: 0x04001EAE RID: 7854
	private int stateMouseTalkPause = 4;

	// Token: 0x04001EAF RID: 7855
	private float updateInterval = 0.083333336f;

	// Token: 0x04001EB0 RID: 7856
	private bool TVStaticIntro = true;

	// Token: 0x04001EB1 RID: 7857
	public bool TVStaticOutro;

	// Token: 0x04001EB2 RID: 7858
	public bool isLocal;

	// Token: 0x04001EB3 RID: 7859
	[Space]
	[Header("___________________ TV Sounds ___________________")]
	public Sound LoopSound;

	// Token: 0x04001EB4 RID: 7860
	public Sound StartSound;

	// Token: 0x04001EB5 RID: 7861
	public Sound StopSound;

	// Token: 0x04001EB6 RID: 7862
	[HideInInspector]
	public bool TVActivated;

	// Token: 0x04001EB7 RID: 7863
	[HideInInspector]
	public bool TrapDone;

	// Token: 0x04001EB8 RID: 7864
	private bool TVStart = true;

	// Token: 0x04001EB9 RID: 7865
	private Material CatMaterial;

	// Token: 0x04001EBA RID: 7866
	private Material MouseMaterial;

	// Token: 0x04001EBB RID: 7867
	private PhotonView photonView;
}
