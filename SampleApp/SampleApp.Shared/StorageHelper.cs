using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SampleApp
{
    public static class StorageHelper
    {
        public enum StorageLocation
        {
            /// <summary>Local, isolated folder</summary>
            Local,
            /// <summary>Cloud, isolated folder. 100k cumulative limit.</summary>
            Roaming,
            /// <summary>Local, temporary folder (not for settings)</summary>
            Temporary
        }

        #region Settings

        public static bool SettingsExists(string key, StorageLocation location = StorageLocation.Local)
        {
            switch (location)
            {
                case StorageLocation.Local:
                    return ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
                case StorageLocation.Roaming:
                    return ApplicationData.Current.RoamingSettings.Values.ContainsKey(key);
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }

        public static T GetSetting<T>(string key, T otherwise = default(T), StorageLocation location = StorageLocation.Local)
        {
            try
            {
                if (!(SettingsExists(key, location)))
                {
                    return otherwise;
                }

                switch (location)
                {
                    case StorageLocation.Local:
                        return (T)ApplicationData.Current.LocalSettings.Values[key];
                    case StorageLocation.Roaming:
                        return (T)ApplicationData.Current.RoamingSettings.Values[key];
                    default:
                        throw new NotSupportedException(location.ToString());
                }
            }
            catch
            {
                return otherwise;
            }
        }

        public static void SetSetting<T>(string key, T value, StorageLocation location = StorageLocation.Local)
        {
            switch (location)
            {
                case StorageLocation.Local:
                    ApplicationData.Current.LocalSettings.Values[key] = value;
                    break;
                case StorageLocation.Roaming:
                    ApplicationData.Current.RoamingSettings.Values[key] = value;
                    break;
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }

        public static void DeleteSetting(string key, StorageLocation location = StorageLocation.Local)
        {
            switch (location)
            {
                case StorageLocation.Local:
                    ApplicationData.Current.LocalSettings.Values.Remove(key);
                    break;
                case StorageLocation.Roaming:
                    ApplicationData.Current.RoamingSettings.Values.Remove(key);
                    break;
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }

        #endregion

        #region File

        public static async Task<bool> FileExistsAsync(string key, StorageLocation location = StorageLocation.Local)
        {
            return (await GetIfFileExistsAsync(key, location)) != null;
        }

        public static async Task<bool> FileExistsAsync(string key, StorageFolder folder)
        {
            return (await GetIfFileExistsAsync(key, folder)) != null;
        }

        public static async Task<bool> DeleteFileAsync(string key, StorageLocation location = StorageLocation.Local)
        {
            var file = await GetIfFileExistsAsync(key, location);
            if (file != null)
            {
                await file.DeleteAsync();
            }

            return !(await FileExistsAsync(key, location));
        }

        public static async Task<T> ReadFileAsync<T>(string key, StorageLocation location = StorageLocation.Local)
        {
            try
            {
                // fetch
                var file = await GetIfFileExistsAsync(key, location);
                if (file == null)
                {
                    return default(T);
                }

                // read
                var textContent = await FileIO.ReadTextAsync(file);

                // convert to obj
                var result = Deserialize<T>(textContent);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> WriteFileAsync<T>(string key, T value, StorageLocation location = StorageLocation.Local)
        {
            // create file
            var file = await CreateFileAsync(key, location, CreationCollisionOption.ReplaceExisting);
            // convert to string
            var _String = Serialize(value);
            // save string to file
            await FileIO.WriteTextAsync(file, _String);
            // result
            return await FileExistsAsync(key, location);
        }

        private async static Task<StorageFile> CreateFileAsync(string key, StorageLocation location = StorageLocation.Local,
                                                                               CreationCollisionOption option = CreationCollisionOption.OpenIfExists)
        {
            switch (location)
            {
                case StorageLocation.Local:
                    return await ApplicationData.Current.LocalFolder.CreateFileAsync(key, option);
                case StorageLocation.Roaming:
                    return await ApplicationData.Current.RoamingFolder.CreateFileAsync(key, option);
                case StorageLocation.Temporary:
                    return await ApplicationData.Current.TemporaryFolder.CreateFileAsync(key, option);
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }
        private static async Task<StorageFile> GetIfFileExistsAsync(string key, StorageFolder folder,
                                                                                    CreationCollisionOption option = CreationCollisionOption.FailIfExists)
        {
            StorageFile file;
            try
            {
                file = await folder.GetFileAsync(key);
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
            return file;
        }

        private static async Task<StorageFile> GetIfFileExistsAsync(string key, StorageLocation location = StorageLocation.Local,
                                                                                    CreationCollisionOption option = CreationCollisionOption.FailIfExists)
        {
            StorageFile file;
            try
            {
                switch (location)
                {
                    case StorageLocation.Local:
                        file = await ApplicationData.Current.LocalFolder.GetFileAsync(key);
                        break;
                    case StorageLocation.Roaming:
                        file = await ApplicationData.Current.RoamingFolder.GetFileAsync(key);
                        break;
                    case StorageLocation.Temporary:
                        file = await ApplicationData.Current.TemporaryFolder.GetFileAsync(key);
                        break;
                    default:
                        throw new NotSupportedException(location.ToString());
                }
            }
            catch (FileNotFoundException e)
            {
                return null;
            }

            return file;
        }
        #endregion


        internal static string Serialize(object objectToSerialize)
        {
            // TODO: Replace with JSON.Net methods
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    var serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
                    serializer.WriteObject(stream, objectToSerialize);
                    stream.Position = 0;
                    StreamReader _Reader = new StreamReader(stream);
                    return _Reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    return string.Empty;
                }
            }
        }

        internal static T Deserialize<T>(string jsonString)
        {
            // TODO: Replace with JSON.Net methods
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(stream);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
