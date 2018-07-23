using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Linq;

namespace cmd_service
{
    public partial class CmdService: ServiceBase
    {
        // docker run -d --name avl-littlefs -it -p 8088:8088 docker-registry.innovation.os/base/littlefs:latest /bin/openresty -p /opt/littlefs/ -g "daemon off;"
        string StartCmd = string.Empty;
        // docker stop avl-littlefs && docker rm avl-littlefs
        string StopCmd = string.Empty;

        public CmdService(string ServiceName)
        {
            this.InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
            this.ServiceName = ServiceName;
            if (!EventLog.SourceExists(ServiceName))
            {
                EventLog.CreateEventSource(ServiceName, ServiceName + "Log");
            }
            evt.Source = ServiceName;
            evt.Log = string.Empty;
            WriteLog(ServiceName);
            StartCmd = ConfigurationManager.AppSettings["StartCmd"];
            StopCmd = ConfigurationManager.AppSettings["StopCmd"];
            WriteLog(ServiceName);
            WriteLog(StartCmd);
            WriteLog(StopCmd);
        }

        protected override void OnStart(string[] args)
        {
            evt.WriteEntry(string.Format("启动服务{0}", ServiceName));
            WriteLog(string.Format("启动服务{0}", ServiceName));

            string output = string.Empty;
            ServiceController[] services = ServiceController.GetServices();
            if (services.Count(t => t.ServiceName == ServiceName) > 0)
            {
                RunCmd(StopCmd, out output);
            }
           
            if (RunCmd(StartCmd, out output))
            {
                evt.WriteEntry(string.Format("启动服务{0}成功", ServiceName));
                WriteLog(string.Format("启动服务{0}成功", ServiceName));
            }
            else
            {
                evt.WriteEntry(string.Format("启动服务{0}失败, \t{1}", ServiceName, output));
                WriteLog(string.Format("启动服务{0}失败, \t{1}", ServiceName, output));
            }
          
        }

        protected override void OnStop()
        {
            evt.WriteEntry(string.Format("关闭服务{0}", ServiceName));
            WriteLog(string.Format("关闭服务{0}", ServiceName));
            string output = string.Empty;
            if (RunCmd(StopCmd, out output))
            {
                evt.WriteEntry(string.Format("关闭服务{0}成功", ServiceName));
                WriteLog(string.Format("关闭服务{0}成功", ServiceName));
            }
            else
            {
                evt.WriteEntry(string.Format("关闭服务{0}失败,\t{1}", ServiceName, output));
                WriteLog(string.Format("关闭服务{0}失败,\t{1}", ServiceName, output));
            }
          
        }

        static bool RunCmd(string cmd, out string output)
        {
            try
            {
                output = RunCmd(cmd);
                return true;
            }
            catch (Exception e)
            {
                output = e.Message;
                return false;
            }
        }

        static string RunCmd(string cmd)
        {
            string output = string.Empty;
            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态  
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动  
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息  
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息  
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出  
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口  
                p.Start();//启动程序  

                //向cmd窗口写入命令  
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息  
                output = p.StandardOutput.ReadToEnd();
                string stderrx = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(stderrx))
                    throw new Exception("command run error, return \n\t" + stderrx);
                p.WaitForExit();//等待程序执行完退出进程  
                p.Close();
                return output;
            }
        }
        
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        private void WriteLog(string msg)
        {

            //string path = @"C:\log.txt";

            //该日志文件会存在windows服务程序目录下
            string path = string.Format("{0}_log.txt", this.GetType().Assembly.Location);
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                FileStream fs;
                fs = File.Create(path);
                fs.Close();
            }

            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString() + "   " + msg);
                }
            }
        }
    }
}
