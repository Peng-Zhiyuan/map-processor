using System;
using System.IO;
using System.Xml;
using System.Text;

namespace mp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = args[0];
            //var dir = @"Z:\Project\magic-tower\assets\resources\map";
            var d = new DirectoryInfo(dir);
            var fileList = d.GetFiles();
            foreach(var file in fileList)
            {
                if(file.Extension == ".tmx")
                {
                    DoFile(file.FullName);
                }
            }
        }

        static void DoFile(string path)
        {
            var xml = new XmlDocument();

            xml.Load(path);

            var root = xml.SelectSingleNode("/map");

            Do(xml, root as XmlElement);

            var fileInfo = new FileInfo(path);
            var dir = fileInfo.DirectoryName;
            var filename = fileInfo.Name;

            var out_path = dir + "/" + filename;

            StreamWriter sw = new StreamWriter(out_path, false, new UTF8Encoding(false));
            xml.Save(sw);
        }

        static void Do(XmlDocument d, XmlElement node)
        {
            if(node == null)
            {
                return;
            }
            if(node.Name == "property")
            {
                var aName = node.Attributes["name"];
                if(aName != null)
                {
                    if(aName.Value == "is" || aName.Value == "script")
                    {
                        var code = "";
                        if(!string.IsNullOrEmpty(node.InnerText))
                        {
                            code = node.InnerText;
                        }
                        else 
                        {
                            var aValue = node.Attributes["value"];
                            if(aValue != null)
                            {
                                code = aValue.Value;
                            }
                        }

                        if(!string.IsNullOrEmpty(code))
                        {
                            //node.InnerText = null;
                            code = code.Replace("\r\n", @"\n");
                            code = code.Replace("\n", @"\n");
                            var isnode = GetNodeInSlibing_is(d, node);
                            isnode.SetAttribute("value", code);
                        }
                    }

                }
            }

            foreach(var c in node)
            {
                var cc = c as XmlElement;
                Do(d, cc);
            }

        }

        static XmlElement GetNodeInSlibing_is(XmlDocument d, XmlElement e)
        {
            var p = e.ParentNode as XmlElement;
            foreach(var c in p)
            {
                var cc = c as XmlElement;
                if(cc.GetAttribute("name") == "_script")
                {
                    return cc;
                }
            }
            XmlElement node = d.CreateElement("property");
            p.AppendChild(node);
            node.SetAttribute("name", "_script");
            return node;
        }
    }


}
