using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: Append 2 files or 2 directories.");
            Console.ReadKey();
            return;
        }
        {
            DirectoryInfo src = new DirectoryInfo(args[0]);
            DirectoryInfo dst = new DirectoryInfo(args[1]);
            if (src.Exists && dst.Exists)
            {
                Console.WriteLine("Comparison Mode: Directory");
                Console.WriteLine("Result: ('+' = Add, '-' = Remove, '*' = Modify, 'D' = Directory, 'F' = File)");
                CompareFolders(src, dst, src.FullName.Length + 1, dst.FullName.Length + 1);
                Console.WriteLine("Comparison done.");
                Console.ReadKey();
                return;
            }
        }
        {
            FileInfo src = new FileInfo(args[0]);
            FileInfo dst = new FileInfo(args[1]);
            if (src.Exists && dst.Exists)
            {
                Console.WriteLine("Comparison Mode: File");
                Console.WriteLine($"Result: {(CompareFiles(src, dst) ? "The Same" : "Different")}");
                Console.WriteLine("Comparison done.");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("Error: Unexpected inputs.");
        Console.ReadKey();
    }

    private static bool CompareFolders(DirectoryInfo src, DirectoryInfo dst, int preSrc, int preDst)
    {
        bool result = true;
        {
            DirectoryInfo[] listSrc = src.GetDirectories();
            DirectoryInfo[] listDst = dst.GetDirectories();
            Dictionary<string, DirectoryInfo> mapSrc = new Dictionary<string, DirectoryInfo>(listSrc.Length);
            Dictionary<string, DirectoryInfo> mapDst = new Dictionary<string, DirectoryInfo>(listDst.Length);
            foreach (DirectoryInfo info in listSrc)
            {
                mapSrc.Add(info.Name.ToLower(), info);
            }
            foreach (DirectoryInfo info in listDst)
            {
                mapDst.Add(info.Name.ToLower(), info);
            }
            IEnumerable<string> common = mapSrc.Keys.Intersect(mapDst.Keys);
            foreach (string name in common)
            {
                if (!CompareFolders(mapSrc[name], mapDst[name], preSrc, preDst))
                {
                    result = false;
                }
            }
            foreach (string name in mapSrc.Keys.Except(common))
            {
                result = false;
                Console.WriteLine($"-D\t{mapSrc[name].FullName.Substring(preSrc)}");
            }
            foreach (string name in mapDst.Keys.Except(common))
            {
                result = false;
                Console.WriteLine($"+D\t{mapDst[name].FullName.Substring(preDst)}");
            }
        }
        {
            FileInfo[] listSrc = src.GetFiles();
            FileInfo[] listDst = dst.GetFiles();
            Dictionary<string, FileInfo> mapSrc = new Dictionary<string, FileInfo>(listSrc.Length);
            Dictionary<string, FileInfo> mapDst = new Dictionary<string, FileInfo>(listDst.Length);
            foreach (FileInfo info in listSrc)
            {
                mapSrc.Add(info.Name.ToLower(), info);
            }
            foreach (FileInfo info in listDst)
            {
                mapDst.Add(info.Name.ToLower(), info);
            }
            IEnumerable<string> common = mapSrc.Keys.Intersect(mapDst.Keys);
            foreach (string name in common)
            {
                if (!CompareFiles(mapSrc[name], mapDst[name]))
                {
                    result = false;
                    Console.WriteLine($"*F\t{mapSrc[name].FullName.Substring(preSrc)}");
                }
            }
            foreach (string name in mapSrc.Keys.Except(common))
            {
                result = false;
                Console.WriteLine($"-F\t{mapSrc[name].FullName.Substring(preSrc)}");
            }
            foreach (string name in mapDst.Keys.Except(common))
            {
                result = false;
                Console.WriteLine($"+F\t{mapDst[name].FullName.Substring(preDst)}");
            }
        }
        return result;
    }

    private static bool CompareFiles(FileInfo src, FileInfo dst) => md5.ComputeHash(File.ReadAllBytes(src.FullName)).SequenceEqual(md5.ComputeHash(File.ReadAllBytes(dst.FullName)));

    private static readonly MD5 md5 = MD5.Create();
}