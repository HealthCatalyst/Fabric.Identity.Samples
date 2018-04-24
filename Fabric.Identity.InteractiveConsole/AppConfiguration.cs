using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Identity.InteractiveConsole
{
    public class AppConfiguration
    {
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public int Port { get; set; }
        public string Authority { get; set; }
        public string HostBaseUri { get; set; }
    }
}
