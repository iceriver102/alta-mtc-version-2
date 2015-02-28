using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public class FontIconDefine : IEnumerable
    {
        private List<FontNode> Datas;
        public FontIconDefine()
        {
            Datas = new List<FontNode>();
            Datas.Add(new FontNode("fa-lock", "f023"));
            Datas.Add(new FontNode("fa-unlock-alt" ,"f13e"));
            Datas.Add(new FontNode("fa-unlock" ,"f09c"));
            Datas.Add(new FontNode("fa-sign-out", "f08b"));
            Datas.Add(new FontNode("fa-info","f129"));

            //fa-life-ring [&#xf1cd;]
            Datas.Add(new FontNode("fa-life-ring", "f1cd"));
            //fa-save (alias) [&#xf0c7;]
            Datas.Add(new FontNode("fa-save", "f0c7"));
        }
        public FontNode this[int index]
        {
            get
            {
                return Datas[index];
            }
            set
            {
                Datas[index] = value;
            }
        }

        public FontNode this[string index]
        {
            get
            {
                foreach (FontNode node in this.Datas)
                {
                    if (node.check(index))
                    {
                        return node;
                    }
                }
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class FontNode
    {
        private string Index;
        private string Code;
        private string HexCode;

        public FontNode(string index, string code)
        {
            this.set(index, code);
        }
        public FontNode() { }

        public string asCode
        {
            get
            {
                return this.Code.DecodeEncodedNonAsciiCharacters();
            }
        }
        public string asHexCode
        {
            get
            {
                return this.HexCode;
            }
        }
        public void set(string index, string code)
        {
            this.Index = index;
            this.HexCode = string.Format("&#x{0};", code);
            this.Code = string.Format(@"\u{0}", code);
        }

        public bool check(string index)
        {
            if (this.Index == index)
            {
                return true;
            }
            return false;
        }
    }
}
