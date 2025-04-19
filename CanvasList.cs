using System;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class CanvasList : MonoBehaviour
{
	// Token: 0x060006D8 RID: 1752 RVA: 0x0004119D File Offset: 0x0003F39D
	private void Awake()
	{
		CanvasList.PopulateAvailableTextures();
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x000411A4 File Offset: 0x0003F3A4
	public static void PopulateAvailableTextures()
	{
		CanvasAssigner.AvailableTextures.Clear();
		Texture2D[] array = Resources.LoadAll<Texture2D>("Canvas");
		if (array.Length == 0)
		{
			Debug.LogWarning("No textures were loaded from the Resources/Canvas folder.");
		}
		foreach (Texture2D texture2D in array)
		{
			if (texture2D != null)
			{
				CanvasAssigner.AvailableTextures.Add(texture2D);
			}
			else
			{
				Debug.LogWarning("A texture was found but is not a Texture2D or could not be cast to Texture2D.");
			}
		}
	}
}
