using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlstate
{
    class HTMLTokenizer
    {
        TreeNode<CustomTag> root;
        TreeNode<CustomTag> currentNode;
        iHtmlParser parser;
        public HTMLTokenizer(TreeNode<CustomTag> root, iHtmlParser parser)
        {
            this.root = root;
            currentNode = this.root;
            this.parser = parser;
        }

        public TreeNode<CustomTag> OnTagBegin(CustomTag tag)
        {
            return AddChild(tag);
        }

        public TreeNode<CustomTag> OnTagEnd(CustomTag tag)
        {
            return RemoveChild();
        }

        public void FromFile()
        {
            string inputText = "";
            System.IO.TextReader reader = new System.IO.StreamReader("D:/New Schedulers/Scheduler_13-09-2017/CMSParserApp/htmlstate/input.html");
            inputText = reader.ReadToEnd();
            reader.Close();
            int pos = 0;
            try { 
            while (inputText.Length > pos)
            {
                //parser.Run((char)inputText[pos]);
                Read((char)inputText[pos]);
                pos++;
            }
            }
            catch (Exception)
            {
                Console.WriteLine("");
                Console.WriteLine("Exception at position: "+ pos);
                Console.WriteLine("");
            }
            //parser.Output();
        }

        public void Read(char c)
        {
            parser.parse(c);
        }

        public bool IsValid()
        {
            Console.WriteLine("currentNode.Value: " + currentNode.Value);
            if (currentNode.Value.TagName == "root")
            {
                return true;
            }
            return false;
        }

        TreeNode<CustomTag> AddChild(CustomTag node)
        {
            currentNode= currentNode.AddChild(node);
            return currentNode;
        }

        TreeNode<CustomTag> RemoveChild()
        {
            currentNode= currentNode.Parent;
            return currentNode;
        }

        public void Traverse(Action<CustomTag> action)
        {
            this.root.Traverse(action);
        }
    }
}
