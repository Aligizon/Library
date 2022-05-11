using Microsoft.Data.Sqlite;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Library.Model
{
    class Book
    {
        public Book()
        {
        }

        public static void addRecord(string Title, string Author, string IssueYear)
        {
            if (!Title.Equals("") && !Author.Equals("") && !IssueYear.Equals(""))
            {
                string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

                using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
                {
                    connection.Open();
                    SqliteCommand CMD_Insert = new SqliteCommand();
                    CMD_Insert.Connection = connection;

                    CMD_Insert.CommandText = "INSERT INTO Book (Title, Author, IssueYear) VALUES(@Title, @Author, @IssueYear);";
                    CMD_Insert.Parameters.AddWithValue("@Title", Title);
                    CMD_Insert.Parameters.AddWithValue("@Author", Author);
                    CMD_Insert.Parameters.AddWithValue("@IssueYear", IssueYear);

                    CMD_Insert.ExecuteReader();

                    connection.Close();
                }
            }
        }

        public static void deleteRecord(int? ID)
        {
            if (ID != null)
            {
                string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

                using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
                {
                    connection.Open();
                    SqliteCommand CMD_Delete = new SqliteCommand();
                    CMD_Delete.Connection = connection;
                    CMD_Delete.CommandText = "DELETE FROM Book WHERE ID = '" + ID + "'";
                    CMD_Delete.ExecuteReader();

                    connection.Close();
                }
            }
        }

        public static void updateRecord(int? ID, String Title, String Author, String IssueYear)
        {
            if (!ID.Equals(null) && !Title.Equals("") && !Author.Equals("") && !IssueYear.Equals(""))
            {
                string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

                using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
                {
                    connection.Open();

                    SqliteCommand CMD_Update = new SqliteCommand();
                    CMD_Update.Connection = connection;
                    CMD_Update.CommandText = "UPDATE Book SET " +
                                             "Title = '" + Title + "', " +
                                             "Author = '" + Author + "', " +
                                             "IssueYear = '" + IssueYear + "' " +
                                             "WHERE ID = '" + ID + "'";
                    CMD_Update.ExecuteReader();

                    connection.Close();
                }
            }
        }

        // Выдача книги читателю
        public static void lendBookUpdate(int? ReaderID, int BookId)
        {
            if (BookId != 0)
            {
                string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

                using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
                {
                    connection.Open();

                    SqliteCommand CMD_Update = new SqliteCommand();
                    CMD_Update.Connection = connection;

                    if (ReaderID == null)
                    {
                        CMD_Update.CommandText = "UPDATE Book SET ReaderID = null WHERE ID = " + BookId;
                    }
                    else
                    {
                        CMD_Update.CommandText = "UPDATE Book SET ReaderID = " + ReaderID + " WHERE ID = " + BookId;
                    }

                    CMD_Update.ExecuteReader();

                    connection.Close();
                }
            }
        }
        //TODO:
        //public static void setViewAccess(DataGrid catalogView)
        //{
        //    foreach (bookDetails item in catalogView.ItemsSource)
        //    {
        //        DataGridRow row = (DataGridRow)catalogView ?;

        //        row.IsEnabled = item.ReaderName = "";
        //    }
        //}

        public static void resetId()
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");
            using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
            {
                connection.Open();

                SqliteCommand CMD_ResetID = new SqliteCommand();
                CMD_ResetID.Connection = connection;
                CMD_ResetID.CommandText = "UPDATE SQLITE_SEQUENCE SET seq = 0";
                CMD_ResetID.ExecuteNonQuery();

                connection.Close();
            }
        }

        public class bookDetails
        {
            public int ID { get; set; }
            public String Title { get; set; }
            public String Author { get; set; }
            public String IssueYear { get; set; }
            public String ReaderName { get; set; }

            public bookDetails()
            {
                this.ID         = 0;
                this.Title      = "";
                this.Author     = "";
                this.IssueYear  = "";
                this.ReaderName = "";
            }

            public bookDetails(int ID, String Title, String Author, String IssueYear, String ReaderName)
            {
                this.ID         = ID;
                this.Title      = Title;
                this.Author     = Author;
                this.IssueYear  = IssueYear;
                this.ReaderName = ReaderName;
            }
        }

        public static List<bookDetails> GetRecords()
        {
            List<bookDetails> bookList = new List<bookDetails>();
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");
            using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
            {
                connection.Open();

                SqliteCommand cmd_getRec = new SqliteCommand("SELECT ID, Title, Author, IssueYear, ReaderID FROM Book", connection);
                SqliteDataReader reader = cmd_getRec.ExecuteReader();

                while (reader.Read())
                {
                    int     readerID   = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                    String  readerName = "";

                    if (readerID != 0)
                    {
                        SqliteCommand cmd_getRec2 = new SqliteCommand("SELECT ReaderName FROM Reader WHERE ID = " + readerID, connection);
                        SqliteDataReader reader2 = cmd_getRec2.ExecuteReader();

                        if (reader2.Read())
                        {
                            readerName += reader2.GetString(0);
                        }
                    }

                    bookList.Add(new bookDetails(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), readerName));
                }

                connection.Close();
                
            }
            return bookList;
        }
        public static async void WarningDialog(string Message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Ошибка!",
                Content = Message,
                CloseButtonText = "OK"
            };

            ContentDialogResult result = await dialog.ShowAsync();
        }
    }
}
