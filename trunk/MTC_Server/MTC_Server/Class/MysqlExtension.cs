using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTC_Server.Code;
using MTC_Server.Code.User;
using MTC_Server.Code.Media;
using MySql.Data.MySqlClient;

namespace Alta.Class
{
    public static class MysqlExtensions
    {
        public static List<MediaData> toMedias(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<MediaData> datas = new List<MediaData>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        MediaData m = new MediaData();
                        m.ID = reader.GetInt32(Define.media_id);
                        m.Name = reader.GetString(Define.media_name);
                        m.Url = reader.GetString(Define.media_url);
                        m.Type = reader.GetInt32(Define.media_type);
                        try
                        {
                            m.FileSize = reader.GetString(Define.media_size);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            m.Duration = reader.GetString(Define.media_duration);
                        }
                        catch (Exception)
                        {
                        }
                        m.Time = reader.GetDateTime(Define.media_time);
                        m.Status = reader.GetBoolean(Define.media_status);
                        m.User_ID = reader.GetInt32(Define.media_user);
                        try
                        {
                            m.Comment = reader.GetString(Define.media_comment);
                        }
                        catch (Exception)
                        {
                        }
                        datas.Add(m);
                    }

                }
                return datas;
            }
            return null;
        }
        public static MediaData toMedia(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                MediaData m = new MediaData();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        m.ID = reader.GetInt32(Define.media_id);
                        m.Name = reader.GetString(Define.media_name);
                        m.Url = reader.GetString(Define.media_url);
                        m.Type = reader.GetInt32(Define.media_type);
                        m.FileSize = reader.GetString(Define.media_size);
                        try
                        {
                            m.FileSize = reader.GetString(Define.media_size);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            m.Duration = reader.GetString(Define.media_duration);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            m.Comment = reader.GetString(Define.media_comment);
                        }
                        catch (Exception)
                        {
                        }
                        m.Status = reader.GetBoolean(Define.media_status);
                        m.User_ID = reader.GetInt32(Define.media_user);
                        m.Time = reader.GetDateTime(Define.media_time);
                    }
                }
                return m;
            }
            return null;
        }
        public static List<TypeMedia> toTypeMedias(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<TypeMedia> types = new List<TypeMedia>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        TypeMedia type = new TypeMedia();
                        type.Id = reader.GetInt32(Define.type_id);
                        type.Name = reader.GetString(Define.type_name);
                        type.Code = reader.GetString(Define.type_code);
                        try
                        {
                            type.Icon = reader.GetString(Define.type_icon);
                        }
                        catch (Exception)
                        {
                        }
                        types.Add(type);
                    }
                }
                return types;
            }
            return null;

        }
        public static TypeMedia toTypeMedia(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                TypeMedia type = new TypeMedia();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {

                        type.Id = reader.GetInt32(Define.type_id);
                        type.Name = reader.GetString(Define.type_name);
                        type.Code = reader.GetString(Define.type_code);
                        try
                        {
                            type.Icon = reader.GetString(Define.type_icon);
                        }
                        catch (Exception)
                        {
                        }

                    }

                }
                return type;
            }
            return null;
        }
        public static UserData toUser(this MySqlDataReader reader)
        {
            UserData u = null;
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    u = new UserData();
                    u.ID = reader.GetInt32(Define.user_id);
                    u.Full_Name = reader.GetString(Define.user_full_name);
                    u.Permision = Permision.Read(reader.GetString(Define.user_permision));
                    u.Status = reader.GetBoolean(Define.user_status);
                    u.Type = reader.GetInt32(Define.user_type);
                    u.Time = reader.GetDateTime(Define.user_time);
                    u.User_Name = reader.GetString(Define.user_name);
                    u.setDirectpass(reader.GetString(Define.user_pass));
                    try
                    {
                        u.Comment = reader.GetString(Define.user_content);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        u.Phone = reader.GetString(Define.user_phone);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        u.Email = reader.GetString(Define.user_email);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return u;
        }
        public static List<UserData> toUsers(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<UserData> results = new List<UserData>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        UserData u = new UserData();
                        u.ID = reader.GetInt32(Define.user_id);
                        u.Full_Name = reader.GetString(Define.user_full_name);
                        u.Permision = Permision.Read(reader.GetString(Define.user_permision));
                        u.Status = reader.GetBoolean(Define.user_status);
                        u.Type = reader.GetInt32(Define.user_type);
                        u.Time = reader.GetDateTime(Define.user_time);
                        u.setDirectpass(reader.GetString(Define.user_pass));
                        try
                        {
                            u.Comment = reader.GetString(Define.user_content);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            u.Phone = reader.GetString(Define.user_phone);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            u.Email = reader.GetString(Define.user_email);
                        }
                        catch (Exception)
                        {
                        }
                        u.User_Name = reader.GetString(Define.user_name);
                        results.Add(u);
                    }

                }
                return results;
            }
            return null;
        }
        public static UserTypeData toTypeUser(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                UserTypeData type = new UserTypeData();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        type.Id = reader.GetInt32(Define.type_id);
                        type.Name = reader.GetString(Define.type_name);
                        type.Status = reader.GetBoolean(Define.type_status);
                        type.DefaultPermision = Permision.Read(reader.GetString(Define.default_permision));
                        type.Icon = reader.GetString(Define.type_icon);
                    }
                }
                return type;
            }
            return null;
        }
        public static List<UserTypeData> toTypeUsers(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<UserTypeData> results = new List<UserTypeData>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        UserTypeData type = new UserTypeData();
                        type.Id = reader.GetInt32(Define.type_id);
                        type.Name = reader.GetString(Define.type_name);
                        type.Status = reader.GetBoolean(Define.type_status);
                        type.DefaultPermision = Permision.Read(reader.GetString(Define.default_permision));
                        type.Icon = reader.GetString(Define.type_icon);
                        results.Add(type);
                    }
                }
                return results;
            }
            return null;
        }
    }
}
