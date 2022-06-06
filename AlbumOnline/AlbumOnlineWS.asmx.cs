using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AlbumOnline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AlbumOnline
{
    /// <summary>
    /// Descripción breve de AlbumOnlineWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class AlbumOnlineWS : System.Web.Services.WebService
    {
        AlbumOnlineModelContainer db = new AlbumOnlineModelContainer();

        [WebMethod]
        public bool AddImg(byte[] img, int tipo, string user)
        {
            Photo item = new Photo();
            item.Img = img;
            item.TipoId = tipo;
            item.User = user;
            item.Fecha = DateTime.Now;
            item.Status = "A";
            db.Photos.Add(item);
            if (db.SaveChanges() > 0)
                return true;
            else
                return false;
        }


        [WebMethod]
        public List<SWPhoto> get_photos(string user)
        {
            return db.Photos.Where(x => x.User == user && x.Status == "A").Select(x => new SWPhoto()
            {
                Id = x.Id,
                Img = x.Img,
                User = x.User,
                Fecha = x.Fecha.ToString(),
                TipoId = x.TipoId,
                Status = x.Status
            }).ToList();
        }


        public List<SWPhoto> get_photos_by_tipo(int tipo)
        {

            return db.Photos.Where(x => x.TipoId == tipo && x.Status == "A").Select(x => new SWPhoto()
            {
                Id = x.Id,
                Img = x.Img,
                User = x.User,
                Fecha = x.Fecha.ToString(),
                TipoId = x.TipoId,
                Status = x.Status
            }).ToList();
        }


        [WebMethod]
        public bool Login(string user, string pass)
        {
            // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
            // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true

            // var result = SignInManager.PasswordSignInAsync(user, pass, false, shouldLockout:false);
            var result = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>().PasswordSignIn(user, pass, false, false);

            if (result == SignInStatus.Success)
            {
                return true;
            }
            else
                return false;
        }

        [WebMethod]
        public bool addUser(string Email, string Pass)
        {
            bool band = false;
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new
            UserStore<ApplicationUser>(context));
            //Here we create a Admin super user who will maintain the website
            var user = new ApplicationUser();
            user.UserName = Email;
            user.Email = Email;
            string userPWD = Pass;
            var chkUser = UserManager.Create(user, userPWD);
            //Add default User to Role Admin
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(user.Id, "freemium");
                band = true;
            }
            else band = false;
            //Agregamos una persona
            return band;
        }

        public partial class SWPhoto
        {
            public int Id { get; set; }
            public byte[] Img { get; set; }
            public string User { get; set; }
            public string Fecha { get; set; }
            public int TipoId { get; set; }
            public string Status { get; set; }
        }

    }
}
