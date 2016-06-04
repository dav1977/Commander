﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Commander
{
    class DirectoryPanel : Panel
    {
        ComboBox comboBoxDrive = new ComboBox();       
        Label labelFreeSpace = new Label();
        Label labelDir = new Label();
        Label labelInfoFolder = new Label();
        Button buttonToRoot = new Button();
        Button buttonToParent = new Button();
        Button buttonFavoriteCatalogs = new Button();
        Button buttonHistory = new Button();
        Panel panelSpaceDisk = new Panel();
        Panel panelDir = new Panel();
        Panel panelInfoFolder = new Panel();
        CustomListView listViewDirectory = new CustomListView();     
        //ListView listViewDirectory = new ListView();
        List<DirectoryList> directoryList = new List<DirectoryList>();

        //ImageList iconList = new ImageList();
        Icon iconUp = Properties.Resources.IconUP;
        Icon IconUnknown = Properties.Resources.IconUnknown;

        int selectionIndex;
        string dateTimeFormat = "dd.MM.yyyy HH.mm.ss";

        public string SaveDir { get; set; }

        protected override void CreateHandle()
        {
            CreateInterface();
            base.CreateHandle();
        }

        public void CreateInterface()
        {
            comboBoxDrive.Location = new Point(2, 2);
            comboBoxDrive.Size = new Size(42, 21);
            comboBoxDrive.Text = "[-c-]";
            this.Controls.Add(comboBoxDrive);

            buttonToRoot.Location = new Point(this.Width - 43, 1);
            buttonToRoot.Size = new Size(21, 23);
            buttonToRoot.Anchor = AnchorStyles.None;
            buttonToRoot.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            buttonToRoot.Text = "/";
            this.Controls.Add(buttonToRoot);

            buttonToParent.Location = new Point(this.Width - 22, 1);
            buttonToParent.Size = new Size(21, 23);
            buttonToParent.Anchor = AnchorStyles.None;
            buttonToParent.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            buttonToParent.Text = "..";
            this.Controls.Add(buttonToParent);

            panelSpaceDisk.Location = new Point(46, 2);
            panelSpaceDisk.Size = new Size(this.Width - 90, 21);
            panelSpaceDisk.Anchor = AnchorStyles.None;
            panelSpaceDisk.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            panelSpaceDisk.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelSpaceDisk);

            labelFreeSpace.Location = new Point(0, 3);
            labelFreeSpace.AutoSize = true;
            labelFreeSpace.Text = "Free space disck";
            this.panelSpaceDisk.Controls.Add(labelFreeSpace);

            buttonFavoriteCatalogs.Location = new Point(this.Width - 43, 24);
            buttonFavoriteCatalogs.Size = new Size(21, 18);
            buttonFavoriteCatalogs.Anchor = AnchorStyles.None;
            buttonFavoriteCatalogs.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            buttonFavoriteCatalogs.Text = "*"; //1F7C9 u1F7CA
            this.Controls.Add(buttonFavoriteCatalogs);

            buttonHistory.Location = new Point(this.Width - 22, 24);
            buttonHistory.Size = new Size(21, 18);
            buttonHistory.Anchor = AnchorStyles.None;
            buttonHistory.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            buttonHistory.Text = "H";
            this.Controls.Add(buttonHistory);

            panelDir.Location = new Point(2, 25);
            panelDir.Size = new Size(this.Width - 46, 16);
            panelDir.Anchor = AnchorStyles.None;
            panelDir.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            panelDir.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelDir);

            labelDir.Location = new Point(0, 0);
            labelDir.AutoSize = true;
            labelDir.Text = "DirDirectory";
            this.panelDir.Controls.Add(labelDir);

            listViewDirectory.Location = new Point(2, 42);
            listViewDirectory.Size = new Size(this.Width - 4, this.Height - 65);
            listViewDirectory.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            listViewDirectory.View = View.Details;
            listViewDirectory.FullRowSelect = true;
            //listViewDirectory.OwnerDraw = true;

            listViewDirectory.ItemSelectionChanged += ListViewDirectory_ItemSelectionChanged;
            listViewDirectory.MouseDoubleClick += ListViewDirectory_MouseDoubleClick;

            listViewDirectory.Columns.Add("Имя", 200, HorizontalAlignment.Left);
            listViewDirectory.Columns.Add("Тип", 50, HorizontalAlignment.Left);
            listViewDirectory.Columns.Add("Размер", 70, HorizontalAlignment.Left);
            listViewDirectory.Columns.Add("Дата", 120, HorizontalAlignment.Left);        

            this.Controls.Add(listViewDirectory);

            panelInfoFolder.Location = new Point(2, this.Height - 22);
            panelInfoFolder.Size = new Size(this.Width - 4, 21);
            panelInfoFolder.Anchor = AnchorStyles.None;
            panelInfoFolder.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            panelInfoFolder.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelInfoFolder);

            labelInfoFolder.Location = new Point(0, 3);
            labelInfoFolder.AutoSize = true;
            labelInfoFolder.Text = "Info folder";
            this.panelInfoFolder.Controls.Add(labelInfoFolder);
        }

        private void ListViewDirectory_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            selectionIndex = e.Item.Index;
        }

        private void ListViewDirectory_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            OpenSelected(selectionIndex);                   
        }        

        private Icon GetIcon(string filePatch)
        {
            Icon icon;
            IntPtr iconSmall;
            SHFILEINFO shinfo = new SHFILEINFO();
            iconSmall = Win32.SHGetFileInfo(filePatch, 0, ref shinfo, /*(uint)Marshal.SizeOf(shinfo)*/ 80, Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            if (iconSmall == IntPtr.Zero)
            {
                icon = IconUnknown;
                //throw (new System.IO.FileNotFoundException());               
            }
            else
            {
                icon = Icon.FromHandle(shinfo.hIcon);
            }
            return (icon);
        }

        // Open selected item ListView
        private void OpenSelected(int Index)
        {
            if (!directoryList[selectionIndex].atrributes.ToString().Contains("Directory"))
            {
                if (directoryList[selectionIndex].type != "")
                {
                    MessageBox.Show("file " + directoryList[selectionIndex].name + "." + directoryList[selectionIndex].type + Environment.NewLine + directoryList[selectionIndex].atrributes.ToString());
                }
                else
                {
                    MessageBox.Show("file " + directoryList[selectionIndex].name + Environment.NewLine + directoryList[selectionIndex].atrributes.ToString());
                }              
            }
            else
            {
                GetFoldersFiles(directoryList[selectionIndex].directory + "\\");
            }
        }

        public void GetFoldersFiles(string directory)
        {           
            directoryList.Clear();
            listViewDirectory.Items.Clear();           

            DirectoryInfo thisDirectory = new DirectoryInfo(directory);

            // if root
            string rootDir = thisDirectory.Root.ToString();
            string currentDir = thisDirectory.FullName.ToString();

            labelDir.Text = currentDir;

            if (rootDir != currentDir)
            {              
                string dirFolder = Directory.GetParent(Directory.GetParent(directory).ToString()).ToString();                            
                DateTime dateFolder = Directory.GetCreationTime(dirFolder);

                directoryList.Add(new DirectoryList()
                {
                    icon = /*iconFolder*/ iconUp,
                    directory = dirFolder,
                    name = "[..]",
                    type = "",
                    size = "",
                    date = dateFolder,
                    atrributes = thisDirectory.Attributes
                });
            }

            // Get folders
            try
            {               
                DirectoryInfo[] folders = thisDirectory.GetDirectories();
                foreach (DirectoryInfo folder in folders)
                {                    
                    string dirFolder = folder.FullName;                 
                    directoryList.Add(new DirectoryList()
                    {
                        icon = GetIcon(dirFolder),
                        directory = dirFolder,
                        name = "[" + folder.Name + "]",
                        type = "",
                        size = "<папка>",
                        date = Directory.GetCreationTime(dirFolder),
                        atrributes = folder.Attributes
                    });
                }               
            }
            catch (Exception e)
            {
                MessageBox.Show("Открытие папки " + e.ToString());
            }

            // Get files
            try
            {
                FileInfo[] files = thisDirectory.GetFiles();
                foreach (FileInfo file in files)
                {               
                    string dirFile = file.FullName;
                    string typeFile = Path.GetExtension(dirFile);
                    try
                    {
                        typeFile = typeFile.Substring(1);
                    }
                    catch { }
                    directoryList.Add(new DirectoryList()
                    {
                        icon = GetIcon(dirFile),
                        directory = dirFile,
                        name = Path.GetFileNameWithoutExtension(dirFile),
                        type = typeFile,
                        size = file.Length.ToString(),
                        date = File.GetCreationTime(dirFile),
                        atrributes = file.Attributes
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Открытие файла " + e.ToString());
            }

            // Adding in ListView
            foreach (DirectoryList lineDirectoryList in directoryList)
            {
                string[] item = { lineDirectoryList.name, lineDirectoryList.type, lineDirectoryList.size, lineDirectoryList.date.ToString(dateTimeFormat) };
                ListViewItem listItem = new ListViewItem(item);
                listViewDirectory.Items.Add(listItem);
            }

            // Tread add icon
            Thread iconFileThread = new Thread(SetIcon);
            iconFileThread.Start();
        }

        // Add icon
        public void AddListViewDirectory(ImageList value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<ImageList>(AddListViewDirectory), new object[] { value });
                return;
            }
            listViewDirectory.SmallImageList = value;
            for (int count = 0; count < listViewDirectory.Items.Count; count++)
            {
                listViewDirectory.Items[count].ImageIndex = count;
            }
        }

        // Tread add icon
        private void SetIcon()
        {
            ImageList iconList = new ImageList();
            iconList.ColorDepth = ColorDepth.Depth32Bit;
            foreach (DirectoryList lineDirectoryList in directoryList)
            {
                iconList.Images.Add(lineDirectoryList.icon);
            }
            AddListViewDirectory(iconList);
        }
    }
}
