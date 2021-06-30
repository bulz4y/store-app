using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Prodavnica
{
    class Baza
    {

        private OleDbConnection conn;

        public Baza() {
            conn = new OleDbConnection();
            //conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pela747\Desktop\C# Everything\projekti\Prodavnica App C#\Prodavnica\Prodavnica\Prodavnica.accdb";
            conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\Web i Ostalo\Other than javascript\C# programiranje\C# Everything\projekti\Prodavnica App C#\Prodavnica\Prodavnica\Prodavnica.accdb";
        }

        public void openConnection() {
            if (conn != null) {
                conn.Open();
            }
        }

        public void closeConnection()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public OleDbConnection Conn {
            get
            {
                return conn;
            }
            set
            {
                conn = value;
            }
        }
    }
}
