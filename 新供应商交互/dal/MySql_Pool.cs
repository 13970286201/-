using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;
using System.Timers;

namespace http请求服务.dal
{
    public sealed class MySql_Pool
    {
        public static long GARBAGE_INTERVAL = 90 * 1000; //90 seconds
        public MySql_Pool(string _connectionString)
        {
            connectionString = _connectionString;
            locked = Hashtable.Synchronized(new Hashtable());
            unlocked = Hashtable.Synchronized(new Hashtable());
            lastCheckOut = DateTime.Now.Ticks;
            //Create a Time to track the expired objects for cleanup.
            Timer aTimer = new Timer();
            aTimer.Enabled = true;
            aTimer.Interval = GARBAGE_INTERVAL;
            aTimer.Elapsed += new ElapsedEventHandler(CollectGarbage);
        }
        private Hashtable unlocked;
        private Hashtable locked;
        private long lastCheckOut;
        private string connectionString = "";

        private void CollectGarbage(object sender, ElapsedEventArgs ea)
        {
            lock (this)
            {
                //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":执行一次回收资源");
                object o;
                long now = DateTime.Now.Ticks;
                IDictionaryEnumerator e = unlocked.GetEnumerator();

                try
                {
                    while (e.MoveNext())
                    {

                        o = e.Key;

                        if ((now - (long)unlocked[o]) > GARBAGE_INTERVAL)
                        {
                            unlocked.Remove(o);
                            Expire(o);
                            o = null;
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        private void Expire(object o)
        {
            try
            {
                MySqlConnection conn = (MySqlConnection)o;
                conn.Close();
            }
            catch (MySqlException) { }
        }

        private object GetObjectFromPool()
        {
            while (locked.Count > 99)
            {
                //如果使用连接数大于100 就等待
                System.Threading.Thread.Sleep(500);
                Console.Write("连接池超过99，等待...");
            }
            long now = DateTime.Now.Ticks;
            lastCheckOut = now;
            object o = null;

            lock (this)
            {
                try
                {
                    foreach (DictionaryEntry myEntry in unlocked)
                    {
                        o = myEntry.Key;
                        unlocked.Remove(o);
                        if (Validate(o))
                        {
                            locked.Add(o, now);
                            return o;
                        }
                        else
                        {
                            Expire(o);
                            o = null;
                        }
                    }
                }
                catch (Exception) { }
                o = Create();
                locked.Add(o, now);
            }
            return o;
        }

        private void ReturnObjectToPool(object o)
        {
            if (o != null)
            {
                lock (this)
                {
                    locked.Remove(o);
                    unlocked.Add(o, DateTime.Now.Ticks);
                }
            }
        }

        private object Create()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        private bool Validate(object o)
        {
            try
            {
                MySqlConnection conn = (MySqlConnection)o;
                return !conn.State.Equals(ConnectionState.Closed);
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public MySqlConnection BorrowDBConnection()
        {
            try
            {
                return (MySqlConnection)GetObjectFromPool();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void ReturnDBConnection(MySqlConnection conn)
        {
            ReturnObjectToPool(conn);
        }


    }
}
