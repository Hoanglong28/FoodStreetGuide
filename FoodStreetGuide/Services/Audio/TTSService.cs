using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Google.Cloud.TextToSpeech.V1;

namespace doanC_.Services.Audio
{
    public class TTSService
    {
        private readonly TextToSpeechClient _client;

        public TTSService()
        {
            // Tự dùng ADC (đã login bằng gcloud)
            _client = TextToSpeechClient.Create();
        }

        public async Task<string> GenerateSpeechAsync(string text)
        {
            var input = new SynthesisInput
            {
                Text = text
            };

            var voice = new VoiceSelectionParams
            {
                LanguageCode = "vi-VN",
                SsmlGender = SsmlVoiceGender.Female
            };

            var config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            var response = await _client.SynthesizeSpeechAsync(input, voice, config);

            string filePath = Path.Combine(FileSystem.CacheDirectory, "tts.mp3");

            using (var output = File.Create(filePath))
            {
                response.AudioContent.WriteTo(output);
            }

            return filePath;
        }
    }
}
