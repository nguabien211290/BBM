using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BBM.Business.Infractstructure
{
    public class Helpers
    {
        public static int FindMin(int pageindex, int pagesize)
        {
            return ((pageindex - 1) * pagesize);
        }
        public static int GetQuantity(int total, int pageindex, int pagesize)
        {
            int max = (pageindex * pagesize);
            if (total > max)
            {
                return pagesize;
            }
            else
            {
                return pagesize - (max - total);
            }
        }
        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string GenerateToken(int length, List<string> listcode = null)
        {
            var isBreak = false;
            var rs = string.Empty;
            while (!isBreak)
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                while (0 < length--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }
                rs = res.ToString();

                if (listcode != null)
                    isBreak = listcode.Contains(rs) ? false : true;
                else
                    isBreak = true;
            }
            return rs;
        }
        public static string RolesAuthor(RolesEnum[] roles)
        {
            var role = "";
            if (roles != null && roles.Length > 0)
            {
                var lastItem = roles.Last();
                foreach (var item in roles)
                {
                    if (item == lastItem)
                        role += (int)Enum.Parse(typeof(RolesEnum), item.ToString());
                    else
                        role += (int)Enum.Parse(typeof(RolesEnum), item.ToString()) + ",";
                    // System.Enum.GetName(typeof(RolesEnum), item.value) + ",";
                }
            }
            return role;

        }
    }
}