mergeInto(LibraryManager.library, {

	getMicrophoneDevices: function()
	{
		if(document.microphoneDevices == undefined)
			document.microphoneDevices = new Array();

		if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
			 console.log("enumerateDevices() not supported.");

			 SendMessage('Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
			 return;
		}

		navigator.mediaDevices.enumerateDevices()
		 .then(function(devices) {

			var outputDevicesArr = new Array();

			devices.forEach(function(device) {
				if(device.kind == "audioinput"){
					outputDevicesArr.push(device);
				}
			  });

			document.microphoneDevices = outputDevicesArr;

		    SendMessage('Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
		 })
		 .catch(function(err) {
			 console.log("get devices exception: " + err.name + ": " + err.message + "; " + err.stack);
		 });
	},
	
	
	start: function(device, loop, length, frequency){
 
		 document.microphoneFrequency = frequency;

		 function begin(){
			if (navigator.mediaDevices.getUserMedia) {
				navigator.mediaDevices.getUserMedia({
						audio: true
					}).then(GetUserMediaSuccess).catch(GetUserMediaFailed);
			}
		 }
 
		 begin();
		 
		 function GetUserMediaSuccess(stream)
		 {
			 if(document.audioContext == null || document.audioContext == undefined){
			 	document.audioContext = new AudioContext();
			 }
			 document.microphone_stream = document.audioContext.createMediaStreamSource(stream);
			 document.script_processor_node = document.audioContext.createScriptProcessor(0, 1, 1);	
			 document.script_processor_node.onaudioprocess = MicrophoneProcess;
 
			 document.script_processor_node.connect(document.audioContext.destination);
			 document.microphone_stream.connect(document.script_processor_node);
 
			 document.isRecording = 1;
 
			 console.log('record started');		
		 }

		 function GetUserMediaFailed(error)
		 {
			console.log('GetUserMedia failed with error ' + error);	
		 }
	 
		 function MicrophoneProcess(event)
		 {		
			if(IsSafari() || IsEdge()){
				var leftFloat32Array = event.inputBuffer.getChannelData(0);
				var stringArray = "";
 
				for (var i = 0; i < leftFloat32Array.length; i++) {
					stringArray = stringArray + leftFloat32Array[i];
					if(i < leftFloat32Array.length - 1){
						stringArray = stringArray + ",";
					}
				}
	
				SendMessage('Microphone', 'WriteBufferFromMicrophoneHandler', stringArray);
			}
			else {
				Resample(event.inputBuffer);
			}
		 }

		 function Resample(sourceAudioBuffer)
		 {
			var TARGET_SAMPLE_RATE = document.microphoneFrequency;

			var offlineCtx = new OfflineAudioContext(sourceAudioBuffer.numberOfChannels, sourceAudioBuffer.duration * sourceAudioBuffer.numberOfChannels * TARGET_SAMPLE_RATE, TARGET_SAMPLE_RATE);
			var buffer = offlineCtx.createBuffer(sourceAudioBuffer.numberOfChannels, sourceAudioBuffer.length, sourceAudioBuffer.sampleRate);
			// Copy the source data into the offline AudioBuffer
			for (var channel = 0; channel < sourceAudioBuffer.numberOfChannels; channel++) {
				buffer.copyToChannel(sourceAudioBuffer.getChannelData(channel), channel);
			}
			// Play it from the beginning.
			var source = offlineCtx.createBufferSource();
			source.buffer = sourceAudioBuffer;
			source.connect(offlineCtx.destination);
			source.start(0);
			offlineCtx.oncomplete = function(e) {
			  // `resampled` contains an AudioBuffer resampled at 16000Hz.
			  // use resampled.getChannelData(x) to get an Float32Array for channel x.
			  var resampled = e.renderedBuffer;
			  var leftFloat32Array = resampled.getChannelData(0);
			  // use this float32array to send the samples to the server or whatever
			  var stringArray = "";
 
			  for (var i = 0; i < leftFloat32Array.length; i++) {
				  stringArray = stringArray + leftFloat32Array[i];
				  if(i < leftFloat32Array.length - 1){
					  stringArray = stringArray + ",";
				  }
			  }
  
			  SendMessage('Microphone', 'WriteBufferFromMicrophoneHandler', stringArray);
			}
			offlineCtx.startRendering();
		 }

		 function IsSafari(){
			return /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification));
		 }

		 function IsEdge(){
			var isIE = /*@cc_on!@*/false || !!document.documentMode;
			return !isIE && !!window.StyleMedia;
		 }
	},
 
	end: function(device){
		 if(document.microphone_stream != undefined){
			 document.microphone_stream.disconnect(document.script_processor_node);
			 document.script_processor_node.disconnect();
		 }

		 document.microphone_stream = null;
		 document.script_processor_node = null;
 
		 document.isRecording = 0;
 
		 console.log('record ended');	
	},
 
	isRecording: function(device){
		 if(document.isRecording == undefined)
			 document.isRecording = 0;
		 return document.isRecording;
	},
 
	getDeviceCaps: function(device){
		 var returnStr = JSON.stringify(new Array(16000, 44100));
		 var bufferSize = lengthBytesUTF8(returnStr) + 1;
		 var buffer = _malloc(bufferSize);
		 stringToUTF8(returnStr, buffer, bufferSize);
		 return buffer;
	},
 
	isAvailable: function(){
		 return !!(navigator.mediaDevices.getUserMedia);
	},
 
	requestPermission: function(){

		if (navigator.mediaDevices.getUserMedia) {
			navigator.mediaDevices.getUserMedia({ audio: true }).then();

			function GetUserMediaSuccess(stream){
			 	SendMessage('Microphone', 'PermissionUpdate', "granted");
			}
		}
	},
 
	hasUserAuthorizedPermission: function(){
		try{
			navigator.permissions.query(
				{ name: 'microphone' }
			).then(function(permissionStatus){	
				SendMessage('Microphone', 'PermissionUpdate', permissionStatus.state.toString());
			});
		}
		catch(err){
			console.log("hasUserAuthorizedPermission exception: " + err.name + ": " + err.message + "; " + err.stack);
		}
	}
 });