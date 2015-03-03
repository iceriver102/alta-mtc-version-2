using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public class FontIcons : IEnumerable
    {
        private List<Icon> Datas;
        public IEnumerator GetEnumerator()
        {
            foreach (Icon icon in this.Datas)
            {
                yield return icon;
            }
        }
        public void Add(string key, string code)
        {
            bool canAdd = true;
            foreach (Icon i in this.Datas)
            {
                if (i.index == key)
                {
                    canAdd = false;
                    break;
                }
            }
            if (canAdd)
                Datas.Add(new Icon(key, code));
        }

        public void Add(Icon icon)
        {
            bool canAdd = true;
            foreach (Icon i in this.Datas)
            {
                if (i.index == icon.index)
                {
                    canAdd = false;
                    break;
                }
            }
            if (canAdd)
                this.Datas.Add(icon);
        }
        public void Remove(string index)
        {
            foreach (Icon i in this.Datas)
            {
                if (i.index == index)
                {
                    this.Datas.Remove(i);
                    break;
                }
            }
        }

        public void Remove(Icon icon)
        {
            this.Datas.Remove(icon);
        }
        public void Remove(int index)
        {
            this.Datas.RemoveAt(index);
        }
        public IEnumerable<Icon> Childs
        {
            get
            {
                foreach (Icon N in this.Datas)
                    yield return N;
            }
        }
        public int Count
        {
            get
            {
                return this.Datas.Count;
            }
        }
        public Icon this[int pos]
        {
            get
            {
                return this.Datas[pos];
            }
        }
        public Icon this[string index]
        {
            get
            {
                foreach (Icon i in this.Datas)
                {
                    if (i.index == index)
                    {
                        return i;
                    }
                }
                throw new Exception("Không tồn tại dữ liệu");
            }
        }
        public FontIcons()
        {
            this.Datas = new List<Icon>();
        }
    }
    public class Icon
    {
        public string index { get; set; }
        public string Hex { get { return string.Format("&#x{0}", this.code); } }
        private string code = string.Empty;
        public string Code
        {
            get
            {
                return string.Format(@"\uf{0}", this.code).DecodeEncodedNonAsciiCharacters();
            }
            private set
            {
                this.code = value;
            }
        }
        public Icon()
        {

        }
        public Icon(params string[] Params)
        {
            if (Params.Length == 1)
            {
                this.index = Params[0];
            }
            else if (Params.Length == 2)
            {
                this.index = Params[0];
                this.Code = Params[1];
            }
            else
            {
                throw new Exception("Không đúng cấu trúc");
            }
        }
        public bool hasIndex
        {
            get
            {
                return !string.IsNullOrEmpty(this.index);
            }
        }
        public bool hasCode
        {
            get
            {
                return !string.IsNullOrEmpty(this.code);
            }
        }
        public void setCode(string code)
        {
            this.code = code;
        }
    }

    public class ExCss
    {
        public static FontIcons Parse(string aJSON)
        {
            FontIcons icons = new FontIcons();
            bool QuoteMode = false;
            bool isIndex = false;
            string token = string.Empty;
            string tokenName = string.Empty;
            Icon ctx = null;
            int i = 0;
            bool isBreak = false;
            while (i < aJSON.Length)
            {
                if (!isBreak || aJSON[i] == '{')
                {
                    isBreak = false;
                    switch (aJSON[i])
                    {
                        case '.':
                            if (QuoteMode)
                            {
                                token += aJSON[i];
                                break;
                            }
                            ctx = null;
                            isIndex = true;
                            token = string.Empty;
                            break;
                        case '"':
                            QuoteMode ^= true;
                            break;
                        case ':':
                            if (QuoteMode)
                            {
                                token += aJSON[i];
                                break;
                            }
                            if (!isIndex)
                            {
                                token = string.Empty;
                            }
                            else
                            {
                                isBreak = true;
                            }
                            break;
                        case '{':
                            if (QuoteMode)
                            {
                                token += aJSON[i];
                                break;
                            }
                            isIndex = false;
                            tokenName = token;
                            token = "";
                            if (!string.IsNullOrEmpty(tokenName))
                                ctx = new Icon(tokenName);
                            break;
                        case '\\':
                            ++i;
                            if (QuoteMode)
                            {
                                char C = aJSON[i];
                                switch (C)
                                {
                                    case 't': token += '\t'; break;
                                    case 'r': token += '\r'; break;
                                    case 'n': token += '\n'; break;
                                    case 'b': token += '\b'; break;
                                    case 'f': break;
                                    case 'u':
                                        {
                                            string s = aJSON.Substring(i + 1, 4);
                                            token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                                            i += 4;
                                            break;
                                        }
                                    default: token += C; break;
                                }
                            }
                            break;
                        case '}':
                            if (QuoteMode)
                            {
                                token += aJSON[i];
                                break;
                            }
                            if (ctx != null)
                            {
                                ctx.setCode(token);
                                token = string.Empty;
                                icons.Add(ctx);
                            }
                            else
                            {
                                token = string.Empty;
                            }

                            break;
                        case '\r':
                        case '\n':
                            break;
                        case ' ':
                        case '\t':
                            if (QuoteMode)
                                token += aJSON[i];
                            break;
                        default:
                            token += aJSON[i];
                            break;
                    }
                    ++i;
                }
                else
                {
                    ++i;
                }
            }
            return icons;
        }
        public static FontIcons ReadFile(string file)
        {
            string text = System.IO.File.ReadAllText(file);
            return Parse(text);
        }
    }
}
