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
using NIDeveloperWP7.ViewModels;

namespace NIDeveloperWP7
{
    public partial class BlogCategory : PhoneApplicationPage
    {
        public BlogCategory()
        {
            InitializeComponent();
            DataContext = App.BlogModel;
        }

        private String categoryID;
        private String categoryName;

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //string blueprintID = "";
            if (NavigationContext.QueryString.TryGetValue("categoryID", out categoryID))
            {
                if (NavigationContext.QueryString.TryGetValue("categoryName", out categoryName))
                {
                    PageTitle.Text = categoryName;
                }

                App.BlogModel.LoadCategory(categoryID);
            }
        }

        private void CategoryPostsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (CategoryPostsList.SelectedIndex == -1)
                return;

            NavigationService.Navigate(new Uri("/BlogPost.xaml?postID=" + ((BlogPostModel)CategoryPostsList.SelectedItem).ID, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            CategoryPostsList.SelectedIndex = -1;
        }
    }
}