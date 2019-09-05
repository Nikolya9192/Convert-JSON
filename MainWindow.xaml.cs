using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Json;
using Newtonsoft.Json;


namespace WindowsExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static DirectoryInfo sourceDir { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmbDrive.ItemsSource = DriveInfo.GetDrives().Where(dr => dr.IsReady == true).ToList();
            this.cmbDrive.DisplayMemberPath = "Name";
            this.cmbDrive.SelectedValuePath = "Name";
        }

        // Вибір дисків
        private void cmbDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tvwDirectory.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(this.cmbDrive.SelectedValue.ToString());

            foreach (DirectoryInfo dr in dir.GetDirectories())
            {
                MyTreeViewItem tvi = new MyTreeViewItem();
                tvi.Header = dr.Name;
                tvi.Path = dr.FullName;
                tvi.Type = dr.GetType().Name;
                tvi.Expanded += new RoutedEventHandler(Ctvi_Expanded);
                if (!dr.Attributes.ToString().Contains("Hidden"))
                {
                    try
                    {
                        foreach (DirectoryInfo cdir in dr.GetDirectories())
                        {
                            MyTreeViewItem ctvi = new MyTreeViewItem();
                            ctvi.Expanded += new RoutedEventHandler(Ctvi_Expanded);
                            ctvi.Header = cdir.Name;
                            ctvi.Path = cdir.FullName;
                            ctvi.Type = cdir.GetType().Name;
                            tvi.Items.Add(ctvi);

                        }
                    }
                    catch(Exception ex )
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                    

                    this.tvwDirectory.Items.Add(tvi);
                }
            }
            btn_ConvertJson.IsEnabled = true;

            foreach (FileInfo FL in dir.GetFiles())
            {

                this.ltbExplorer.Items.Add(FL.Name);
            }
        }
        // завантаження дерева каталогів
        void Ctvi_Expanded(object sender, RoutedEventArgs e)
        {
            MyTreeViewItem tvi = (MyTreeViewItem)sender;


            foreach (MyTreeViewItem ctvi in tvi.Items)
            {
                if (ctvi.Type == "DirectoryInfo")
                {
                    DirectoryInfo dir = new DirectoryInfo(ctvi.Path);
                    foreach (DirectoryInfo cdir in dir.GetDirectories())
                    {
                        MyTreeViewItem ctvi1 = new MyTreeViewItem();
                        ctvi1.Expanded += new RoutedEventHandler(Ctvi_Expanded);
                        ctvi1.Header = cdir.Name;
                        ctvi1.Path = cdir.FullName;
                        ctvi1.Type = cdir.GetType().Name;
                        if (ctvi.Items.Contains(ctvi1.Header) == false)
                            ctvi.Items.Add(ctvi1);
                    }
                }
            }
            e.Handled = true;
        }


        //Перехід по папках та вибір папки для конверсії в Json
        private void tvwDirectory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                this.ltbExplorer.Items.Clear();
                MyTreeViewItem tvi = (MyTreeViewItem)e.NewValue;

                DirectoryInfo dir = new DirectoryInfo(tvi.Path);
                sourceDir = dir;
                foreach (DirectoryInfo FD in dir.GetDirectories())
                {
                    ltbExplorer.Items.Add(FD.Name);
                }

                DirectoryInfo DIRF = new DirectoryInfo(tvi.Path);
                foreach (FileInfo FL in DIRF.GetFiles())
                {
                    ltbExplorer.Items.Add(FL.Name);
                }
                
            }
            e.Handled = true;

        }


        List<Children> lstChildren = new List<Children>();
       
        List<Files> files;
        List<Children> children;

        // Рекурсія завантаження структури вибраного каталога в список List<Children> lstChildren 
        private List<Children> Run(DirectoryInfo dir)
        {
            
            foreach (DirectoryInfo fd in dir.GetDirectories())
            {
                
                files = Getfiles(fd);

                children = Getdirectories(fd);

                
                lstChildren.Add(new Children(fd.Name, fd.CreationTimeUtc.ToUniversalTime().ToString(), files, children));
                lstChildren = Run(fd);
            }
            
            return lstChildren;
        }
        //Пошук каталогів
        private List<Children> Getdirectories(DirectoryInfo dir)
        {
            children = new List<Children>();
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                children.Add(new Children(di.Name, di.CreationTimeUtc.ToUniversalTime().ToString()));
                
            }
            return children;
        }
        //Пошук файлів
        private List<Files> Getfiles(DirectoryInfo file)
        {
            files = new List<Files>();
            foreach (FileInfo fl in file.GetFiles())
            {
                files.Add(new Files(fl.Name, fl.Length.ToString() + " B", fl.FullName));
            }
            return files;
        }

        //Кнопка Серіалізація в Json
        public JsonSerializer ser = new JsonSerializer();
        private void btn_ConvertJson_Click(object sender, RoutedEventArgs e)
        {
            lstChildren.Clear();
            foreach (DirectoryInfo FD in sourceDir.GetDirectories())
            {               
                Getfiles(sourceDir);

                Getdirectories(sourceDir);

                lstChildren.Add(new Children(sourceDir.Name, sourceDir.CreationTimeUtc.ToUniversalTime().ToString(), files, children));

                Run(sourceDir);
            }
            

            using (var fileStream = new FileStream(sourceDir + ".json", FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                
                ser.Formatting = Newtonsoft.Json.Formatting.Indented; 
                ser.TypeNameHandling = TypeNameHandling.Auto;
                ser.Serialize(jsonWriter, sourceDir.ToString());
                
                foreach (Children Child in lstChildren)
                {
                    this.DataContext = Child;
                    ser.Serialize(jsonWriter, Child);
                }
                

                jsonWriter.Flush();
            }

            MessageBox.Show("File saved to " + sourceDir + ".json");
            
        }        
    }
}





