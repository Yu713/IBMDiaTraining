﻿using UnityEngine;
using FrostweepGames.Plugins.Native;
using UnityEngine.UI;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System;
#endif
namespace FrostweepGames.Plugins.WebGL.Examples
{
	public class LeapSync_Example : MonoBehaviour
	{
		public AudioSource _audioSource;

		public Button startRecord,
					  stopRecord;

		private int _sampleRate = 44100;

		private int _recordingTime = 1;

#if UNITY_WEBGL && !UNITY_EDITOR

		private bool _audioClipReadyToUse;

		private float _delay;

		private bool _playing;
	
		private Buffer _buffer;
#endif

		private void Start()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			_buffer = new Buffer();
			_audioSource.clip = AudioClip.Create("BufferedClip", _sampleRate * _recordingTime, 1, _sampleRate, false);
#endif
			CustomMicrophone.RequestMicrophonePermission();

			startRecord.onClick.AddListener(StartRecordHandler);
			stopRecord.onClick.AddListener(StopRecordHandler);
		}

#if UNITY_WEBGL && !UNITY_EDITOR
		private void Update()
		{
			try
			{
				if (CustomMicrophone.IsRecording(string.Empty))
				{
					float[] array = new float[0];
					CustomMicrophone.GetRawData(ref array);

					if (_buffer.position != CustomMicrophone.GetPosition(CustomMicrophone.devices[0]) && array.Length > 0)
					{	
						int lastPosition = _buffer.position;
						_buffer.position = CustomMicrophone.GetPosition(CustomMicrophone.devices[0]);

						if (lastPosition > _buffer.position)
						{
							_buffer.data.AddRange(array.ToList().GetRange(lastPosition, array.Length - lastPosition));
							_buffer.data.AddRange(array.ToList().GetRange(0, _buffer.position));
						}
						else
						{
							_buffer.data.AddRange(array.ToList().GetRange(lastPosition, _buffer.position - lastPosition));
						}
					}

					_audioClipReadyToUse = _buffer.data.Count >= _sampleRate * _recordingTime;

					if (_playing)
					{
						_delay -= Time.deltaTime;

						if (_delay <= 0)
						{
							_playing = false;
						}
					}
					else
					{
						if (_audioClipReadyToUse)
						{
							List<float> chunk;

							if (_buffer.data.Count >= _sampleRate)
							{
								chunk = _buffer.data.GetRange(0, _sampleRate);
								_buffer.data.RemoveRange(0, _sampleRate);
							}
							else
							{
								chunk = _buffer.data;
								_buffer.data.Clear();
								for (int i = chunk.Count; i < _sampleRate; i++)
								{
									chunk.Add(0);
								}
							}

							_audioSource.clip.SetData(chunk.ToArray(), 0);
							_audioSource.Play();

							_delay = _recordingTime;
							_playing = true;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.Log(ex.Message + " | " + ex.StackTrace);
			}
		}

#endif
		private void StartRecordHandler()
		{
			if (!CustomMicrophone.HasConnectedMicrophoneDevices())
				return;

#if UNITY_EDITOR
			_audioSource.clip =
#endif
			CustomMicrophone.Start(CustomMicrophone.devices[0], true, _recordingTime, _sampleRate);
#if UNITY_EDITOR
			_audioSource.loop = true;
			_audioSource.Play();
#endif
		}

		private void StopRecordHandler()
		{
			if (!CustomMicrophone.HasConnectedMicrophoneDevices())
				return;

			CustomMicrophone.End(CustomMicrophone.devices[0]);
			_audioSource.Stop();

#if UNITY_WEBGL && !UNITY_EDITOR
			_buffer.data.Clear();
			_buffer.position = 0;
#endif
		}

#if UNITY_WEBGL && !UNITY_EDITOR
		private class Buffer
		{
			public int position;
			public List<float> data;

			[UnityEngine.Scripting.Preserve]
			public Buffer()
			{
				position = 0;
				data = new List<float>();
			}
		}
#endif
	}
}