//Copyright (c) Team Vulcan Studios
//All rights reserved
//Programmed by Tactical Laptop Bag

using UnityEngine;
using TMPro;

namespace TeamVulcan.Assets.XBL_UI.Singleplayer {
	/// <summary>
	/// Random utility script needed for XBL UI. Simply replicates any text from one TMPro text object to another.
	/// </summary>
	public class CopyText : MonoBehaviour {

		[Tooltip("The TMPro text you want to copy from")]
		[SerializeField] private TextMeshProUGUI copyText;
		[Tooltip("The TMPro text you want to copy to")]
		[SerializeField] private TextMeshProUGUI myText;

		private void OnGUI() {
			myText.text = copyText.text;
		}

	}
}
