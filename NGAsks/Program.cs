using System;
using Tweetinvi.Models;
using System.Text;
using Tweetinvi.Parameters;
using Tweetinvi;
using Newtonsoft.Json;

namespace NGAsks
{
    class Program
    {

        private static string ApiKey = Environment.GetEnvironmentVariable("ApiKey", EnvironmentVariableTarget.Machine);
        private static string ApiSecret = Environment.GetEnvironmentVariable("ApiSecret", EnvironmentVariableTarget.Machine);
        private static string AccessToken = Environment.GetEnvironmentVariable("AccessToken", EnvironmentVariableTarget.Machine);
        private static string AccessSecret = Environment.GetEnvironmentVariable("AccessSecret", EnvironmentVariableTarget.Machine);

        public static async Task Main(string[] args)
        {

            Console.WriteLine("***************| TwitterAPI Operations |***************");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("1 | Search Tweets");
            Console.WriteLine("2 | Share Tweets");
            Console.WriteLine("3 | View Home Tweets");
            Console.WriteLine("4 | List a User's Tweets");
            Console.WriteLine("5 | List People Following A User");
            Console.WriteLine("6 | List A User's Followers");
            Console.WriteLine("------------------------------------------------------");
            Console.Write("Select (1 - 6) : ");
            int methodSelector = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\n \n");
            if (methodSelector == 1)
            {
                Console.Write("Enter the Tweet Parameter You Want to Search:  ");
                string searchParam = Console.ReadLine();
                Console.WriteLine(searchParam);
                await GetTweetsWithKeyword(searchParam);
            }
            else if (methodSelector == 2)
            {
                Console.Write("Select to tweet with or without a picture. (With Picture - Without Picture) : ");
                string withPicture = Console.ReadLine();
                if (withPicture.ToLower() == "with picture")
                {
                    Console.Write("Enter the Tweet to be posted: ");
                    string tweet = Console.ReadLine();
                    Console.Write("Enter the Image to be Shared: ");
                    string path = Console.ReadLine();
                    await TweetWithImage(tweet, path);
                }
                else if (withPicture.ToLower() == "without picture")
                {
                    Console.Write("Enter the Tweet to be posted: ");
                    string tweet = Console.ReadLine();
                    await TweetText(tweet);
                }
                else
                {
                    Console.WriteLine("Enter a Valid Command!");
                }
            }
            else if (methodSelector == 3)
            {
                Console.WriteLine("Showing Home Tweets");
                string timelineUser = Console.ReadLine();

                await GetTimeline();
            }
            else if (methodSelector == 4)
            {
                Console.Write("Enter the Username of the User You Want to List Tweets: ");
                string timelineUser = Console.ReadLine();

                await GetUserTimeline(timelineUser);
            }
            else if (methodSelector == 5)
            {
                Console.Write("Enter the username of the followed person:");
                string listfriends = Console.ReadLine();

                await ListFriends(listfriends);
            }
            else if (methodSelector == 6)
            {
                Console.Write("Username of the user you want to see: ");
                string userfollowers = Console.ReadLine();

                await UserFollowers(userfollowers);
            }
            else
            {
                Console.WriteLine("Enter a valid command");
            }

            Console.ReadKey();

        }

        static async Task UserFollowers(string username)
        {
            TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
            var followerIds = await client.Users.GetFollowerIdsAsync(username);
            foreach (var follower in followerIds)
            {
                var userResponse = await client.UsersV2.GetUserByIdAsync(follower);
                Console.WriteLine(userResponse.User.Name + " " + userResponse.User.Url);
            }
        }

        static async Task ListFriends(string username)
        {
            TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
            var friendIds = await client.Users.GetFriendIdsAsync(username);
            foreach (var friend in friendIds)
            {
                var userResponse = await client.UsersV2.GetUserByIdAsync(friend);
                Console.WriteLine(userResponse.User.Name + " " + userResponse.User.Url);
            }

        }

        static async Task GetUserTimeline(string username)
        {
            TwitterClient userClient = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);

            var homeTimelineTweets = await userClient.Timelines.GetUserTimelineAsync(username);
            foreach (var tweets in homeTimelineTweets)
            {

                Console.WriteLine("Tweet : " + tweets.Text);
            }
        }
        static async Task GetTimeline()
        {
            TwitterClient userClient = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);

            var homeTimelineTweets = await userClient.Timelines.GetHomeTimelineAsync();
            foreach (var tweets in homeTimelineTweets)
            {
                Console.WriteLine("User : " + tweets.CreatedBy.ToString());
                Console.Write("Tweet : " + tweets.Text + "\n");
            }
        }

        static async Task TweetWithImage(string text, string path)
        {
            try
            {
                byte[] ImageBytes = File.ReadAllBytes(path);
                TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
                IMedia ImageIMedia = await client.Upload.UploadTweetImageAsync(ImageBytes);
                ITweet tweet = await client.Tweets.PublishTweetAsync(new PublishTweetParameters(text) { Medias = { ImageIMedia } });
                Console.WriteLine("Successfully Tweeted.");
            }
            catch
            {
                Console.WriteLine("Tweet failed.");
            }

        }
        static async Task TweetText(string text)
        {
            try
            {
                TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
                ITweet tweet = await client.Tweets.PublishTweetAsync(text);
                Console.WriteLine("Successfully Tweeted.");
            }
            catch
            {
                Console.WriteLine("Tweet failed.");
            }

        }

        public static async Task GetTweetsWithKeyword(string keyword)
        {
            try
            {
                TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
                var searchResults = await client.SearchV2.SearchTweetsAsync(keyword);
                var tweets = searchResults.Tweets;
                Console.WriteLine(tweets);
                Console.Write("Write Information to JSON File? (Yes-1/No-2) : ");
                int jsonWrite = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine(jsonWrite);
                if (jsonWrite == 1)
                {
                    try
                    {
                        string convertJson = JsonConvert.SerializeObject(tweets);
                        File.WriteAllText("files/TweetDetails.json", convertJson);
                        Console.WriteLine("Information successfully exported to JSON file.");
                    }
                    catch
                    {
                        Console.WriteLine("Error");
                    }
                }
                else if (jsonWrite == 2)
                {
                    foreach (var tweet in tweets)
                    {
                        Console.WriteLine("User Id : " + tweet.AuthorId.ToString());
                        Console.WriteLine("Tweet : " + tweet.Text.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Enter 'Yes' or 'No'");
                }

            }
            catch
            {
                Console.Write("Error");
            }

        }
    }


}
