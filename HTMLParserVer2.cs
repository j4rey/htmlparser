using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlstate
{
    class HTMLParserVer2 : iHtmlParser
    {
        public delegate TreeNode<CustomTag> TokenHandler(CustomTag tag);
        public event TokenHandler OnTagBegin;
        public event TokenHandler OnTagEnd;

        public enum States { TEXT, NODESTART, NODEEND, ATTRIBUTENAME, ATTRIBUTEVALUESTART, ATTRIBUTEVALUEEND, ATTRIBUTEVALUE, COMMENT, DOCTYPE };

        List<string> nodetag = new List<string>();
        int nodecharpos = 0;

        States state;
        TreeNode<CustomTag> currentNode;
        private string CurrentTAG
        {
            get { return string.Join("", nodetag); }
        }


        public HTMLParserVer2()
        {
            state = States.TEXT;
        }

        public void parse(char c)
        {
            switch (state)
            {
                case States.TEXT:
                    {
                        state = Text(c, States.TEXT);
                    }
                    break;
                case States.NODESTART:
                    {
                        state = NODESTART(c, state);
                    }
                    break;
                case States.NODEEND:
                    {
                        state = NODEEND(c, state);
                    }
                    break;
                case States.ATTRIBUTENAME:
                    {
                        state = ATTRIBUTENAME(c, state);
                    }break;
                case States.ATTRIBUTEVALUESTART:
                    {
                        state = ATTRIBUTEVALUESTART(c, state);
                    }
                    break;
                case States.ATTRIBUTEVALUE:
                    {
                        state = ATTRIBUTEVALUE(c, state);
                    }
                    break;
                case States.DOCTYPE:
                    {
                        state = DOCTYPE(c, state);
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }

        private States DOCTYPE(char c, States state)
        {
            States returnState = States.ATTRIBUTEVALUE;
            //if(c)
            return returnState;
        }

        private States ATTRIBUTEVALUE(char c, States state)
        {
            States returnState = States.ATTRIBUTEVALUE;
            if (c == '"')
            {
                returnState = States.ATTRIBUTENAME;
                nodecharpos = 0;
                Console.WriteLine("Attribute Value: " + String.Join("", nodetag));
                Console.WriteLine("Current Tag:" + currentNode.Value.TagName);
                nodetag.Clear();
                return returnState;
            }
            else
            {
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }

        private States ATTRIBUTEVALUESTART(char c, States state)
        {
            if (c == '"')
            {
                return States.ATTRIBUTEVALUE;
            }
            throw new Exception(currentNode.Value.TagName + " has invalid attribute.");
        }

        private States ATTRIBUTENAME(char c, States state)
        {
            States returnState = States.ATTRIBUTENAME;
            if (c == '>')
            {
                returnState = States.TEXT;
                nodecharpos = 0;
                if (!String.IsNullOrWhiteSpace(CurrentTAG)) { 
                    Console.WriteLine("ATTRIBUTENAME: " + String.Join("", nodetag));
                }
                //currentNode = OnTagEnd(null);
                //currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                Console.WriteLine("Current Tag:" + currentNode.Value.TagName);
                nodetag.Clear();
            }
            else if(c=='=')
            {
                returnState = States.ATTRIBUTEVALUESTART;
                nodecharpos = 0;
                Console.WriteLine("Attribute Name: " + String.Join("", nodetag));
                nodetag.Clear();
            }
            else if (Char.IsLetter(c))
            {
                nodetag.Add(Convert.ToString(c));
            }
            else if (c == '/')
            {
                returnState = States.NODEEND;
                nodetag = currentNode.Value.TagName.ToCharArray().Select(x => Convert.ToString(x)).ToList();
            }
            else if(c == '>')
            {
                returnState = States.TEXT;
                nodecharpos = 0;
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " closed");
                currentNode = OnTagEnd(null);
                Console.WriteLine("Current Tag:" + currentNode.Value.TagName);
                nodetag.Clear();
            }
            else if (String.IsNullOrWhiteSpace(Convert.ToString(c)))
            {

            }
            else
            {
                throw new Exception(currentNode.Value.TagName + " has invalid attribute.");
            }
                //if (Char.IsLetter(c))
                //{
                //    nodetag.Add(Convert.ToString(c));
                //}
                return returnState;
        }

        private States NODEEND(char c, States state)
        {
            States returnState = States.TEXT;
            string nodechar = Convert.ToString(currentNode.Value.TagName.ElementAtOrDefault(nodecharpos));
            if (nodechar == Convert.ToString(c))
            {
                returnState = States.NODEEND;
                nodecharpos++;
                nodetag.Add(Convert.ToString(c));
            }
            else if (c == '>' && String.Join("", nodetag).Equals(currentNode.Value.TagName))
            {
                returnState = States.TEXT;
                nodecharpos = 0;
                Console.WriteLine("Tag: " + String.Join("", nodetag) + " closed");
                currentNode = OnTagEnd(null);
                Console.WriteLine("Current Tag:" + currentNode.Value.TagName);
                nodetag.Clear();
            }
            else
            {
                throw new Exception(currentNode.Value.TagName + " wasn't closed appropriately.");
            }
            return returnState;
        }

        private States NODESTART(char c, States state)
        {
            States returnState = States.NODESTART;
            if (c == '<')
            {
                returnState = States.NODESTART;
                nodecharpos = 0;
                nodetag.Clear();
            }
            else if (c == '!' && nodetag.Count() == 0)
            {
                returnState = States.DOCTYPE;
            }
            else if (c == '/')
            {
                if (nodetag.Count() > 0)
                {
                    returnState = States.NODEEND;
                    nodecharpos = 0;
                    currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                    Console.WriteLine("Tag: " + currentNode.Value.TagName + " opened");
                    //nodetag.Clear();
                }
                else
                {
                    returnState = States.NODEEND;
                }
            }
            else if (c == '>')
            {
                returnState = States.TEXT;
                nodecharpos = 0;
                currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                Console.WriteLine("Tag: " + currentNode.Value.TagName + " opened");
                nodetag.Clear();
            }
            else if(c == ' ')
            {
                returnState = States.ATTRIBUTENAME;
                nodecharpos = 0;
                currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                Console.WriteLine("Tag: " + currentNode.Value.TagName+ " opened");
                nodetag.Clear();
            }
            else if (Char.IsLetter(c))
            {
                nodetag.Add(Convert.ToString(c));
            }
            else
            {
                returnState = States.TEXT;
            }
            return returnState;
        }

        private States Text(char c, States tEXT)
        {
            States returnState = States.TEXT;
            if (c == '<')
            {
                returnState = States.NODESTART;
                if (!String.IsNullOrWhiteSpace(CurrentTAG))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Text is:" + CurrentTAG);
                    Console.WriteLine("");
                    currentNode = OnTagBegin(new CustomTag("TEXT"));
                    currentNode = OnTagEnd(null);
                }
                nodetag.Clear();
            }
            else
            {
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }
    }
}
