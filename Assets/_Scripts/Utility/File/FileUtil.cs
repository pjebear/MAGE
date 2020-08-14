using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


static class FileUtil
{
    public enum FolderName
    {
        SaveFiles,
        DB
    }

    public enum FileName
    {
        ActionDB,
        AppearanceDB,
        CharacterDB,
        ConversationDB,
        EquipmentDB,
        ItemDB,
        PropDB,
        ScenarioDB,
        SpecializationDB,
        StoryDB,
        TeamDB,
    }

    public static bool FileExists(string relativePath, string fileName)
    {
        bool exists = false;

        string directoryPath = Path.Combine(Application.dataPath, relativePath.ToString());

        if (Directory.Exists(directoryPath))
        {
            string filePath = directoryPath + Path.DirectorySeparatorChar + fileName.ToString() + ".txt";

            if (File.Exists(filePath))
            {
                exists = true;
            }
        }
        return exists;
    }

    public static List<string> GetFolders(string relativePath)
    {
        List<string> folders = new List<string>();

        string path = Path.Combine(Application.dataPath, relativePath.ToString());

        if (Directory.Exists(path))
        {
            foreach (string folderPath in Directory.EnumerateDirectories(path))
            {
                string[] splitOnDirectory = folderPath.Split(Path.DirectorySeparatorChar);
                folders.Add(splitOnDirectory[splitOnDirectory.Length - 1]);
            }
        }
        
        return folders;
    }

    public static void WriteFile(string relativePath, string fileName, string content)
    {
        string path = Path.Combine(Application.dataPath, relativePath.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.WriteAllText(path + Path.DirectorySeparatorChar + fileName.ToString() + ".txt", content);
    }

    public static string ReadFile(string relativePath, string fileName)
    {
        string content = "";

        string path = Path.Combine(Application.dataPath, relativePath.ToString());
        if (File.Exists(path + Path.DirectorySeparatorChar + fileName.ToString() + ".txt"))
        {
            content = File.ReadAllText(path + Path.DirectorySeparatorChar + fileName.ToString() + ".txt");
        }

        return content;
    }
}



