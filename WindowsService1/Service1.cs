using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SQLite;
using System.Data.Linq;
using Newtonsoft.Json.Linq;
using WindowsService1.Models.Request;
using Newtonsoft.Json;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        

        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            using (var conn = new System.Data.SQLite.SQLiteConnection("data source=C:/Users/colintu/source/repos/WindowsService1/OverPaymentDB.db"))
            {
                using (var db = new OverpaymentDBModelContainer1())
                {
                    var overpaymentList = db.OverPayments; 
                    foreach (var overpayment in overpaymentList)
                    {
                        //var overpaymentRequest = new OverpaymentRequest
                        //{
                        //    OverPyamentID = overpayment.OverPaymentID,
                        //    MemberId = overpayment.MemberId,
                        //    ClaimNumber = overpayment.ClaimNumber,
                        //    BalanceAmt = overpayment.BalanceAmt,
                        //    OverPaymentAmt = overpayment.OverPaymentAmt
                        //};
                        JObject overpaymentDetail = new JObject
                                                                    {
                                                                        new JProperty("overpayment_id", overpayment.OverPaymentID),
                                                                        new JProperty("claim_number", overpayment.ClaimNumber),
                                                                        new JProperty("member_id", overpayment.MemberId),
                                                                        new JProperty("balamce_amt", overpayment.BalanceAmt),
                                                                        new JProperty( "overpayment_amt", overpayment.OverPaymentAmt)
                                                                    };
                        //using (var client = new HttpClient())
                        //{
                        //    client.BaseAddress = new Uri("http://overpaymentapi.local/");
                        //    //var content = new StringContent(overpaymentDetail.ToString(), Encoding.UTF8, "application/json");
                        //    var postTask = client.PostAsJsonAsync<string>("api/overpayment/post", overpaymentDetail.ToString());
                        //    postTask.Wait();
                        //    postTask.Wait();

                        //    var result = postTask.Result;
                        //}

                        using (var client = new HttpClient())
                        using (var request = new HttpRequestMessage(HttpMethod.Post, "http://overpaymentapi.local/api/overpayment/post"))
                        {
                            var json = JsonConvert.SerializeObject(overpaymentDetail);
                            using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                            {
                                request.Content = stringContent;

                                using (var response = await client
                                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                                    .ConfigureAwait(false))
                                {
                                    response.EnsureSuccessStatusCode();
                                }
                            }
                        }
                    }



                    //var overpaymentIDList = (from overpayment in db.OverPayments select overpayment.OverPaymentID).ToList();
                    //var overPaymentDetails = new JArray(from overPayment in db.OverPayments
                    //                                        select new JObject
                    //                                                {
                    //                                                    new JProperty("overpayment_id", overPayment.OverPaymentID),
                    //                                                    new JProperty("claim_number", overPayment.ClaimNumber),
                    //                                                    new JProperty("member_id", overPayment.MemberId),
                    //                                                    new JProperty("balamce_amt", overPayment.BalanceAmt),
                    //                                                    new JProperty( "overpayment_amt", overPayment.OverPaymentAmt)
                    //                                                });
                    //foreach (var overpaymentDetail in overPaymentDetails)
                    //{
                    //    using (var client = new HttpClient())
                    //    {
                    //        client.BaseAddress = new Uri("http://webapi.local/");
                    //        var postTask = client.PostAsJsonAsync<string>("api/overpayment/post", overpaymentDetail.ToString());
                    //        postTask.Wait();
                    //    }
                    //}
                }
            }

            await Task.Delay(1000);
        }







        protected override void OnStop()
        {
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
