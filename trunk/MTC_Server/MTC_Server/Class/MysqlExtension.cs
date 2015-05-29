using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTC_Server.Code;
using MTC_Server.Code.User;
using MTC_Server.Code.Media;
using MySql.Data.MySqlClient;
using MTC_Server.Code.Device;
using MTC_Server.Code.Playlist;

namespace Alta.Class
{
    public static class MysqlExtensions
    {
        public static MediaEvent toMediaEvent(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                MediaEvent d = null;
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        d = new MediaEvent();
                        d.ID = reader.GetInt32(Define.detail_id);
                        d.media_id = reader.GetInt32(Define.media_id);
                        d.playlist_id = reader.GetInt32(Define.playlist_id);
                        d.TimeBegin = reader.GetTimeSpan(Define.time_begin);
                        d.TimeEnd = reader.GetTimeSpan(Define.time_end);
                        d.Time = reader.GetDateTime(Define.detail_date);

                    }
                }
                return d;
            }
            return null;
        }

        public static List<MediaEvent> toMediaEvents(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<MediaEvent> datas = new List<MediaEvent>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        MediaEvent d = new MediaEvent();
                        d.ID = reader.GetInt32(Define.detail_id);
                        d.media_id = reader.GetInt32(Define.media_id);
                        d.playlist_id = reader.GetInt32(Define.playlist_id);
                        d.TimeBegin = reader.GetTimeSpan(Define.time_begin);
                        d.TimeEnd = reader.GetTimeSpan(Define.time_end);
                        d.Time = reader.GetDateTime(Define.detail_date);
                        datas.Add(d);
                    }
                }
                return datas;
            }
            return null;
        }

        public static Playlist toPlaylist(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                Playlist d = null;
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        d = new Playlist();
                        d.ID = reader.GetInt32(Define.playlist_id);
                        d.Name = reader.GetString(Define.playlist_name);
                        d.Time = reader.GetDateTime(Define.playlist_datetime);
                        d.user_id = reader.GetInt32(Define.playlist_user);
                        d.Status = reader.GetBoolean(Define.playlist_status);
                        d.Default = reader.GetBoolean(Define.playlist_default);
                        try
                        {
                            d.Comment = reader.GetString(Define.playlist_comment);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                return d;
            }
            return null;
        }
        public static List<Playlist> toPlaylists(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<Playlist> data = new List<Playlist>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        Playlist d = new Playlist();
                        d.ID = reader.GetInt32(Define.playlist_id);
                        d.Name = reader.GetString(Define.playlist_name);
                        d.Time = reader.GetDateTime(Define.playlist_datetime);
                        d.user_id = reader.GetInt32(Define.playlist_user);
                        d.Status = reader.GetBoolean(Define.playlist_status);
                        d.Default = reader.GetBoolean(Define.playlist_default);
                        try
                        {
                            d.Comment = reader.GetString(Define.playlist_comment);
                        }
                        catch (Exception)
                        {
                        }
                        data.Add(d);
                    }
                }
                return data;
            }
            return null;
        }
        public static DeviceData toDevice(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                DeviceData d = null;
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        d = new DeviceData();
                        d.ID = reader.GetInt32(Define.device_id);
                        d.Name = reader.GetString(Define.device_name);
                        d.Status = reader.GetBoolean(Define.device_status);
                        d.Time = reader.GetDateTime(Define.device_time);
                        d.Type = reader.GetInt32(Define.device_type);
                        d.IP = reader.GetString(Define.device_ip);
                        d.Pass = reader.GetString(Define.device_pass);
                        try
                        {
                            d.Comment = reader.GetString(Define.device_comment);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                return d;
            }
            return null;
        }
        public static List<DeviceData> toDevices(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<DeviceData> datas = new List<DeviceData>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        DeviceData d = new DeviceData();
                        d.ID = reader.GetInt32(Define.device_id);
                        d.Name = reader.GetString(Define.device_name);
                        d.Status = reader.GetBoolean(Define.device_status);
                        d.Time = reader.GetDateTime(Define.device_time);
                        d.Type = reader.GetInt32(Define.device_type);
                        d.IP = reader.GetString(Define.device_ip);
                        d.Pass = reader.GetString(Define.device_pass);
                        try
                        {
                            d.Comment = reader.GetString(Define.device_comment);
                        }
                        catch (Exception)
                        {
                        }
                        datas.Add(d);

                    }
                }
                return datas;
            }
            return null;
        }
        public static List<MTC_Server.Code.Schedule.Event> toSchedules(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<MTC_Server.Code.Schedule.Event> events = new List<MTC_Server.Code.Schedule.Event>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        MTC_Server.Code.Schedule.Event e = new MTC_Server.Code.Schedule.Event();
                        e.Id = reader.GetInt32(Define.schedule_id);
                        try
                        {
                            e.Content = reader.GetString(Define.schedule_comment);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            e.playlist_id = reader.GetInt32(Define.schedule_playlist);
                        }
                        catch (Exception)
                        {

                        }
                        e.parent_id = reader.GetInt32(Define.schedule_parent);
                        e.device_id = reader.GetInt32(Define.schedule_device);
                        e.user_id = reader.GetInt32(Define.schedule_user);
                        e.status = reader.GetBoolean(Define.schedule_status);
                        e.Begin = reader.GetDateTime(Define.schedule_time_begin);
                        e.End = reader.GetDateTime(Define.schedule_time_end);
                        e.loop = reader.GetBoolean(Define.schedule_loop);

                        events.Add(e);
                    }
                }
                return events;
            }
            return null;
        }
        public static List<TypeDevice> toTypeDevices(this MySqlDataReader reader)
        {
            if (reader.HasRows)
            {
                List<TypeDevice> datas = new List<TypeDevice>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        TypeDevice t = new TypeDevice();
                        t.ID = reader.GetInt32(Define.type_id);
                        t.Name = reader.GetString(Define.type_name);
                        t.Time = reader.GetDateTime(Define.type_time);
                        try
                        {
                            t.Comment = reader.GetString(Define.type_comment);
                        }
                        catch (Exception)
                        {
                        }
                        datas.Add(t);
                    }
                }
                return datas;
            }
            return null;
        }
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
                            m.Md5 = reader.GetString(Define.media_md5);
                        }
                        catch (Exception)
                        {

                        }
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
                            m.Md5 = reader.GetString(Define.media_md5);
                        }
                        catch (Exception)
                        {

                        }
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

                    try
                    {
                        u.Finger_Print = new byte[10*1024*1024];
                        long  bytesRead=reader.GetBytes(reader.GetOrdinal(Define.user_finger_print), 0, u.Finger_Print, 0, u.Finger_Print.Length);
                        Console.WriteLine(bytesRead + " bytes downloaded from table to file."); 
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
