using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlstate
{
    class HTMLParser : iHtmlParser
    {

        public delegate TreeNode<CustomTag> TokenHandler(CustomTag tag);
        public event TokenHandler OnTagBegin;
        public event TokenHandler OnTagEnd;

        public enum States { NONE, START, LESSTHAN, EXCLAIMATION, HYPHEN, SECONDHYPHEN, COMMENT, D, O, C, T, Y, P, DOCTYPE, NODE, ENDING_NODE, ATTRIBUTE, BACKSLASH }

        String readChars = String.Empty;
        String CurrentTag = String.Empty;
        bool isTagOpen = false;
        List<string> nodetag = new List<string>();
        int nodecharpos = 0;
        TreeNode<string> tree;
        TreeNode<string> currentNode;
        States state;

        public HTMLParser()
        {
            tree = new TreeNode<string>("root");
            currentNode = tree;
            state = States.START;
        }

        void AddChild(string node)
        {
            currentNode = currentNode.AddChild(node);
        }
        void RemoveChild()
        {
            currentNode = currentNode.Parent;
        }

        public void parse(char inputchar)
        {
            Console.Write(inputchar +" tag:"+isTagOpen);
            //char inputchar;
            //string inputText = "";
            //System.IO.TextReader reader = new System.IO.StreamReader("D:/New Schedulers/Scheduler_13-09-2017/CMSParserApp/htmlstate/input.html");
            //inputText = reader.ReadToEnd();
            //reader.Close();

            //int pos = 0;
            //while (inputText.Length > pos)
            //while ((inputchar = Console.ReadKey().KeyChar) != 'Q')
            {
                //inputchar = inputText[pos];
                //pos++;
                //if (inputchar == '~')
                //{
                //    state = States.START;
                //}
                //if (inputchar == '+')
                //{
                //    Console.WriteLine("");
                //    tree.Traverse((x) =>
                //    {
                //        Console.Write(x + "->");
                //    });
                //    Console.WriteLine("");
                //    continue;
                //}
                //Console.WriteLine("input: " + inputchar, state);
                readChars += inputchar;
                if (state == States.START)
                {
                    state = Start(inputchar, States.NONE);
                }
                else if (state == States.LESSTHAN)
                {
                    state = LessThan(inputchar, state);
                }
                else if (state == States.EXCLAIMATION)
                {
                    state = Exclaimation(inputchar, state);
                }
                else if (state == States.HYPHEN)
                {
                    state = Hyphen(inputchar, state);
                }
                else if (state == States.SECONDHYPHEN)
                {
                    state = SecondHyphen(inputchar, state);
                }
                else if (state == States.COMMENT)
                {
                    state = Comment(inputchar, state);
                }
                else if (state == States.D)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.O)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.C)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.T)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.Y)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.P)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.DOCTYPE)
                {
                    state = DOCTYPE(inputchar, state);
                }
                else if (state == States.NODE)
                {
                    state = Node(inputchar, state);
                }
                else if (state == States.ATTRIBUTE)
                {
                    state = Attribute(inputchar, state);
                }
                else if (state == States.BACKSLASH)
                {
                    state = BACKSLASH(inputchar, state);
                }
                else if (state == States.ENDING_NODE)
                {
                    state = ENDING_Node(inputchar, state);
                }

                Console.WriteLine("|State:" + state);
            }
        }

        public void Output()
        {
            Console.WriteLine("");
            tree.Traverse((x) =>
            {
                Console.Write(x + "->");
            });

            Console.WriteLine("");
            Console.WriteLine("currentNode.Value: " + currentNode.Value);
            if (currentNode.Value != "root")
            {
                Console.WriteLine("InValid Html");
            }
            else
            {
                Console.WriteLine("Valid Html");
            }
            Console.WriteLine("");
            Console.WriteLine("Quit");
            //Console.ReadKey();
        }

        private States BACKSLASH(char c, States previousState)
        {
            States returnState;
            if (c == '>')
            {
                //returnState = States.START;
                //Console.WriteLine("Tag: " + CurrentTag + " end");
                //isTagOpen = false;
                //CurrentTag = String.Empty;

                returnState = States.START;
                //isTagOpen = false;
                //CurrentTag = String.Empty;
                nodecharpos = 0;
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " closed");
                nodetag = currentNode.Parent.Value.ToArray().Select(x => Convert.ToString(x)).ToList();
                Console.WriteLine("Current Tag:" + String.Join("", nodetag));
                RemoveChild();
                OnTagEnd(null);
                nodetag.Clear();
            }
            else if (c == '/')
            {
                returnState = States.BACKSLASH;
                //Console.WriteLine("Tag: " + CurrentTag + " end");
            }
            else
            {
                if (isTagOpen == true && CurrentTag == "NODE")
                {
                    returnState = States.NODE;
                }
                else
                {
                    returnState = States.START;
                }
            }
            return returnState;
        }

        private States Attribute(char c, States previousState)
        {
            States returnState = States.ATTRIBUTE;
            //if (c == '>')
            //{
            //    returnState = States.ATTRIBUTE;
            //    isTagOpen = false;
            //    CurrentTag = "START";
            //    Console.WriteLine("Tag: Node closed");
            //}
            //else 
            if (c == '<')
            {
                returnState = States.LESSTHAN;
            }
            else if (c == '/')
            {
                returnState = States.BACKSLASH;
            }
            return returnState;
        }
        private States Node(char c, States previousState)
        {
            States returnState = States.NODE;
            if (c == '>')
            {
                //if (isTagOpen == true && CurrentTag == "NODE")
                //{
                //    returnState = States.START;
                //    isTagOpen = false;
                //    CurrentTag = String.Empty;
                //    nodecharpos = 0;
                //    Console.WriteLine("Tag: " + String.Join("", nodetag) + " Closed");
                //    nodetag.Clear();
                //}
                //else
                {
                    returnState = States.START;
                    isTagOpen = true;
                    CurrentTag = "NODE";
                    nodecharpos = 0;
                    Console.WriteLine("Tag: " + String.Join("", nodetag) + " opened");
                    OnTagBegin(new CustomTag(string.Join("", nodetag)));
                    AddChild(string.Join("", nodetag));
                    nodetag.Clear();
                }
            }
            else if (c == '/')
            {
                returnState = States.BACKSLASH;
            }
            else if (c == ' ')
            {
                returnState = States.ATTRIBUTE;
                isTagOpen = true;
                CurrentTag = "NODE";
                nodecharpos = 0;
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " opened");
                OnTagBegin(new CustomTag(string.Join("", nodetag)));
                AddChild(String.Join("", nodetag));
                nodetag.Clear();
            }
            else
            {
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }
        private States ENDING_Node(char c, States previousState)
        {
            States returnState = States.START;
            if (isTagOpen)
            {
                string nodechar = Convert.ToString(currentNode.Value.ElementAtOrDefault(nodecharpos));
                if (nodechar == Convert.ToString(c))
                {
                    returnState = States.ENDING_NODE;
                    nodecharpos++;
                    nodetag.Add(Convert.ToString(c));
                }
                else if (c == '>' && String.Join("", nodetag).Equals(currentNode.Value))
                {
                    returnState = States.START;
                    //isTagOpen = false;
                    //CurrentTag = String.Empty;
                    nodecharpos = 0;
                    Console.WriteLine("Tag: " + String.Join("", nodetag) + " closed");
                    nodetag = currentNode.Parent.Value.ToArray().Select(x => Convert.ToString(x)).ToList();
                    Console.WriteLine("Current Tag:" + String.Join("", nodetag));
                    RemoveChild();
                    OnTagEnd(null);
                    nodetag.Clear();
                }
                else
                {
                    Console.WriteLine("Current Tag: " + currentNode.Value);
                    nodecharpos = 0;
                    nodetag.Clear();
                }
            }
            return returnState;
        }

        #region DOCTYPE
        private States DOCTYPE(char c, States previousState)
        {
            States returnState = States.START;

            if (isTagOpen == false && c == 'o' || c == 'O' && previousState == States.D)
            {
                returnState = States.O;
            }
            else if (isTagOpen == false && c == 'c' || c == 'C' && previousState == States.O)
            {
                returnState = States.C;
            }
            else if (isTagOpen == false && c == 't' || c == 'T' && previousState == States.C)
            {
                returnState = States.T;
            }
            else if (isTagOpen == false && c == 'y' || c == 'Y' && previousState == States.T)
            {
                returnState = States.Y;
            }
            else if (isTagOpen == false && c == 'p' || c == 'P' && previousState == States.Y)
            {
                returnState = States.P;
            }
            else if (isTagOpen == false && c == 'e' || c == 'E' && previousState == States.P)
            {
                returnState = States.DOCTYPE;
                isTagOpen = true;
                CurrentTag = "DOCTYPE";

                nodecharpos = 0;
                nodetag = (("DOCTYPE").ToCharArray()).Select(x => Convert.ToString(x)).ToList();
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " opened");
                OnTagBegin(new CustomTag(string.Join("", nodetag)));
                AddChild(string.Join("", nodetag));
                nodetag.Clear();
            }
            else if (isTagOpen == true && c == '>' && previousState == States.DOCTYPE)
            {
                returnState = States.START;
                isTagOpen = false;
                CurrentTag = String.Empty;
                Console.WriteLine("Tag: Doctype end");
                
                nodecharpos = 0;
                nodetag = ("DOCTYPE").ToCharArray().Select(x => Convert.ToString(x)).ToList();
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " closed");
                nodetag = currentNode.Parent.Value.ToArray().Select(x => Convert.ToString(x)).ToList();
                Console.WriteLine("Current Tag:" + String.Join("", nodetag));
                RemoveChild();
                OnTagEnd(null);
                nodetag.Clear();
            }
            else
            {
                if (isTagOpen == true && previousState == States.DOCTYPE)
                {
                    returnState = previousState;
                }
            }
            return returnState;
        }
        #endregion

        public States Start(char c, States previousState)
        {
            States returnState = States.START;
            if (c == '<')
            {
                returnState = States.LESSTHAN;
                Console.WriteLine("");
                Console.WriteLine("Text is:" + String.Join("", nodetag));
                Console.WriteLine("");
                if(!String.IsNullOrWhiteSpace(String.Join("", nodetag)))
                {
                    OnTagBegin(new CustomTag("TEXT"));
                    OnTagEnd(null);
                }
                nodetag.Clear();
            }
            else
            {
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }

        private States LessThan(char c, States previousState)
        {
            States returnState = States.NODE;
            if (c == '<')
            {
                returnState = States.LESSTHAN;
            }
            else if (c == '!')
            {
                returnState = States.EXCLAIMATION;
            }
            else if (c == '/')
            {
                returnState = States.ENDING_NODE;
            }
            else
            {
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }

        #region COMMENT
        private States Exclaimation(char c, States previousState)
        {
            States returnState = States.START;
            if (c == '-')
            {
                returnState = States.HYPHEN;
            }
            else if (c == 'D' || c == 'd')
            {
                returnState = States.D;
            }
            return returnState;
        }

        private States Hyphen(char c, States previousState)
        {
            States returnState;
            if (c == '-')
            {
                if (CurrentTag == "COMMENT" && isTagOpen == true)
                {
                    returnState = States.SECONDHYPHEN;
                }
                else
                {
                    returnState = States.COMMENT;
                    CurrentTag = "COMMENT";
                    isTagOpen = true;
                    
                    nodecharpos = 0;
                    nodetag = (("COMMENT").ToCharArray()).Select(x => Convert.ToString(x)).ToList();
                    Console.WriteLine("Tag: " + String.Join("", nodetag) + " opened");
                    OnTagBegin(new CustomTag(string.Join("", nodetag)));
                    AddChild(string.Join("", nodetag));
                    nodetag.Clear();
                }
            }
            else
            {
                if (CurrentTag == "COMMENT" && isTagOpen == true)
                {
                    returnState = States.COMMENT;
                }
                else
                {
                    returnState = States.START;
                }
            }
            return returnState;
        }

        private States SecondHyphen(char c, States previousState)
        {
            States returnState = States.COMMENT;
            if (c == '>')
            {
                returnState = States.START;
                CurrentTag = String.Empty;
                isTagOpen = false;

                nodecharpos = 0;
                nodetag = ("COMMENT").ToCharArray().Select(x => Convert.ToString(x)).ToList();
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " closed");
                nodetag = currentNode.Parent.Value.ToArray().Select(x => Convert.ToString(x)).ToList();
                Console.WriteLine("Current Tag:" + String.Join("", nodetag));
                RemoveChild();
                OnTagEnd(null);
                nodetag.Clear();
            }
            return returnState;
        }

        private States Comment(char c, States previousState)
        {
            States returnState = States.COMMENT;
            if (c == '-')
            {
                // returnState = "CLOSE_HYPHEN_FIRST";
                returnState = States.HYPHEN;
            }
            return returnState;
        }
        #endregion
    }
}
