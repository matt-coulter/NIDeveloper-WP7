using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NIDeveloperWP7.ViewModels;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NIDeveloperWP7
{
    public class BlogModel : INotifyPropertyChanged
    {
        public BlogModel()
        {
            this.Items = new ObservableCollection<BlogPostModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BlogPostModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            var webClient = new WebClient();

            webClient.OpenReadCompleted += onLoaded;

            webClient.OpenReadAsync(new Uri("http://www.nideveloper.co.uk/json", UriKind.Absolute));

            this.IsDataLoaded = true;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadCategory(String id)
        {
            var webClient = new WebClient();

            webClient.OpenReadCompleted += onLoaded;

            webClient.OpenReadAsync(new Uri("http://www.nideveloper.co.uk/json/category/"+id, UriKind.Absolute));

            this.IsDataLoaded = true;
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
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
