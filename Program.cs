using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ConfigPreprocessor
{
    class Program
    {
        static void Main(string[] args)
        {
            // 配置文件路径
            string configFilePath = @"D:\OW\Hearthstone\BepInEx\config\HsMod.cfg";

            // 设置端口范围
            int minPort = 58744;
            int maxPort = 65535;

            // 生成一个随机端口
            Random random = new Random();
            int randomPort = random.Next(minPort, maxPort);

            // 检查端口是否可用，如果不可用则重新生成
            int maxAttempts = 100;
            int attempts = 0;
            while (!IsPortAvailable(randomPort) && attempts < maxAttempts)
            {
                randomPort = random.Next(minPort, maxPort);
                attempts++;
            }

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"错误：找不到配置文件 {configFilePath}");
                return;
            }

            // 读取配置文件内容
            string[] configLines = File.ReadAllLines(configFilePath);

            // 查找并更新端口配置
            bool found = false;
            for (int i = 0; i < configLines.Length; i++)
            {
                if (configLines[i].TrimStart().StartsWith("网站端口"))
                {
                    // 替换为随机端口
                    configLines[i] = $"网站端口 = {randomPort}";
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("警告：未找到 '网站端口' 配置项");
                return;
            }

            // 将更新后的内容写回配置文件
            File.WriteAllLines(configFilePath, configLines);

            // 输出成功信息
            Console.WriteLine($"配置文件已更新，端口设置为：{randomPort}");
        }

        static bool IsPortAvailable(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}