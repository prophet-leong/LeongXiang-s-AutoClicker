using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace WindowsFormsApplication2
{
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    }
    public struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    };
    public struct Clicks
    {
        //INPUT i;
        public Point MousePos;
        public int delay;
    }
    public partial class Form1 : Form
    {
        List<Clicks> MousePosList = new List<Clicks>();
        int MousePosListCount = 0;
        //mouse event constants
        const int MOUSEEVENTF_LEFTDOWN = 2;
        const int MOUSEEVENTF_LEFTUP = 4;
        //input type constant
        const int INPUT_MOUSE = 0;

        int StartButton;
        //int AddMacros;
        Timer timerPoint = new Timer();
        Point clickLocation = new Point(0, 0);

        int delay = 100;

        HotKey startKey;
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs,
                                           int cbSize);
        //[DllImport("user32.dll")]
        //private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        //[DllImport("user32.dll")]
        //private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Form1()
        {
            InitializeComponent();
            timerPoint.Interval = 1000;
            textBox1.Text = "1000";
            StartButton = 0x7a;
            //AddMacros = 0x0D;
            startKey = new HotKey(0x0000, Keys.RShiftKey, this);
            startKey.Register();
            this.AcceptButton = button1;

        }
        //the two ways to start auto clicker
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
                StartAutoClicker();
            base.WndProc(ref m);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            StartAutoClicker();
        }
        /*********************************/
        private void StartAutoClicker()
        {
            if (!timer1.Enabled)
            {
                if (MousePosList.Count == 0)
                {
                    int min = Int32.Parse(textBox3.Text) * 100 * 60;
                    int sec = Int32.Parse(textBox2.Text) * 100;
                    int millsec = Int32.Parse(textBox1.Text);
                    delay = min + sec + millsec;
                }
                else
                {
                    delay = MousePosList[MousePosListCount].delay;
                    clickLocation = MousePosList[MousePosListCount].MousePos;
                }
                timerPoint.Interval = delay;
                timer1.Interval = delay;
                timerPoint.Start();
                timer1.Start();
                this.Text = "autoclicker - started";
            }
            else
            {
                timer1.Stop();
                this.Text = "autoclicker - stopped";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int min = Int32.Parse(textBox3.Text) * 100 * 60;
            int sec = Int32.Parse(textBox2.Text) * 100;
            int millsec = Int32.Parse(textBox1.Text);
            Clicks temp = new Clicks();
            temp.delay = min + sec + millsec;
            temp.MousePos = Cursor.Position;
            MousePosList.Add(temp);

            //add to UI
            richTextBox1.Text += MousePosList.Count.ToString() + ".  " + MousePosList[MousePosList.Count - 1].MousePos.ToString() + "   " + MousePosList[MousePosList.Count - 1].delay.ToString() + "\n";
        }

        private void timerPoint_Tick(object sender, EventArgs e)
        {
            //show the location on window title
            this.Text = "autoclicker " + clickLocation.ToString();
            timerPoint.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            INPUT i = new INPUT();
            i.type = INPUT_MOUSE;
            i.mi.dx = 0;
            i.mi.dy = 0;
            i.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            i.mi.dwExtraInfo = IntPtr.Zero;
            i.mi.mouseData = 0;
            i.mi.time = 0;
            //send the input 
            Cursor.Position = clickLocation;
            SendInput(1, ref i, Marshal.SizeOf(i));
            //set the INPUT for mouse up and send it
            i.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, ref i, Marshal.SizeOf(i));
            nextClick();
        }

        void nextMousePosCount()
        {
            ++MousePosListCount;
            if(MousePosListCount>= MousePosList.Count)
            {
                MousePosListCount = 0;
            }
        }

        void nextClick()
        {
            if(MousePosList.Count >0)
            {
                nextMousePosCount();
                delay = MousePosList[MousePosListCount].delay;
                clickLocation = MousePosList[MousePosListCount].MousePos;
                timer1.Interval = delay;
                timer1.Stop();
                timer1.Start();
            }
        }

    }
}
