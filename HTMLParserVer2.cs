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

        public enum States
        {
            TEXT, NODESTART, NODEEND, ATTRIBUTENAME, ATTRIBUTEVALUESTART, ATTRIBUTEVALUEEND, ATTRIBUTEVALUE, COMMENT, DOCTYPESTART, DOCTYPEATTRIBUTE, DOCTYPEATTRIBUTESTART,
            COMMENTSTART, SCRIPTCONTENT, SCRIPTCONTENTQUOTE
        };

        List<string> nodetag = new List<string>();
        int nodecharpos = 0;

        States state;
        TreeNode<CustomTag> currentNode;
        private string CurrentTAG
        {
            get { return string.Join("", nodetag).Trim(); }
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
                    }
                    break;
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
                case States.DOCTYPESTART:
                    {
                        state = DOCTYPESTART(c, state);
                    }
                    break;
                case States.DOCTYPEATTRIBUTE:
                    {
                        state = DOCTYPEATTRIBUTE(c, state);
                    }
                    break;
                case States.DOCTYPEATTRIBUTESTART:
                    {
                        state = DOCTYPEATTRIBUTESTART(c, state);
                    }
                    break;
                case States.COMMENTSTART:
                    {
                        state = COMMENTSTART(c, state);
                    }
                    break;
                case States.COMMENT:
                    {
                        state = COMMENT(c, state);
                    }
                    break;
                case States.SCRIPTCONTENT:
                    {
                        state = SCRIPTCONTENT(c, state);
                    }
                    break;
                case States.SCRIPTCONTENTQUOTE:
                    {
                        state = SCRIPTCONTENT(c, state);
                    }
                    break;

                default:
                    {

                    }
                    break;
            }
        }

        private States SCRIPTCONTENT(char c, States state)
        {
            States returnState = States.SCRIPTCONTENT;

            if (state == States.SCRIPTCONTENT)
            {
                if (c == '"')
                {
                    returnState = States.SCRIPTCONTENTQUOTE;
                    nodetag.Add(Convert.ToString(c));
                }
                else if (String.Join("", nodetag).ToLower().EndsWith("</script") && c == '>')
                {
                    returnState = States.TEXT;
                    int lastIndexOf = String.Join("", nodetag).ToLower().IndexOf("</script");
                    string scripts = String.Join("", nodetag).Substring(0, nodetag.Count() - (nodetag.Count() - lastIndexOf));
                    Console.WriteLine(currentNode.Value.TagName + ": " + scripts);
                    Console.WriteLine("Tag: " + currentNode.Value.TagName + " closed");
                    currentNode = OnTagEnd(null);
                    nodetag.Clear();
                }
                else
                {
                    nodetag.Add(Convert.ToString(c));
                }
            }
            else
            {
                nodetag.Add(Convert.ToString(c));
                if (c == '"')
                {
                    returnState = States.SCRIPTCONTENT;
                }
            }
            return returnState;
        }

        private States COMMENT(char c, States state)
        {
            States returnState = States.COMMENT;
            nodetag.Add(Convert.ToString(c));
            //if (c == '>')
            {
                if (String.Join("", nodetag).EndsWith("-->"))//check for -->
                {
                    returnState = States.TEXT;
                    String node = String.Join("", nodetag);
                    Console.WriteLine("COMMENT: " + node.Replace("-->", ""));
                    currentNode = OnTagBegin(new CustomTag("COMMENT"));
                    currentNode = OnTagEnd(null);
                    nodetag.Clear();
                }
            }

            return returnState;
        }

        private States COMMENTSTART(char c, States state)
        {
            States returnState = States.COMMENTSTART;

            if (c == '-') //<!--
            {
                returnState = States.COMMENT;
            }
            else throw new Exception("Invalid Comment tag.");

            return returnState;
        }

        private States DOCTYPEATTRIBUTESTART(char c, States state)
        {
            States returnState = States.DOCTYPEATTRIBUTESTART;
            if (c == '"')
            {
                nodetag.Add(Convert.ToString(c));
                returnState = States.DOCTYPEATTRIBUTE;
                Console.WriteLine("DOCTYPEATTRIBUTE3: " + String.Join("", nodetag));
                nodetag.Clear();
            }
            else if (c == '>')
            {
                throw new Exception(currentNode.Value.TagName + " ended abruptly.");
            }
            else
            {
                returnState = States.DOCTYPEATTRIBUTESTART;
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }

        private States DOCTYPEATTRIBUTE(char c, States state)
        {
            States returnState = States.DOCTYPEATTRIBUTE;
            if (c == '>')
            {
                returnState = States.TEXT;
                if (nodetag.Count() > 0)
                {
                    Console.WriteLine("DOCTYPEATTRIBUTE1: " + String.Join("", nodetag));
                    nodetag.Clear();
                }
                currentNode = OnTagEnd(null);
            }
            else if (c == '"')
            {
                nodetag.Add(Convert.ToString(c));
                returnState = States.DOCTYPEATTRIBUTESTART;
            }
            else if (c == ' ')
            {
                returnState = States.DOCTYPEATTRIBUTE;
                if (nodetag.Count() > 0)
                {
                    Console.WriteLine("DOCTYPEATTRIBUTE2: " + String.Join("", nodetag));
                }
                nodetag.Clear();
            }
            else
            {
                returnState = States.DOCTYPEATTRIBUTE;
                nodetag.Add(Convert.ToString(c));
            }
            return returnState;
        }

        private States DOCTYPESTART(char c, States state)
        {
            States returnState = States.DOCTYPESTART;
            if ((new char[15] { 'd', 'o', 'c', 't', 'y', 'p', 'e', 'D', 'O', 'C', 'T', 'Y', 'P', 'E', ' ' }).Contains(c))
            {
                nodetag.Add(Convert.ToString(c));
                string dtype = CurrentTAG.ToLower().Trim();
                if (dtype.StartsWith(String.Join("", (new char[7] { 'd', 'o', 'c', 't', 'y', 'p', 'e' }).Take(nodetag.Count()))))
                {
                    if (String.Equals(dtype, "doctype") && c == ' ')
                    {
                        returnState = States.DOCTYPEATTRIBUTE;
                        nodecharpos = 0;
                        currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                        Console.WriteLine("Tag: " + currentNode.Value.TagName + " opened");
                        nodetag.Clear();
                    }
                }
                else
                {
                    throw new Exception(CurrentTAG + " is an invalid tag. Expected doctype.");
                }
            }
            else if (c == '-' && nodetag.Count() == 0) //<!-
            {
                returnState = States.COMMENTSTART;
            }
            else
            {
                throw new Exception(CurrentTAG + " is an invalid tag. Expected doctype.");
            }
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
                if (!String.IsNullOrWhiteSpace(CurrentTAG))
                {
                    Console.WriteLine("ATTRIBUTENAME: " + String.Join("", nodetag));
                }
                //currentNode = OnTagEnd(null);
                //currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                Console.WriteLine("Current Tag:" + currentNode.Value.TagName);
                nodetag.Clear();

                //DOCTYPE END
                if (currentNode.Value.TagName.Trim().ToLower() == "doctype")
                {
                    currentNode = OnTagEnd(null);
                }
            }
            else if (c == '=')
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
            else if (c == '>')
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
                returnState = States.DOCTYPESTART;
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
                if (CurrentTAG.ToLower().Equals("script"))
                {
                    returnState = States.SCRIPTCONTENT;
                }
                else
                    returnState = States.TEXT;
                nodecharpos = 0;
                currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                Console.WriteLine("Tag: " + currentNode.Value.TagName + " opened");
                nodetag.Clear();
            }
            else if (c == ' ')
            {

                returnState = States.ATTRIBUTENAME;
                nodecharpos = 0;
                currentNode = OnTagBegin(new CustomTag(CurrentTAG));
                Console.WriteLine("Tag: " + currentNode.Value.TagName + " opened");
                nodetag.Clear();
            }
            else if (Char.IsLetter(c) && nodetag.Count() == 0)
            {
                nodetag.Add(Convert.ToString(c));
            }
            else if (nodetag.Count() > 0)
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
