using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Management;
using System.Security.Permissions;

namespace DeleteUsers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> users = new List<string>();
        string selectedUser;
        private void updateButton_Click(object sender, EventArgs e)
        {
            usersListBox.Items.Clear();
            string[] allfiles = Directory.GetDirectories("C:\\Users");
            foreach (string filename in allfiles)
            {
                if(!(filename.Substring(9).Equals("All Users") || filename.Substring(9).Equals("Default") || filename.Substring(9).Equals("Default User") || filename.Substring(9).Equals("Public")))
                {
                    usersListBox.Items.Add(filename.Substring(9));
                    users.Add(filename.Substring(9));
                }
                    
            }
        }


        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(usersListBox.SelectedItems.Count == 0) { MessageBox.Show("Выберите пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                selectedUser = usersListBox.SelectedItem.ToString();
                var response = MessageBox.Show("Удалить пользователя " + selectedUser + "?", "Удаление пользователя", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (response == DialogResult.Yes)
                {
                    string[] cmdArr = { "net user ", selectedUser, " /delete" };
                    string path = string.Concat("C:\\Users\\", selectedUser);
                    System.Diagnostics.Process.Start("cmd.exe", "/C " + string.Concat(cmdArr));

                    FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, path);
                    try
                    {
                        f2.Demand();
                    }
                    catch (Exception s)
                    {
                        MessageBox.Show(s.Message);
                    }


                    var di = new DirectoryInfo(path);
                    //di.Attributes |= FileAttributes.Normal;
                    del(di);

                    //System.Diagnostics.Process.Start("cmd.exe", "/C rm -r " + path);
                    Directory.Delete(path, true);

                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void del(DirectoryInfo dir)
        {
            
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
                //File.Delete(file.FullName);
            }

            if (dir.GetDirectories().Length == 0) return;
            foreach (var subDir in dir.GetDirectories())
            {
                if (subDir.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    subDir.Attributes = FileAttributes.Normal;
                    subDir.Delete(true);
                    
                }
                else
                {
                    //subDir.Attributes = FileAttributes.Directory;
                    del(subDir);
                    subDir.Attributes = FileAttributes.Normal;
                    subDir.Delete(true);
                    //subDir.Attributes = FileAttributes.Normal;
                }
                
            }
            
        }

    }
}
