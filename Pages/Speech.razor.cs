using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Components;
using ReadmeAloud.Service;
using Microsoft.JSInterop;
using System.Net.Http;

namespace ReadmeAloud
{

    public partial class SpeechIndex : ComponentBase
    {

        [Inject]
        protected SpeechService _speechService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        public string FullText { get; set; }


        [Parameter]
        public string SearchTerm { get; set; }


        public async Task OnClickDownloadViaHttpClientButton()
        {

            var bytes = await _speechService.SynthesizeAudioAsync(FullText);
            if (bytes == null)
            {
            await Alert("Something Went Wrong");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync(
                                "downloadFromByteArray",
                                new
                                {
                                    ByteArray = bytes,
                                    FileName = "ReadmeAloudAsMp3.mp3",
                                    ContentType = "audio/mpeg"
                                });
            }


        }
        protected void ListenAudio()
        {            
            _speechService.SynthesisToSpeakerAsync(FullText);
        }

        protected void Stop()
        {
            _speechService.StopSynthesisToSpeakerAsync();
        }

        private async void Search(string gitHubRawURL)
        {
            FullText = await GetFullTextFromReadme(gitHubRawURL);
            StateHasChanged();
        }

        private async Task<string> GetFullTextFromReadme(string gitubURL)
        {
            if (Uri.IsWellFormedUriString(gitubURL, UriKind.Absolute))
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(gitubURL);
                string rawReadmeURL = response.Content.ReadAsStringAsync().Result;
                return rawReadmeURL;
            }
            else
            {
                await Alert("Please Enter a valid raw Github Readme URL (e.g. https://raw.githubusercontent.com/jayendranarumugam/DemoSecrets/master/README.md) ");
                return null;
            }


        }

        private async Task Alert(string message)
        {
            await JSRuntime.InvokeAsync<object>("Alert", message);
            ClearClick();
        }

        protected async void SearchClick()
        {
            if (string.IsNullOrEmpty(SearchTerm))
            {
                await Alert("Please Enter a valid raw Github Readme URL (e.g. https://raw.githubusercontent.com/jayendranarumugam/DemoSecrets/master/README.md) ");
            }
            else
            {
                Search(SearchTerm);
            }

        }


        protected void ClearClick()
        {
            SearchTerm = "";
            FullText = "";
            StateHasChanged();
        }



    }



}
