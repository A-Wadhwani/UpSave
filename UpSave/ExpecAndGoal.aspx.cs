﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UpSave.Our_Scripts;

namespace UpSave
{
    public partial class ExpecAndGoal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String store = HttpContext.Current.Request.Url.Query;
            if(store != null)
            {
                if (store.Split('=').Length >= 2)
                {
                    if (store.Split('=')[0].Equals("?time_range"))
                    {
                        int time = int.Parse(store.Split('=')[1]);
                        string user_id = Cookies.ReadCookie(this.Request, this.Response);
                        String account_id = Account.GetAccountID(user_id);
                        Payments[] payments = Payments.RetrievePayments(account_id);
                        decimal[] stuff = Payments.spending(account_id);
                        decimal[] save = RequestHandling.ExpectedSavings(Payments.spending_, 20000, Account.GetAccount(user_id).balance, time);
                        approx.InnerText = String.Format("Approximate Savings over {0} months: ${1:#.##}", time, save[1]);
                        Months.InnerText = String.Format("Time Span: {0} Months", time);
                        Range.InnerText = String.Format("Range: ${0:#.##} - ${1:#.##}", save[2], save[0]);
                        Likely.InnerText = String.Format("Likely: ${0:#.##}", save[1]);
                        time_range.Value = time.ToString();
                    }
                    string store1= store.Split('=')[0];
                    if (store1.Split('&')[0].Equals("?number"))
                    {

                        string user_id = Cookies.ReadCookie(this.Request, this.Response);
                        string account_id = Account.GetAccountID(user_id);

                        Payments[] allPayments = Payments.RetrievePayments(account_id);
                        decimal[] lastMonth = new decimal[6];
                        for (int i = 0; i < allPayments.Length; i++)
                        {
                            Payments currPayment = allPayments[i];
                            string[] date = currPayment.purchase_date.Split('-');
                            if ((date[0].CompareTo("2020") == 0) && (date[1].CompareTo("02") == 0))
                            {
                                if (currPayment.description.ToLower() == "entertainment") lastMonth[0] += currPayment.amount;
                                if (currPayment.description.ToLower() == "outing") lastMonth[1] += currPayment.amount;
                                if (currPayment.description.ToLower() == "shopping") lastMonth[2] += currPayment.amount;
                                if (currPayment.description.ToLower() == "transport") lastMonth[3] += currPayment.amount;
                                if (currPayment.description.ToLower() == "grocery") lastMonth[4] += currPayment.amount;
                                if (currPayment.description.ToLower() == "dues") lastMonth[5] += currPayment.amount;
                            }
                        }

                        float goal = float.Parse(store.Split('=')[1].Split('&')[0]);
                        decimal[] suggestions = new decimal[6];
                        decimal[] new_spending = Payments.spending(account_id);
                        string message = "";
                        string[] category = { "Entertainment", "Outing", "Shopping", "Transport", "Groceries", "Dues" };

                        decimal currentSavings = Account.GetAccount(user_id).balance;
                        suggestions = RequestHandling.Goals(Payments.spending(account_id), lastMonth, 20000, currentSavings, goal);

                        int flag = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            new_spending[i] -= suggestions[i];
                            decimal[] savings = RequestHandling.ExpectedSavings(new_spending, 20000, currentSavings, 12);
                            message += category[i] + ": " + savings[1] + "\n";
                            float[] goalArray = new float[1];
                            goalArray[0] = goal;
                            decimal[] arr = RequestHandling.DecToFloat(goalArray);
                            if ((currentSavings + savings[1]) >= arr[0])
                            {
                                flag = 1;
                                break;
                            }
                        }
                        if (flag == 0)
                        {
                            message = "Our predictions believe that it will be very difficult to meet this goal within the next year. Please try again later.";
                        }
                        decimal[] savings_1 = RequestHandling.ExpectedSavings(new_spending, 20000, currentSavings,  12);
                        if (decimal.ToDouble(savings_1[1]) >= goal)
                        {
                            message = "You don't have to change your habits to reach your goals";
                        }
                        text_f.Value = message;
                    }
                }
            }
        }

        protected void get_range_ServerClick(object sender, EventArgs e)
        {
            
            System.Diagnostics.Debug.WriteLine(time_range.Value);
            int time = int.Parse(time_range.Value);
            string user_id = Cookies.ReadCookie(this.Request, this.Response);
            String account_id = Account.GetAccountID(user_id);
            Payments[] payments = Payments.RetrievePayments(account_id);
            decimal[] stuff = Payments.spending(account_id);
            decimal[] save = RequestHandling.ExpectedSavings(Payments.spending_, 20000, Account.GetAccount(user_id).balance, time);
            approx.InnerText= String.Format("Approximate Savings over {0} months: ${1:#.##}", time, save[1]);
            Months.InnerText = String.Format("Time Span: {0} Months", time);
            Range.InnerText = String.Format("Range: ${0:#.##} - ${1:#.##}", save[2], save[0]);
            Likely.InnerText = String.Format("Likely: ${0:#.##}", save[1]);
        }

        protected void get_suggestions(object sender, EventArgs e)
        {
            

        }
    }
}