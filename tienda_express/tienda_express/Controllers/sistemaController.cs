using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using tienda_express.Models;

namespace tienda_express.Controllers
{
    public class sistemaController : Controller
    {
        //
        // GET: /sistema/
        public ActionResult Index()
        {
            return View();
        } 
//login admin y usuario vista
        public ActionResult log()
        {
            return View();
        }
//login admin y usuario controlador
        public ActionResult logc(tienda_express.Models.usuario u)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var ru = u.rut;
                var cl = u.clave;

                if (ru == null || ru == "" || cl == null || cl == "")
                {
                    return RedirectToAction("Error", "sistema");
                }
                else
                {
                    var l = dbc.usuario.Where(p => p.rut == ru && p.clave == cl).SingleOrDefault();

                    Session["r"] = l.rut.ToString();
                    Session["n"] = l.nombre.ToString();
                    Session["a"] = l.apellido.ToString();


                    if (l.tipo_id == 1)
                    {
                        return RedirectToAction("Index_admin", "admin");
                    }
                    else if (l.tipo_id == 2)
                    {
                        return RedirectToAction("Index_usuario", "usuario");
                    }
                    else
                    {
                        return RedirectToAction("Error", "sistema");
                    }
                }
            };
        } 
// regsitrarse como usuario vista
        public ActionResult reg()
        {
            return View();
        }
//registrarse como usuario controlador
        public ActionResult nusc(tienda_express.Models.usuario u)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                var rus = u.rut;
                var nus = u.nombre;
                var aus = u.apellido;
                var dus = u.direccion;
                var cus = u.clave;

                if (rus==null || rus==""||nus == null || nus == "" || aus == null || aus == "" || dus == null || dus == "" || cus == null || cus == "")
                {
                    return RedirectToAction("Error", "sistema");
                }
                else
                {

                    usuario us = new usuario
                    {
                        rut=rus,
                        nombre=nus,
                        apellido=aus,
                        direccion=dus,
                        clave=cus,
                        tipo_id=2
                    };
                    dbc.usuario.Add(us);
                    dbc.SaveChanges();
                //capturar los datos para las variables sesiones

                    var regdatos = dbc.usuario.SingleOrDefault(p => p.rut == rus);

                    Session["r"] = regdatos.rut.ToString();
                    Session["n"] = regdatos.nombre.ToString();
                    Session["a"] = regdatos.apellido.ToString();

                    return RedirectToAction("Index_usuario", "usuario");


                }
            };
        }
//pagina de error 
        public ActionResult Error()
        {
            return View();
        }
//








	}
}