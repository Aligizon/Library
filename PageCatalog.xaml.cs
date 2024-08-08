using Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Library.Model.Book;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageCatalog : Page
    {
        public PageCatalog()
        {
            this.InitializeComponent();
            catalogView.ItemsSource = Book.GetRecords();
            //setViewAccess(catalogView);
            
        }

        private void AddOK_Click(object sender, RoutedEventArgs e)
        {
            if(TitleTB.Text != "" && AuthorTB.Text != "" && IssueYearTB.Text != "")
            {
                Book.addRecord(TitleTB.Text, AuthorTB.Text, IssueYearTB.Text);
                TitleTB.Text = "";
                AuthorTB.Text = "";
                IssueYearTB.Text = "";
                catalogView.ItemsSource = Book.GetRecords();
            } 

            AddFlyout.Hide();
        }
        private void DeleteOK_Click(object sender, RoutedEventArgs e)
        {
            bookDetails details = new bookDetails();
            details = (bookDetails)catalogView.SelectedItem;

            if (details != null)
            {
                if (details.ReaderName == "")
                {
                    Book.deleteRecord(details.ID);
                    catalogView.ItemsSource = Book.GetRecords();
                }
                else
                    WarningDialog("Can't delete selected book");
            }
            Reader.resetId();
            DeleteFlyout.Hide();
        }

        private void AddCancel_Click(object sender, RoutedEventArgs e)
        {
            AddFlyout.Hide();
        }
        private void DeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            DeleteFlyout.Hide();
        }

        private void catalogView_RowEditEnding(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowEditEndingEventArgs e)
        {
            bookDetails details = new bookDetails();
            details = (bookDetails)catalogView.SelectedItem;

            if (details != null)
            {
                Book.updateRecord(details.ID, details.Title, details.Author, details.IssueYear);
            }
        }
    }
}
