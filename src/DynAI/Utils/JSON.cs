using System;
using System.IO;
using Newtonsoft.Json;

namespace AI.Utilities
{
    internal static class Json
    {
        public static JsonSerializerSettings Settings { get; set; }

        /// <summary>
        /// Static constructor, used to initialise settings.
        /// </summary>
        static Json()
        {
            // (Radu) We don't want the deserialisation to break if some properties are empty.
            // So we need to specify the behaviour when such values are encountered.
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                CheckAdditionalContent = true,
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Deserialises a JSON string to the specified type.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to.</typeparam>
        /// <param name="json">The JSON string that needs to be deserialised.</param>
        /// <returns>The response deserialised as the supplied type.</returns>
        public static T FromJsonTo<T>(string json) where T : new()
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(json, Settings);
                return obj;
            }
            catch (Exception e)
            {
                throw new Exception("Deserialisation failed.");
            }
        }

        /// <summary>
        /// Deserialises a JSON file. Calls ReadFromFile and Deserialize in that order.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to.</typeparam>
        /// <param name="filepath">The JSON file to read.</param>
        /// <returns></returns>
        public static T FromJsonFileTo<T>(string filepath) where T : new()
        {
            try
            {
                var json = ReadJsonFileAsString(filepath);
                var obj = FromJsonTo<T>(json);
                return obj;
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Serialise any object into JSON string.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <returns>Json formatted string.</returns>
        public static string ToJson(object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj, Settings);
                return json;
            }
            catch (Exception e)
            {
                throw new Exception("Serialisation failed");
            }
        }

        /// <summary>
        /// Serialize any object into JSON string and saves it to the specified file.
        /// Wraps Serialize() and WriteToFile()
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="filepath">The JSON file to save to.</param>
        /// <returns>True if Serialization and WriteToFile succeeded, false if failed.</returns>
        public static bool ToJsonFile(object obj, string filepath)
        {
            var json = ToJson(obj);
            return WriteJsonStringToFile(json, filepath);
        }

        #region Private methods

        /// <summary>
        /// Saves JSON string to drive.
        /// </summary>
        /// <param name="json">Json formatted string.</param>
        /// <param name="filepath">Full file path including json extension.</param>
        /// <returns>True if succeeded, false if failed.</returns>
        private static bool WriteJsonStringToFile(string json, string filepath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filepath))) Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            try
            {
                File.WriteAllText(filepath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the entire contents of a file as a string
        /// </summary>
        /// <param name="filepath">The file to read</param>
        /// <returns>The file contents as a string</returns>
        private static string ReadJsonFileAsString(string filepath)
        {
            // validate filename, see https://msdn.microsoft.com/en-us/library/system.io.fileinfo.fileinfo(v=vs.110).aspx
            var path = new FileInfo(filepath);

            if (!path.Exists)
            {
                throw new FileNotFoundException("Could not find specified file!");
            }
            if (!IsAllowedFileExtension(path.Extension.ToUpper()))
            {
                throw new FormatException("The supplied extension is not valid.");
            }
            return File.ReadAllText(filepath);
        }

        /// <summary>
        /// Returns whether a supplied file extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static bool IsAllowedFileExtension(string extension)
        {
            // (Radu) don't forget we need to remove the . from the extension
            var ext = extension.TrimStart('.');

            AllowedFileExtensions a;
            return Enum.TryParse(ext, true, out a);
        }

        private enum AllowedFileExtensions
        {
            JSON
        }

        #endregion
    }
}