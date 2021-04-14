using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winform_view_drag_and_drop
{
    public partial class Form1 : Form
    {
        const int FOLDER_INDEX = 0;
        const int OPEN_FOLDER_INDEX = 1;
        const int FILE_INDEX = 2;
        const int SEL_FILE_INDEX= 3;

        Stack<string> visitedFolders = new Stack<string>();
        List<string> items = new List<string>();
        string currectPath="";
        ImageList largeimages;
        ImageList smallimages;
        int numberImege=4;
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var views = Enum.GetNames(typeof(View));
            comboBox1.Items.AddRange(views);
            comboBox1.SelectedIndex = 0;
           
            Image[] images = new Image[]
            {
                Image.FromFile(@"Images\folder.png"),
                Image.FromFile(@"Images\open_folder.png"),
                Image.FromFile(@"Images\file.png"),
                Image.FromFile(@"Images\check_file.png"),
            };
            smallimages = new ImageList();
            smallimages.Images.AddRange(images);
            smallimages.ImageSize = new Size(32, 32);

            largeimages = new ImageList();
            largeimages.Images.AddRange(images);
            largeimages.ImageSize = new Size(64, 64);

            listView.LargeImageList = largeimages;
            listView.SmallImageList = smallimages;

            listView.Columns.Add("Имя");
            ColumnHeader lastChancheHeader = new ColumnHeader
            {
                Text = "Дата изменения",
                Width = 150
            };
            listView.Columns.Add(lastChancheHeader);
            listView.Columns.Add("Тип");
            listView.Columns.Add("Размер");

            LoadDrives();
        }
        private int GetImageFromFile(string path)
        {
           
                string extensionFilePattern = @"^.+(\.png)|(\.jpg)|(\.JPG)$";
                var filename = Path.GetFileName(path);
                if (Regex.IsMatch(filename, extensionFilePattern))
                {
                    var image = new Image[]
                    {
                    Image.FromFile(path)
                    };
                  largeimages.Images.AddRange(image);
                  smallimages.Images.AddRange(image);

                listView.LargeImageList= largeimages;
                listView.SmallImageList= smallimages;
                return numberImege++;
                }
                return 2;
        }
        private void LoadDrives()
        {
            //диски системы
            var drives = Directory.GetLogicalDrives();
            LoadItemsByPath(drives);
        }
        private void LoadItemsByPath(string[] paths)
        {
            //формирование списка елементов
            var listview = paths.Select(path => 
            {
                
                //получение имени папки
                var name = Path.GetFileName(path);
                
                var listitem = new ListViewItem
                {
                    Text = string.IsNullOrEmpty(name) ? path : name,
                    Tag = path,
                    ImageIndex = Directory.Exists(path) ? FOLDER_INDEX : GetImageFromFile(path)
                    
                };
                
                string changeDate, type, size="";
                if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    changeDate = di.LastWriteTime.ToString();
                    type = "папка с файлами";
                    
                }
                else
                {
                    FileInfo fi = new FileInfo(path);
                    changeDate = fi.LastWriteTime.ToString();
                    type = "файл";
                    size = (fi.Length / 1000).ToString();
                }

                listitem.SubItems.Add(changeDate);
                listitem.SubItems.Add(type);
                listitem.SubItems.Add(size);

                return listitem;
            });

            listView.Items.AddRange(listview.ToArray());

        }

        private void ComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex !=-1)
            {
                var selView = comboBox1.SelectedItem.ToString();
                View view = (View)Enum.Parse(typeof(View), selView);
                listView.View = view;
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            visitedFolders.Push(currectPath);
            var selItem = listView.SelectedItems[0];
            var path = selItem.Tag.ToString();
            currectPath = path;
            LoadItems(path);
        }

        private void LoadItems(string path)
        {
            listView.Items.Clear();
            if (string.IsNullOrWhiteSpace(path))
            {
                LoadDrives();
            }
            else
            {

                var folders = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);
                LoadItemsByPath(folders);
                LoadItemsByPath(files);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            if (visitedFolders.Count > 0)
            {
                var preFolder = visitedFolders.Pop();
                currectPath = preFolder;
                LoadItems(preFolder);
            }
        }
    }
}
