using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK;
using IBM.Watson.Assistant.V2;
using IBM.Cloud.SDK.DataTypes;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Logging;
using System;
using IBM.Watson.Assistant.V2.Model;
using FullSerializer;

public class DialogueService : MonoBehaviour
{

    public Text ResponseTextField; // inspector slot for drag & drop of the Canvas > Text gameobject
    private SpeechOutputService dSpeechOutputMgr;
    private SpeechInputService dSpeechInputMgr;

    private fsSerializer _serializer = new fsSerializer();

    [Space(10)]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string iamApikey = "API key";
    [Tooltip("The service URL (optional). This defaults to \"https://api.eu-gb.assistant.watson.cloud.ibm.com\"")]
    [SerializeField]
    private string serviceUrl = "URL";
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string versionDate = "Date";
    [Tooltip("The assistantId to run the example.")]
    [SerializeField]
    private string assistantId = "Assistant ID";

    public Animator anim;

    private AssistantService service;

    private string username;

    private bool createSessionTested = false;
    private bool deleteSessionTested = false;
    private string sessionId;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        dSpeechOutputMgr = GetComponent<SpeechOutputService>();

        dSpeechInputMgr = GetComponent<SpeechInputService>();
        dSpeechInputMgr.onInputReceived += OnInputReceived;

        Runnable.Run(CreateService());

    }

    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamApikey))
        {
            throw new IBMException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        Credentials credentials = null;

        //  Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = iamApikey
        };

        credentials = new Credentials(tokenOptions, serviceUrl);

        //  Wait for tokendata
        while (!credentials.HasIamTokenData())
            yield return null;

        service = new AssistantService(versionDate, credentials);

        Runnable.Run(CreateSession());

        //Runnable.Run(Examples());
    }

    private IEnumerator CreateSession()
    {
        //Log.Debug("ExampleAssistantV2.RunTest()", "Attempting to CreateSession");
        service.CreateSession(OnCreateSession, assistantId);

        while (!createSessionTested)
        {
            yield return null;
        }
    }

    private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
    {
        //Log.Debug("ExampleAssistantV2.OnDeleteSession()", "Session deleted.");
        deleteSessionTested = true;
    }


    public void SendMessageToAssistant(string theText)
    {
        Debug.Log("Sending to assistant service: " + theText);
        if (createSessionTested)
        {
            service.Message(OnResponseReceived, assistantId, sessionId, input: new MessageInput()
            {
                Text = theText,
                Options = new MessageInputOptions()
                {
                    ReturnContext = true
                }
            }
            );

        }
        else
        {
            Debug.Log("WARNING: trying to SendMessageToAssistant before session is established.");
        }

    }

    private void MakeDance()
    {
        Debug.Log(">>> Starting to dance");
        anim.Play("Waving", -1, 0);

    }

    private void OnResponseReceived(DetailedResponse<MessageResponse> response, IBMError error)
    {

        Debug.Log("DialogueService response: " + response.Result.Output.Generic[0].Text);

        Debug.Log("    -> " + response.Result.Output.Intents[0].Intent.ToString());
        string intent = response.Result.Output.Intents[0].Intent.ToString();
        if (intent == "MakeDance")
        {
            MakeDance(); 
        } else if (intent == "name")
        {
            // Debug.Log("    NAME === " + response.Result.Output.Entities[0].Entity.ToString() );
            username = response.Result.Output.Entities.Find( (x) => x.Entity.ToString()=="sys-person").Value.ToString();
            Debug.Log("username = " + username);
        }

        dSpeechOutputMgr.Speak(response.Result.Output.Generic[0].Text + ", " + username);
        //dSpeechInputMgr.Active = false;

        //myTTS.myVoice = "de-DE_DieterV3Voice";
        //myTTS.Speak(myTranslator.lastTranslationResult);
        //myTTS.myVoice = "en-GB_KateV3Voice";


        //  Convert resp to fsdata
        //fsData fsdata = null;
        //fsResult r = _serializer.TrySerialize(response.GetType(), response, out fsdata);
        //if (!r.Succeeded)
        //    throw new IBMException(r.FormattedMessages);

        ////  Convert fsdata to MessageResponse
        //MessageResponse messageResponse = new MessageResponse();
        //object obj = messageResponse;
        //r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
        //if (!r.Succeeded)
        //    throw new IBMException(r.FormattedMessages);

        //object _tempContext = null;
        //(resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);
        //if (_tempContext != null)
        //{

        //    _tempContext = _tempContext as Dictionary<string, object>;
        //}
        //else
        //{
        //    Log.Debug("ExampleConversation.Dialogue()", "Failed to get context");
        //}

        ////object tempIntentsObj = null;
        ////(response as Dictionary<string, object>).TryGetValue("intents", out tempIntentsObj);


        //object _tempText = null;
        //object _tempTextObj = (_tempText as List<object>)[0];
        //string output = _tempTextObj.ToString();
        //if (output != null)
        //{
        //    //replace any <waitX> tags with the value expected by the TTS service
        //    string replaceActionTags = output.ToString();
        //    int pos3 = replaceActionTags.IndexOf("<wait3>");
        //    if (pos3 != -1)
        //    {
        //        replaceActionTags = output.Replace("<wait3>", "<break time='3s'/>");
        //    }
        //    int pos4 = replaceActionTags.IndexOf("<wait4>");
        //    if (pos4 != -1)
        //    {
        //        replaceActionTags = output.Replace("<wait4>", "<break time='4s'/>");
        //    }
        //    int pos5 = replaceActionTags.IndexOf("<wait5>");
        //    if (pos5 != -1)
        //    {
        //        replaceActionTags = output.Replace("<wait5>", "<break time='5s'/>");
        //    }
        //    output = replaceActionTags;
        //}
        //else
        //{
        //    Log.Debug("Extract outputText", "Failed to extract outputText and set for speaking");
        //}
    }

    public void OnInputReceived(string text )
    {
        //Debug.Log("onInputReceived arrived in DialogueService: '" + text + "'");
        ResponseTextField.text = text;
        SendMessageToAssistant(text);
    }

    public void OnDestroy()
    {

        Debug.Log("DialogueService: deregestering callback for speech2text input");
        dSpeechInputMgr.onInputReceived -= OnInputReceived;

        Debug.Log("DialogueService: Attempting to delete session");
        service.DeleteSession(OnDeleteSession, assistantId, sessionId);

    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("ExampleAssistantV2.OnCreateSession()", "Session: {0}", response.Result.SessionId);
        sessionId = response.Result.SessionId;
        createSessionTested = true;
    }

}
