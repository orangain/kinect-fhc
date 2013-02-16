﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Kinect_FHC
{
    class Program
    {
        static double confidenceThreshold = 0.7;

        static void Main(string[] args)
        {
            var fhcHost = args[0];
            var fhcApiKey = args[1];
            var yobikake = args[2];
            try
            {
                confidenceThreshold = Double.Parse(args[3]);
            }
            catch (IndexOutOfRangeException)
            {
                // do nothing
            }

            var api = new FutureHomeControllerApi(fhcHost, fhcApiKey);
            var electronics = api.GetDetailList();


            KinectSensor sensor = null;

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                    break;
                }
            }

            if (null != sensor)
            {
                try
                {
                    // Start the sensor!
                    sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    sensor = null;
                }
            }

            if (null == sensor)
            {
                Console.WriteLine("Kinect not found");
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null == ri)
            {
                Console.WriteLine("Recognizer not found");
                return;
            }

            // Create a SpeechRecognitionEngine object for the default recognizer in the en-US locale.
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(ri.Id))
            {
                var voiceCommandChoices = new Choices();
                foreach (var elec in electronics)
                {
                    foreach (var action in elec.Actions)
                    {
                        foreach (var voiceCommand in action.VoiceCommands)
                        {
                            voiceCommandChoices.Add(new SemanticResultValue(voiceCommand, elec.Name + "|" + action.Name));
                        }
                    }
                }

                GrammarBuilder findServices = new GrammarBuilder();
                findServices.Append(yobikake);
                findServices.Append(voiceCommandChoices);

                // Create a Grammar object from the GrammarBuilder and load it to the recognizer.
                Grammar servicesGrammar = new Grammar(findServices);
                recognizer.LoadGrammar(servicesGrammar);

                // Add a handler for the speech recognized event.
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

                // Configure the input to the speech recognizer.
                recognizer.SetInputToDefaultAudioDevice();

                // Start asynchronous, continuous speech recognition.
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                // Keep the console window open.
                Console.WriteLine("Waiting...");
                while (true)
                {
                    Console.ReadLine();
                }
            }
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "ja-JP".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        // Handle the SpeechRecognized event.
        static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Recognized text: " + e.Result.Text + ", with confidence + " + e.Result.Confidence);
            if (e.Result.Confidence >= confidenceThreshold)
            {
                // todo
            }
        }
    }
}
