using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001FB RID: 507
public class MenuButtonPopUp : MonoBehaviour
{
	// Token: 0x04001BF3 RID: 7155
	public UnityEvent option1Event;

	// Token: 0x04001BF4 RID: 7156
	public UnityEvent option2Event;

	// Token: 0x04001BF5 RID: 7157
	public string headerText = "Oh really?";

	// Token: 0x04001BF6 RID: 7158
	public Color headerColor = new Color(1f, 0.55f, 0f);

	// Token: 0x04001BF7 RID: 7159
	[TextArea(3, 10)]
	public string bodyText = "Is that really so?";

	// Token: 0x04001BF8 RID: 7160
	public string option1Text = "Yes!";

	// Token: 0x04001BF9 RID: 7161
	public string option2Text = "No";
}
