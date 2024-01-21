using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DesktopPet
{
    /// <summary>
    /// Main for the application. Once the application is started, this class will create all objects.
    /// </summary>
    class Program
    {
        /// <summary>
        /// StartUp is the main program.
        /// </summary>
        public static StartUp Mainthread;

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

#if PORTABLE
        /// <summary>
        /// Argument: load local animation XML.
        /// </summary>
        public static string ArgumentLocalXML = "";

        /// <summary>
        /// Argument: load animation XML from web.
        /// </summary>
        public static string ArgumentWebXML = "";

        /// <summary>
        /// Argument: open the installer when application starts.
        /// </summary>
        public static string ArgumentInstall = "";

        public static LocalData MyData = new LocalData();

        /// <summary>
        /// Open the option dialog, to show some options like reset XML animation or load animation from the webpage.
        /// </summary>
        public static void OpenOptionDialog()
        {
            FormOptions formoptions = new FormOptions();
            switch (formoptions.ShowDialog())
            {
                case DialogResult.Retry:
                    StartUp.AddDebugInfo(StartUp.DEBUG_TYPE.warning, "restoring default XML");

                    MyData.SetIcon("");
                    MyData.SetImages("");
                    MyData.SetXml("","");
                    break;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EmbeddedAssembly.Load("DesktopPet.Portable.NAudio.dll", "NAudio.dll");
            EmbeddedAssembly.Load("DesktopPet.Portable.Newtonsoft.Json.dll", "Newtonsoft.Json.dll");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            // Check and parse the arguments
            string SearchStringLocalXml = "localxml=";
            string SearchStringWebXml = "webxml=";
            string SearchStringInstall = "install=";
            foreach (string s in args)
            {
                if (s.IndexOf(SearchStringLocalXml) >= 0)
                {
                    ArgumentLocalXML = s.Substring(s.IndexOf(SearchStringLocalXml) + SearchStringLocalXml.Length);
                    if (ArgumentLocalXML.IndexOf(" ") >= 0)
                    {
                        ArgumentLocalXML = ArgumentLocalXML.Substring(0, ArgumentLocalXML.IndexOf(" "));
                    }
                }
                else if (s.IndexOf(SearchStringWebXml) >= 0)
                {
                    ArgumentWebXML = s.Substring(s.IndexOf(SearchStringWebXml) + SearchStringWebXml.Length);
                    if (ArgumentWebXML.IndexOf(" ") >= 0)
                    {
                        ArgumentWebXML = ArgumentWebXML.Substring(0, ArgumentWebXML.IndexOf(" "));
                    }
                }
                else if (s.IndexOf(SearchStringInstall) >= 0)
                {
                    ArgumentInstall = s.Substring(s.IndexOf(SearchStringInstall) + SearchStringInstall.Length);
                    if (ArgumentInstall.IndexOf(" ") >= 0)
                    {
                        ArgumentInstall = ArgumentInstall.Substring(0, ArgumentInstall.IndexOf(" "));
                    }
                }
            }

            // Show the system tray icon.					
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();

                Mainthread = new StartUp(pi);

                // Make sure the application runs!
                Application.Run();
            }
        }

#else

        public static LocalData.LocalData MyData = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //string resource1 = "DesktopPet.dll.NAudio.dll";
            //EmbeddedAssembly.Load(resource1, "NAudio.dll");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            MyData = new LocalData.LocalData(Windows.Storage.ApplicationData.Current.LocalFolder.Path, Application.ExecutablePath);

            // Show the system tray icon.					
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();

                Mainthread = new StartUp(pi);

                // Make sure the application runs!
                Application.Run();
            }
        }
#endif

        /// <summary>
        /// Check if application is started from the installation path.
        /// </summary>
        /// <returns>true if the executed application is installed.</returns>
        public static bool IsApplicationInstalled()
        {
            string appPath = Application.StartupPath;
            string installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DesktopPet");
            return (string.Compare(appPath, installPath) == 0);
        }
    }
}
