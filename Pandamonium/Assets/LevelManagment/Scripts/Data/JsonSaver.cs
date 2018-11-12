using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;

namespace LevelManagment.Data
{
    public class JsonSaver
    {
        private static readonly string _filename = "saveData1.sav";

        public static string GetSaveFilename()
        {
            return Application.persistentDataPath + "/" + _filename;
            //returns full file name with a path (internal to unity) for each platform (Mac, Win, iOS, android etc))
        }

        public void Save (SaveData data)
        {
            data.hashValue = String.Empty;

            string json = JsonUtility.ToJson(data);
            //string hashString = GetSHA256(json);
            //Debug.Log("hash string = " + hashString);

            data.hashValue = GetSHA256(json);
            json = JsonUtility.ToJson(data);

            string saveFilename = GetSaveFilename(); // get full file name(including the path)

            FileStream fileStream = new FileStream(saveFilename, FileMode.Create);

            // create a temporary object to write to a file 
            // generates json object on disk and automatically open and closes a file 
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(json); 
            }
        }

        public bool Load(SaveData data)
        {
            string loadFilenam = GetSaveFilename();
            if (File.Exists(loadFilenam))
            {
                using (StreamReader reader = new StreamReader(loadFilenam))
                {
                    string json = reader.ReadToEnd();

                    // check hash before reading 
                    if (CheckData(json))
                    {
                        Debug.Log("hashes are equal");
                        JsonUtility.FromJsonOverwrite(json, data);
                    }
                    else
                    {
                        Debug.LogWarning("JsonSaver Load(): invalid hash! Save file has been corrupted!");
                    }
                }
                return true;
            }
            return false;
        }

        private bool CheckData(string json)
        {
            SaveData tempSaveData = new SaveData(); //creating new SaveData with default values
            JsonUtility.FromJsonOverwrite(json, tempSaveData); //overriding tempSaveData with values from json 
        
            string oldHash = tempSaveData.hashValue; //getting old hash from json

            tempSaveData.hashValue = String.Empty; //we always clear hashValue before hashing!!

            string tempJson = JsonUtility.ToJson(tempSaveData); //converting tempSaveData to json string (with hasValue = Empty.String)
            string newHash = GetSHA256(tempJson); //hashing tempSaveData 

            return (oldHash == newHash); 
        }

        public void Delete()
        {
            File.Delete(GetSaveFilename());
        }

        public string GetHexStringFromHash(byte[] hash)
        {
            string hexString = String.Empty;

            foreach (byte b in hash)
            {
                hexString += b.ToString("x2"); //prints the input in Hexadecimal (x means - Hexadecimal string & 2 means - two digits) 
            }
            return hexString;
        }

        private string GetSHA256(string text)
        {
            byte[] textToBytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed mySHA256 = new SHA256Managed(); // temporary instance that we will use to calculate hash value 

            byte[] hashValue = mySHA256.ComputeHash(textToBytes);

            // return hex string
            return GetHexStringFromHash(hashValue);
        }
    } 
}
