using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

var scopes = new[] { YouTubeService.Scope.Youtube };

// Load the client secret JSON file
using (var stream = new FileStream("D:/Persoonlijke projecten/YoutubeOpenener/YoutubeOpenener/client_secret_1033042725019-37caki1452erfvnb5hk52sp4p3sbtbok.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
{
  var clientSecrets = GoogleClientSecrets.Load(stream).Secrets;
  // Use clientSecrets for the authentication flow

  var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
    clientSecrets,
    scopes,
    "user",
    CancellationToken.None).Result;

  var youtubeService = new YouTubeService(new BaseClientService.Initializer()
  {
    HttpClientInitializer = credential,
    ApplicationName = "YoutubeVideoOpener",
  });

  var subscriptionsRequest = youtubeService.Subscriptions.List("snippet");
  subscriptionsRequest.Mine = true;
  var subscriptionListResponse = subscriptionsRequest.Execute();

  List<string> channels = new List<string>
  {
    "UCi-QR0jFereBXjU9SBxcdeg", "UCzXwjTI6c6mVn6oui_p6oiw", "UC4USoIAL9qcsx5nCZV_QRnA", "UCpnkp_D4FLPCiXOmDhoAeYA", "UCtBFqR-itrGB4C566GkfrTA", "UCorikuuyFz1dZLo3N9nHc4w", "UCHnyfMqiRRG1u-2MsSQLbXA", "UCvmWj7t7nP3yOSU_rl9doCQ", "UC-gW4TeZAlKm7UATp24JsWQ", "UCifRgVk-GEo1_vvf8x53t6A", "UCHXJ0dhS3NpTBFg7lR_5w8Q", "UCPNPv2AYNvQ5qdZo8KBKKFA", "UCdPui8EYr_sX6q1xNXCRPXg", "UCc0kHafEIzm6PiqyrsC5lyg", "UCj7KXMG2Bc4QAe1mSAOShtA", "UCEUJmFa1ItxuZg6RvlAwRYg", "UCtb8P4rf_1n8KS8eZk_lNNw", "UCPKgIhTC3BdkAwMw6s-GEug", "UCfbnTUxUech4P1XgYUwYuKA", "UClq42foiSgl7sSpLupnugGA", "UCBKhHQPLXQqru8Zs-5LwmiA", "UC6nSFpj9HTCZ5t-N3Rm3-HA", "UCX_fEsnSHXaJV2f2JjsDvow", "UCJLZe_NoiG0hT7QCX_9vmqw", "UCY1kMZp36IQSyNx_9h4mpCg", "UC-lHJZR3Gqxm24_Vd_AJ5Yw", "UCb3n4k6_smEbOUqiXu4rlOg", "UCDa8HbNrmkFhKKOeiB7JaRw", "UCsXVk37bltHxD1rDPwtNM8Q", "UCvK4bOhULCpmLabd2pDMtnA", "UCMdGPato0IC5-zZjJIf-P9w", "UC_S45UpAYVuc0fYEcHN9BVQ", "UCZbIPAVd6Zg9vs6Q4cRZisA", "UCcKvm2VL8xTInRggo8A3alA", "UCU5O8FCtOTI4BWhfHF2LHJw", "UCSKUNcjmYDX5A-KSxnNk7QQ", "UCtHaxi4GTYDpJgMSGy7AeSw", "UC9n8unUsC6coX-T0wmX5uPg"
  };

  foreach (var channelId in channels)
  {
    var searchRequest = youtubeService.Search.List("snippet");
    searchRequest.ChannelId = channelId;
    searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

    var searchResponse = searchRequest.Execute();

    var currentDateTime = DateTime.UtcNow;

    foreach (var searchResult in searchResponse.Items)
    {
      var videoPublishedAt = searchResult.Snippet.PublishedAt;
      var videoPublishedDateTime = DateTime.Parse(videoPublishedAt.ToString());

      var timeDifference = currentDateTime - videoPublishedDateTime;

      if (timeDifference.TotalHours <= 24)
      {
        // Video uploaded within the last 24 hours, add it to the playlist
        var playlistItem = new PlaylistItem();
        playlistItem.Snippet = new PlaylistItemSnippet();
        playlistItem.Snippet.PlaylistId = "PLoyG5hnFbgJVNm27Br5VF-HXQSdLGv_Eo";
        playlistItem.Snippet.ResourceId = new ResourceId();
        playlistItem.Snippet.ResourceId.Kind = "youtube#video";
        playlistItem.Snippet.ResourceId.VideoId = searchResult.Id.VideoId;

        var playlistItemsInsertRequest = youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
        playlistItemsInsertRequest.Execute();
      }
    }
  }

  var playlistItemsRequest = youtubeService.PlaylistItems.List("snippet");
  playlistItemsRequest.PlaylistId = "PLoyG5hnFbgJVNm27Br5VF-HXQSdLGv_Eo";
  playlistItemsRequest.MaxResults = 100;

  var playlistItemsResponse = playlistItemsRequest.Execute();

  foreach(var playlistItem in playlistItemsResponse.Items)
  {
    string videoId = playlistItem.Snippet.ResourceId.VideoId;
    DateTime videoUpdatedAt = playlistItem.Snippet.PublishedAt.Value;

    var currentDateTime = DateTime.UtcNow;

    var timeDifference = currentDateTime - videoUpdatedAt;
    if (timeDifference.TotalHours > 24)
    {
      var playlistItemsDeleteRequest = youtubeService.PlaylistItems.Delete(videoId);
      playlistItemsDeleteRequest.Execute();
    }
  }
}

