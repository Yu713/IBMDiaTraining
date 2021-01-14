# DAIMON: Dialogue-oriented Artificial Intelligence deMONstrator

DAIMON is a demonstrator for holographic AIs written by Xinyu Huang and Prof Dr Fridolin Wild.
The 3D character model is a (younger, beardless) version of Fridolin, which we reconstructed from a 3D scan following the process documented in [this paper][1], see [preprint][2].
It can easily be exchanged for other, rigged character models.

DAIMON consists of several service wrappers, most notably the DaimonManager script, which ties all other services together.
The services it utilises are following a pipeline, listening to speech input (SpeechInputService), 
possibly using a translation service (LangTransService), to then communicate with an 
assistant using the transcribed, recognised, possibly translated text (DialogueService). 

Responses received from the assistant are then converted via the SpeechOutputService to an 
audio clip which is played to the user, pausing the speech input service to avoid loops of
the assistant listening to himself. All services are attached to an empty game object 
called WatsonServices in Scenes/MainWatsonScene. The scripts of the services are all located 
in the project browser under _scripts.

## IBM Watson installation

We have left the Watson version we used in the source tree. The IBM Watson Unity SDK and the IBM Unity SDK core both come under an Apache 2.0 open source license which allows that. 

Should you ever have to update to a newer version, you would download the latest IBM Watson Unity SDK release from [here](https://github.com/watson-developer-cloud/unity-sdk) and the IBM Unity SDK core from [there](https://github.com/IBM/unity-sdk-core), and follow their installation instruction (basically moving it to the Assets folder).

## IBM Watson configuration

DAIMON uses IBM Watson, so you have to register a free IBM cloud account and instantiate the services for:

* Speech to Text
* Text to Speech
* Watson Assistant
* language-translator-unity-demo (optional)

Then you have to create a file named "ibm-credentials.env" in the root folder of the project,
adding all your IBM Watson credentials. The file is in the .gitignore, so will not be syndicated
with the source tree.

This will look something like:

```
TONE_ANALYZER_APIKEY=xxx
TONE_ANALYZER_IAM_APIKEY=xxx
TONE_ANALYZER_URL=https://gateway-lon.watsonplatform.net/tone-analyzer/api
TONE_ANALYZER_AUTH_TYPE=iam
ASSISTANT_APIKEY=yyy
ASSISTANT_IAM_APIKEY=yyy
ASSISTANT_URL=https://gateway-lon.watsonplatform.net/assistant/api
ASSISTANT_AUTH_TYPE=iam
SPEECH_TO_TEXT_APIKEY=zzz
SPEECH_TO_TEXT_IAM_APIKEY=zzz
SPEECH_TO_TEXT_URL=https://gateway-lon.watsonplatform.net/speech-to-text/api
SPEECH_TO_TEXT_AUTH_TYPE=iam
TEXT_TO_SPEECH_APIKEY=uuu
TEXT_TO_SPEECH_IAM_APIKEY=uuu
TEXT_TO_SPEECH_URL=https://gateway-lon.watsonplatform.net/text-to-speech/api
TEXT_TO_SPEECH_AUTH_TYPE=iam
LANGUAGE_TRANSLATOR_APIKEY=mmm
LANGUAGE_TRANSLATOR_IAM_APIKEY=mmm
LANGUAGE_TRANSLATOR_URL=https://gateway-lon.watsonplatform.net/language-translator/api
LANGUAGE_TRANSLATOR_AUTH_TYPE=iam
```

## Lip Sync and Eye Movement

DAIMON uses SALSA, which is a for-pay Unity package. You can obtain your own copy [here](http://crazyminnowstudio.com/projects/salsa-with-randomeyes-lipsync/).

[1]: https://ieeexplore.ieee.org/document/8975993i
[2]: Documentation/2019-ic3d-huang-wild-twycross.pdf

## WebGL Microphone Support

WebGL in Unity currently does not provide native support for access to the microphone, so we have obtained a copy of FrostweepGames WebGL Microphone Library, which is commercially available through the asset store. 

## License

All original parts of this project are licensed under Apache 2.0. The IBM Watson 
Unity SDK and the IBM Unity SDK core are also governed by an Apache 2.0 license. 
SALSA is commercial and you will have to obtain your own license. 
WebGL Microphone is commercial and you will have to obtain your own license.
The full Apache 2.0 license text is available in [LICENSE](LICENSE.md).

## References

Xinyu Huang, Fridolin Wild, John Twycross (2019): A process for the semi-automated generation of life-sized, interactive 3D character models for holographic projection, In: International Conference on 3D Immersion (IC3D), December 11, 2019, Brussels, Belgium, IEEE, [link](https://ieeexplore.ieee.org/document/8975993), [preprint](Documentation/2019-ic3d-huang-wild-twycross.pdf)
