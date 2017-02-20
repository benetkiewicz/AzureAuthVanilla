namespace AzureAuthVanilla
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;

    public class GraphClient
    {
        const string aadInstance = "https://login.microsoftonline.com/";
        const string aadGraphResourceId = "https://graph.windows.net/";
        const string aadGraphVersion = "api-version=1.6";

        private readonly string tenant;
        private readonly AuthenticationContext authContext;
        private readonly ClientCredential credential;

        public GraphClient(string clientId, string secret, string tenant)
        {
            this.tenant = tenant;
            this.authContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}", tenant));
            this.credential = new ClientCredential(clientId, secret);
        }

        public async Task<List<string>> MemberOf(string objectId)
        {
            string jsonResponse = await SendGraphPostRequest("/users/" + objectId + "/getMemberGroups", "{securityEnabledOnly: false}").ConfigureAwait(false);
            var deserializedResponse = JsonConvert.DeserializeObject<GetMemberGroupsResponse>(jsonResponse);
            return deserializedResponse.Value;
        }

        private async Task<string> SendGraphPostRequest(string api, string json)
        {
            AuthenticationResult result = await authContext.AcquireTokenAsync(aadGraphResourceId, credential).ConfigureAwait(false);
            HttpClient http = new HttpClient();
            string url = aadGraphResourceId + tenant + api + "?" + aadGraphVersion;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}