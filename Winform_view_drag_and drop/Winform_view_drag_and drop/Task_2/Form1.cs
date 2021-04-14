using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_2
{
    public partial class Form1 : Form
    {
        const int FOLDER_INDEX = 0;
        const int OPEN_FOLDER_INDEX = 1;
        const int FILE_INDEX = 2;
        const int OPEN_FILE_INDEX = 3;

        ImageList largeimages;
        ImageList smallimages;
        int numberImege = 4;
        public Form1()
        {
            InitializeComponent();
            InitTreeView();
            InitLogicalDrives();
        }

        private void InitTreeView()
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

            AddColomnToListView();

            ImageList imgList = new ImageList();
            imgList.Images.AddRange(images);
            imgList.ImageSize= new Size(16, 16);

            treeView.ImageList = imgList;
        }
        private void ComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                var selView = comboBox1.SelectedItem.ToString();
                View view = (View)Enum.Parse(typeof(View), selView);
                listView.View = view;
            }
        }
        private void InitLogicalDrives()
        {
            var drivers = Directory.GetLogicalDrives();
            var nodes = drivers.Select(drive =>
            {
                var node = new TreeNode(drive, FOLDER_INDEX, OPEN_FOLDER_INDEX);
                node.Tag = drive;
                node.Nodes.Add("");

                return node;
            }).ToArray();
            treeView.Nodes.AddRange(nodes);
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var path = e.Node.Tag.ToString();
            InitNode(path, e.Node);
           
            InitViewList(path);

        }
        private void InitNode(string path,TreeNode node)
        {
            try
            {
                node.Nodes.Clear();
                var folders = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);

                LoadItemsToNode(folders, node);
                LoadItemsToNode(files, node);
            }
            catch (UnauthorizedAccessException)
            { }
        }
        private void LoadItemsToNode(string[] paths,TreeNode node)
        {
            var nodes = paths.Select(path =>
            {

                bool isDir = Directory.Exists(path);
                var imgIdx = Directory.Exists(path) ? FOLDER_INDEX : FILE_INDEX;
                var selImgIdx = Directory.Exists(path) ? OPEN_FOLDER_INDEX : OPEN_FILE_INDEX;
                var name=Path.GetFileName(path);
                var createdNode = new TreeNode(name, imgIdx, selImgIdx);
                createdNode.Tag = path;

                if(isDir)
                {
                    createdNode.Nodes.Add("");
                }

                return createdNode;
            }).ToArray();

            node.Nodes.AddRange(nodes);
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var path = e.Node.Tag.ToString();

            if(!Directory.Exists(path))
            {
                Process.Start(path);
            }
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

                string changeDate, type, size = "";

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

                listView.LargeImageList = largeimages;
                listView.SmallImageList = smallimages;
                return numberImege++;
            }
            return 2;
        }
        
        private void InitViewList(string path)
        {

            try
            {
                var selIdx = treeView.SelectedImageIndex;
                if (selIdx != -1)
                {
                   // var path = treeView.SelectedNode.Tag.ToString();
                    var folders = Directory.GetDirectories(path);
                    var files = Directory.GetFiles(path);

                    listView.Clear();

                    AddColomnToListView();

                    LoadItemsByPath(folders);
                    LoadItemsByPath(files);
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
        }

        private void AddColomnToListView()
        {
            listView.Columns.Add("Имя");
            ColumnHeader lastChancheHeader = new ColumnHeader
            {
                Text = "Дата изменения",
                Width = 150
            };
            listView.Columns.Add(lastChancheHeader);
            listView.Columns.Add("Тип");
            listView.Columns.Add("Размер");
        }
    }
}
