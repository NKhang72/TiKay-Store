using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TiKayStore.Models
{
    public class User_DAO
    {
        PhoneStoreEntities1 db = null;
        public User_DAO()
        {
            db = new PhoneStoreEntities1();
        }
        public tb_User getItem(string email)
        {
            return db.tb_User.FirstOrDefault(x => x.email == email);
        }
        public List<tb_User> getList()
        {
            return db.tb_User.ToList();

        }
        public tb_User Add(tb_User tb_User)
        {
            db.tb_User.Add(tb_User);
            db.SaveChanges();
            return tb_User;
        }
        public tb_User Update(tb_User tb_User)
        {
            var user = db.tb_User.FirstOrDefault(x => x.email == tb_User.email);
            user.password = tb_User.password;
            user.firstName = tb_User.firstName;
            user.lastName = tb_User.lastName;
            user.status = tb_User.status;
            user.image = tb_User.image;
            db.SaveChanges();
            return user;
        }
        public int Login(string email, string password)
        {
            var user = db.tb_User.FirstOrDefault(x => x.email == email);
            if (user == null)
            {
                return -2; //tk khong ton tai
            }
            else
            {
                if (user.status == false)
                {
                    return 0; //ch kich hoat
                }
                else
                {
                    if (user.password == password)
                    {
                        return 1; //thanh cong
                    }
                    else if (user.password.Length < 6)
                    {
                        return -4;//mk thieu hon 6
                    }
                    else
                    {
                        return -3; //sai mk
                    }
                }
            }
        }
    }
}