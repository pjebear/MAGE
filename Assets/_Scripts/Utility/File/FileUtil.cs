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
        SaveFiles
    }

    public enum FileName
    {
        CharacterDB,
        EquipmentDB,
        SpecializationDB,
        TeamDB,
    }

    public static void Write(FolderName folder, FileName fileName, string content)
    {
        string path = Path.Combine(Application.dataPath, folder.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
       
        File.WriteAllText(path + Path.DirectorySeparatorChar + fileName.ToString() + ".txt", content);
    }

    public static string Read(FolderName folder, FileName fileName)
    {
        string content = "";

        string path = Path.Combine(Application.dataPath, folder.ToString());
        if (File.Exists(path + Path.DirectorySeparatorChar + fileName.ToString() + ".txt"))
        {
            content = File.ReadAllText(path + Path.DirectorySeparatorChar + fileName.ToString() + ".txt");
        }

        return content;
    }
}

