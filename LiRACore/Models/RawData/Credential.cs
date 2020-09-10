using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.RawData
{
    public class Credential
    {
        //host: lira-db.documents.azure.com
        //port: 10255
        //ssl: self-signed(true)
        //username: lira-db
        //password: hNnB67meta8nPbLFn5zWpRlcAXwpOkGYgbCceruIKFfGT06fJtsgN9UWUXXQ7KijZzB5Ezlc5CjBjLw5PHdBYA==

        public string Host { get; set; }
        public string Port { get; set; }
        public string SSl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }




    }
}
