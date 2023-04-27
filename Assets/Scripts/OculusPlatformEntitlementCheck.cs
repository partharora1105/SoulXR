using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Platform;
using Oculus.Platform.Models;

#if UNITY_4 || UNITY_5
using UnityEngine.Events;
#endif
/*
    Step 1: Download & Install Oculus Platform SDK
    Step 2: Update your App ID in the Editor
    Step 3: Add this script or the sample prefab to the first scene in your build
    
    * Entitlement verification is required to distribute apps through the Oculus Store
    * The app must perform an Oculus Platform entitlement check within 10 seconds of launch
    * You may not allow the user to proceed in your app after a failed entitlement check.
    * You may wish to handle the situation more gracefully by showing the user a message stating that you were unable to verify their credentials, suggest that they check their internet connection, then quit the app

    * GearVR - If you’re testing your Gear VR app in the Unity editor you’ll need to set a test token. Select Oculus Platform / Platform Settings / Paste Token. You can retrieve your token from the API page on the Developer Center.
    * Standalone mode allows you to initialize the Platform SDK in test and development environments. 
    * Note: Initializing in standalone mode uses a different set of credentials from the other initialization processes. When initializing in standalone mode, use your Oculus developer account email and password.

    Oculus Platform Download (required)
    https://developer.oculus.com/downloads/package/oculus-platform-sdk/

    Update your app id by selecting 'Oculus Platform' -> 'Edit Settings' in the Unity Editor (required)

    Initializing and Checking Entitlements Info:
    https://developer.oculus.com/documentation/platform/latest/concepts/pgsg-get-started-with-sdk/
    https://developer.oculus.com/distribute/latest/concepts/publish-reqs-rift-security/
     
    App ID: Used to initialize the Platform SDK
    App Secret: This secret token is used to make requests to Oculus APIs on behalf of your app rather than a user.
    User Token: User tokens are needed any time your app calls an API to read, modify or write a specific person's Oculus data on their behalf.
*/

public class OculusPlatformEntitlementCheck : MonoBehaviour {

    [Tooltip("Show debug messages")]
    public bool debugMode = false;
    [Tooltip("Quit app on Entitlement Check Fail")]
    public bool quitOnFail = true;
    [Tooltip("Standalone mode allows you to initialize the Platform SDK in test and development environments")]
    private bool standaloneMode = false;

    private string appID = "";

    // init params for standalone mode
    struct OculusInitParams
    {
        public int sType;
        public string email;            // oculus developer account email
        public string password;         // oculus developer account password
        public System.UInt64 appId;
        public string uriPrefixOverride;
    };

    // run on awake
    void Awake()
    {
        //if(debugMode)
        //    Oculus.Platform.Core.LogMessages = true;

        // set the pc app id
        appID = Oculus.Platform.PlatformSettings.AppID;

        // if on mobile use the mobile app id
#if UNITY_ANDROID
        appID = Oculus.Platform.PlatformSettings.MobileAppID;
#endif

        // Keep this alive until finished checking
        DontDestroyOnLoad(this);

        // check for valid appID
        CheckAppID();

        // check if running in the first scene
        CheckScene();

        // Asynchronous method (recommended)
        if(!standaloneMode)
            Oculus.Platform.Core.AsyncInitialize();

        // Synchronous method
        // if(!standaloneMode)
        //      Oculus.Platform.Core.Initialize(appID);

        //if (standaloneMode)
            //Oculus.Platform.InitializeStandaloneOculus(OculusInitParams);

        // handle the callback message
        Oculus.Platform.Entitlements.IsUserEntitledToApplication().OnComplete(CheckCallback);
    }

    // check for valid appID
    private void CheckAppID()
    {
        bool badAppID = false;

        // handle bad app id
        if (appID == "")
        {
            Debug.LogError("Entitlement Check: Error! missing appID " + System.Environment.NewLine +
                " You can create a new application and obtain an App ID from the developer dashboard" + System.Environment.NewLine +
                " https://dashboard.oculus.com/");
            badAppID = true;
        }

        if(badAppID)                
            Debug.LogWarning("Invalid App ID");
    }

    // check if running in the first scene
    private void CheckScene()
    {
        // check to make sure we're running in the first scene to improve chance of checking within 10 seconds
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        if (sceneID == 0 && debugMode)
        {
            Debug.Log("Entitlement Check: Loaded in first scene");
        }
        else if (sceneID != 0 && debugMode)
        {
            Debug.LogWarning("Entitlement Check: Not loaded in first scene! " + sceneID);
        }
    }

    // handle the callback message
    private void CheckCallback(Oculus.Platform.Message msg)
    {
        if (!msg.IsError)
        {
            // Entitlement check passed
            if(debugMode)
                Debug.LogWarning("Entitlement Check: Passed");
        }
        else
        {
            // Entitlement check failed
            // NOTE: You may not allow the user to proceed in your app after a failed entitlement check.
            Debug.LogWarning("Entitlement Check: Failed!");
            Debug.Log("Entitlement Check: Core Initialized " + Oculus.Platform.Core.IsInitialized() );
            
            // time since startup check
            if (Time.realtimeSinceStartup > 10)
                Debug.LogWarning("Entitlement Check: Timeout. Must check within 10 seconds.");

            // default to quiting the application on faild entitlement check
            if (quitOnFail)
            {
                UnityEngine.Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        if (debugMode)        
            Debug.Log("Entitlement Check: " + Time.realtimeSinceStartup + " seconds");        

        FinishCheck();
    }

    // finish the check and cleanup
    private void FinishCheck()
    {
        if (debugMode)
            Debug.Log("Entitlement Check: Completed");

        //Oculus.Platform.Users.GetLoggedInUser().OnComplete(UpdateUser);

        Destroy(this);
    }

    private void UpdateUser(Message<User> message)
    {
        var msg = message;
        var user = msg.GetUser();
        this.GetComponent<Manager>().SetUserDisplayName(user.DisplayName);
    }

        // Note: With older versions of Unity, you may need to call Request.RunCallbacks() to process the callbacks and retrieve the results of the check.
#if UNITY_4 || UNITY_5
    public void Update()
    {
        Oculus.Platform.Request.RunCallbacks();
    }
#endif
    }