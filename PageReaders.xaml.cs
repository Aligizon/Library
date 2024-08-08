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
using static Library.Model.Reader;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageReaders : Page
    {
        public PageReaders()
        {
            this.InitializeComponent();
            readerView.ItemsSource = Reader.GetRecords();
        }
        //-----------------------------------------------------------------------------------------------------------------------
        private void AddOK_Click(object sender, RoutedEventArgs e)
        {
            Reader.addRecord(readerNameTB.Text, BirthYearTB.Text);
            readerNameTB.Text = "";
            BirthYearTB.Text = "";
            readerView.ItemsSource = Reader.GetRecords();

            AddFlyout.Hide();
        }
        private void DeleteOK_Click(object sender, RoutedEventArgs e)
        {
            readerDetails details = new readerDetails();
            details = (readerDetails)readerView.SelectedItem;

            if (details != null)
            {
                Reader.deleteRecord(details.ID);
                readerView.ItemsSource = Reader.GetRecords();
            }
            Reader.resetId();
            DeleteFlyout.Hide();
        }
        private void LendOK_Click(object sender, RoutedEventArgs e)
        {
            readerDetails details = new readerDetails();
            details = (readerDetails)readerView.SelectedItem;

            if (details != null)
            {
                List<lendBookDetails> SelectionList = new List<lendBookDetails>();
                SelectionList = lendBookGrid.SelectedItems.Cast<lendBookDetails>().ToList();

                if (SelectionList.Count > 0 && SelectionList.Count <= 6)
                {
                    foreach (lendBookDetails book in SelectionList)
                    {
                        lendBookUpdate(details.ID, book.BookID);
                    }

                    readerView.ItemsSource = Reader.GetRecords();
                }
                else if (SelectionList.Count > 6)
                {
                    WarningDialog("Can't check out more than 6 books");
                }
                else
                {
                    WarningDialog("Book isn't selected");
                }
            }
            else
            {
                WarningDialog("To give out books, first select a reader");
            }

            LendFlyout.Hide();
        }
        private void ReturnOK_Click(object sender, RoutedEventArgs e)
        {
            List<returnBookDetails> SelectionList = new List<returnBookDetails>();
            SelectionList = returnBookGrid.SelectedItems.Cast<returnBookDetails>().ToList();

                foreach (returnBookDetails book in SelectionList)
                {
                    lendBookUpdate(null, book.BookID);
                }

            readerView.ItemsSource = Reader.GetRecords();
            ReturnFlyout.Hide();
        }


        private void AddCancel_Click(object sender, RoutedEventArgs e)
        {
            AddFlyout.Hide();
        }
        private void DeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            DeleteFlyout.Hide();
        }
        private void LendCancel_Click(object sender, RoutedEventArgs e)
        {
            LendFlyout.Hide();
        }
        private void ReturnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReturnFlyout.Hide();
        }
        //-----------------------------------------------------------------------------------------------------------------------

        private void readerView_RowEditEnding(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowEditEndingEventArgs e)
        {
            readerDetails details = new readerDetails();
            details = (readerDetails)readerView.SelectedItem;

            if (details != null)
            {
                Reader.updateRecord(details.ID, details.ReaderName, details.BirthYear);
            }
        }

        private void BorrowBook_Click(object sender, RoutedEventArgs e)
        {
            lendBookGrid.ItemsSource = FreeBookGetRecords();
        }

        private void ReturnBook_Click(object sender, RoutedEventArgs e)
        {  
            readerDetails details = new readerDetails();
            details = (readerDetails)readerView.SelectedItem;

            if (details != null)
            {
                returnBookGrid.ItemsSource = ReturnBookGetRecords(details.ID);
            }
            else
            {
                WarningDialog("Select a reader");
                ReturnFlyout.Hide();
            }
        }

    }
}
