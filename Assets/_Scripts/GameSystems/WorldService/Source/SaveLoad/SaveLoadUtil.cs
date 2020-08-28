using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MAGE.GameSystems.World.Internal
{

    static class SaveLoadUtil
    {
        public static string GetNextAvailableSaveFileName()
        {
            List<string> saveFileNames = GetSaveFiles();
            string nextAvailableName = string.Format("SAVE_FILE_{0}", saveFileNames.Count);

            return nextAvailableName;
        }

        public static List<string> GetSaveFiles()
        {
            return FileUtil.GetFolders(FileUtil.FolderName.SaveFiles.ToString());
        }

        public static SaveLoad.SaveFile Load(string saveFileName)
        {
            string saveFilePath = Path.Combine(FileUtil.FolderName.SaveFiles.ToString(), saveFileName);

            DBService.Get().Load(saveFilePath);

            SaveLoad.SaveFile saveFile = UnityEngine.JsonUtility.FromJson<SaveLoad.SaveFile>(FileUtil.ReadFile(saveFilePath, saveFileName));

            return saveFile;
        }

        public static void Save(SaveLoad.SaveFile saveFile)
        {
            string saveFilePath = Path.Combine(FileUtil.FolderName.SaveFiles.ToString(), saveFile.Name);

            DBService.Get().Save(saveFilePath);

            FileUtil.WriteFile(saveFilePath, saveFile.Name, UnityEngine.JsonUtility.ToJson(saveFile));
        }

        public static void AddParty(SaveLoad.SaveFile saveFile, PartyInfo party)
        {
            saveFile.Party.CharacterIds.AddRange(party.CharacterIds);
            saveFile.Party.Currency = party.Currency;

            saveFile.Party.ItemIds.AddRange(party.Inventory.Items.Keys);
            saveFile.Party.ItemCounts.AddRange(party.Inventory.Items.Values);
        }

        public static PartyInfo ExtractParty(SaveLoad.SaveFile saveFile)
        {
            PartyInfo party = new PartyInfo();

            party.CharacterIds.AddRange(saveFile.Party.CharacterIds);
            party.Currency = saveFile.Party.Currency;

            for (int i = 0; i < saveFile.Party.ItemIds.Count; ++i)
            {
                party.Inventory.Add(saveFile.Party.ItemIds[i], saveFile.Party.ItemCounts[i]);
            }

            return party;
        }
    }
}