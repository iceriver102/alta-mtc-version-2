using Alta.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTC_Server.Code.Media;

namespace MTC_Server.Code.User
{
    public partial class UserData
    {
        public int ID { get; set; }
        public string User_Name { get; set; }
        public string Full_Name { get; set; }
        public bool Status { get; set; }
        public Permision @Permision { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Time { get; set; }

        private string _pass;
        public string Pass { get
            {
                return this._pass;
            }
            set
            {
                this._pass = value.MD5String();
            }
        }

        private int _type;
        public int Type
        {
            get
            {
                if (this._type <= 0)
                {
                    this._type = getTypeUser(this.ID);
                    this._TypeUser = App.selectType(this._type);
                }
                return this._type;
            }
            set
            {
                this._type = value;
                this._TypeUser = App.selectType(value);
            }
        }
        public string Comment { get; set; }
        private UserTypeData _TypeUser;
        public UserTypeData TypeUser
        {
            get
            {
                if (this._TypeUser == null)
                {
                    this._TypeUser = UserData.getType(this.ID);
                    this._type = this._TypeUser.Id;
                }
                return this._TypeUser;
            }
        }
        public byte[] Finger_Print;
    }
}
