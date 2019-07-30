# Team Vulcan Shared Assets

These Assets were created to make developing new games easier for ourselves. There are different folders containing different types of assets.
The assets must follow Microsoft Store Gaming and Xbox Policies as set by Microsoft. These policies can be found [here](https://docs.microsoft.com/en-us/windows/uwp/publish/store-policies#1013-gaming-and-xbox)

To download, go to [the releases tab](https://github.com/team-vulcan/Team-Vulcan-Assets/releases/)

# Xbox Live Singleplayer UI

The XBL (Xbox Live) Singleplayer UI allows the developer to drop a prefab into an existing Canvas which can then be configured to the developer's liking.
The Singleplayer UI must follow MSP (Microsoft Store Policy) 10.13.5. Attempting to put this UI into a multiplayer game will violate MSP 10.13.6 and the game 
will be taken down for either not implementing parental controls or allowing the player to continue to gameplay without signing in to XBL.

## Dependencies

Xbox Live Singleplayer UI uses TextMeshPro and [XBL SDK](https://github.com/microsoft/xbox-live-unity-plugin).
You may also need to import a package from Unity Tools for Visual Studio if you are getting errors from the XBL SDK. This can occurr if you install
Visual Studio from online rather than through the Unity Installer. Go to `C:\Program Files (x86)\Microsoft Visual Studio Tools for Unity` and look
around for a `.unitypackage`. Import that package into your project and you should be good to go.

TextMeshPro is built into Unity in the package manager.

# Copyright

Copyright© Team Vulcan Studios 2019
All Rights Reserved
