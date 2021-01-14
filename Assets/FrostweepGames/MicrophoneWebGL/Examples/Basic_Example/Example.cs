using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.Native;

namespace FrostweepGames.UniversalMicrophoneLibrary.Examples
{
    [RequireComponent(typeof(AudioSource))]
    public class Example : MonoBehaviour
    {
        private AudioClip _workingClip;

        public AudioSource audioSource;

        public Button startRecordButton,
                      stopRecordButton,
                      playRecordedAudioButton,
                      requestPermissionButton;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            startRecordButton.onClick.AddListener(StartRecord);
            stopRecordButton.onClick.AddListener(StopRecord);
            playRecordedAudioButton.onClick.AddListener(PlayRecordedAudio);
            requestPermissionButton.onClick.AddListener(RequestPermission);
        }

        private void RequestPermission()
        {
            CustomMicrophone.RequestMicrophonePermission();
        }

        private void StartRecord()
        {
            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            _workingClip = CustomMicrophone.Start(CustomMicrophone.devices[0], true, 4, 44100);
        }

        private void StopRecord()
        {
            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            if (!CustomMicrophone.IsRecording(CustomMicrophone.devices[0]))
                return;

            CustomMicrophone.End(CustomMicrophone.devices[0]);
        }

        private void PlayRecordedAudio()
        {
            if (_workingClip == null)
                return;

            audioSource.clip = _workingClip;
            audioSource.Play();
        }
    }
}