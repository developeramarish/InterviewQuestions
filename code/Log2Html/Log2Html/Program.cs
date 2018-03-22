using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Log2Html
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("cmd:logconv inputfile outputfile");
            while (true)
            {
                try
                {
                    var cmd = Console.ReadLine();
                    var cmdArr = cmd.Split(' ');

                    var logconv = cmdArr[0];
                    var inputfile = cmdArr[1];
                    var outputfile = cmdArr[2];

                    if (logconv == "logconv")
                    {
                        var isExistFile = IsExistFile(inputfile);
                        if (isExistFile)
                        {
                            var log = "";

                            using (var stream = new StreamReader(inputfile))
                            {
                                log = stream.ReadToEnd();
                            }

                            var logArr = log.Split('[');
                            var index = 0;
                            var log_content = "";
                            foreach (var item in logArr)
                            {
                                if (index != 0)
                                {
                                    var itemArr = item.Split(']');

                                    var content = itemArr[1].Trim().Replace("\r\n", "<br>");
                                    var level = "";
                                    var tag = "";
                                    try
                                    {
                                        level = itemArr[0].Split(' ')[0].Split('.')[0];
                                        tag = itemArr[0].Split(' ')[0].Split('.')[1];
                                    }
                                    catch (Exception)
                                    {
                                        level = itemArr[0].Split(' ')[0];
                                        tag = "";
                                    }

                                    var time = $"{itemArr[0].Split(' ')[1]} {itemArr[0].Split(' ')[2]}";

                                    time = String2Unicode(time).Replace("\\u200e", "");
                                    time = Unicode2String(time);
                                    time = Convert.ToDateTime(time).ToString("yyyy年M月dd日 HH:mm:ss");

                                    log_content += $"<tr class=\"{level.ToLower()} {(index % 2 == 0 ? "tr1" : "tr0")}\"><td>‎{time}</td><td>{level}</td><td>{tag}</td><td>{content}</td>";
                                }
                                index++;
                            }

                            log_content = String2Unicode(log_content).Replace("\\u200e", "");
                            log_content = Unicode2String(log_content);

                            var tpl_content = "";
                            using (var stream = new StreamReader($"{Directory.GetCurrentDirectory()}/html/template.html"))
                            {
                                tpl_content = stream.ReadToEnd();
                            }

                            tpl_content = tpl_content.Replace("$log_content$", log_content);

                            var buffer = Encoding.UTF8.GetBytes(tpl_content);

                            CreateFile(outputfile, buffer);

                            Console.WriteLine("done.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("inputfile inexistence");
                        }
                    }
                    else
                    {
                        Console.WriteLine("please use the command：logconv inputfile outputfile");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("error");
                }
            }
        }

        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                if (!IsExistFile(filePath))
                {
                    FileInfo file = new FileInfo(filePath);

                    FileStream fs = file.Create();

                    fs.Write(buffer, 0, buffer.Length);

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string String2Unicode(string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        public static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
    }
}