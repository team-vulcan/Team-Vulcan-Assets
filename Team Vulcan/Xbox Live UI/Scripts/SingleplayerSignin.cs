//Copyright (c) Team Vulcan Studios
//All rights reserved
//Programmed by Tactical Laptop Bag

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.Client;

namespace TeamVulcan.Assets.XBL_UI.Singleplayer {
	public class SingleplayerSignin : MonoBehaviour {

		#region Singleton
		public static SingleplayerSignin Instance = null;

		private void Awake() {
			if(Instance == null){
				Instance = this;
			} else {
				Debug.LogError("Only one SingleplayerSignin can run at a time!");
				Destroy(gameObject);
			}
		}
		#endregion

		[Header("Sign in Prompt")]

		[Tooltip("Should we prompt the player to sign in? If this is turned on, the first time the player launches the game, a prompt will appear asking the player to sign in to XBL. If the player signs in, the prompt will not appear again and will automatically sign in every time the game is launched. If this is off, the game will not automatically sign in to XBL unless the player successfully signs into XBL once.")]
		[SerializeField] private bool promptSigninOnStart = true; //Read Tooltip
		[Tooltip("The UI that should be selected when the prompt appears. Default is the XBL_CopyButton in the prompt")]
		[SerializeField] private Selectable selectOnPrompt; //Read Tooltip
		[Tooltip("The UI that should be selected when the prompt closes. Default is null. This does not need to be assigned")]
		[SerializeField] private Selectable selectOnFinishPrompt; //Read Tooltip

		[Space]

		[Header("Object Assignments")]
		[Tooltip("The Canvas for the Initial Prompt")]
		[SerializeField] private Canvas initialPrompt; //Read Tooltip
		[Tooltip("The Animator for the Initial Prompt. Should be found on the Prompt Panel")]
		[SerializeField] private Animator initialPrompt_Anim; //Read Tooltip
		[Tooltip("The button in the corner for XBL sign in")]
		[SerializeField] private Button signinButton; // Read Tooltip
		[Tooltip("The TextMeshPro text of the button for XBL sign in")]
		[SerializeField] private TextMeshProUGUI signinButton_Text; //Read Tooltip

		[Space]

		[Header("Text")]
		[Tooltip("Mainly for locale. What shows on the XBL signin button while attempting to sign in. Do not add any dots as the script does this for you")]
		[Multiline] public string signingInText = "Signing in"; //Read Tooltip
		[Tooltip("Mainly for locale. What shows on the XBL signin button when the player has not signed into XBL")]
		[Multiline] public string signinToXBLText = "Sign in to \nXbox Live"; //Read Tooltip
		[Tooltip("Mainly for locale. What shows on the XBL signin button when the player is currently signed into XBL")]
		[Multiline] public string signedinAsText = "Signed in as: \n"; //Read Tooltip

		[Space]

		[Tooltip("Print more information out to console")]
		[SerializeField] private bool debug = false;

		//Fancy constructor I discovered for variables. { get; private set; } will allow an outside script to read the variable, but not change it,
		//while this script can change the variable
		/// <summary>
		/// The stored XboxLiveUser when signed into XBL.
		/// If not signed in, this is set to null.
		/// </summary>
		public XboxLiveUser XBL_User { get; private set; } = null;
		/// <summary>
		/// Returns a bool determining whether or not the player is signed into XBL
		/// </summary>
		public bool XBL_SignedIn { get; private set; } = false;
		/// <summary>
		/// Check this before allowing any other UI interaction. 
		/// This will make sure the player knows about XBL and allows the game to conform to Microsoft Store Policy 10.13.5:
		/// https://docs.microsoft.com/en-us/windows/uwp/publish/store-policies#1013-gaming-and-xbox
		/// </summary>
		public bool promptClosed { get; private set; } = false;

		/// <summary>
		/// The current instance of the XBL SignInManager
		/// This is set in Start()
		/// </summary>
		private SignInManager mSignInManager;
		/// <summary>
		/// Whether or not to put the 3 loading dots on the text of the button
		/// </summary>
		private bool signinAnim = false;

		private void Start() {
			mSignInManager = SignInManager.Instance; //Set the instance

			//https://docs.microsoft.com/en-us/gaming/xbox-live/get-started/setup-ide/creators/unity-win10/signin/sign-in-manager
			//Add callbacks to XBL sign in and out.
			try {
				mSignInManager.OnPlayerSignOut(1, OnXBLSignOut);
				mSignInManager.OnPlayerSignIn(1, OnXBLSignIn);
			} catch(Exception ex){
				Debug.LogError(ex);
			}

			bool signedInBefore = PlayerPrefs.GetInt("TeamVulcan.Assets.XBL_UI.Singleplayer.SignedInBefore", 0) == 1; //This is referenced twice (thrice if debug), might as well store it
			if (debug) { Debug.Log("Prompt: " + promptSigninOnStart + " signedinBefore: " + signedInBefore); }
			if (promptSigninOnStart && !signedInBefore){ //If player has already signed in before, no need to prompt again
				initialPrompt.enabled = true; //Canvas should be disabled by default
				initialPrompt_Anim.SetTrigger("Open"); //Cool open animation
				signinButton.interactable = false; //Prevent the corner button from interaction
				selectOnPrompt.Select();
			} else if(signedInBefore){ //If we aren't prompting but the player has signed in before, go ahead and do it
				promptClosed = true;
				AttemptSignin();
			} else { //Otherwise, we aren't prompting and we haven't signed in before. We can give the all clear to any other scripts.
				promptClosed = true;
			}
		}

		/// <summary>
		/// Use this instead of mSignInManager.SignInPlayer()
		/// </summary>
		private void AttemptSignin(){ 
			StartCoroutine(mSignInManager.SignInPlayer(1));
			signinButton_Text.text = signingInText+"...";
			signinAnim = true;
			StartCoroutine("SigningInAnim");
		}

		/// <summary>
		/// When any XBL login button is pushed, this should be invoked. 
		/// NOTE: SwitchUser does not seem to work in Editor... will check on platforms
		/// </summary>
		public void SignInButton(){
			if(XBL_SignedIn){
				StartCoroutine(mSignInManager.SwitchUser(1));
			} else {
				AttemptSignin();
			}
		}

		/// <summary>
		/// This is for the "No Thanks" button on the login prompt
		/// This simply starts the "ClosePrompt" Coroutine
		/// </summary>
		public void NoThanks(){
			StartCoroutine("ClosePrompt");
		}

		/// <summary>
		/// Makes the fun 3 dot animation thing for the button
		/// </summary>
		IEnumerator SigningInAnim(){
			while (signinAnim) {
				signinButton_Text.text = signingInText;
				yield return new WaitForSeconds(0.5f);
				if (!signinAnim) { break; }
				signinButton_Text.text = signingInText+".";
				yield return new WaitForSeconds(0.5f);
				if(!signinAnim) { break; }
				signinButton_Text.text = signingInText+"..";
				yield return new WaitForSeconds(0.5f);
				if (!signinAnim) { break; }
				signinButton_Text.text = signingInText+"...";
				yield return new WaitForSeconds(0.5f);
			}
		}

		/// <summary>
		/// This is set as a XBL Sign out callback in Start()
		/// This will invoke every time a player tries to sign out
		/// </summary>
		/// <param name="user">The XboxLiveUser that is signing out</param>
		/// <param name="status">How did the sign out go? Should be Succeeded if complete.</param>
		/// <param name="errorMessage">What went wrong if not Succeeded</param>
		private void OnXBLSignOut(XboxLiveUser user, XboxLiveAuthStatus status, string errorMessage){
			try {
				Debug.Log("Signed out: " + status);
				if(status == XboxLiveAuthStatus.Succeeded){
					XBL_User = null;
					XBL_SignedIn = false;
					signinButton_Text.text = signinToXBLText;
				}
			} catch(Exception ex){
				Debug.LogError(ex);
			}
		}

		/// <summary>
		/// This is set as a XBL Sign in callback in Start()
		/// This will invoke every time a player tries to sign in
		/// </summary>
		/// <param name="user">The XboxLiveUser that is signing in</param>
		/// <param name="status">How did the sign in go? Should be Succeeded if complete.</param>
		/// <param name="errorMessage">What went wrong if not Succeeded</param>
		private void OnXBLSignIn(XboxLiveUser user, XboxLiveAuthStatus status, string errorMessage) {
			try {
				signinAnim = false;
				Debug.Log("Signed in: " + status);
				if (status == XboxLiveAuthStatus.Succeeded) {
					PlayerPrefs.SetInt("TeamVulcan.Assets.XBL_UI.Singleplayer.SignedInBefore", 1);
					XBL_User = user;
					XBL_SignedIn = true;

					signinButton_Text.text = signedinAsText + user.Gamertag;

				} else if(!XBL_SignedIn) {
					signinButton_Text.text = signinToXBLText;
				}
				if (initialPrompt.enabled) {
					StartCoroutine("ClosePrompt");
				}
			} catch(Exception ex){
				Debug.LogError(ex);
			}
		}

		/// <summary>
		/// Simply closes the prompt and disables the canvas after the animation is complete
		/// </summary>
		IEnumerator ClosePrompt(){
			signinButton.interactable = true;
			promptClosed = true;
			if (selectOnFinishPrompt != null) { selectOnFinishPrompt.Select(); }
			initialPrompt_Anim.SetTrigger("Close");
			yield return new WaitForSeconds(1);
			initialPrompt.enabled = false;
		}

	}
}
