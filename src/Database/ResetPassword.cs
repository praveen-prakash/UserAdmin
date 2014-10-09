﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.Web;
namespace UserAdminApp.Database {

    [Database]
    public class ResetPassword {

        public Concepts.Ring3.SystemUser User { get; private set; }
        public DateTime Expire { get; private set; }
        public string Token { get; private set; }

        public ResetPassword(Concepts.Ring3.SystemUser user) {
            this.User = user;

            //this.Token = System.Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            this.Token = HttpUtility.UrlEncode(Guid.NewGuid().ToString()); 
            this.Expire = DateTime.UtcNow.AddMinutes(1440);
        }

        public ResetPassword(Concepts.Ring3.SystemUser user, int expire) {
            this.User = user;
            this.Token = HttpUtility.UrlEncode(Guid.NewGuid().ToString());
            //this.Token = System.Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            this.Expire = DateTime.UtcNow.AddMinutes(expire);
        }

    }
}