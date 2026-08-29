using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ConfigPreprocessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFilePath = ResolveConfigPath(args);
            if (string.IsNullOrWhiteSpace(configFilePath))
            {
                Console.WriteLine("错误：未能自动找到 HsMod.cfg，请在命令行传入配置文件路径。");
                return;
            }

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

        static string ResolveConfigPath(string[] args)
        {
            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                return args[0];
            }

            string baseDirectory = AppContext.BaseDirectory;
            foreach (string path in EnumerateCandidatePaths(baseDirectory))
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            string current = Directory.GetCurrentDirectory();
            foreach (string path in EnumerateCandidatePaths(current))
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return FindConfigFileInParents(baseDirectory, maxLevels: 4);
        }

        static IEnumerable<string> EnumerateCandidatePaths(string baseDirectory)
        {
            yield return Path.Combine(baseDirectory, "HsMod.cfg");
            yield return Path.Combine(baseDirectory, "BepInEx", "config", "HsMod.cfg");
        }

        static string FindConfigFileInParents(string startDirectory, int maxLevels)
        {
            if (string.IsNullOrWhiteSpace(startDirectory))
            {
                return null;
            }

            string current = Path.GetFullPath(startDirectory);
            for (int level = 0; level <= maxLevels && !string.IsNullOrEmpty(current); level++)
            {
                foreach (string path in EnumerateCandidatePaths(current))
                {
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                DirectoryInfo parent = Directory.GetParent(current);
                if (parent == null)
                {
                    break;
                }

                current = parent.FullName;
            }

            return null;
        }
    }
}
