using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UmaMacro
{
    public partial class MainForm : Form
    {
        // WinAPI 함수 정의
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID_Space = 1; // 핫키 ID
        private const int HOTKEY_ID_Q = 2; // 핫키 ID
        private const int HOTKEY_ID_W = 3; // 핫키 ID
        private const int HOTKEY_ID_E = 4; // 핫키 ID
        private const int HOTKEY_ID_R = 5; // 핫키 ID
        private const int HOTKEY_ID_T = 6; // 핫키 ID
        private const int HOTKEY_ID_Y = 7; // 핫키 ID
        private const int HOTKEY_ID_F12 = 8; // 핫키 ID
        private const uint MOD_NONE = 0x0000; // 수정 키 없음
        private const uint VK_SPACE = 0x20;  // Space 키
        private const uint VK_Q = 0x51;
        private const uint VK_W = 0x57;
        private const uint VK_E = 0x45;
        private const uint VK_R = 0x52;
        private const uint VK_T = 0x54;
        private const uint VK_Y = 0x59;
        private const uint VK_F12 = 0x7B;
        private bool isActive = false;
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        // POINT 구조체 선언 (마우스 좌표 저장)
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }


        public MainForm()
        {
            InitializeComponent();
            //AllocConsole(); // 콘솔 창 활성화

            label1.Text = isActive ? "활성" : "비활성";
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312; // 핫키 메시지
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_Space)
            {
                PerformClickAction(949, 751); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_Q)
            {
                PerformClickAction(787, 800); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_W)
            {
                PerformClickAction(863, 800); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_E)
            {
                PerformClickAction(927, 800); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_R)
            {
                PerformClickAction(1001, 803); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_T)
            {
                PerformClickAction(1052, 799); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_Y)
            {
                PerformClickAction(1126, 800); // Space 키가 눌렸을 때 실행할 동작

            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_F12)
            {
                isActive = !isActive;
            }
            base.WndProc(ref m);
        }

        private async void PerformClickAction(int x, int y)
        {

                GetCursorPos(out POINT point);
                SetCursorPos(x, y);

                // 마우스 클릭 이벤트 시뮬레이션
                mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
                await Task.Delay(50);

                SetCursorPos(point.X, point.Y);
            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ReleaseHook();
            base.OnFormClosing(e);
        }

        protected void StartHook()
        {
            // 폼 로드 시 글로벌 핫키 등록
            RegisterHotKey(this.Handle, HOTKEY_ID_Space, MOD_NONE, VK_SPACE);
            RegisterHotKey(this.Handle, HOTKEY_ID_Q, MOD_NONE, VK_Q);
            RegisterHotKey(this.Handle, HOTKEY_ID_W, MOD_NONE, VK_W);
            RegisterHotKey(this.Handle, HOTKEY_ID_E, MOD_NONE, VK_E);
            RegisterHotKey(this.Handle, HOTKEY_ID_R, MOD_NONE, VK_R);
            RegisterHotKey(this.Handle, HOTKEY_ID_T, MOD_NONE, VK_T);
            RegisterHotKey(this.Handle, HOTKEY_ID_Y, MOD_NONE, VK_Y);
            RegisterHotKey(this.Handle, HOTKEY_ID_F12, MOD_NONE, VK_F12);

            // 폼 로드 시 글로벌 마우스 후킹 설정
            _hookID = SetHook(_proc);
        }
        protected void ReleaseHook()
        {
            // 폼 종료 시 핫키 해제
            UnregisterHotKey(this.Handle, HOTKEY_ID_Space);
            UnregisterHotKey(this.Handle, HOTKEY_ID_Q);
            UnregisterHotKey(this.Handle, HOTKEY_ID_W);
            UnregisterHotKey(this.Handle, HOTKEY_ID_E);
            UnregisterHotKey(this.Handle, HOTKEY_ID_R);
            UnregisterHotKey(this.Handle, HOTKEY_ID_T);
            UnregisterHotKey(this.Handle, HOTKEY_ID_Y);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F12);
            // 폼 종료 시 후킹 해제
            UnhookWindowsHookEx(_hookID); // 후킹 해제
        }

        // WinAPI 함수 정의
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private static IntPtr _hookID = IntPtr.Zero;
        private static HookProc _proc = HookCallback;

        // 후킹을 설정하기 위한 델리게이트
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Windows API Import
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_MOUSE_LL = 14; // 글로벌 마우스 후킹
        private const int WM_LBUTTONDOWN = 0x0201; // 마우스 왼쪽 클릭


        private static IntPtr SetHook(HookProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                // 마우스 클릭 좌표 가져오기
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int x = hookStruct.pt.X;
                int y = hookStruct.pt.Y;

                // 메시지 박스 표시
                Console.WriteLine($"Mouse clicked at: X={x}, Y={y}");
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isActive = !isActive;
            label1.Text = isActive ? "활성" : "비활성";
            if (isActive)
            {
                StartHook();
            }
            else
            {
                ReleaseHook();
            }
            
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}
