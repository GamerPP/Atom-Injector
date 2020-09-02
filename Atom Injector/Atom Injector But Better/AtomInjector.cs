using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing.Text;

namespace Atom_Injector_But_Better
{
    public partial class AtomInjector : Form
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // privileges
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;

        // used for memory allocation
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;

        static bool alreadyAttemptedInject = false;

        public AtomInjector()
        {
            InitializeComponent();
        }

        WebClient client = new WebClient();

        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string dllpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Atom Injector\Atom.Client.dll";

        private async void specifyBtn_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            output("\nGetting Latest DLL!");
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileAsync(new Uri("https://github.com/EchoHackCmd/Atom-Client-Releases/releases/latest/download/Atom.Client.dll"), filePath + @"\Atom Injector\Atom.Client.dll");
        }

        private async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            panel12.Size = new Size(panel11.Width, panel11.Height);
            AtomInjector.InjectDLL(dllpath);
            output("\nInjected!");
            await Task.Delay(2000);
            panel12.Size = new Size(0, panel11.Height);
            output("\nPress Inject!");
        }

        private void AtomInjector_Load(object sender, EventArgs e)
        {
            panel12.Size = new Size(0, panel11.Height);
            Directory.CreateDirectory(filePath + @"\Atom Injector");
            if (!File.Exists(filePath + @"\Atom Injector\Settings.txt"))
            {
                File.WriteAllText(filePath + @"\Atom Injector\Settings.txt", "Aqua\nAqua\n#200020\n#303030\n#404040\n#202020\nBahnschrift");
            }

            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                comboBox1.Items.Add(font.Name);
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath + @"\Atom Injector\Settings.txt").ToArray();
                txtColours(ColorTranslator.FromHtml(lines[0]));
                tabicColours(ColorTranslator.FromHtml(lines[1]));
                panelColours(ColorTranslator.FromHtml(lines[2]));
                btnColours(ColorTranslator.FromHtml(lines[3]));
                obColours(ColorTranslator.FromHtml(lines[4]));
                bgColours(ColorTranslator.FromHtml(lines[5]));
                fontChange(lines[6]);
            }
            catch
            {

            }
        }

        public void txtColours(Color color)
        {
            txtclric.BackColor = color;
            button1.ForeColor = color;
            button2.ForeColor = color;
            button3.ForeColor = color;
            button4.ForeColor = color;
            exitBtn.ForeColor = color;
            injectdevBtn.ForeColor = color;
            specifyBtn.ForeColor = color;
            specifydllBtn.ForeColor = color;
            label1.ForeColor = color;
            btnclrbtn.ForeColor = color;
            ticlrbtn.ForeColor = color;
            txtclrbtn.ForeColor = color;
            bgclrbtn.ForeColor = color;
            obclrbtn.ForeColor = color;
            pnlclrbtn.ForeColor = color;
            colourtabbtn.ForeColor = color;
            misctabBtn.ForeColor = color;
            button6.ForeColor = color;
            button5.ForeColor = color;
        }

        public void tabicColours(Color color)
        {
            panel5.BackColor = color;
            panel6.BackColor = color;
            panel7.BackColor = color;
            panel8.BackColor = color;
            panel10.BackColor = color;
            ticlric.BackColor = color;
        }

        public void panelColours(Color color)
        {
            panel1.BackColor = color;
            panel2.BackColor = color;
            panel3.BackColor = color;
            panel4.BackColor = color;
            pnlclric.BackColor = color;
            label1.BackColor = color;
        }

        public void btnColours(Color color)
        {
            button1.BackColor = color;
            button2.BackColor = color;
            button3.BackColor = color;
            button4.BackColor = color;
            btnclrbtn.BackColor = color;
            exitBtn.BackColor = color;
            injectdevBtn.BackColor = color;
            specifyBtn.BackColor = color;
            specifydllBtn.BackColor = color;
            pnlclrbtn.BackColor = color;
            ticlrbtn.BackColor = color;
            txtclrbtn.BackColor = color;
            obclrbtn.BackColor = color;
            bgclrbtn.BackColor = color;
            btnclric.BackColor = color;
            colourtabbtn.BackColor = color;
            misctabBtn.BackColor = color;
            button5.BackColor = color;
            button6.BackColor = color;
        }

        public void obColours(Color color)
        {
            outputTxt.BackColor = color;
            devoutput.BackColor = color;
            obclric.BackColor = color;
        }

        public void bgColours(Color color)
        {
            panel9.BackColor = color;
            helptab.BackColor = color;
            injectorTab.BackColor = color;
            devinject.BackColor = color;
            colourtab.BackColor = color;
            textBox1.BackColor = color;
        }

        public void fontChange(string font)
        {
            var cvt = new FontConverter();
            button1.Font = new Font(font.ToString(), button1.Font.Size);
            button2.Font = new Font(font.ToString(), button2.Font.Size);
            button3.Font = new Font(font.ToString(), button3.Font.Size);
            button4.Font = new Font(font.ToString(), button4.Font.Size);
            exitBtn.Font = new Font(font.ToString(), exitBtn.Font.Size);
            injectdevBtn.Font = new Font(font.ToString(), injectdevBtn.Font.Size);
            specifyBtn.Font = new Font(font.ToString(), specifyBtn.Font.Size);
            specifydllBtn.Font = new Font(font.ToString(), specifydllBtn.Font.Size);
            label1.Font = new Font(font.ToString(), label1.Font.Size);
            btnclrbtn.Font = new Font(font.ToString(), btnclrbtn.Font.Size);
            ticlrbtn.Font = new Font(font.ToString(), ticlrbtn.Font.Size);
            txtclrbtn.Font = new Font(font.ToString(), txtclrbtn.Font.Size);
            bgclrbtn.Font = new Font(font.ToString(), bgclrbtn.Font.Size);
            obclrbtn.Font = new Font(font.ToString(), obclrbtn.Font.Size);
            pnlclrbtn.Font = new Font(font.ToString(), pnlclrbtn.Font.Size);
            colourtabbtn.Font = new Font(font.ToString(), colourtabbtn.Font.Size);
            misctabBtn.Font = new Font(font.ToString(), misctabBtn.Font.Size);
            button6.Font = new Font(font.ToString(), button6.Font.Size);
            button5.Font = new Font(font.ToString(), button5.Font.Size);
            textBox1.Font = new Font(font.ToString(), textBox1.Font.Size);
            devoutput.Font = new Font(font.ToString(), devoutput.Font.Size);
            outputTxt.Font = new Font(font.ToString(), outputTxt.Font.Size);
        }

        public void output(string Text)
        {
            outputTxt.Text = Text;
        }

        public static void InjectDLL(string DLLPath)
        {
            if (Process.GetProcessesByName("Minecraft.Windows").Length != 0)
            {
                AtomInjector.applyAppPackages(DLLPath);
                Process process = Process.GetProcessesByName("Minecraft.Windows")[0];
                IntPtr hProcess = AtomInjector.OpenProcess(1082, false, process.Id);
                IntPtr procAddress = AtomInjector.GetProcAddress(AtomInjector.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                IntPtr intPtr = AtomInjector.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((DLLPath.Length + 1) * Marshal.SizeOf(typeof(char))), 12288U, 4U);
                UIntPtr uintPtr;
                AtomInjector.WriteProcessMemory(hProcess, intPtr, Encoding.Default.GetBytes(DLLPath), (uint)((DLLPath.Length + 1) * Marshal.SizeOf(typeof(char))), out uintPtr);
                AtomInjector.CreateRemoteThread(hProcess, IntPtr.Zero, 0U, procAddress, intPtr, 0U, IntPtr.Zero);
                AtomInjector.alreadyAttemptedInject = false;
                MessageBox.Show("Injected!");
                return;
            }
            if (!AtomInjector.alreadyAttemptedInject)
            {
                AtomInjector.alreadyAttemptedInject = true;
                MessageBox.Show("\nLaunching Minecraft Now!");
                Process.Start("minecraft://");
                Thread.Sleep(2000);
                AtomInjector.InjectDLL(DLLPath);
                return;
            }
            MessageBox.Show("Minecraft Isn't Downloaded >:(");
            AtomInjector.alreadyAttemptedInject = false;
        }

        public static void applyAppPackages(string DLLPath)
        {
            FileInfo fileInfo = new FileInfo(DLLPath);
            FileSecurity accessControl = fileInfo.GetAccessControl();
            accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-15-2-1"), FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            fileInfo.SetAccessControl(accessControl);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/RUSTeNu");
        }

        private void injectdevBtn_Click(object sender, EventArgs e)
        {
            try
            {
                AtomInjector.InjectDLL(this.dllpath);
            }
            catch
            {
                MessageBox.Show("Ensure you ahve selected a dll!");
            }
        }

        private void specifydllBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.SafeFileName.ToLower().EndsWith(".dll"))
                {
                    this.dllpath = openFileDialog.FileName;
                    devoutput.Text = "\"" +  openFileDialog.FileName + "\" Is Selected!";
                    devoutput.Font = new Font(devoutput.Font.FontFamily, 12);
                    return;
                }
                MessageBox.Show("You did not specify a DLL!");
            }
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mainTabs.SelectedTab = injectorTab;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mainTabs.SelectedTab = devinject;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mainTabs.SelectedTab = helptab;
        }

        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _start_point = new Point(e.X, e.Y);
        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private void txtclrbtn_Click(object sender, EventArgs e)
        {
            ColorDialog txtColour = new ColorDialog();
            txtColour.AnyColor = true;
            txtColour.Color = panel1.BackColor;
            if (txtColour.ShowDialog() == DialogResult.OK)
            {
                Color txtColourClr = txtColour.Color;
                lineChanger(ColorTranslator.ToHtml(txtColourClr), filePath + @"\Atom Injector\Settings.txt", 1);

                txtColours(txtColourClr);
            }
        }

        private void colourtabbtn_Click(object sender, EventArgs e)
        {
            mainTabs.SelectedTab = colourtab;
        }

        private void ticlrbtn_Click(object sender, EventArgs e)
        {
            ColorDialog tiColour = new ColorDialog();
            tiColour.AnyColor = true;
            tiColour.Color = panel1.BackColor;
            if (tiColour.ShowDialog() == DialogResult.OK)
            {
                Color tiColourClr = tiColour.Color;
                lineChanger(ColorTranslator.ToHtml(tiColourClr), filePath + @"\Atom Injector\Settings.txt", 2);

                tabicColours(tiColourClr);
            }
        }

        private void pnlclrbtn_Click(object sender, EventArgs e)
        {
            ColorDialog pnlColour = new ColorDialog();
            pnlColour.AnyColor = true;
            pnlColour.Color = panel1.BackColor;
            if (pnlColour.ShowDialog() == DialogResult.OK)
            {
                Color pnlColourClr = pnlColour.Color;
                lineChanger(ColorTranslator.ToHtml(pnlColourClr), filePath + @"\Atom Injector\Settings.txt", 3);

                panelColours(pnlColourClr);
            }
        }

        private void btnclrbtn_Click(object sender, EventArgs e)
        {
            ColorDialog btnColour = new ColorDialog();
            btnColour.AnyColor = true;
            btnColour.Color = panel1.BackColor;
            if (btnColour.ShowDialog() == DialogResult.OK)
            {
                Color btnColourClr = btnColour.Color;
                lineChanger(ColorTranslator.ToHtml(btnColourClr), filePath + @"\Atom Injector\Settings.txt", 4);

                btnColours(btnColourClr);
            }
        }

        private void obclrbtn_Click(object sender, EventArgs e)
        {
            ColorDialog txtboxColour = new ColorDialog();
            txtboxColour.AnyColor = true;
            txtboxColour.Color = panel1.BackColor;
            if (txtboxColour.ShowDialog() == DialogResult.OK)
            {
                Color txtboxColourClr = txtboxColour.Color;
                lineChanger(ColorTranslator.ToHtml(txtboxColourClr), filePath + @"\Atom Injector\Settings.txt", 5);

                obColours(txtboxColourClr);
            }
        }

        private void bgclrbtn_Click(object sender, EventArgs e)
        {
            ColorDialog bgColour = new ColorDialog();
            bgColour.AnyColor = true;
            bgColour.Color = panel1.BackColor;
            if (bgColour.ShowDialog() == DialogResult.OK)
            {
                Color bgColourClr = bgColour.Color;
                lineChanger(ColorTranslator.ToHtml(bgColourClr), filePath + @"\Atom Injector\Settings.txt", 6);

                bgColours(bgColourClr);
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.DownloadFileCompleted += Client_DownloadFileCompleted1;
            client.DownloadFileAsync(new Uri("https://github.com/GamerPP/idk/releases/download/1/X-Ray.v1.5.0.Nether.Update.mcpack"), filePath + @"\Atom Injector\X-Ray.v1.5.0.Nether.Update.mcpack");
        }

        private float progress = 0f;

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            panel12.Size = new Size((int)((panel11.Width / 100) * Math.Round(percentage)), panel2.Height);                 //int.Parse(Math.Truncate(percentage).ToString());
        }

        private async void Client_DownloadFileCompleted1(object sender, AsyncCompletedEventArgs e)
        {
            Process.Start(filePath + @"\Atom Injector\X-Ray.v1.5.0.Nether.Update.mcpack");
            await Task.Delay(10000);
            File.Delete(filePath + @"\Atom Injector\X-Ray.v1.5.0.Nether.Update.mcpack");
        }

        List<string> fonts = new List<string>();
        InstalledFontCollection installedFonts = new InstalledFontCollection();

        private void misctabBtn_Click(object sender, EventArgs e)
        {
            mainTabs.SelectedTab = miscTab;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            lineChanger(comboBox1.Text, filePath + @"\Atom Injector\Settings.txt", 7);

            fontChange(comboBox1.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            File.Delete(filePath + @"\Atom Injector\Settings.txt");
            MessageBox.Show("For the reset to take effect restart the injector.","Warning!");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
