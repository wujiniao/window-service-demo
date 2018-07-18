using System.ComponentModel;
using System.Configuration.Install;
using System.Xml;

namespace cmd_service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstaller1.ServiceName = Get_ConfigValue(this.GetType().Assembly.Location + ".config", "ServiceName");
            serviceInstaller1.Description = Get_ConfigValue(this.GetType().Assembly.Location + ".config", "ServiceDescription");
        }

        protected static string Get_ConfigValue(string configpath, string strKeyName)
        {
            using (XmlTextReader tr = new XmlTextReader(configpath))
            {
                while (tr.Read())
                {
                    if (tr.NodeType == XmlNodeType.Element)
                    {
                        if (tr.Name == "add" && tr.GetAttribute("key") == strKeyName)
                        {
                            return tr.GetAttribute("value");
                        }
                    }
                }
            }
            return "";
        }
    }
}
