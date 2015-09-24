using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.IO;

namespace ImportPDFs
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Z:\\DBs\\PoliciesProcedures\\dbPP.mdb";

            string queryString = "SELECT FileName,FileRawData from tblDirectives";
            int bufferSize = 4096;
            byte[] outByte = new byte[bufferSize];
            long retval;
            long startIndex = 0;

            using (OleDbConnection connection = new OleDbConnection(connectionString)) {
                OleDbCommand command = new OleDbCommand(queryString, connection);
                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //Response.Write("<p>" + reader[0] + "</p>");
                        string fn = "C:\\work\\Directives\\" + reader[0].ToString();
                        if (File.Exists(fn))
                        {
                            fn = fn + "-duplicate";
                        }
                        using (FileStream fs = new FileStream(fn, FileMode.CreateNew)) {
                            using (BinaryWriter w = new BinaryWriter(fs))
                            {
                                startIndex = 0;
                                retval = reader.GetBytes(1, startIndex, outByte, 0, bufferSize);
                                while (retval == bufferSize)
                                {
                                    w.Write(outByte);
                                    w.Flush();
                                    startIndex += bufferSize;
                                    retval = reader.GetBytes(1, startIndex, outByte, 0, bufferSize);
                                }
                                w.Write(outByte, 0, (int)retval - 1);
                                w.Flush();
                                w.Close();
                                fs.Close();
                            }
                        }

                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
        
        }
    }
}