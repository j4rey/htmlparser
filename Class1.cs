using htmlstate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlstate
{
    public class Class1
    {
        static TreeNode<CustomTag> tree;
        static TreeNode<CustomTag> currentNode;
        static HTMLTokenizer mainroot;
        public static void Main(string[] args)
        {
            tree = new TreeNode<CustomTag>(new CustomTag("root"));
            //HTMLParser parser = new HTMLParser();
            HTMLParserVer2 parser = new HTMLParserVer2();
            currentNode = tree;
            mainroot = new HTMLTokenizer(tree, parser);
            parser.OnTagBegin += mainroot.OnTagBegin;
            parser.OnTagEnd += mainroot.OnTagEnd;

            mainroot.FromFile();
            //FromInput();

            Console.WriteLine("ISvalid: " + mainroot.IsValid());
            Console.WriteLine("------------------------");
            Console.WriteLine("OUTPUT:");
            tree.Traverse((x) =>
            {
                Console.Write(x.TagName + "->");
            });
            
            Console.WriteLine("");
            Console.WriteLine("Quit");
            Console.ReadKey();
        }

        static void FromInput()
        {
            char inputchar;
            while ((inputchar = Console.ReadKey().KeyChar) != 'Q')
            {
                if (inputchar == '+')
                {
                    Console.WriteLine("");
                    mainroot.Traverse((x) =>
                    {
                        Console.Write(x.TagName + "->");
                    });
                    Console.WriteLine("");
                }
                else
                {
                    mainroot.Read(inputchar);
                }
            }
        }
    }
}
