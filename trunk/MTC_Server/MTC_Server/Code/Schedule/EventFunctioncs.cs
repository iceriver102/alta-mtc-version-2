using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.Schedule
{
    public partial class Event
    {
        internal void setPlaylist(int p)
        {
            if (p > 0)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "`p_set_playlist_to_schedule`";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_s_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.Id });
                            cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = p });
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                        };
                        conn.Close();
                    };
                    this.playlist_id = p;
                }
                catch (Exception)
                {

                }
            }
            else
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                    {
                        conn.Open();
                        string query = "`p_reset_playlist_schedule`";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Add(new MySqlParameter("@_s_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.Id });
                            // cmd.Parameters.Add(new MySqlParameter("@_playlist_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = p });
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ExecuteScalar();
                        };
                        conn.Close();
                    };
                    this.playlist_id = p;
                }
                catch (Exception)
                {

                }
            }
        }
        public int EndIndex(DateTime curTime)
        {
            if (this.isAllDay(curTime))
            {
                return 0;
            }
            if (this.End.Date > curTime.Date)
                return 24;
            return this.End.Hour + 1;
        }

        public bool isAllDay(DateTime curDateView)
        {
            if ((this.End.Date > curDateView.Date || this.End.TimeOfDay == new TimeSpan(23, 59, 59)) && (this.Begin < curDateView.Date || this.Begin.TimeOfDay == new TimeSpan(23, 59, 59)))
                return true;
            return false;
        }

        public double getHeight(DateTime curDateView)
        {
            if (this.Begin == null)
                return double.NaN;
            if (this.End == null)
                return 0;
            if (this.isAllDay(curDateView))
                return 30;
            TimeSpan tmp;
            if (this.End.Date > curDateView.Date)
            {
                curDateView = new DateTime(curDateView.Year, curDateView.Month, curDateView.Day, 23, 59, 59);
                tmp = curDateView - this.Begin;
            }
            else if (this.Begin.Date == curDateView.Date)
            {
                tmp = this.End - this.Begin;
            }
            else
            {
                tmp = this.End - curDateView.Date;
            }
            return (tmp.TotalMinutes * 2 < 20) ? 20 : tmp.TotalMinutes * 2;
        }

        public double getTop(DateTime curDateView)
        {
            if (this.Begin == null)
                return double.NaN;
            if (curDateView == null)
            {
                curDateView = DateTime.Now;
            }
            if (this.loop)
            {
                return this.Begin.TimeOfDay.Minutes * 2;
            }
            if (this.isAllDay(curDateView))
                return 0;
            if (curDateView.Date > this.Begin.Date)
                return 0;
            return this.Begin.TimeOfDay.Minutes * 2;
        }
        public int beginIndex(DateTime curTime)
        {
            if (this.isAllDay(curTime))
                return 0;
            if (this.Begin.Date < curTime.Date)
                return 1;
            return this.Begin.Hour + 1;
        }

        public int comitBeginIndex(DateTime curTime)
        {
            if (this.isAllDay(curTime))
                this.BeginIndex = 0;
            else
                if (this.Begin.Date < curTime.Date)
                    this.BeginIndex = 1;
                else
                    this.BeginIndex = this.Begin.Hour + 1;
            return this.BeginIndex;
        }

        public static void delete(Event e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_delete_schedule`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_s_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = e.Id });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
        }

        public int Save()
        {
            int result = 0;
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_update_schedule`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_id", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.Id });
                        cmd.Parameters.Add(new MySqlParameter("@_device", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.device_id });
                        cmd.Parameters.Add(new MySqlParameter("@_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.user_id });
                        cmd.Parameters.Add(new MySqlParameter("@_parent", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.parent_id });
                        cmd.Parameters.Add(new MySqlParameter("@_time_begin", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = this.Begin });
                        cmd.Parameters.Add(new MySqlParameter("@_time_end", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = this.End });
                        cmd.Parameters.Add(new MySqlParameter("@_loop", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = this.loop });
                        cmd.Parameters.Add(new MySqlParameter("@_comment", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = this.Content });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@result"].Value);

                    };
                    conn.Close();
                };
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static int Insert(Event e)
        {
            int result = 0;
            try
            {

                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "`p_insert_schedule_device`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@_device", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = e.device_id });
                        cmd.Parameters.Add(new MySqlParameter("@_user", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = e.user_id });
                        cmd.Parameters.Add(new MySqlParameter("@_parent", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = e.parent_id });
                        cmd.Parameters.Add(new MySqlParameter("@_time_begin", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = e.Begin });
                        cmd.Parameters.Add(new MySqlParameter("@_time_end", MySqlDbType.DateTime) { Direction = System.Data.ParameterDirection.Input, Value = e.End });
                        cmd.Parameters.Add(new MySqlParameter("@_loop", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Input, Value = e.loop });
                        cmd.Parameters.Add(new MySqlParameter("@_comment", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Input, Value = e.Content });
                        cmd.Parameters.Add(new MySqlParameter("@result", MySqlDbType.Int32) { Direction = System.Data.ParameterDirection.Output, Value = 0 });
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteScalar();
                        result = Convert.ToInt32(cmd.Parameters["@result"].Value);

                    };
                    conn.Close();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
            }
            return result;
        }


    }
}
