using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UmaMacro
{
    public partial class MainForm : Form
    {
        #region WinAPI 선언
        
        // 핵키 관련 API
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
        private const int HOTKEY_ID_BACKTICK = 8; // 핫키 ID (` 키)
        private const uint MOD_NONE = 0x0000; // 보조 키 없음
        private const uint VK_SPACE = 0x20;  // Space 키
        private const uint VK_Q = 0x51;
        private const uint VK_W = 0x57;
        private const uint VK_E = 0x45;
        private const uint VK_R = 0x52;
        private const uint VK_T = 0x54;
        private const uint VK_Y = 0x59;
        private const uint VK_BACKTICK = 0xC0; // ` 키 (백틱)
        private bool isActive = false;
        
        // 29분 타이머 관련 변수 추가
        private System.Windows.Forms.Timer autoClickTimer;
        // private const int AUTO_CLICK_INTERVAL = 29 * 60 * 1000; // 29분을 밀리초로 변환
        //확인을 위해 1초로 변경
        private const int AUTO_CLICK_INTERVAL = 3000; // 1초
        
        // 2025-07-18, 김병현 수정 - Debug/Release 모드별 비활성 자동 활성화 기능
        private System.Windows.Forms.Timer inactivityTimer;
        
#if DEBUG
        // Debug 모드: 1초마다 체크, 3초 이상 비활성 시 자동 활성화
        private const int INACTIVITY_CHECK_INTERVAL = 1000; // 1초
        private const int INACTIVITY_TIMEOUT = 3 * 1000; // 3초
#else
        // Release 모드: 5분마다 체크, 15분 이상 비활성 시 자동 활성화
        private const int INACTIVITY_CHECK_INTERVAL = 5 * 60 * 1000; // 5분
        private const int INACTIVITY_TIMEOUT = 15 * 60 * 1000; // 15분
#endif
        
        private DateTime lastInputTime;

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        // POINT 구조체 정의 (마우스 좌표 저장)
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }


        public MainForm()
        {
            InitializeComponent();
            AllocConsole(); // 콘솔 창 활성화 (디버깅용)

            label1.Text = isActive ? "활성" : "비활성";
            
            // 29분 타이머 초기화
            InitializeAutoClickTimer();
            
            // 2025-07-18, 김병현 수정 - 비활성 타이머 초기화
            InitializeInactivityTimer();
            
            // 폼이 로드된 후 핫키 등록
            this.Load += MainForm_Load;
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            // ` 핫키는 항상 등록되어야 함 (활성/비활성 토글용)
            bool hotKeyRegistered = RegisterHotKey(this.Handle, HOTKEY_ID_BACKTICK, MOD_NONE, VK_BACKTICK);
            if (!hotKeyRegistered)
            {
                // ` 키 등록 실패 시 메시지 표시
                MessageBox.Show("` 핫키 등록에 실패했습니다. 다른 프로그램에서 ` 키를 사용 중일 수 있습니다.\n" +
                              "` 키 대신 버튼을 클릭해서 활성/비활성을 변경하세요.", "핫키 등록 실패", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void InitializeAutoClickTimer()
        {
            autoClickTimer = new System.Windows.Forms.Timer();
            autoClickTimer.Interval = AUTO_CLICK_INTERVAL; // 29분
            autoClickTimer.Tick += AutoClickTimer_Tick;
        }
        
        // 2025-07-18, 김병현 수정 - Debug/Release 모드별 비활성 타이머 초기화
        private void InitializeInactivityTimer()
        {
            inactivityTimer = new System.Windows.Forms.Timer();
            inactivityTimer.Interval = INACTIVITY_CHECK_INTERVAL;
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();
            UpdateLastInputTime();
            
#if DEBUG
            Console.WriteLine($"Debug 모드: {INACTIVITY_CHECK_INTERVAL/1000}초마다 체크, {INACTIVITY_TIMEOUT/1000}초 이상 비활성 시 자동 활성화");
#else
            Console.WriteLine($"Release 모드: {INACTIVITY_CHECK_INTERVAL/(60*1000)}분마다 체크, {INACTIVITY_TIMEOUT/(60*1000)}분 이상 비활성 시 자동 활성화");
#endif
        }
        
        private void AutoClickTimer_Tick(object sender, EventArgs e)
        {
            // 프로그램이 활성화되어 있을 때만 자동 클릭 수행
            if (isActive)
            {
                // Q키와 동일한 동작 수행 (787, 800 좌표 클릭)
                PerformClickAction(787, 800);
            }
        }
        
        // 2025-07-18, 김병현 수정 - 비활성 타이머 이벤트 처리
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            // 현재 비활성 상태일 때만 체크
            if (!isActive)
            {
                uint idleTime = GetIdleTime();
                
                // 설정된 시간 이상 입력이 없으면 자동 활성화
                if (idleTime >= INACTIVITY_TIMEOUT)
                {
#if DEBUG
                    Console.WriteLine($"Debug: {INACTIVITY_TIMEOUT/1000}초 비활성 감지! 자동 활성화 (대기시간: {idleTime/1000}초)");
#else
                    Console.WriteLine($"Release: {INACTIVITY_TIMEOUT/(60*1000)}분 비활성 감지! 자동 활성화 (대기시간: {idleTime/1000}초)");
#endif
                    isActive = true;
                    UpdateStatusLabel();
                    StartMacro();
                }
            }
        }
        
        // 2025-07-18, 김병현 수정 - 시스템 대기 시간 확인 메서드
        private uint GetIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            
            if (GetLastInputInfo(ref lastInputInfo))
            {
                return (uint)Environment.TickCount - lastInputInfo.dwTime;
            }
            return 0;
        }
        
        // 2025-07-18, 김병현 수정 - 마지막 입력 시간 업데이트
        private void UpdateLastInputTime()
        {
            lastInputTime = DateTime.Now;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312; // 핫키 메시지
            
            // 핫키 메시지 디버깅
            if (m.Msg == WM_HOTKEY)
            {
                Console.WriteLine($"핫키 메시지 받음: ID={m.WParam.ToInt32()}");
            }
            
            // ` 키는 항상 처리 (활성/비활성 토글)
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID_BACKTICK)
            {
                Console.WriteLine("` 키 눌림 감지!");
                ToggleMacroState();
            }
            // 다른 핫키들은 활성 상태일 때만 동작
            else if (m.Msg == WM_HOTKEY && isActive)
            {
                if (m.WParam.ToInt32() == HOTKEY_ID_Space)
                {
                    PerformClickAction(949, 751);
                }
                else if (m.WParam.ToInt32() == HOTKEY_ID_Q)
                {
                    PerformClickAction(787, 800);
                }
                else if (m.WParam.ToInt32() == HOTKEY_ID_W)
                {
                    PerformClickAction(863, 800);
                }
                else if (m.WParam.ToInt32() == HOTKEY_ID_E)
                {
                    PerformClickAction(927, 800);
                }
                else if (m.WParam.ToInt32() == HOTKEY_ID_R)
                {
                    PerformClickAction(1001, 803);
                }
                else if (m.WParam.ToInt32() == HOTKEY_ID_T)
                {
                    PerformClickAction(1052, 799);
                }
                else if (m.WParam.ToInt32() == HOTKEY_ID_Y)
                {
                    PerformClickAction(1126, 800);
                }
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
            UnregisterHotKey(this.Handle, HOTKEY_ID_BACKTICK); // ` 핫키 해제
            autoClickTimer?.Stop(); // 타이머 정지
            autoClickTimer?.Dispose(); // 타이머 리소스 해제
            // 2025-07-18, 김병현 수정 - 비활성 타이머 정리
            inactivityTimer?.Stop();
            inactivityTimer?.Dispose();
            base.OnFormClosing(e);
        }

        protected void StartHook()
        {
            // 다른 핫키들만 등록 (F12는 이미 생성자에서 등록됨)
            RegisterHotKey(this.Handle, HOTKEY_ID_Space, MOD_NONE, VK_SPACE);
            RegisterHotKey(this.Handle, HOTKEY_ID_Q, MOD_NONE, VK_Q);
            RegisterHotKey(this.Handle, HOTKEY_ID_W, MOD_NONE, VK_W);
            RegisterHotKey(this.Handle, HOTKEY_ID_E, MOD_NONE, VK_E);
            RegisterHotKey(this.Handle, HOTKEY_ID_R, MOD_NONE, VK_R);
            RegisterHotKey(this.Handle, HOTKEY_ID_T, MOD_NONE, VK_T);
            RegisterHotKey(this.Handle, HOTKEY_ID_Y, MOD_NONE, VK_Y);

            // 마우스 후킹 등록
            _hookID = SetHook(_proc);
        }
        protected void ReleaseHook()
        {
            // 다른 핫키들만 해제 (F12는 항상 등록되어 있어야 함)
            UnregisterHotKey(this.Handle, HOTKEY_ID_Space);
            UnregisterHotKey(this.Handle, HOTKEY_ID_Q);
            UnregisterHotKey(this.Handle, HOTKEY_ID_W);
            UnregisterHotKey(this.Handle, HOTKEY_ID_E);
            UnregisterHotKey(this.Handle, HOTKEY_ID_R);
            UnregisterHotKey(this.Handle, HOTKEY_ID_T);
            UnregisterHotKey(this.Handle, HOTKEY_ID_Y);
            // 마우스 후킹 해제
            UnhookWindowsHookEx(_hookID);
        }

        // 마우스 관련 API
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        private static IntPtr _hookID = IntPtr.Zero;
        private static readonly HookProc _proc = HookCallback;

        // 핫키 후킹 콜백 함수 선언
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        // 훅 관련 API
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        
        // 2025-07-18, 김병현 수정 - 입력 감지를 위한 추가 API
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        
        #endregion
        
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        private const int WH_MOUSE_LL = 14; // 핫키 후킹 등록
        private const int WM_LBUTTONDOWN = 0x0201; // 좌클릭 마우스 함수


        private static IntPtr SetHook(HookProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                // 좌클릭 마우스 좌표 출력
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int x = hookStruct.pt.X;
                int y = hookStruct.pt.Y;

                // 핫키 메시지 출력
                Console.WriteLine($"Mouse clicked at: X={x}, Y={y}");
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void ToggleButton_Click(object sender, EventArgs e) => ToggleMacroState();
        
        // 2025-07-18, 김병현 수정 - 매크로 상태 토글 로직 분리
        private void ToggleMacroState()
        {
            isActive = !isActive;
            UpdateStatusLabel();
            
            if (isActive)
            {
                StartMacro();
            }
            else
            {
                StopMacro();
            }
        }
        
        // 2025-07-18, 김병현 수정 - 매크로 시작 메서드
        private void StartMacro()
        {
            StartHook();
            autoClickTimer.Start();
        }
        
        // 2025-07-18, 김병현 수정 - 매크로 중지 메서드
        private void StopMacro()
        {
            ReleaseHook();
            autoClickTimer.Stop();
        }
        
        // 2025-07-18, 김병현 수정 - 상태 라벨 업데이트 메서드 분리
        private void UpdateStatusLabel()
        {
            label1.Text = isActive ? "활성" : "비활성";
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
