using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using NIDeveloperWP7.ViewModels;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NIDeveloperWP7
{
    public partial class BlogPost : PhoneApplicationPage
    {
        private string blogPostID;
        private string blogPostName;
        private string blogPostDescription;

        List<string> URLStack = new List<string>();
        int position = -1;
        bool navigationButtonClicked = false;
        private bool IsDataLoaded;

        public BlogPost()
        {
            InitializeComponent();
            this.Items = new ObservableCollection<BlogPostModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BlogPostModel> Items { get; private set; }

        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.webBrowser1.Source != null && !this.navigationButtonClicked)
            {
                if (position == 0)
                {
                    string firstURL = this.URLStack[0];
                    this.URLStack.Clear();
                    this.URLStack.Add(firstURL);
                }
                this.URLStack.Add(this.webBrowser1.Source.ToString());
                position++;
            }

            this.navigationButtonClicked = false;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData(String id)
        {
            var webClient = new WebClient();

            webClient.OpenReadCompleted += onLoaded;

            webClient.OpenReadAsync(new Uri("http://www.nideveloper.co.uk/json/post/"+id, UriKind.Absolute));

            this.IsDataLoaded = true;
        }

        private void Back_Click(object sender, EventArgs e)
        {
            if (position > 0)
            {
                this.navigationButtonClicked = true;
                this.webBrowser1.Navigate(new Uri(this.URLStack[position - 1].ToString()));
                position--;
            }
        }



        private void Forward_Click(object sender, EventArgs e)
        {
            if (this.URLStack.Count > position + 1)
            {
                this.navigationButtonClicked = true;
                this.webBrowser1.Navigate(new Uri(this.URLStack[position + 1].ToString()));
                position++;
            }
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //string blueprintID = "";
            if (NavigationContext.QueryString.TryGetValue("postID", out blogPostID))
            {
                this.webBrowser1.Navigate(new Uri("http://mobile.nideveloper.co.uk/blog/post/" + blogPostID));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            LoadData(blogPostID);
        }

        private void create_tile()
        {
            // Look to see if the tile already exists and if so, don't try to create again.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=BlogPost" + blogPostID));

            // Create the tile if we didn't find it already exists.
            if (TileToFind == null)
            {

                // Create the tile object and set some initial properties for the tile.
                // The Count value of 12 will show the number 12 on the front of the Tile. Valid values are 1-99.
                // A Count value of 0 will indicate that the Count should not be displayed.
                StandardTileData NewTileData = new StandardTileData
                {
                    //BackgroundImage = new Uri("Red.jpg", UriKind.Relative),
                    Title = blogPostName,
                    BackTitle = "NI Developer",
                    BackContent = blogPostName,
                    //BackBackgroundImage = new Uri("Blue.jpg", UriKind.Relative)
                };

                // Create the tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri("/BlogPost.xaml?postID=" + blogPostID + "&DefaultTitle=BlogPost" + blogPostID, UriKind.Relative), NewTileData);
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static object Deserialize(Stream streamObject, Type serializedObjectType)
        {
            if (serializedObjectType == null || streamObject == null)
                return null;

            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializedObjectType);
            return ser.ReadObject(streamObject);
        }

        private void onLoaded(object sender, OpenReadCompletedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            e.Result.Position = 0;
            var sr = new StreamReader(e.Result);
            var myStr = sr.ReadToEnd();
            ms.Write(StrToByteArray(myStr), 0, myStr.Length);
            ms.Position = 0;

            // deserialization
            BlogJsonContainer sampleData = (BlogJsonContainer)Deserialize(ms, typeof(BlogJsonContainer));

            ms.Close();
            this.Items.Clear();
            foreach (BlogPostModel post in sampleData.posts)
            {
                this.Items.Add(post);
                this.blogPostName = post.Name;
                this.blogPostDescription = post.Description;
            }

            create_tile();
        }
    }
}