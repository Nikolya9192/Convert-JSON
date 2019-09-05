using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace WindowsExplorer
{
    public class MyTreeViewItem:TreeViewItem
    {
        public string Path { get; set; }
        public string Type { get; set; }

        public DirectoryInfo di { get; set; }
    }
}
