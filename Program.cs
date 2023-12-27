using System;
using System.Diagnostics;
using System.Data.SqlClient;


namespace BackupDatabase
{
    class Program
    {

        static void Main()
        {
            // ... (your existing code)

            // Set up a timer to trigger the backup every 24 hours
            Timer timer = new Timer(BackupDatabaseCallback, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            Timer timerLog = new Timer(BackupDatabaseCallbackLog, null, TimeSpan.Zero, TimeSpan.FromHours(2));

            // Keep the application running
            Console.WriteLine("");
            Console.ReadLine();
        }

        private static void BackupDatabaseCallback(object state)
        {
            try
            {
                string serverName = "DESKTOP-UPKCF43\\SQLEXPRESS";
                string databaseName = "dbhcms";
                string backupPath = @"C:\MediConsult\BackupDatabase\BackupDatabase\bin\Debug\net7.0\BackupFile.bak";
                string compressedBackupPath = $"backup{DateTime.Now.ToString("dd-MM-yyyy")}.7z"; // Compressed file path

                string connectionString = $"Data Source={serverName};Initial Catalog={databaseName};Integrated Security=True;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {


                    Console.WriteLine("Wait .....");
                    // Create a backup command
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = $"BACKUP DATABASE [{databaseName}] TO DISK='{backupPath}' WITH INIT, FORMAT";
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    CompressFile(new[] { backupPath }, compressedBackupPath);
                }

                // Compress the backup file using 7-Zip

                Console.WriteLine($"Backup files compressed successfully. {compressedBackupPath}");

                Console.WriteLine($"Backup completed at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Backup failed: {ex.Message}");
            }
        }
        private static void BackupDatabaseCallbackLog(object state)
        {
            try
            {
                string serverName = "DESKTOP-UPKCF43\\SQLEXPRESS";
                string databaseName = "dbhcms";

                string backupPathLog = @"C:\MediConsult\BackupDatabase\BackupDatabase\bin\Debug\net7.0\BackupFile.trn";
                string compressedBackupPath = $"backup{DateTime.Now.ToString("dd-MM-yyyy")}.7z"; // Compressed file path

                string connectionString = $"Data Source={serverName};Initial Catalog={databaseName};Integrated Security=True;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string backupCommandText = $"BACKUP LOG [{databaseName}] TO DISK = '{backupPathLog}' ";
                    using (SqlCommand command = new SqlCommand(backupCommandText, connection))
                    {
                        connection.Open();

                        // Execute the backup command
                        command.ExecuteNonQuery();
                    }
                    CompressFile(new[] { backupPathLog }, compressedBackupPath);
                }

                // Compress the backup file using 7-Zip

                Console.WriteLine($"Backup Log files compressed successfully. {compressedBackupPath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Backup failed: {ex.Message}");
            }
        }



        private static void CompressFile(string[] sourceFilePaths, string destinationFilePath)
        {
            // Use 7-Zip to compress the file
            Process process = new Process();
            process.StartInfo.FileName = @"C:\MediConsult\BackupDatabase\BackupDatabase\bin\Debug\net7.0\7z.exe";
            string sourceFiles = string.Join(" ", sourceFilePaths);

            process.StartInfo.Arguments = $"a \"{destinationFilePath}\" {sourceFiles}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            // Wait for the compression process to finish
            process.WaitForExit();
        }
    }
}

