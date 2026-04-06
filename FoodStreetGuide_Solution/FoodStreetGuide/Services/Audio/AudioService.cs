using System.Threading.Tasks;
using Microsoft.Maui.Media;
using Plugin.Maui.Audio;

namespace doanC_.Services
{
    public class AudioService
    {
        private readonly IAudioManager _audioManager;

        public AudioService()
        {
            _audioManager = AudioManager.Current;
        }

        // đọc text bằng TTS
        public async Task SpeakAsync(string text)
        {
            await TextToSpeech.Default.SpeakAsync(text);
        }

        // phát file audio mp3
        public async Task PlayAsync(string filePath)
        {
            var stream = await FileSystem.OpenAppPackageFileAsync(filePath);
            var player = _audioManager.CreatePlayer(stream);
            player.Play();
        }
    }
}