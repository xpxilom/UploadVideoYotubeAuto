using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetBrowser;
using DotNetBrowser.DOM;
using DotNetBrowser.Events;
using DotNetBrowser.WinForms;

 
using System.Windows.Input;
using System.Threading;

namespace Upload_Video
{    
    public partial class Form1 : Form
    {
        List<Cookie> COOKIES = new List<Cookie>();
        List<Canal> canales = new List<Canal>(); 
        List<string> DirDeVideosDeVideos= new List<string>();
        BrowserContextParams params1;
        BrowserContext context1;
        Browser browser1;
        public WinFormsBrowserView BrowserView;
        FinishLoadingEventArgs ee;
        string DireccionDeVideo;
        string[] filePathsCookies;
        CustomDialogHandler dialogHandler;
        string URLChangeToYola = "https://www.youtube.com/signin?next=%2Fupload&feature=channel_switcher&action_handle_signin=true&authuser=1&skip_identity_prompt=False&pageid=";
        string URLChangeToXpxilom = "https://www.youtube.com/signin?next=%2Fupload&feature=channel_switcher&action_handle_signin=true&authuser=0&skip_identity_prompt=False&pageid=";
        public Form1()
        {
             
            InitializeComponent();
            var lines = File.ReadLines("xpxilom.txt");
            foreach (var line in lines)
            {
                string[] DataTex = line.Split('+');
                Canal news = new Canal();
                news.Nombre = DataTex[0];
                news.URL = URLChangeToXpxilom + DataTex[1];
                canales.Add(news);
            }
            lines = File.ReadLines("LaYola.txt");
            foreach (var line in lines)
            {
                string[] DataTex = line.Split('+');
                Canal news = new Canal();
                news.Nombre = DataTex[0];
                news.URL = URLChangeToYola + DataTex[1];
                canales.Add(news);
            } 
        /*    this.BrowserView = new DotNetBrowser.WinForms.WinFormsBrowserView();
            params1 = new BrowserContextParams(@"C:/my-data1");
            context1 = new BrowserContext(params1);
            browser1 = BrowserFactory.Create(context1);
            BrowserView = new WinFormsBrowserView(browser1);
            Controls.Add((Control)BrowserView); 
           BrowserView.Browser.LoadURL("https://www.youtube.com/upload");
            dialogHandler = new CustomDialogHandler(this);
          

            BrowserView.Browser.LoadURL("https://www.youtube.com/channel_switcher?next=%2F");
            BrowserView.FinishLoadingFrameEvent += new FinishLoadingFrameHandler(this.FinishLoadingFrameEvente);*/

           
            //  CargarCookies();
            //  CargarArchivosDeCookies();
        }
        ManualResetEvent waitEvent = new ManualResetEvent(false);
        public void FinishLoadingFrameEvente(object sender, FinishLoadingEventArgs e)
        {
            if (e.IsMainFrame)
            {
                waitEvent.Set();
            }
        }
        void LoadClomplete()
        {
            var x = 458;
            var y = 203;
            var clickCount = 1;
            BrowserView.InputSimulator.SimulateMouseButtonEvent(MouseButtons.Left, System.Windows.Input.MouseButtonState.Pressed, clickCount, x, y);
            BrowserView.InputSimulator.SimulateMouseButtonEvent(MouseButtons.Left, System.Windows.Input.MouseButtonState.Released, clickCount, x, y);
            Thread.Sleep(5000);
            DOMDocument document = BrowserView.Browser.GetDocument();
            List<DOMNode> divs = document.GetElementsByTagName("input");
            foreach (DOMNode div in divs)
            {
                if (div.NodeType == DOMNodeType.ElementNode)
                {
                    DOMElement divElement = (DOMElement)div;
                    if (divElement.GetAttribute("class").Contains("yt-uix-form-input-text video-settings-title"))
                    {
                        divElement.SetInnerText("Titulo");
                    }

                }
            }

        }
        int VideosNuevos()
        {
           string DireccionDeVideos = Environment.CurrentDirectory + "/Canales";
            string[] temporar_ = Directory.GetFiles(DireccionDeVideos, "*.mp4*", SearchOption.AllDirectories);
            foreach (var item in temporar_)
            {
                if(!DirDeVideosDeVideos.Contains(item))
                DirDeVideosDeVideos.Add(item);
            }
            return DirDeVideosDeVideos.Count;
        }

            void CargarArchivosDeCookies()
        {
             filePathsCookies = Directory.GetFiles(System.Environment.CurrentDirectory, "*.dat"); 

        }
        void CargarCookies(string file)
        {
            if(!System.IO.File.Exists(file)) { return; }

            BinaryFormatter formatter = new BinaryFormatter();

            using (Stream fStream = File.OpenRead(file))
            {
                COOKIES = (List<Cookie>)formatter.Deserialize(fStream);
            }

            COOKIES.ForEach(o=> browser1.CookieStorage.SetCookie(o.Domain, o.Name, o.Value, o.Domain, o.Path, o.ExpirationTime, o.Secure, o.HttpOnly));
            //MessageBox.Show("Cookies: " + browser1.CookieStorage.GetAllCookies().Count.ToString());
        }

        void GuardarCookies(String dir)
        {

            //Lista para almacenar Cookies
            List<Cookie> COOKIES = new List<Cookie>();
            //Ciclo para capturar las DotNetCookie y convertirlas a Cookie (Objeto personalizado)
            foreach (var cookie in browser1.CookieStorage.GetAllCookies())
            {
                var c = new Cookie();
                c.CreationTime = cookie.CreationTime;
                c.Domain = cookie.Domain;
                c.ExpirationTime = cookie.ExpirationTime;
                c.HttpOnly = cookie.HttpOnly;
                c.Name = cookie.Name;
                c.Path = cookie.Path;
                c.Secure = cookie.Secure;
                c.Session = cookie.Session;
                c.Value = cookie.Value;
                COOKIES.Add(c);
            }
            //Serializado / salvado
            FileStream fs = new FileStream("dir", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, COOKIES);
            fs.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GuardarCookies("Cookie.dat");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadClomplete();

        }
        int numerodeCookies = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            GuardarCookies("Cookie"+numerodeCookies +".dat");
            numerodeCookies++;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var elementsx = webBrowser1.Document.GetElementsByTagName("input"); 
            foreach (HtmlElement file in elementsx)
            {
                if (file.GetAttribute("type") == "file")
                { 
                    file.Focus();
                    file.InvokeMember("Click");
                    Task.Delay(500).ContinueWith(t => SendFileName(@"C:\Users\Rhoger\Desktop\Videos de mireya\vestido niña.mp4"), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
        private void SendFileName(string fileName)
        {
            SendKeys.Send(fileName + "{ENTER}");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            VideosNuevos();
        }
        private void CambiarDeCanal(string NombreDelCanal)
        { 
              
        }
        int TiempoDeSubir = 0;
        int descasar = 0;
        public void IntentarSubirVideos_Tick(object sender, EventArgs e)
        {
            TiempoDeSubir++;
            progressBar1.Maximum = 60*30 ;
            try
            {

                progressBar1.Value = TiempoDeSubir;
            }
            catch { }
            if(TiempoDeSubir > 60 * 30 && descasar >= 24)
            {
                descasar++;
                TiempoDeSubir = 0;
            }
            if (descasar >= 48)
            {
                descasar = 0;
            }
            if (TiempoDeSubir > 60*30 &&  descasar < 24)
                if (VideosNuevos() > -1)
                {
                    descasar++;
                    TiempoDeSubir = 0;
                    string CanalDondeSubir = Directory.GetParent(DirDeVideosDeVideos[0]).Name;
                    string URLdelCanalDondeSubir = canales.FirstOrDefault(xt => xt.Nombre.Contains(CanalDondeSubir)).URL;
                    waitEvent.Reset();
                    BrowserView.Browser.LoadURL(URLdelCanalDondeSubir);
                    dialogHandler.DireccionDeVideo = DirDeVideosDeVideos[0];
                    BrowserView.Browser.DialogHandler = dialogHandler;
                    waitEvent.WaitOne();
                    
                    Thread.Sleep(3000);
                    DOMDocument document = BrowserView.Browser.GetDocument();
                    DOMNode div = document.GetElementById("start-upload-button-single");
                    DOMElement input = div.GetElementByTagName("button");
                    //Coordinates of the input element 
                    int x = input.BoundingClientRect.X;
                    int y = input.BoundingClientRect.Y;
                    //Modify to click on element. Otherwise - beside 
                    x += 1;
                    y += 1;
                    BeginInvoke(new Action(() =>
                    {
                        BrowserView.InputSimulator.SimulateMouseButtonEvent(MouseButton.Left, MouseButtonState.Pressed,
                        1, x, y);
                        //Simulate the button click 
                        Thread.Sleep(50);
                        BrowserView.InputSimulator.SimulateMouseButtonEvent(MouseButton.Left, MouseButtonState.Released,
                        1, x, y);
                    }));
                    Thread.Sleep(25000);
                    Directory.Delete(DirDeVideosDeVideos[0]);
                    canales.Remove(canales.FirstOrDefault(xt => xt.Nombre.Contains(CanalDondeSubir)));

                }
        }


    }
    internal class CustomDialogHandler : WinFormsDefaultDialogHandler
    {
        private Form form;
        public string DireccionDeVideo;
        public CustomDialogHandler(Form form) : base(form)
        {
            this.form = form;
        }

        public override CloseStatus OnFileChooser(FileChooserParams parameters)
        {
            parameters.SelectedFiles = DireccionDeVideo;
            CloseStatus returnValue = CloseStatus.OK;
            return returnValue;
        }
    }
    [Serializable]
    public class Cookie
    {
        public string Name;
        public string Value;
        public string Domain;
        public string Path;
        public long CreationTime;
        public long ExpirationTime;
        public bool Secure;
        public bool HttpOnly;
        public bool Session;

        public Cookie()
        {

        }
    }
    public class Canal
    {
        public string Nombre;
        public string URL;
        public string Gmail;       
        public List<string> DirVideos = new List<string>();

        public Canal()
        {

        }

        public bool TieneVideos()
        { 
            if (DirVideos.Count > 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}
