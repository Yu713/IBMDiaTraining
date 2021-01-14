using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Logging;
using IBM.Watson.TextToSpeech.V1;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.DataTypes;
using System;



public class SpeechOutputService : MonoBehaviour
{

    public Text ResponseTextField; // inspector slot for drag & drop of the Canvas > Text gameobject

    private TextToSpeechService myService;

    public string versionDate = "Date";
    public string apiKey = "API key"; 
    public string serviceUrl = "URL"; 

    public string myVoice = "en-US_MichaelVoice";

    private AudioClip AudioClip;
    private AudioSource audioSrc;
    private string _microphoneID = null;
    private AudioClip _recording = null;

    private float wait;
    private bool check;

    private DaimonManager dDaimonMgr;

    // Start is called before the first frame update
    void Start()
    {
        LogSystem.InstallDefaultReactors();
        audioSrc = GameObject.Find("02 FW").GetComponent<AudioSource>();
        dDaimonMgr = GetComponent<DaimonManager>();

        StartCoroutine(ConnectToTTSService());

    }
       
    private IEnumerator ConnectToTTSService()
    {
        TokenOptions myTokenOptions = new TokenOptions()
        {
            IamApiKey = apiKey
        };
        Credentials myCredentials = new Credentials(myTokenOptions, serviceUrl);
        while (!myCredentials.HasIamTokenData()) yield return null;

        myService = new TextToSpeechService(myCredentials);
       
    }

    public void Speak(string text)
    {

        Debug.Log("Sending to Watson to generate voice audio output: " + text);
        if (text != null && text != "")
        {
            myService.Synthesize(
                callback: onSynthCompleted,
                text: text,
                voice: myVoice,
                accept: "audio/wav"
               );

        } else
        {
            Debug.Log("WARNING: text to speech: text was empty");
        }

    }

    public void onSynthCompleted(DetailedResponse<byte[]> response, IBMError error)
    {
        
        byte[] synthesizeResponse = null;
        AudioClip clip = null;
        synthesizeResponse = response.Result;
        clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
        PlayClip(clip);
    }

     private void PlayClip(AudioClip clip)
    {

        //Debug.Log("Received audio file from Watson Text To Speech");

        if (Application.isPlaying && clip != null)
        {

            dDaimonMgr.mySpeechInputMgr.Active = false;

            audioSrc.spatialBlend = 0.0f;
            audioSrc.volume = 1.0f;
            audioSrc.loop = false;
            audioSrc.clip = clip;
            audioSrc.Play();

            dDaimonMgr.wait = clip.length; // may be useful to add +0.5f
            dDaimonMgr.check = true;
            Debug.Log("Speech output playing, set DaimonMgr waiting time for SpeechInput to reactivate again (when done) after " + clip.length + " seconds");

        } else
        {
            Debug.Log("ERROR: something is already playing or we did not get a clip.");
        }
    }



}