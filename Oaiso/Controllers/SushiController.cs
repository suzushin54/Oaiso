using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Oaiso.Models;

namespace Oaiso.Controllers
{
    public class SushiController : ApiController
    {
        /// <summary>
        /// Slackの「/oaiso」コマンドから呼び出され、寿司の支払いを記録します。
        /// </summary>
        /// <param name="param">Slackが送ってくるOutgoing Data</param>
        /// <returns>orderlogへの登録が成功したか否か</returns>
        [Route("api/sushi/oaiso")]
        public HttpResponseMessage Oaiso([FromBody]SlackParameters param)
        {
            int amount = 0;
            var response = new SlackResponseParameter();
            string[] values = param.text.Split(' ');

            try
            {
                var slotName = Environment.GetEnvironmentVariable("SlackToken") ?? "None";
                if (slotName.Equals("None"))
                    return Result(HttpStatusCode.OK, "お前誰？");

                // 受け取ったtextをチェック。支払者と金額が正しく入力されていればOK.
                if ((values.Length == 2) && Int32.TryParse(values[1], out amount))
                {
                    // 支払者と金額をDB登録して、その結果を受け取る
                    var calc = new Calculator();
                    int execRetval = calc.Exec(values[0].ToString(), amount);

                    // 登録に成功したら、orderlogに存在する支払者リストを取得して、支払い合計金額を支払者別に取得する
                    response.text = (execRetval == 1) ? "支払いを記録したぜ。" : "支払いの記録に失敗したぜ？";
                } else
                {
                    response.text = "なに言ってんだお前？";
                }
            }
            catch (Exception)
            {
                response.text = "なんかヘンだぜ？";
            }
            response.response_type = "in_channel";
            return Result(HttpStatusCode.OK, response);
        }


        /// <summary>
        /// Slackの「/tsuke」コマンドから呼び出され、現在の支払記録を照会します。
        /// </summary>
        /// <param name="param">Slackが送ってくるOutgoing Data</param>
        /// <returns>orderlogに存在する支払者と支払合計金額</returns>
        [Route("api/sushi/tsuke")]
        public HttpResponseMessage Tsuke([FromBody]SlackParameters param)
        {
            var response = new SlackResponseParameter();
            var calc = new Calculator();
            string retval = string.Empty;
            try
            {
                var slotName = Environment.GetEnvironmentVariable("SlackTokenOkami") ?? "None";
                if (slotName.Equals("None"))
                    return Result(HttpStatusCode.OK, "あなた誰？");
                
                // Optionがclearならtsukeをクリア、showなら支払記録を照会する
                if (param.text.Equals("clear"))
                {
                    int truncResult = calc.TruncateOrderTable();
                    retval = "支払記録をリセットしましたよ。";
                }
                else if(param.text.Equals("show"))
                {
                    retval = calc.FetchPaymentResultByName();
                } else
                {
                    retval = "よくわからないわ。";
                }

                // orderlogに存在する支払者リストを取得して、支払い合計金額を支払者別に取得する
                response.text = retval;
                response.response_type = "in_channel";
            }
            catch (Exception)
            {
                response.text = "なにかおかしいわね。" + param.text;
            }
            return Result(HttpStatusCode.OK, response);
        }

        /// <summary>
        /// Slackへ返すデータを組み立てる
        /// </summary>
        private HttpResponseMessage Result<T>(HttpStatusCode status, T content)
        {
            HttpResponseMessage response = new HttpResponseMessage(status);

            if (content != null)
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(content));
            }
            response.StatusCode = status;
            response.Content.Headers.ContentType.MediaType = "application/json";

            return response;
        }
    }
}
