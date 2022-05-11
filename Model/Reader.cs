using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using static Library.Model.Book;

namespace Library.Model
{
    class Reader
    {
        public Reader()
        {
        }
        public static void addRecord(string ReaderName, string BirthYear)
        {
            if (!ReaderName.Equals("") && !BirthYear.Equals(null))
            {
                string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

                using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
                {
                    connection.Open();
                    SqliteCommand CMD_Insert = new SqliteCommand();
                    CMD_Insert.Connection = connection;

                    CMD_Insert.CommandText = "INSERT INTO Reader (ReaderName, BirthYear) VALUES(@ReaderName, @BirthYear);";
                    CMD_Insert.Parameters.AddWithValue("@ReaderName", ReaderName);
                    CMD_Insert.Parameters.AddWithValue("@BirthYear", BirthYear);

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
                    CMD_Delete.CommandText = "DELETE FROM Reader WHERE ID = '" + ID + "'";

                    try
                    {
                        CMD_Delete.ExecuteReader();
                    }
                    catch(Exception ex)
                    {
                        if (ex.Message.Contains("FOREIGN KEY"))
                        {
                            WarningDialog("Нельзя удалить читателя, которому выданы книги");
                        }
                        else
                        {
                            WarningDialog("Произошла непредвиденная ошибка");
                        }
                    }

                    connection.Close();
                }
            }
        }

        public static void updateRecord(int? ID, String ReaderName, String BirthYear)
        {
            if (ID != null && ReaderName != "" && BirthYear != "")
            {
                string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

                using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
                {
                    connection.Open();

                    SqliteCommand CMD_Update = new SqliteCommand();
                    CMD_Update.Connection = connection;
                    CMD_Update.CommandText = "UPDATE Reader SET " +
                                             "ReaderName = '" + ReaderName + "', " +
                                             "BirthYear = '" + BirthYear + "'" +
                                             " WHERE ID = '" + ID + "'";
                    CMD_Update.ExecuteReader();

                    connection.Close();
                }
            }
        }

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
        public class readerDetails
        {
            public int ID { get; set; }
            public string ReaderName { get; set; }
            public String BirthYear { get; set; }
            public String BorrowedBooks { get; set; }

            public readerDetails()
            {
                this.ID = 0;
                this.ReaderName = "";
                this.BirthYear = "";
                this.BorrowedBooks = "";
            }

            public readerDetails(int ID, String ReaderName, String BirthYear, String BorrowedBooks)
            {
                this.ID = ID;
                this.ReaderName = ReaderName;
                this.BirthYear = BirthYear;
                this.BorrowedBooks = BorrowedBooks;
            }
        }

        public static List<readerDetails> GetRecords()
        {
            List<readerDetails> readerList = new List<readerDetails>();
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");
            using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
            {
                connection.Open();

                SqliteCommand cmd_getRec = new SqliteCommand("SELECT ID, ReaderName, BirthYear FROM Reader", connection);
                SqliteDataReader reader = cmd_getRec.ExecuteReader();

                while (reader.Read())
                {
                    int readerID = reader.GetInt32(0);
                    String borrowedBooks = "";

                    SqliteCommand cmd_getRec2 = new SqliteCommand("SELECT Title FROM Book WHERE ReaderID = " + readerID, connection);
                    SqliteDataReader reader2 = cmd_getRec2.ExecuteReader();

                    while (reader2.Read())
                    {
                        borrowedBooks += reader2.GetString(0) + ", ";
                    }

                    if (borrowedBooks.Length > 0)
                    {
                        borrowedBooks = borrowedBooks.Remove(borrowedBooks.Length - 2); // удаляем последнюю запятую и пробел
                    }

                    readerList.Add(new readerDetails(readerID, reader.GetString(1), reader.GetString(2), borrowedBooks));
                }

                connection.Close();
            }
            return readerList;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        public class lendBookDetails
        {
            public int BookID { get; set; }
            public String Title { get; set; }
            public String Author { get; set; }
            public String IssueYear { get; set; }

            public lendBookDetails(int BookID, String Title, String Author, String IssueYear)
            {
                this.BookID = BookID;
                this.Title = Title;
                this.Author = Author;
                this.IssueYear = IssueYear;
            }
        }
        
        public static List<lendBookDetails> FreeBookGetRecords()
        {
            List<lendBookDetails> freeBookList = new List<lendBookDetails>();
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

            using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
            {
                connection.Open();

                SqliteCommand cmd_getRec = new SqliteCommand("SELECT ID, Title, Author, IssueYear FROM Book WHERE ReaderID IS NULL", connection);
                SqliteDataReader reader = cmd_getRec.ExecuteReader();

                while (reader.Read())
                {
                    freeBookList.Add(new lendBookDetails(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }

                connection.Close();
            }

            return freeBookList;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class returnBookDetails
        {
            public int BookID { get; set; }
            public String Title { get; set; }
            public String Author { get; set; }
            public String IssueYear { get; set; }

            public returnBookDetails(int BookID, String Title, String Author, String IssueYear)
            {
                this.BookID = BookID;
                this.Title = Title;
                this.Author = Author;
                this.IssueYear = IssueYear;
            }
        }
        public static List<returnBookDetails> ReturnBookGetRecords(int ID)
        {
            List<returnBookDetails> returnBookList = new List<returnBookDetails>();
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Library.db");

            using (SqliteConnection connection = new SqliteConnection($"Filename = {pathToDb}"))
            {
                connection.Open();

                SqliteCommand cmd_getRec = new SqliteCommand("SELECT ID, Title, Author, IssueYear FROM Book WHERE ReaderID = " + ID, connection);
                SqliteDataReader reader = cmd_getRec.ExecuteReader();

                while (reader.Read())
                {
                    returnBookList.Add(new returnBookDetails(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }

                connection.Close();
            }

            return returnBookList;
        }
    }
}
