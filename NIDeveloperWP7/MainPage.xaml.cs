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
using NIDeveloperWP7.ViewModels;

namespace NIDeveloperWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.BlogModel;
            CategoriesList.DataContext = App.CategoryModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.BlogModel.IsDataLoaded)
            {
                App.BlogModel.LoadData();
            }
        }

        private void Pivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            switch ((String)(((PivotItem)(((Pivot)sender).SelectedItem)).Header))
            {
                case "Latest Posts": App.BlogModel.Items.Clear(); App.BlogModel.LoadData(); break;
                case "Categories": App.CategoryModel.Items.Clear(); App.CategoryModel.LoadData(); break;
                default: break;
            }
        }

        private void LatestPosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (LatestPosts.SelectedIndex == -1)
                return;

            NavigationService.Navigate(new Uri("/BlogPost.xaml?postID=" + ((BlogPostModel)LatestPosts.SelectedItem).ID, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            LatestPosts.SelectedIndex = -1;
        }

        private void CategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (CategoriesList.SelectedIndex == -1)
                return;
            App.BlogModel.Items.Clear();
            NavigationService.Navigate(new Uri("/BlogCategory.xaml?categoryID=" + ((BlogCategoryModel)CategoriesList.SelectedItem).ID + "&categoryName=" + ((BlogCategoryModel)CategoriesList.SelectedItem).Name, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            CategoriesList.SelectedIndex = -1;
        }

        private void callQuery()
        {
            String query = SearchQuery.Text;
            if (query != "")
                NavigationService.Navigate(new Uri("/SearchResults.xaml?query=" + query, UriKind.Relative));
        }

        private void SearchQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                callQuery();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            callQuery();
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }
}