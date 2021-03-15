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
        private SpeechService _speechService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected string FullText { get; set; }

        protected string AudioURL { get; set; }

        protected bool IsPlaying { get; set; }

        protected ElementReference Audio { get; set; }

        private byte[] bytes{get;set;}


        [Parameter]
        public string SearchTerm { get; set; }



        public async Task OnClickDownloadViaHttpClientButton()
        {            
            if (bytes == null)
            {
                await Alert("Something Went Wrong");
            }

            await JSRuntime.InvokeVoidAsync(
                            "downloadFromByteArray",
                            new
                            {
                                ByteArray = bytes,
                                FileName = "ReadmeAloudAsMp3.mp3",
                                ContentType = "audio/mpeg"
                            });

        }


        private async void Search(string gitHubRawURL)
        {
            FullText = await GetFullTextFromReadme(gitHubRawURL);            
            if (FullText != null)
            {
                bytes = await _speechService.SynthesizeAudioAsync(FullText);
                
                if (bytes == null)
                {
                    await Alert("Something Went Wrong");
                }
                AudioURL = "data:audio/mpeg;base64," + Convert.ToBase64String(bytes);
                StateHasChanged();
            }

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


        protected async Task PlayAudio()
        {
            await JSRuntime.InvokeVoidAsync("playAudio", Audio);
            IsPlaying = true;
        }

        protected async Task StopAudio()
        {
            await JSRuntime.InvokeVoidAsync("stopAudio", Audio);
            IsPlaying = false;
        }

        [JSInvokable]
        protected async Task OnEnd()
        {
            IsPlaying = false;
            StateHasChanged();
        }



    }



}
