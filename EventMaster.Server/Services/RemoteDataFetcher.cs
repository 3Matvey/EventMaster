namespace EventMaster.Server.Services
{
    public static class RemoteDataFetcher
    {
        private static readonly HttpClient httpClient = new();

        public static async Task<byte[]> FetchImageFromRemoteUrlAsync(string url)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new Exception("HTTP request error while fetching image.", httpRequestException);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the image.", ex);
            }
        }
    }
}
