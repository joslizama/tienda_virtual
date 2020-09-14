using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using tienda_express.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;



namespace tienda_express.Controllers
{
    public class usuarioController : Controller
    {
        //
        // GET: /usuario/
        public ActionResult Index_usuario()
        {
            return View();
        } 
       //ver catalogo general 
        public ActionResult vcat()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var lcat = dbc.producto;

                return View(lcat.ToList());

            };
        }
  //mostar la imagen en la vista
        public ActionResult getid(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                producto producto = dbc.producto.Find(id);

                byte[] byteimagen = producto.foto;

                MemoryStream memorystream = new MemoryStream(byteimagen);

                Image img = Image.FromStream(memorystream);

                memorystream = new MemoryStream();

                img.Save(memorystream,ImageFormat.Jpeg);

                memorystream.Position = 0;

                return File(memorystream, "Image/Jpg");
            };
        }
//filtar por categoria vista
        public ActionResult Fcat()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                var listcategoria = dbc.categoria;

                return View(listcategoria.ToList());
            }
        }
//filtrar por categoria controlador
        public ActionResult fcatc()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var bporcategoria = Convert.ToInt32(Request.Form["bprod"]);

                if (bporcategoria == null || bporcategoria <= 0)
                {
                    return RedirectToAction("Error_usuario", "usuario");
                }
                else
                {
                // buscar por categoria
                    var buscar = dbc.producto.SqlQuery("SELECT * FROM producto WHERE cat_id ='" + bporcategoria + "'").ToList();


                    return View(buscar);
                }
            };
        }
//pagina de error usuario
        public ActionResult Error_usuario()
        {
            return View();
        }

// ver el carro de compras con libertad
         
        public ActionResult Agregar2()
        {
            return View("Agregar");
        }





//agregar al carro de compra cuando hay productos 
        public ActionResult Agregar(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                //crear sesion de carro de compras para mostrar en la vista

                    if (Session["carro"] == null)
                    {

                    List<carroitem> compras = new List<carroitem>();

                    //agregar producto en al listado en base la id del producto seleccionado
                    compras.Add(new carroitem(dbc.producto.Find(id), 1));
                    Session["carro"] = compras;

                    }
                    else
                    {
                    // crear una variable que obtiene la id del producto(utilizar la funcion de obtener la id)
                        int Existente = idprod(id);
                    //llamar la lista con una variable sesion
                        List<carroitem> compras = (List<carroitem>)Session["carro"];    
                    //comprarar si una id se repite
                        if (Existente == -1)
                        {
                            //agregar producto
                            compras.Add(new carroitem(dbc.producto.Find(id),1));
                        }
                        else
                        {
                            //agregar cantidad del producto
                            compras[Existente].Cantidad++;
                            Session["carro"] = compras;

                        }
                        return View();
                }     

            };

            return View();
        }

//ver si la id del producto se repite
// debe ser un metodo privado ya que es una accion interna del programa
        private int idprod(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
            //llamar la lista
                List<carroitem> compras = (List<carroitem>)Session["carro"];
            //recorrer el listado
                for (int i = 0; i < compras.Count; i++)
                {
                // verificar si la id del producto se repite

                    if (compras[i].Producto.id == id)
                    {
                        return i;
                    }
                }
                return -1;
            };
        }
//eliminar producto del listado

        public ActionResult Eliminar(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
               
            //llamar el listado de productos
             List<carroitem> compras = (List<carroitem>)Session["carro"];
            //en el carro de compras la variable compras se debe utilizar en todo momento para ejecutar cualquier accion

             int misma_id = idprod(id);

             for (int j = 0; j < compras.Count; j++)
             {
                 if (compras[j].Cantidad > 1)
                 {
                     compras[j].Cantidad = compras[j].Cantidad - 1;
                     return View("Agregar");
                 }
             }
             compras.RemoveAt(idprod(id));
             return View("Agregar");



            };
        }
//realizar la venta del producto 
        public ActionResult Terminar_Compra(producto producto)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                
                //llamar a la lista
                List<carroitem> compras = (List<carroitem>)Session["carro"];
                //verirficar si el listado tiene datos 

                if (compras.Count > 0 && compras != null)
                {
                //recorrer el carro de compras
                    

                    for (int k = 0; k < compras.Count; k++)
                    {
                    // verificar si la cantidad es mayor al stock de producto
                        if (compras[k].Cantidad > compras[k].Producto.stock)
                        {
                            return RedirectToAction("Error_usuario", "usuario");
                        }
                        else
                        {
                          

                            var mod = dbc.producto.SingleOrDefault(w => w.id == producto.id);

                            producto.stock = producto.stock - compras[k].Cantidad;
                            dbc.SaveChanges();
                            if (producto.stock <=0)
                            {
                                return RedirectToAction("Error_usuario", "usuario");
                            }
                        }



                        
                    } // --> end for
                 
                    //funciona pero no elimina el stock de producto
                   

                    

                //realizar la venta y registrar en la bases de datos
                    var rut=Session["r"].ToString();

                    venta v = new venta();

                    v.dia_venta = DateTime.Today;
                    v.sub_total = compras.Sum(x => x.Cantidad * x.Producto.precio);
                    v.iva = 0.19;
                    v.total = v.sub_total * v.iva + v.sub_total;
                    v.usuario_id = rut;

                    

                //realizar la lista de la venta(boleta)

                    v.lista_venta = (from item in compras
                                     select new lista_venta
                                     {
                                         venta_id = v.id,
                                         producto_id = item.Producto.id,
                                         cantidad = item.Cantidad,
                                         total = item.Cantidad * item.Producto.precio,

                                     }).ToList();
                   
                    
                    dbc.venta.Add(v);
                    dbc.SaveChanges(); 

                    return RedirectToAction("comprarealizada", "usuario");

                    

                }
                else
                {
                    return RedirectToAction("ntienedatos", "usuario");
                } 
                
            };
        }
//no tiene datos mensaje de error
        public ActionResult ntienedatos()
        {
            return View();
        }

//mensaje de compra realizada
        public ActionResult comprarealizada()
        {
            return View();
        }
//modificar datos de usuario vista
        public ActionResult mdatos()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var rut = Convert.ToString(Session["r"]);

                if (rut == null)
                {
                    return RedirectToAction("Error_usuario", "usuario");
                }
                else
                {

                    var moddatosv = dbc.usuario.Find(rut);


                    if (moddatosv == null)
                    {
                        return RedirectToAction("Error_usuario", "usuario");
                    }
                    else
                    {
                        return View(moddatosv);
                    }
                }
            }; 
        }
//modificar datos de usuario controlador
        public ActionResult modusuarioc(usuario usuario)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                var r = usuario.rut;
                var n = usuario.nombre;
                var a = usuario.apellido;
                var d = usuario.direccion;
                var c = usuario.clave;


                if (r == null || r == "" || n == null || n == "" || a == null || a == "" || d == null || d == "" || c == null || c == "")
                {
                    return RedirectToAction("Error_usuario", "usuario");
                }
                else
                {

                    var mod = dbc.usuario.SingleOrDefault(b => b.rut == r);

                    mod.rut = r;
                    mod.nombre = n;
                    mod.apellido = a;
                    mod.direccion = d;
                    mod.clave = c;
                    mod.tipo_id = 2;


                    Session["r"] = r.ToString();
                    Session["n"] = n.ToString();
                    Session["a"] = a.ToString();


                    dbc.SaveChanges();

                    return RedirectToAction("Index_usuario", "usuario");

                }
            };
        }
//ver ventas realizadas
        public ActionResult vventas()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                var rut = Convert.ToString(Session["r"]);

                var lista = dbc.venta.Where(c => c.usuario_id == rut).ToList();


                return View(lista);
            };
        }
//ver listado de ventas realizadas
        public ActionResult Vlvent(int id)
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {

                if (id == null)
                {
                    return RedirectToAction("Error_usuario", "usuario");
                }
                else
                {

                    var llv1 = dbc.venta.Find(id);

                    if (llv1 == null)
                    {
                        return RedirectToAction("Error_usuario", "usuario");
                    }
                    else
                    {
                        var llv2 = dbc.lista_venta.Where(z => z.venta_id == llv1.id);


                        var lfinal = llv2.Include("producto");

                        return View(lfinal.ToList());
                    }
                }
            };
        }
//eliminar cuenta de usuario
        public ActionResult edatos()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                var rut2 = Convert.ToString(Session["r"]);

                if (rut2 == null)
                {
                    return RedirectToAction("Index_usuario", "usuario");
                }
                else
                {
                    var eliminar = dbc.usuario.Find(rut2);

                    if (eliminar == null)
                    {
                        return RedirectToAction("Index_usuario", "usuario");
                    }
                    else
                    {

                        dbc.usuario.Remove(eliminar);
                        Session["r"] = null;
                        dbc.SaveChanges();
                        return RedirectToAction("Index", "sistema");
                    }
                }
            };
        }
//cerrar sesion 
        public ActionResult csession()
        {
            using (ejemplodatacontext dbc = new ejemplodatacontext())
            {
                Session["r"] = null;
                Session.Abandon();
                Session.Clear();
                return View();

            };
        }










	}
}