using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PInvoke;



namespace SystemProgramming_06__WinAPI_
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct StartupInfo
    {
        public int cb;
        public String reserved;
        public String desktop;
        public String title;
        public int x;
        public int y;
        public int xSize;
        public int ySize;
        public int xCountChars;
        public int yCountChars;
        public int fillAttribute;
        public int flags;
        public UInt16 showWindow;
        public UInt16 reserved2;
        public byte reserved3;
        public IntPtr stdInput;
        public IntPtr stdOutput;
        public IntPtr stdError;
    }

    internal struct ProcessInformation
    {
        public IntPtr process;
        public IntPtr thread;
        public int processId;
        public int threadId;
    }

    class Class1
    {


        public const UInt32 Infinite = 0xffffffff;
        public const Int32 Startf_UseStdHandles = 0x00000100;
        public const Int32 StdOutputHandle = -11;
        public const Int32 StdErrorHandle = -12;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithLogonW(
            String userName,
            String domain,
            String password,
            UInt32 logonFlags,
            String applicationName,
            String commandLine,
            UInt32 creationFlags,
            UInt32 environment,
            String currentDirectory,
            ref StartupInfo startupInfo,
            out ProcessInformation processInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr process, ref UInt32 exitCode);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr handle, UInt32 milliseconds);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(IntPtr handle);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);


    }

    class Program
    {


        static class UnmanagedCodeCall
        {
            public enum CRED_TYPE
            {
                GENERIC = 1,
                DOMAIN_PASSWORD = 2,
                DOMAIN_CERTIFICATE = 3,
                DOMAIN_VISIBLE_PASSWORD = 4,
                MAXIMUM = 5
            }
            [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode)]
            //функция удаляет учетные данные из набора учетных данных пользователя.
            public static extern bool CredDelete(string target, CRED_TYPE type, int flags);
            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]

            static extern IntPtr OpenEventLog(string UNCServerName, string sourceName);

            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool BackupEventLog(IntPtr hEventLog, string backupFile);

            [DllImport("advapi32.dll", SetLastError = true)]
            static extern bool CloseEventLog(IntPtr hEventLog);

            public static void SaveLog(string eventLogName, string destinationDirectory)
            {
                string exportedEventLogFileName = Path.Combine(destinationDirectory, eventLogName + ".evt");

                //Returns handle to Application log if Custom log does not exist.    
                IntPtr logHandle = OpenEventLog(Environment.MachineName, eventLogName);

                if (IntPtr.Zero != logHandle)
                {
                    bool retValue = BackupEventLog(logHandle, exportedEventLogFileName);
                    //If false, notify.
                    CloseEventLog(logHandle);
                }
            }

            [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
            static extern bool ConvertSidToStringSid(
                [MarshalAs(UnmanagedType.LPArray)] byte[] pSID,
                out IntPtr ptrSid);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool CryptAcquireContext(ref IntPtr hProv, string pszContainer,
                string pszProvider, uint dwProvType, uint dwFlags);

            [DllImport("Advapi32.dll")]
            public static extern bool GetUserName(StringBuilder lpBuffer, ref int nSize);
          
        }
        public class Crypt32
        {
            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptAcquireContext(
                out IntPtr phProv,
                string pszContainer,
                string pszProvider,
                uint dwProvType,
                uint dwFlags);
            public static uint CRYPT_NEWKEYSET = 0x8;

            private static long CRYPT_DELETEKEYSET = 0x10;

            private static long CRYPT_MACHINE_KEYSET = 0x20;

            private static long CRYPT_SILENT = 0x40;

            private static long CRYPT_DEFAULT_CONTAINER_OPTIONAL = 0x80;

            private static long CRYPT_VERIFYCONTEXT = 0xF0000000;

            public static uint PROV_RSA_FULL = 1;
        }
        static void Main(string[] args)
        {


            try
            {
                {//Первая функция удаляет учетные данные из набора учетных данных пользователя.
                    //UnmanagedCodeCall.CredDelete("Some", UnmanagedCodeCall.CRED_TYPE.GENERIC, 0);
                }
                {
                    //Вторая функция BackupEventLog
                    //DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                    //var userPathToDesktop = dir.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent
                    //    .GetDirectories().ToList()
                    //    .FirstOrDefault(w => w.Name == Environment.UserName).GetDirectories().FirstOrDefault(w=>w.Name=="Desktop");
                    //UnmanagedCodeCall.SaveLog("Some", userPathToDesktop?.FullName);
                    //Console.WriteLine("Успешно выполнилось!");
                }

                {
                    //Третья Функция Запускает новый процесс, открывает приложение в этом процессе и использует переданный Идентификатор пользователя и пароль. Открытое приложение выполняется под учетными данными и полномочиями переданного идентификатора пользователя.

                    /* StartupInfo startupInfo = new StartupInfo();
                     startupInfo.reserved = null;
                     startupInfo.flags &= Class1.Startf_UseStdHandles;
                     startupInfo.stdOutput = (IntPtr)Class1.StdOutputHandle;
                     startupInfo.stdError = (IntPtr)Class1.StdErrorHandle;

                     UInt32 exitCode = 123456;
                     ProcessInformation processInfo = new ProcessInformation();

                     String command = @"c:\windows\notepad.exe";
                     String user = "user";
                     String domain = System.Environment.MachineName;
                     String password = "";
                     String currentDirectory = System.IO.Directory.GetCurrentDirectory();

                     try
                     {
                         Class1.CreateProcessWithLogonW(
                             user,
                             domain,
                             password,
                             (UInt32)1,
                             command,
                             command,
                             (UInt32)0,
                             (UInt32)0,
                             currentDirectory,
                             ref startupInfo,
                             out processInfo);
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine(e.ToString());
                     }

                     Console.WriteLine("Running ...");
                     Class1.WaitForSingleObject(processInfo.process, Class1.Infinite);
                     Class1.GetExitCodeProcess(processInfo.process, ref exitCode);

                     Console.WriteLine("Exit code: {0}", exitCode);

                     Class1.CloseHandle(processInfo.process);
                     Class1.CloseHandle(processInfo.thread);*/
                }

                {
                    //Четвертая функция Используется для получения дескриптора к определенному контейнеру ключей в определенном поставщике служб шифрования (CSP).
                    /*IntPtr hProv = new IntPtr();
                    bool res = Crypt32.CryptAcquireContext(out hProv, "User", null, Crypt32.PROV_RSA_FULL, Crypt32.CRYPT_NEWKEYSET);
                    Console.WriteLine(hProv.ToString());
                    */
                }

                {
                    //5 Функция получение имени юзера
                    //StringBuilder Buffer = new StringBuilder(64);
                    //int nSize = 64;
                    //UnmanagedCodeCall.GetUserName(Buffer, ref nSize);
                    //Console.WriteLine(Buffer.ToString());
                }
                {
                  
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }
    }
}
