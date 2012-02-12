using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;


namespace NIDeveloperWP7.ViewModels
{
    [DataContract]
    public class BlogCategoryModel : INotifyPropertyChanged
    {
        public BlogCategoryModel()
        {
            this.Items = new ObservableCollection<BlogCategoryModel>();
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<BlogCategoryModel> Items { get; private set; }

        private int _id;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _name;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _description;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            var webClient = new WebClient();

            webClient.OpenReadCompleted += onLoaded;

            webClient.OpenReadAsync(new Uri("http://www.nideveloper.co.uk/json/categories", UriKind.Absolute));

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
            BlogCategoryJsonContainer sampleData = (BlogCategoryJsonContainer)Deserialize(ms, typeof(BlogCategoryJsonContainer));

            ms.Close();
            this.Items.Clear();
            foreach (BlogCategoryModel category in sampleData.categories)
            {
                this.Items.Add(category);
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
