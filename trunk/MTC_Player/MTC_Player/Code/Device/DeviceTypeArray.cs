using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Device
{
    public class DeviceTypeArray : IEnumerable
    {
        private List<TypeDevice> Datas;
        public List<TypeDevice> toList()
        {
            return this.Datas;
        }

        public TypeDevice this[int index]
        {
            get
            {
                if (this.Datas == null)
                    throw new Exception("Không tồn tại dữ liệu");
                foreach(TypeDevice type in this.Datas)
                {
                    if(type.ID== index)
                    {
                        return type;
                    }
                }
                throw new Exception("Không tồn tại id");

            }
        }

        public bool Find(int id)
        {
            if (this.Datas == null)
                return false;
            foreach (TypeDevice type in this.Datas)
            {
                if (type.ID == id)
                {
                    return true ;
                }
            }
            return false;
        }


        public void Add(params TypeDevice[] d)
        {
            for(int i = 0; i < d.Length; i++)
            {
                if (d[i] != null)
                {
                    this.Datas.Add(d[i]);
                }
            }
        }
        public void set(List<TypeDevice> Datas)
        {
            this.Datas = Datas;
        }

        public void Remove(TypeDevice d)
        {
            this.Datas.Remove(d);
        }
        public int Count
        {
            get
            {
                return this.Datas.Count;
            }
        }

        public void Remove(int index)
        {
            if(index>0 && index< this.Count)
            {
                this.Datas.RemoveAt(index);
                return;
            }
            throw new Exception("Không tồn tại phần tử");
        }
        

        public IEnumerator GetEnumerator()
        {
           foreach(TypeDevice type in this.Datas)
            {
                yield return type;
            }
        }
        public DeviceTypeArray()
        {
            this.Datas = new List<TypeDevice>();
        }
        public DeviceTypeArray(List<TypeDevice> datas)
        {
            this.Datas = datas;
        }
    }
}
