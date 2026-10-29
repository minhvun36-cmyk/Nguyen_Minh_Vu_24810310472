using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CafePOSSystem
{
    /// <summary>
    /// Lop truy cap du lieu theo mo hinh disconnected:
    /// SqlDataAdapter nap du lieu vao DataSet/DataTable; sau khi nap, UI lam viec voi du lieu trong bo nho.
    /// </summary>
    public static class Db
    {
        private static readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["CafePOS"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static DataSet QueryDataSet(string sql, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                adapter.Fill(ds);
            }
            return ds;
        }

        public static DataTable Query(string sql, params SqlParameter[] parameters)
        {
            DataSet ds = QueryDataSet(sql, parameters);
            if (ds.Tables.Count == 0) return new DataTable();
            return ds.Tables[0];
        }

        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static SqlParameter P(string name, SqlDbType type, object value)
        {
            return new SqlParameter(name, type) { Value = value ?? DBNull.Value };
        }

        public static SqlParameter NVarChar(string name, int size, string value)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, size) { Value = value ?? "" };
        }

        public static SqlParameter Decimal(string name, decimal value)
        {
            SqlParameter p = new SqlParameter(name, SqlDbType.Decimal);
            p.Precision = 18;
            p.Scale = 2;
            p.Value = value;
            return p;
        }
    }
}
