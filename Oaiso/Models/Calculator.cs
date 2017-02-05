using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Oaiso.Models
{
    public class Calculator
    {
        private string connStr = ConfigurationManager.ConnectionStrings["AzureSQLConnectionString"].ConnectionString;

        /// <summary>
        /// 支払者と金額を受け取り、orderlogテーブルに挿入します。
        /// </summary>
        /// <param name="payer">支払者</param>
        /// <param name="amount">金額</param>
        /// <returns>ExecuteNonQueryの戻り値</returns>
        public int Exec(string payer, int amount, string user_id)
        {
            int result = -1;
            string insSql = 
                "insert into orderlog(name, amount, insert_user) values ('" + payer + "', " + amount + "', " + user_id + ");";
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = new SqlCommand(insSql, conn);
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }


        /// <summary>
        /// ユーザ名と支払合計金額をペアとして、すべてのユーザの結果を結合して返却します。
        /// </summary>
        /// <returns>すべてのユーザ名と支払合計金額</returns>
        public string FetchPaymentResultByName()
        {
            string selSql = "select name, sum(amount) as total from orderlog group by name;";
            var sb = new StringBuilder();
            sb.Append("はい、これまでの支払合計金額です。\n");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(selSql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    sb.Append(dr["name"].ToString());
                    sb.Append("さんは、");
                    int n = Int32.Parse(dr["total"].ToString());
                    string str = String.Format("{0:#,0} 円", n); // 変換後
                    sb.Append(str + "  ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// orderlogテーブルを空にします。
        /// </summary>
        /// <returns>ExecuteNonQueryの戻り値</returns>
        public int TruncateOrderTable()
        {
            int result = -1;
            string trunSql = "truncate table orderlog;";
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = new SqlCommand(trunSql, conn);
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

    }
}