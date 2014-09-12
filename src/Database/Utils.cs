﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UserAdminApp.Database {

    public class Utils {

        static public bool IsValidEmail(string email) {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        static public string GetGravatarUrl(string email) {

            using (MD5 md5Hash = MD5.Create()) {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++) {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return "http://www.gravatar.com/avatar/" + sBuilder.ToString() + "?s=32&d=mm";
            }
        }
    }
}
