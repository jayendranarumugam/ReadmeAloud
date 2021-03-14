using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;

namespace ReadmeAloud.Service
{
    public class SpeechService
    {
        private SpeechSynthesizer synthesizer;

        private readonly IConfiguration Configuration;
        
        public SpeechService(IConfiguration configuration)
        {
           Configuration = configuration;
        }

        public async void SynthesisToSpeakerAsync(string text)
        {
            SpeechConfig speechConfig= SpeechConfig.FromSubscription(Configuration["CognitiveAPIKey"], Configuration["CognitiveAPIRegion"]);
            synthesizer = new SpeechSynthesizer(speechConfig);
            await synthesizer.SpeakTextAsync(text);
        }
                
        public void StopSynthesisToSpeakerAsync()
        {
            synthesizer.StopSpeakingAsync();
        }

        public async Task<byte[]> SynthesizeAudioAsync(string text)
        {
            SpeechConfig speechConfigForAudioAsync = SpeechConfig.FromSubscription(Configuration["CognitiveAPIKey"], Configuration["CognitiveAPIRegion"]);
            speechConfigForAudioAsync.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);


            using (var synthesizer = new SpeechSynthesizer(speechConfigForAudioAsync, null as AudioConfig))
            {                                

                    using (var result = await synthesizer.SpeakTextAsync(text))
                    {
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            return result.AudioData;
                            
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);                            
                        }
                        return null;
                    }
            }

        }



    }
}
