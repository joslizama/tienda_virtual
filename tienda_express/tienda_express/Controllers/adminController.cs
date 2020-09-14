using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using tienda_express.Models;
using System.Web.Helpers;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;



namespace tienda_express.Controllers
{
    public class adminController : Controller
    {
        //
        // GET: /admin/
        public ActionResult Index_admin()
        {
            return View();
        } 
    //listado de categorias
        public ActionResult lcat()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var list_cat = dbc.categoria.SqlQuery("SELECT * FROM categoria").ToList();

                return View(list_cat);

            };
        }
//nueva categoria vista
        public ActionResult Ncat()
        {
            return View();
        }
//nueva categoria controlador
        public ActionResult ncatc(tienda_express.Models.categoria c)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                var nc = c.nombre;

                if (nc == null || nc == "")
                {
                    return RedirectToAction("Error_admin", "admin");
                }
                else
                {

                    categoria cat = new categoria
                    {
                        nombre=nc,
                    };
                    dbc.categoria.Add(cat);
                    dbc.SaveChanges();

                    return RedirectToAction("lcat", "admin");

                }
            };
        }
//eliminar categoria 
        public ActionResult Ecat(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                if (id == null)
                {
                    return RedirectToAction("Error_admin", "admin");
                }
                else
                {
                    var eliminarcat = dbc.categoria.Find(id);

                    if (eliminarcat == null)
                    {
                        return RedirectToAction("Error_admin", "admin");
                    }
                    else
                    {
                        dbc.categoria.Remove(eliminarcat);
                        dbc.SaveChanges();
                        return RedirectToAction("lcat", "admin");

                    }
                }
            };
        }
//editar categoria vista
        public ActionResult Editcat(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                if (id == null)
                {
                    return RedirectToAction("Error_admin", "admin");
                }
                else
                {
                    var editcat = dbc.categoria.Find(id);

                    if (editcat == null)
                    {
                        return RedirectToAction("Error_admin", "admin");
                    }
                    else
                    {
                        return View(editcat);
                    }
                }
            };
        }
//modificar categoria controlador
        public ActionResult modcatc([Bind(Include = "id,nombre")]categoria categoria)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                dbc.Entry(categoria).State = EntityState.Modified;
                dbc.SaveChanges();

                return RedirectToAction("lcat", "admin");
            };
        }
//listado de productos 
        public ActionResult lprod()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var list = dbc.producto;

                return View(list.ToList());
            };
        }
//nuevo producto vista
        public ActionResult Nprod()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var lc = dbc.categoria;

                return View(lc.ToList());


            };
        }
//nuevo producto controlador
        public ActionResult nprodc()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                HttpPostedFileBase filebase = Request.Files[0];
                var nprod = Request.Form["n"].ToString();
                var dprod = Request.Form["d"].ToString();
                var fecrea = Convert.ToDateTime(Request.Form["fe"]);
                var pprod = Convert.ToDouble(Request.Form["p"]);
                var sprod = Convert.ToInt32(Request.Form["s"]);
                var cprod = Convert.ToInt32(Request.Form["cid"]);

                //if ( nprod == "" || dprod == null || dprod == "" || fecrea == null || pprod == null || pprod <= 0 || sprod == null || sprod <= 0 || cprod == null || cprod <= 0)
                //{
                //    return RedirectToAction("Error_admin", "admin");
                //}
                //else
                //{
                    //transformar a binario la imagen
                    WebImage img = new WebImage(filebase.InputStream);

                    producto pr = new producto();
                    
                        pr.foto =img.GetBytes();
                        pr.nombre=nprod;
                        pr.descripcion=dprod;
                        pr.fecha_creacion=fecrea;
                        pr.precio=pprod;
                        pr.stock=sprod;
                        pr.cat_id=cprod;
                    
                    dbc.producto.Add(pr);
                    dbc.SaveChanges();

                    return RedirectToAction("lprod", "admin");
                //}
            };
        }
//eliminar producto
        public ActionResult Eprod(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                if (id == null)
                {
                    return RedirectToAction("Error_admin", "admin");
                }
                else
                {
                    var eprod = dbc.producto.Find(id);

                    if (eprod == null)
                    {
                        return RedirectToAction("Error_admin", "admin");
                    }
                    else
                    {
                        dbc.producto.Remove(eprod);
                        dbc.SaveChanges();

                        return RedirectToAction("lprod", "admin");
                    }
                }
            };
        }
//mostrar la imagen del libro
        public ActionResult getimagen(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                //declarar el objeto y buscar su id
             producto foto=dbc.producto.Find(id); 
                // transforma en bytes la foto
             byte[] byteimagen = foto.foto;
            //decalrar la matriz de memoria
             MemoryStream memory = new MemoryStream(byteimagen);

             Image img = Image.FromStream(memory);
            //decalarar el objeto de memorystream
             memory = new MemoryStream();
            // convertir en jpg
             img.Save(memory, ImageFormat.Jpeg);
             //matriz en la posicion de la imagen
             memory.Position = 0;
             return File(memory, "img/jpg");



            };
        }
//editar producto vista
        public ActionResult Editprod(int id)
        {
        using(ejemplodatacontext dbc = new ejemplodatacontext())
        {

            if (id == null)
            {
                return RedirectToAction("Error_admin", "admin");
            }
            else
            {
                var idprod = dbc.producto.Find(id);

                if (idprod == null)
                {
                    return RedirectToAction("Error_admin", "admin");
                }
                else
                {

                    return View(idprod);
                }
            }
        };
        }
//editar producto controlador
        public ActionResult edprodc([Bind(Include="id,foto,nombre,descripcion,fecha_creacion,precio,stock,cat_id")] producto producto)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                producto pr = dbc.producto.Find(producto.id); 

                HttpPostedFileBase file = Request.Files[0]; 

                WebImage img = new WebImage(file.InputStream);

                var bit = img.GetBytes();


                var mod = dbc.producto.SingleOrDefault(v => v.id == pr.id);

                mod.id = producto.id;
                mod.foto = bit;
                mod.nombre = producto.nombre;
                mod.descripcion = producto.descripcion;
                mod.fecha_creacion = producto.fecha_creacion;
                mod.precio = producto.precio;
                mod.stock = producto.stock;
                mod.cat_id = producto.cat_id;

                dbc.SaveChanges();

                return RedirectToAction("lprod", "admin");

            }; 
        }
//cerrar sesion de usuario
        public ActionResult csession()
        {
            Session["r"] = null;
            Session.Abandon();

            if(Session["r"] == null)
            {
                return RedirectToAction("Index", "sistema");
            }
            else
            {
                return View("Index_admin");
            }
            
        }
//




	}
}