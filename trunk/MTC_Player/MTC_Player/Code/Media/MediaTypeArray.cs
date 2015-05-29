using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Media
{
    public class MediaTypeArray : IEnumerable
    {
        private List<TypeMedia> Datas;
        public List<TypeMedia> toList()
        {
            return Datas;
        }

        public MediaTypeArray()
        {
            this.Datas = new List<TypeMedia>();
        }

        public MediaTypeArray(List<TypeMedia> datas)
        {
            this.Datas = datas;
        }

        public void setData(List<TypeMedia> datas)
        {
            this.Datas = datas;
        }

        public TypeMedia this[string code]
        {
            get
            {
                if (this.Datas == null)
                    throw new Exception("Dữ liệu trống");
                foreach(TypeMedia t in this.Datas)
                {
                    if(code.ToUpper()== t.Code.ToUpper())
                    {
                        return t;
                    }
                }
                throw new Exception("Không tồn tại dữ liệu");
            }
        }
        public TypeMedia this[int id]
        {
            get
            {
                if (this.Datas == null)
                    throw new Exception("Dữ liệu trống");
                foreach (TypeMedia t in this.Datas)
                {
                    if (id == t.Id)
                    {
                        return t;
                    }
                }
                throw new Exception("Không tồn tại dữ liệu");
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (TypeMedia t in this.Datas)
            {
               yield return t;
            }
        }
    }
}
