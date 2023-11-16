
namespace _2210_001_CollinsSam_BraxtonOlterman_Project_3
{
    class Utils
    {
        /// <summary>
        /// Writes to the file at the given file path and writes the given content into it.
        /// </summary>
        /// <param name="filePath">The file path of the file to write to</param>
        /// <param name="content">The content to write to the file</param>
        public static void WriteToFile(string filePath, string content)
        {
            try
            {
                using (StreamWriter wtr = new StreamWriter(filePath))
                {
                    wtr.Write(content);
                }
            }
            catch
            {
                Console.WriteLine($"The file at path \"{filePath}\" could not be written to!");
            }
        }
    }
}
