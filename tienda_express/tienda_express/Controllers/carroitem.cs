using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using tienda_express.Models;

namespace tienda_express.Controllers
{
    //clase de carro de compras.
    public class carroitem
    { 
    // definir atributos
        private producto _producto;
        private int _cantidad;

        // get y set de los atributos
        public producto Producto
        {
            get { return _producto; }
            set { _producto = value; }
        }

        public int Cantidad
        {
            get { return _cantidad; }
            set { _cantidad = value; }
        }

       //crear los metodos para realizar la funcionalidad

        public carroitem(producto _producto, int _cantidad)
        {
            this._producto = _producto;
            this._cantidad = _cantidad;


        }










    }
}